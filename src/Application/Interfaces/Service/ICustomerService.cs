using Application.Dtos.Customer;
using Application.DTOs.Authorization.Accounts;
using Application.DTOs.Pagination;
using Application.DTOs.Verification;

namespace Application.Interfaces.Service
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetLoginResultAsync(string username, string password);

        Task<SendVerificationEmailOutputModel> ForgotPassword(SendPasswordResetCodeInput input);

        Task<ResetPasswordOutput> ValidResetPasswordCode(ValidateResetPasswordCodeInput input);

        Task<string> ResetPassword(ResetPasswordInput input);

        Task<CustomerDto> Register(CustomerDto input);

        Task<CustomerDto> GetById(int id);

        Task<PagedResultDto<CustomerDto>> GetAll(GetCustomerInput input);

        Task<List<CustomerDto>> GetCustomersForSelect(string? keySearch, int pageSize);

        Task<CustomerDto> Insert(CustomerDto entity);

        Task<CustomerDto> Update(int key, CustomerDto entity);

        Task<CustomerDto> UpdatePassword(int key, UpdateCustomerPasswordInput input);

        Task<CustomerDto> UpdateFull(int key, CustomerDto entity);

        Task DeleteById(int key);

        Task<CustomerDto> GetUserByIdAsync(int userId);
    }
}