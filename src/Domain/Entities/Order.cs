using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Order")]
    public class Order : BaseAuditInfo
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(12)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(50)]
        public string SourceFrom { get; set; } = string.Empty;

        public int SystemStatus { get; set; }

        [MaxLength(50)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CustomerEmail { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Address { get; set; } = string.Empty;

        public double? TotalPrice { get; set; }
        public double? ShippingPrice { get; set; }
        public double? DiscountPrice { get; set; }
        public double? TotalPriceAfterDiscount { get; set; }
        public double? TotalWithShippingPrice { get; set; }
        public double? Deposit { get; set; }
        public int Status { get; set; }
        public bool IsCancel { get; set; }
        public bool IsFinish { get; set; }
        public string Note { get; set; } = string.Empty;
        public string StockNote { get; set; } = string.Empty;
        public  Customer? Customer { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}