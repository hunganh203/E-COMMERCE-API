using Application.Dtos;
using Application.DTOs.Cloud.AWS;
using Application.Enums;
using Application.Interfaces.CloudService.AWS;
using Application.Interfaces.Service.Invoice;
using Domain.Entities;
using SelectPdf;

namespace Cloud.Service.Invoice
{
    public class HandleInvoiceService : IHandleInvoiceService
    {
        private readonly IInvoiceProvider _invoiceProvider;
        private readonly IAwsS3Service _awsS3Service;

        public HandleInvoiceService(IInvoiceProvider invoiceProvider, IAwsS3Service awsS3Service)
        {
            _invoiceProvider = invoiceProvider;
            _awsS3Service = awsS3Service;
        }

        public async Task<FileStorageDto> GenerateInvoiceOfOrder(Order order)
        {
            var template = await _invoiceProvider.GetTemplateOrderInvoice(order, "vi");

            var converter = new HtmlToPdf
            {
                Options =
                {
                    DisplayHeader = true,
                    DisplayFooter = true,
                    PdfPageSize = PdfPageSize.A4,
                    PdfPageOrientation = PdfPageOrientation.Portrait,
                    WebPageHeight = 0,
                    MarginTop = 20,
                    MarginBottom = 20,
                    MarginRight = 10,
                    MarginLeft = 10
                }
            };

            var doc = converter.ConvertHtmlString(template, string.Empty);
            var docData = doc.Save();

            var currentDate = DateTime.UtcNow;
            var month = currentDate.Month.ToString().PadLeft(2, '0');

            var folder = $"{currentDate.Year}/{month}/invoice";

            var fileId = $"{order.Code.ToLower()}_{order.Id}_{order.CreatedDate.ToString("yyyyMMddTHHmmss")}";
            return await _awsS3Service.UploadFileAsync(docData!, new AwsInput
            {
                Id = fileId,
                FileName = $"{fileId}.pdf",
                BucketType = BucketType.Product,
                ContentType = "application/pdf",
            }, 123, folder);
        }
    }
}