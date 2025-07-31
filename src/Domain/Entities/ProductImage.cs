using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ProductImage")]
    public class ProductImage
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        [MaxLength(256)]
        public string Image { get; set; } = string.Empty;

        public Product? Product { get; set; }
    }
}