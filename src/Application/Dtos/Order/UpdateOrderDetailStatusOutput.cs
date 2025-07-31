namespace Application.Dtos.Order
{
    public class UpdateOrderDetailStatusOutput
    {
        public int OrderStatus { get; set; }
        public int OrderSystemStatus { get; set; }
        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public int OrderDetailStatus { get; set; }
    }
}