using Application.Dtos;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{ 
    public class WebsitesController : BaseNonAuthClientApiController
    {
        private readonly IWebsiteService _websiteService;

        public WebsitesController(IWebsiteService websiteService)
        {
            _websiteService = websiteService;
        }

        [HttpGet("get-website-info")]
        public async Task<AjaxResponse<WebsiteDto>> GetWebsiteInfo()
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
    }
}