using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces.Service.Invoice
{
    public interface IHandleInvoiceService
    {
        Task<FileStorageDto> GenerateInvoiceOfOrder(Order order);
    }
}