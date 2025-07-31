using Application.Dtos.UserRole;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IUserRoleService
    {
        Task DeleteById(int userId, int roleId);

        Task<PagedResultDto<UserRoleDto>> GetAll(GetUserRoleInput input);

        Task<UserRoleDto> GetById(int userId, int roleId);

        Task<UserRoleDto> Insert(UserRoleDto entity);

        Task Update(int userId, int roleId, UserRoleDto entity);
    }
}