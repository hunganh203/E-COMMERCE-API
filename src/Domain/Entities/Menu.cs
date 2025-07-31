using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Domain.Entities
{
    [Table("Menu")]
    public class Menu
    {
        public int Id { get; set; }
        public int? PMenuId { get; set; }

        [MaxLength(256)]
        public string Group { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string Alias { get; set; } = string.Empty;

        public int? Index { get; set; }

        public bool? ShowHomePage { get; set; }

        [MaxLength(256)]
        public string Type { get; set; } = string.Empty;

        public bool Active { get; set; }

        [ForeignKey("PMenuId")]
        public virtual Menu PMenu { get; set; }

        public List<Article> Articles { get; set; }

        public List<Product> Products { get; set; }
    }
}