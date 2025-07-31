using Application.Dtos.User;
using Application.Dtos.UserRole;
using Application.DTOs.Authorization.Accounts;
using Application.DTOs.Pagination;
using Application.DTOs.Verification;

namespace Application.Interfaces.Service
{
    public interface IUserService
    {
        Task<UserDto> GetLoginResultAsync(string username, string password);

        Task<UserDto> GetUserByIdAsync(int userId);

        Task<PagedResultDto<UserDto>> GetAll(GetUserInput input);

        Task<CheckingItemExistModel> CheckEmailExisted(string email);

        Task<SendVerificationEmailOutputModel> ForgotPassword(SendPasswordResetCodeInput input);

        Task<ResetPasswordOutput> ValidResetPasswordCode(ValidateResetPasswordCodeInput input);

        Task<string> ResetPassword(ResetPasswordInput input);

        Task<UserDto> Insert(UserDto entity);

        Task<UserDto> Update(int key, UserDto entity);

        Task<UserDto> UpdatePassword(int key, UpdateUserPasswordInput input);

        Task<UserDto> UpdateFull(int key, UserDto entity);

        Task DeleteById(int key);

        Task UpdateUserRoles(UserRolesDtoInput input);
    }
}