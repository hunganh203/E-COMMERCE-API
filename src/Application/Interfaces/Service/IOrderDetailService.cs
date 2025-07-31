using Application.Dtos.Order;

namespace Application.Interfaces.Service
{
    public interface IOrderDetailService
    {
        Task DeleteById(int key);

        Task<bool> UpdateQuantity(int orderDetailId, int quantity);

        Task<bool> UpdatePriceStockReceiving(int orderDetailId, double priceStockReceiving);

        Task<bool> UpdateStockReceivingName(int orderDetailId, string receivingName);

        Task<UpdateOrderDetailStatusOutput> UpdateStatus(int orderDetailId, int status);

        Task<bool> UpdateNote(int orderDetailId, string note);

        Task<bool> UpdateCheckingCode(int orderDetailId, string checkingCode);

        Task<bool> UpdatePriceProduct(int orderDetailId, double productPrice, double productDiscountPrice);

        Task<OrderDetailDto> GetById(int key);

        Task<OrderDetailDto> Insert(OrderDetailDto entity);

        Task Update(int key, OrderDetailDto entity);
    }
}