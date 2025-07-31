using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Role")]
    public class Role
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string NormalizedName { get; set; } = string.Empty;
    }
}