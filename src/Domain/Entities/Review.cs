using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Review")]
    public class Review : BaseAuditInfo
    {
        public int Id { get; set; }
        public int? OrderDetailId { get; set; }

        public Guid? ProductId { get; set; }
        public int? Star { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Status { get; set; }
        public Product? Product { get; set; }
    }
}