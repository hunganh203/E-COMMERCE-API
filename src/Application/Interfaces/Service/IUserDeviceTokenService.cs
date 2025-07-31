using Application.Dtos.User;
using Application.DTOs.UserDeviceToken;
using Domain.Entities;

namespace Application.Interfaces.Service
{
    public interface IUserDeviceTokenService
    {
        Task<UserDeviceToken> RegisterDevice(UserDto user, UserDeviceTokenInput userDeviceTokenInput);

        Task RemoveDevice(int userId, RemoveUserDeviceTokenInput removeUserDeviceTokenInput);

        Task RemoveDeviceService(List<string> tokens);
    }
}