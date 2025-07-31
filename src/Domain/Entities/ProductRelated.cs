using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("ProductRelated")]
    public class ProductRelated
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductRelatedId { get; set; }

        public Product? Product { get; set; }
    }
}