namespace Application.Dtos.UserClaim
{
    public class UserClaimDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ClaimType { get; set; } = string.Empty;

        public string ClaimValue { get; set; } = string.Empty;

        public Domain.Entities.User User { get; set; } = new();
    }
}