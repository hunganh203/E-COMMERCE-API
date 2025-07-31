namespace Application.DTOs.Authorization
{
    public class RefreshTokenModel
    {
        public string JwtToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}