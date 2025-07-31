using Application.DTOs.Authorization;
using Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [Route("api/admin/auth/")]
    public class TokenAuthController : BaseNonAuthAdminApiController
    {
        private readonly IJwtAuthenticationManager _jWtAuthenticationManager;
        private readonly ITokenRefresher _tokenRefresher;

        /// <summary>
        ///
        /// </summary>
        /// <param name="jWtAuthenticationManager"></param>
        /// <param name="tokenRefresher"></param>
        public TokenAuthController(IJwtAuthenticationManager jWtAuthenticationManager, ITokenRefresher tokenRefresher)
        {
            _jWtAuthenticationManager = jWtAuthenticationManager;
            _tokenRefresher = tokenRefresher;
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<AjaxResponse<AuthenticateResultModel>> Authenticate([FromBody] AuthenticateModel model)
        {
            try
            {
                var token = await _jWtAuthenticationManager.Authenticate(model.UserName, model.Password);
                return new AjaxResponse<AuthenticateResultModel>(token);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<AuthenticateResultModel>(new ErrorInfo
                {
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = string.IsNullOrEmpty(ex.Message) ? "Invalid username or password" : ex.Message
                }, true);
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<AjaxResponse> Refresh([FromBody] RefreshTokenModel refresh)
        {
            var token = await _tokenRefresher.Refresh(refresh);

            if (token == null)
                return new AjaxResponse(new ErrorInfo
                {
                    Code = (int)HttpStatusCode.Unauthorized,
                    Message = "Unauthorized"
                }, true);

            return new AjaxResponse(token);
        }
    }
}