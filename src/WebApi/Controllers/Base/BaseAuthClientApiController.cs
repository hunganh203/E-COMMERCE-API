using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Security.Claims;

namespace WebApi.Controllers.Base
{
    [Authorize]
    [OpenApiIgnore]
    [Route("api/client/[controller]")]
    [ApiController]
    public class BaseAuthClientApiController : ControllerBase

    {
        protected readonly ICustomerService CustomerService;
        //private readonly IConfiguration _configuration;

        /// <summary>
        ///
        /// </summary>
        /// <param name="customerService"></param>
        protected BaseAuthClientApiController(ICustomerService customerService)
        {
            CustomerService = customerService;

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