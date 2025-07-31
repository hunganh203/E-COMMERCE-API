using Application.Constants;
using Application.Dtos;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin}")]
    public class WebsitesController : BaseAuthAdminApiController
    {
        private readonly IWebsiteService _websiteService;

        public WebsitesController(IUserService userService, IWebsiteService websiteService)
            : base(userService)
        {
            _websiteService = websiteService;
        }

        [HttpGet]
        public async Task<AjaxResponse<WebsiteDto>> Get()
        {
            try
            {
                var website = (await _websiteService.GetAllAsync()).FirstOrDefault() ?? new WebsiteDto();
                return new AjaxResponse<WebsiteDto>(website);
            }
            catch (Exception e)
            {
                return new AjaxResponse<WebsiteDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost]
        public async Task<AjaxResponse<WebsiteDto>> Update(WebsiteDto website)
        {
            try
            {
                await _websiteService.UpdateAsync(website);
                return new AjaxResponse<WebsiteDto>(true);
            }
            catch (Exception ex)
            {
                return new AjaxResponse<WebsiteDto>(new ErrorInfo(ex.Message));
            }
        }
    }
}