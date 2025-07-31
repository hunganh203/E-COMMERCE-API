using Application.DTOs.Authorization;
using System.Security.Claims;

namespace Application.Interfaces.Authentication
{
    public interface IJwtAuthenticationManager
    {
        Task<AuthenticateResultModel> Authenticate(string username, string password, bool isUser = true);

        Task<AuthenticateResultModel> Authenticate(int userId, Claim[] claims, bool isUser = true);

        Task<bool> ValidToken(int userId, string refreshToken, bool isUser = true);
    }
}