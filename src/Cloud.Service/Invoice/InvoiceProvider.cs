using Application.Dtos;
using Application.Extensions;
using Application.Interfaces.Service.Invoice;
using Domain.Entities;
using System.Collections.Concurrent;
using System.Text;

namespace Cloud.Service.Invoice
{
    public class InvoiceProvider : IInvoiceProvider
    {
        private readonly ICurrencyProvider _currencyProvider;
        private readonly ConcurrentDictionary<string, string> _defaultTemplates;

        private readonly string _nameSpace = "Cloud.Service.Invoice.InvoiceTemplate";

        public InvoiceProvider(ICurrencyProvider currencyProvider)
        {
            _currencyProvider = currencyProvider;
            _defaultTemplates = new ConcurrentDictionary<string, string>();
        }

        public async Task<string> GetTemplateOrderInvoice(Order order, string languageCode = "en")
        {
            var usdExchangeRate = await _currencyProvider.GetExchangeRateByCode("USD");

            return _defaultTemplates.GetOrAdd("OrderInvoice", key =>
            {
                var assembly = typeof(HandleInvoiceService).Assembly;
                using var stream = assembly.GetManifestResourceStream($"{_nameSpace}.Order.{languageCode}.OrderInvoice.html");
                byte[] bytes;

                using (var streamReader = new MemoryStream())
                {
                    stream?.CopyTo(streamReader);
                    bytes = streamReader.ToArray();
                }

                var template = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                var orderGenData = new
                {
                    deposit = order.Deposit?.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-us")),
                    depositUsd = CalculatePriceToUsd(usdExchangeRate.Transfer, order.Deposit.GetValueOrDefault()),
                    customerName = order.Customer?.FirstName + " " + order.Customer?.LastName,
                    customerAddress = order.Address,
                    customerPhone = order.PhoneNumber,
                    discountPrice = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:#,##0 đ}", order.DiscountPrice.GetValueOrDefault()),
                    orderDate = order.CreatedDate.ToString("dd/MM/yyyy"),
                    orderCode = order.Code,
                    shipPrice = order.OrderDetails.Sum(x => x.PriceShipping)
                        .GetValueOrDefault()
                        .ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-us")),

                    shipPriceUsd = CalculatePriceToUsd(usdExchangeRate.Transfer, order.OrderDetails.Sum(x => x.PriceShipping).GetValueOrDefault()),
                    totalPrice = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:#,##0 đ}", order.TotalPrice.GetValueOrDefault()),
                    totalPriceUsd = CalculatePriceToUsd(usdExchangeRate.Transfer, order.TotalPrice.GetValueOrDefault()),
                    note = order.Note,
                    createdDate = order.CreatedDate.ToString("dd/MM/yyyy"),
                };

                foreach (var fieldInfo in orderGenData.GetType().GetProperties())
                {
                    var value = fieldInfo.GetValue(orderGenData) != null ? fieldInfo.GetValue(orderGenData)!.ToString() : "";
                    var fieldName = fieldInfo.Name;
                    var oldValue = "{{" + fieldName.ToSnakeCase() + "}}"; ;
                    template = template.Replace(oldValue, value);
                }
                var templateOrderDetail = GetOrderDetailTemplate(usdExchangeRate, order, languageCode);
                template = template.Replace("{{order_detail_item}}", templateOrderDetail);
                return template;
            });
        }

        public string GetOrderDetailTemplate(CurrencyExchangeRateDto usdExchangeRate, Order order, string languageCode = "en")
        {
            var assembly = typeof(HandleInvoiceService).Assembly;
            using var stream = assembly.GetManifestResourceStream($"{_nameSpace}.Order.{languageCode}.OrderInvoiceLoop.html");
            byte[] bytes;

            using (var streamReader = new MemoryStream())
            {
                stream?.CopyTo(streamReader);
                bytes = streamReader.ToArray();
            }

            var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

            var result = "";
            var stt = 1;
            foreach (var orderDetail in order.OrderDetails)
            {
                var orderDetailGenData = new
                {
                    stt,
                    productDiscountPrice = orderDetail.ProductDiscountPrice,
                    attribute = orderDetail.Attribute,
                    extendClass = string.IsNullOrEmpty(orderDetail.Attribute) ? "d-none" : "inline-block",
                    productName = orderDetail.ProductName,
                    quantity = orderDetail.Quantity,
                    productPrice = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:#,##0 đ}", (orderDetail.Quantity * orderDetail.ProductDiscountPrice).GetValueOrDefault()),
                    productPriceUsd = CalculatePriceToUsd(usdExchangeRate.Transfer, (orderDetail.Quantity * orderDetail.ProductDiscountPrice).GetValueOrDefault()),
                };

                var bodyHtml = template;

                foreach (var fieldInfo in orderDetailGenData.GetType().GetProperties())
                {
                    var value = fieldInfo.GetValue(orderDetailGenData) != null ? fieldInfo.GetValue(orderDetailGenData)!.ToString() : "";
                    var fieldName = fieldInfo.Name;
                    var oldValue = "{{" + fieldName.ToSnakeCase() + "}}"; ;
                    bodyHtml = bodyHtml.Replace(oldValue, value);
                }

                result += bodyHtml;
                stt += 1;
            }

            return result;
        }

        private string CalculatePriceToUsd(double exchangeRateUsd, double priceVnd)
        {
            return Math.Round(priceVnd / exchangeRateUsd, 3).ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-us"));
        }
    }
}