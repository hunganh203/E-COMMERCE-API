using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public Guid ProductId { get; set; }

        [MaxLength(256)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(256)]
        public string ProductImage { get; set; } = string.Empty;

        [MaxLength(256)]
        public string StockReceivingName { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public bool IsUsed { get; set; } = true;

        public int? RefOrderDetailId { get; set; }

        [MaxLength(256)]
        public string CheckingCode { get; set; } = string.Empty;

        public double? ProductPrice { get; set; }

        public double? PriceStockReceiving { get; set; }

        public double? PriceShipping { get; set; }

        public int Status { get; set; }

        public double? ProductDiscountPrice { get; set; }

        public int? Quantity { get; set; }

        public string Attribute { get; set; } = string.Empty;

        public Order? Order { get; set; }

        public Product? Product { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}