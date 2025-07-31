using Application.Dtos.User;
using Application.DTOs.UserDeviceToken;
using Application.Interfaces.Repositories.EFCore;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Shared.Services
{
    public class UserDeviceTokenService : IUserDeviceTokenService
    {
        private readonly IUserDeviceTokenRepository _deviceTokenRepository;
        private readonly IMapper _mapper;

        public UserDeviceTokenService(IUserDeviceTokenRepository deviceTokenRepository, IMapper mapper)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _mapper = mapper;
        }

        public async Task RemoveDeviceService(List<string> tokens)
        {
            try
            {
                var deviceTokens = await _deviceTokenRepository.AsQueryable()
                    .Where(x => tokens.Any(t => t.Equals(x.DeviceToken))).ToListAsync();

                if (deviceTokens.Count > 0)
                {
                    await _deviceTokenRepository.DeleteRangeAsync(deviceTokens);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw;
            }
        }

        public async Task<UserDeviceToken> RegisterDevice(UserDto user, UserDeviceTokenInput userDeviceTokenInput)
        {
            var existUserDeviceToken = await _deviceTokenRepository
                .FirstOrDefaultAsync(x =>
                    x.UserId == user.Id &&
                    x.DeviceToken.Equals(userDeviceTokenInput.DeviceToken));

            if (existUserDeviceToken == null)
            {
                var userDeviceToken = _mapper.Map<UserDeviceToken>(userDeviceTokenInput);

                userDeviceToken.UserId = user.Id;
                userDeviceToken.DeviceGmt = 7;

                userDeviceToken.CreatedDate = DateTime.UtcNow;
                userDeviceToken.UpdateDate = DateTime.UtcNow;
                var result = await _deviceTokenRepository.AddAsync(userDeviceToken);

                return result;
            }

            existUserDeviceToken.UpdateDate = DateTime.UtcNow;
            await _deviceTokenRepository.UpdateAsync(existUserDeviceToken);
            return existUserDeviceToken;
        }

        public async Task RemoveDevice(int userId, RemoveUserDeviceTokenInput removeUserDeviceTokenInput)
        {
            // Todo: Use this case because has token of old user
            var existUserDeviceToken = await _deviceTokenRepository
                    .FirstOrDefaultAsync(x =>
                        x.UserId == userId &&
                        x.DeviceToken.Equals(removeUserDeviceTokenInput.DeviceToken));

            if (existUserDeviceToken == null)
            {
                throw new Exception("Device token does not exist");
            }

            await _deviceTokenRepository.DeleteAsync(existUserDeviceToken);
        }
    }
}