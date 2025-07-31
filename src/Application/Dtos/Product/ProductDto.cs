using Application.Dtos.Base;
using Application.Dtos.Menu;
using Application.Dtos.Review;

namespace Application.Dtos.Product
{
    public class ProductDto : BaseAuditAndSeoInfo
    {
        public Guid Id { get; set; }
        public int? MenuId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int? Index { get; set; }
        public int? Status { get; set; }
        public double? Price { get; set; }
        public double? DiscountPrice { get; set; }
        public int? Quantity { get; set; }
        public bool? Selling { get; set; }
        public bool? ShowPrice { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public MenuDto Menu { get; set; } = new();
        public List<ProductAttributeDto> ProductAttributes { get; set; } = new();
        public List<ProductImageDto> ProductImages { get; set; } = new();
        public List<ProductRelatedDto> ProductRelateds { get; set; } = new();
        public List<ReviewDto> Reviews { get; set; } = new();

        public List<AttributeDto> Attributes { get; set; } = new();

        public double? TotalQty { get; set; }
        public double? TotalAmount { get; set; }
        public double? RateAvg { get; set; }
    }
}