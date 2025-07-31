using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Gallery")]
    public class Gallery
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Image { get; set; } = string.Empty;

        public int Type { get; set; }

        public string Metadata { get; set; } = string.Empty;
    }
}