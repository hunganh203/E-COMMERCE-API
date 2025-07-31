using Application.Dtos;
using Application.Dtos.Order;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IOrderService
    { 
         

        Task<PagedResultDto<OrderDto>> GetOrders(GetOrdersInput input);

        Task<PagedResultDto<OrderDto>> GetOrdersForCustomer(GetOrdersByCustomerInput input, int customerId);

        Task<OrderDto> GetById(int key);

        Task<OrderDto> GetByIdForCustomer(int key, int customerId);

        Task<OrderDto> ChangeStatus(int key, int systemStatus, int status); 

        Task<OrderDto> Insert(OrderDto entity, int userId);

        Task Update(int key, OrderDto entity, int userId);
        Task Delete(int key);

        Task<OrderDto> UpdateDeposit(int key, double value, int userId);

        Task<OrderDto> UpdateDiscountPrice(int key, double value, int userId);

        Task<OrderDto> UpdateAddress(int key, string value, int userId);

        Task<OrderDto> UpdateShippingInfo(int key, OrderShippingInfo value, int userId);

        Task<OrderDto> UpdatePhoneNumber(int key, string value, int userId);

        Task<OrderDto> UpdateNote(int key, string value, int userId);

        Task<OrderDto> UpdateStockNote(int key, string value, int userId);

        Task<FileStorageDto> GenerateInvoiceOfOrder(int orderId);
    }
}