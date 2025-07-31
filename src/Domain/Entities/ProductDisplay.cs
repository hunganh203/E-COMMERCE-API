using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ProductDisplay")]
    public class ProductDisplay
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Type { get; set; }
        public string Metadata { get; set; } = string.Empty;
        public Product? Product { get; set; } 
    }
}