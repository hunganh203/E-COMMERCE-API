using Application.Constants;
using Application.Dtos.User;
using Application.Dtos.UserRole;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin}")]
    public class UsersController : BaseAuthAdminApiController
    {
        public UsersController(IUserService userService)
            : base(userService)
        {
        }

        [HttpGet("get-all")]
        public async Task<AjaxResponse<PagedResultDto<UserDto>>> GetAll([FromQuery] GetUserInput input)
        {
            try
            {
                var result = await UserService.GetAll(input);
                return new AjaxResponse<PagedResultDto<UserDto>>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<PagedResultDto<UserDto>>(new ErrorInfo(e.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<AjaxResponse<UserDto>> GetById(int id)
        {
            try
            {
                var user = await UserService.GetUserByIdAsync(id);
                return new AjaxResponse<UserDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<UserDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("add")]
        public async Task<AjaxResponse<UserDto>> Add(UserDto customerInput)
        {
            try
            {
                var user = await UserService.Insert(customerInput);
                return new AjaxResponse<UserDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<UserDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update")]
        public async Task<AjaxResponse<UserDto>> Update(int id, UserDto input)
        {
            try
            {
                var user = await UserService.UpdateFull(id, input);
                return new AjaxResponse<UserDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<UserDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<AjaxResponse<UserDto>> Delete(int id)
        {
            try
            {
                await UserService.DeleteById(id);
                return new AjaxResponse<UserDto>();
            }
            catch (Exception e)
            {
                return new AjaxResponse<UserDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-user-roles")]
        public async Task<AjaxResponse<UserDto>> UpdateUserRoles(UserRolesDtoInput input)
        {
            try
            {
                await UserService.UpdateUserRoles(input);
                return new AjaxResponse<UserDto>();
            }
            catch (Exception e)
            {
                return new AjaxResponse<UserDto>(new ErrorInfo(e.Message));
            }
        }
    }
}