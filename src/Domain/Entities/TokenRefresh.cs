using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class TokenRefresh
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }

        [MaxLength(256)]
        public string Token { get; set; } = string.Empty;

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
    }
}