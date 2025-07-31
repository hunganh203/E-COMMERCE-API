using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers.Base
{
    /// <summary>
    /// This is base controller
    /// </summary>
    [Authorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public abstract class BaseAuthAdminApiController : ControllerBase
    {
        protected readonly IUserService UserService;
        //private readonly IConfiguration _configuration;

        /// <summary>
        ///
        /// </summary>
        /// <param name="userService"></param>
        protected BaseAuthAdminApiController(IUserService userService)
        {
            UserService = userService;

            var user = HttpContext?.User;
        }

        /// <summary>
        /// Get Id user
        /// </summary>
        public int? UserId
            => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                ? userId
                : (int?)null;
    }
}