using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Domain.Entities
{
    [Table("Product")]
    public class Product : BaseAuditAndSeoInfo
    {
        public Guid Id { get; set; }

        [MaxLength(12)]
        public string Code { get; set; } = string.Empty;

        public int? MenuId { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Alias { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Image { get; set; } = string.Empty;

        public int? Index { get; set; }
        public int? Status { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public double? RateAvg { get; set; }

        public double? DiscountPrice { get; set; }
        public bool? Selling { get; set; }
        public bool? ShowPrice { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Menu Menu { get; set; }

        public ICollection<Attribute> Attributes { get; set; }
        public ICollection<ProductAttribute> ProductAttributes { get; set; }

        public List<ProductImage> ProductImages { get; set; }

        public List<ProductRelated> ProductRelateds { get; set; }
        public List<Review> Reviews { get; set; }
    }
}