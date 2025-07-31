using Application.DTOs.Authorization;
using Application.Interfaces.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Authentication
{
    public class TokenRefresher : ITokenRefresher
    {
        private readonly byte[] _key;

        private readonly IJwtAuthenticationManager _jWtAuthenticationManager;

        public TokenRefresher(byte[] key, IJwtAuthenticationManager jWtAuthenticationManager)
        {
            _key = key;
            _jWtAuthenticationManager = jWtAuthenticationManager;
        }

        public async Task<AuthenticateResultModel> Refresh(RefreshTokenModel refreshInput, bool isUser = true)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(refreshInput.JwtToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
                    }, out var validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;

                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token passed!");
                }

                var nameIdentifier = principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(nameIdentifier, out var userId))
                {
                    var isValidToken = await _jWtAuthenticationManager.ValidToken(userId, refreshInput.RefreshToken, false);

                    if (!isValidToken)
                    {
                        throw new SecurityTokenException("Invalid token passed!");
                    }
                }
                else
                {
                    throw new SecurityTokenException("Invalid token passed!");
                }

                return await _jWtAuthenticationManager.Authenticate(userId, principal!.Claims.ToArray(), isUser);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}