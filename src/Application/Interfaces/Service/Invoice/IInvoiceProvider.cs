using Domain.Entities;

namespace Application.Interfaces.Service.Invoice
{
    public interface IInvoiceProvider
    {
        Task<string> GetTemplateOrderInvoice(Order order, string languageCode = "en");
    }
}