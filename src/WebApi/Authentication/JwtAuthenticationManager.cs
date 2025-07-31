using Application.Constants;
using Application.Dtos.Customer;
using Application.Dtos.User;
using Application.DTOs.Authorization;
using Application.Interfaces.Authentication;
using Application.Interfaces.Service;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Authentication
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly ITokenRefreshService _tokenRefreshService;

        private readonly string _tokenKey;

        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IConfiguration _configuration;

        public JwtAuthenticationManager(string tokenKey,
            IRefreshTokenGenerator refreshTokenGenerator,
            IUserService userService, ITokenRefreshService tokenRefreshService,
            IConfiguration configuration,
            ICustomerService customerService)
        {
            _tokenKey = tokenKey;
            _refreshTokenGenerator = refreshTokenGenerator;
            _userService = userService;
            _tokenRefreshService = tokenRefreshService;
            _configuration = configuration;
            _customerService = customerService;
        }

        public async Task<AuthenticateResultModel> Authenticate(string username, string password, bool isUser = true)
        {
            if (isUser)
            {
                var userDto = await _userService.GetLoginResultAsync(username, password);
                if (userDto == null)
                {
                    throw new Exception("Invalid username or password");
                }

                var token = GenerateTokenString(userDto, DateTime.Now);
                var refreshToken = _refreshTokenGenerator.GenerateToken();
                await _tokenRefreshService.AddOrUpdateToken(userDto.Id, refreshToken);

                return new AuthenticateResultModel
                {
                    JwtToken = token,
                    RefreshToken = refreshToken
                };
            }
            else
            {
                var customer = await _customerService.GetLoginResultAsync(username, password);
                if (customer == null)
                {
                    throw new Exception("Invalid username or password");
                }

                var token = GenerateTokenStringForCustomer(customer, DateTime.Now);
                var refreshToken = _refreshTokenGenerator.GenerateToken();
                await _tokenRefreshService.AddOrUpdateToken(customer.Id, refreshToken, false);

                return new AuthenticateResultModel
                {
                    JwtToken = token,
                    RefreshToken = refreshToken
                };
            }
        }

        public async Task<AuthenticateResultModel> Authenticate(int userId, Claim[] claims, bool isUser = true)
        {
            if (isUser)
            {
                var userDto = await _userService.GetUserByIdAsync(userId);

                var token = GenerateTokenString(userDto, DateTime.Now);
                var refreshToken = _refreshTokenGenerator.GenerateToken();
                await _tokenRefreshService.AddOrUpdateToken(userDto.Id, refreshToken);

                return new AuthenticateResultModel
                {
                    JwtToken = token,
                    RefreshToken = refreshToken
                };
            }
            else
            {
                var customer = await _customerService.GetById(userId);

                var token = GenerateTokenStringForCustomer(customer, DateTime.Now);
                var refreshToken = _refreshTokenGenerator.GenerateToken();
                await _tokenRefreshService.AddOrUpdateToken(customer.Id, refreshToken, false);

                return new AuthenticateResultModel
                {
                    JwtToken = token,
                    RefreshToken = refreshToken
                };
            }
        }

        public async Task<bool> ValidToken(int userId, string refreshToken, bool isUser = true)
        {
            return await _tokenRefreshService.ValidToken(userId, refreshToken, isUser);
        }

        private string GenerateTokenString(UserDto user, DateTime expires, Claim[]? claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);

            var timeExpiredRefreshToken = _configuration["Authentication:Token:Expired"];

            if (!int.TryParse(timeExpiredRefreshToken, out var time))
            {
                time = 60;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                 claims ?? new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.GivenName, user.FullName),
                }),

                //NotBefore = expires,
                Expires = expires.AddDays(time),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            tokenDescriptor.Subject.AddClaims(user.Roles.Select(x => new Claim(ClaimTypes.Role, x.NormalizedName)));
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        private string GenerateTokenStringForCustomer(CustomerDto customer, DateTime expires, Claim[]? claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenKey);

            var timeExpiredRefreshToken = _configuration["Authentication:Token:Expired"];

            if (!int.TryParse(timeExpiredRefreshToken, out var time))
            {
                time = 60;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    claims ?? new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                        new Claim(ClaimTypes.Name, customer.UserName),
                        new Claim(ClaimTypes.GivenName, customer.FullName),
                        new Claim(ClaimTypes.Role, Common.UserTypeCustomer.ToString()),
                    }),
                //NotBefore = expires,
                Expires = expires.AddDays(time),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}