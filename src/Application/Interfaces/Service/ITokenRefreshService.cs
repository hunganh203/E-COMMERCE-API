namespace Application.Interfaces.Service
{
    public interface ITokenRefreshService
    {
        Task<bool> ValidToken(int userId, string token, bool isUser = true);

        Task AddOrUpdateToken(int userId, string token, bool isUser = true);
    }
}