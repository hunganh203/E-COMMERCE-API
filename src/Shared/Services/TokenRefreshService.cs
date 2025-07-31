using Application.Constants;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class TokenRefreshService : ITokenRefreshService
    {
        private readonly ITokenRefreshRepository _tokenRefreshRepository;
        private readonly IConfiguration _configuration;

        public TokenRefreshService(ITokenRefreshRepository tokenRefreshRepository, IConfiguration configuration)
        {
            _tokenRefreshRepository = tokenRefreshRepository;
            _configuration = configuration;
        }

        public async Task AddOrUpdateToken(int userId, string token, bool isUser = true)
        {
            var utcNow = DateTime.UtcNow;
            var rfTokens = (await
                _tokenRefreshRepository.GetAllAsync()).Where(x => x.ExpiredDate < utcNow && x.UserId == userId && (isUser ? x.Type == Common.UserTypeAdmin : x.Type == Common.UserTypeCustomer));

            foreach (var tokenRefresh in rfTokens)
            {
                await _tokenRefreshRepository.DeleteAsync(tokenRefresh);
            }

            var timeExpiredRefreshToken = _configuration["Authentication:Token:RefreshExpired"];

            if (!int.TryParse(timeExpiredRefreshToken, out var time))
            {
                time = 5;
            }

            await _tokenRefreshRepository.AddAsync(new Domain.Entities.TokenRefresh
            {
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Token = token,
                UserId = userId,
                Type = isUser ? Common.UserTypeAdmin : Common.UserTypeCustomer,
                ExpiredDate = DateTime.UtcNow.AddMinutes(time)
            });
        }

        public async Task<bool> ValidToken(int userId, string token, bool isUser = true)
        {
            var rfToken = await
                _tokenRefreshRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.Token.Equals(token) && (isUser ? x.Type == Common.UserTypeAdmin : x.Type == Common.UserTypeCustomer));
            return !string.IsNullOrEmpty(rfToken.Token);
        }
    }
}