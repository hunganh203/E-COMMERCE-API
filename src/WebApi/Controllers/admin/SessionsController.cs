using System.Security.Authentication;
using Application.Dtos.User;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    /// <summary>
    /// User
    /// </summary>
    ///
    public class SessionsController : BaseAuthAdminApiController
    {
        public SessionsController(IUserService userService) : base(userService)
        {
        }

        /// <summary>
        /// Get user info
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-account-info")]
        public async Task<AjaxResponse<UserDto>> GetAccountInfo()
        {
            if (!UserId.HasValue)
            {
                throw new AuthenticationException("You are not authorized");
            }

            try
            {
                var user = await UserService.GetUserByIdAsync(UserId.Value);
                return new AjaxResponse<UserDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<UserDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-profile")]
        public async Task<AjaxResponse<UserDto>> UpdateProfile(UserDto input)
        {
            if (!UserId.HasValue)
            {
                throw new AuthenticationException("You are not authorized");
            }
            try
            {
                var user = await UserService.UpdateFull(UserId.Value, input);
                return new AjaxResponse<UserDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<UserDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update-password")]
        public async Task<AjaxResponse<UserDto>> UpdatePassword(UpdateUserPasswordInput input)
        {
            if (!UserId.HasValue)
            {
                throw new AuthenticationException("You are not authorized");
            }
            try
            {
                var user = await UserService.UpdatePassword(UserId.Value, input);
                return new AjaxResponse<UserDto>(user);
            }
            catch (Exception e)
            {
                return new AjaxResponse<UserDto>(new ErrorInfo(e.Message));
            }
        }
    }
}