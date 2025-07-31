using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ProductAttribute")]
    public class ProductAttribute
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int AttributeId { get; set; }

        public Attribute? Attribute { get; set; }
    }
}