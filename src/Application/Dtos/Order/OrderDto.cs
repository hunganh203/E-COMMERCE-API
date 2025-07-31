using Application.Dtos.Base;
using Application.Dtos.Customer;

namespace Application.Dtos.Order
{
    public class OrderDto : BaseAuditInfo
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public bool IsCancel { get; set; }
        public bool IsFinish { get; set; }
        public string SourceFrom { get; set; } = string.Empty;
        public double? TotalPrice { get; set; }
        public double? ShippingPrice { get; set; }

        public double? DiscountPrice { get; set; }
        public double? TotalPriceAfterDiscount { get; set; }
        public string ShippingProviderName { get; set; } = string.Empty;
        public string ShippingName { get; set; } = string.Empty;
        public double? TotalWithShippingPrice { get; set; }
        public double? Deposit { get; set; }
        public int Status { get; set; }
        public int SystemStatus { get; set; }
        public string Note { get; set; } = string.Empty;
        public string StockNote { get; set; } = string.Empty;

        public CustomerDto Customer { get; set; } = new();
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}