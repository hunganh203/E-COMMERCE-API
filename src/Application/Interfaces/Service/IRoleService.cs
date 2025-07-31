using Application.Dtos.Role;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IRoleService
    {
        Task DeleteById(int key);

        Task<PagedResultDto<RoleDto>> GetAll(GetRoleInput input);

        Task<RoleDto> GetById(int key);

        Task<RoleDto> Insert(RoleDto entity);

        Task Update(int key, RoleDto entity);
    }
}