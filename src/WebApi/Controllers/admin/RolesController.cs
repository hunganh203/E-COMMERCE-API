using Application.Dtos.Role;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    /// <summary>
    /// User role
    /// </summary>
    ///
    public class RolesController : BaseAuthAdminApiController
    {
        private readonly IRoleService _roleService;

        public RolesController(IUserService userService, IRoleService roleService) : base(userService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get user info
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        public async Task<AjaxResponse<PagedResultDto<RoleDto>>> GetAll([FromQuery] GetRoleInput input)
        {
            try
            {
                var roles = await _roleService.GetAll(input);
                return new AjaxResponse<PagedResultDto<RoleDto>>(roles);
            }
            catch (Exception e)
            {
                return new AjaxResponse<PagedResultDto<RoleDto>>(new ErrorInfo(e.Message));
            }
        }
    }
}