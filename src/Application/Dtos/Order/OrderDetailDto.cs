using Application.Dtos.Review;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Order
{
    public class OrderDetailDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductImage { get; set; } = string.Empty;

        public string ProductImageUrl { get; set; } = string.Empty;

        public double? ProductPrice { get; set; }

        public double? ProductDiscountPrice { get; set; }

        public double? PriceShipping { get; set; }

        public int Status { get; set; }

        public string StockReceivingName { get; set; } = string.Empty;

        public double? PriceStockReceiving { get; set; }

        public string Note { get; set; } = string.Empty;

        public bool IsUsed { get; set; } = true;

        public int? RefOrderDetailId { get; set; }

        [MaxLength(256)]
        public string CheckingCode { get; set; } = string.Empty;

        public int? Quantity { get; set; }

        public string Attribute { get; set; } = string.Empty;

        public List<ReviewDto> Reviews { get; set; } = new();
    }
}