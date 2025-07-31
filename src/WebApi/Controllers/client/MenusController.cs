using Application.Dtos.Menu;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{
    public class MenusController : BaseNonAuthClientApiController
    {
        private readonly IMenuService _menuService;

        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [Route("get-sub-menu")]
        public async Task<AjaxResponse<List<MenuDto>>> GetSubMenu(string? keySearch)
        {
            var menus = await _menuService.GetSubMenu(keySearch);
            return new AjaxResponse<List<MenuDto>>(menus);
        }

        [Route("get-main-menu")]
        public async Task<AjaxResponse<List<MenuDto>>> GetMainMenu(string? keySearch)
        {
            var menus = await _menuService.GetMainMenu(keySearch);
            return new AjaxResponse<List<MenuDto>>(menus);
        }

        [Route("get-parent-main-menu")]
        public async Task<AjaxResponse<List<MenuDto>>> GetParentMainMenu()
        {
            var menus = await _menuService.GetParentMainMenu();
            return new AjaxResponse<List<MenuDto>>(menus);
        }

        [HttpGet("get-by-type")]
        public async Task<AjaxResponse<List<MenuDto>>> GetByType([FromQuery] List<string> types)
        {
            try
            {
                var menus = await _menuService.GetByTypeForAdmin(types);
                return new AjaxResponse<List<MenuDto>>(menus);
            }
            catch (Exception e)
            {
                return new AjaxResponse<List<MenuDto>>(new ErrorInfo(e.Message));
            }
        }

        
    }
}