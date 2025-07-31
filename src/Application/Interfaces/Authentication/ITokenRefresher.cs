using Application.DTOs.Authorization;

namespace Application.Interfaces.Authentication
{
    public interface ITokenRefresher
    {
        Task<AuthenticateResultModel> Refresh(RefreshTokenModel refreshInput, bool isUser = true);
    }
}