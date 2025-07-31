using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Attribute")]
    public class Attribute
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();

        public List<ProductAttribute> ProductAttributes { get; set; } = new();
    }
}