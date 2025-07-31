using Application.Dtos.UserClaim;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IUserClaimService
    {
        Task DeleteById(int key);

        Task<PagedResultDto<UserClaimDto>> GetAll(GetUserClaimInput input);

        Task<UserClaimDto> GetById(int key);

        Task<UserClaimDto> Insert(UserClaimDto entity);

        Task Update(int key, UserClaimDto entity);
    }
}