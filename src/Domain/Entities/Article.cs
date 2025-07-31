using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Article")]
    public class Article : BaseAuditAndSeoInfo

    {
        public int Id { get; set; }
        public int? MenuId { get; set; }

        [MaxLength(256)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Alias { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Image { get; set; } = string.Empty;

        public int? Index { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }

        public Menu? Menu { get; set; }
    }
}