using Application.Constants;
using Application.Dtos.Menu;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale}")]
    public class MenusController : BaseAuthAdminApiController
    {
        private readonly IMenuService _menuService;

        public MenusController(IUserService userService, IMenuService menuService) : base(userService)
        {
            _menuService = menuService;
        }

        [HttpGet("get-list")]
        public async Task<AjaxResponse<PagedResultDto<MenuDto>>> GetList([FromQuery] GetMenuInput input)
        {
            var menus = await _menuService.GetAll(input);
            return new AjaxResponse<PagedResultDto<MenuDto>>(menus);
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

        [HttpGet("{id}")]
        public async Task<AjaxResponse<MenuDto>> GetById(int id)
        {
            try
            {
                var menu = await _menuService.GetById(id);
                return new AjaxResponse<MenuDto>(menu);
            }
            catch (Exception e)
            {
                return new AjaxResponse<MenuDto>(new ErrorInfo(e.Message));
            }
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

        [HttpPost("add")]
        public async Task<AjaxResponse<MenuDto>> Add(MenuDto menuInput)
        {
            try
            {
                var menu = await _menuService.Insert(menuInput);
                return new AjaxResponse<MenuDto>(menu);
            }
            catch (Exception e)
            {
                return new AjaxResponse<MenuDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update")]
        public async Task<AjaxResponse<MenuDto>> Update(int id, MenuDto menuInput)
        {
            try
            {
                await _menuService.Update(id, menuInput);
                return new AjaxResponse<MenuDto>(menuInput);
            }
            catch (Exception e)
            {
                return new AjaxResponse<MenuDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<AjaxResponse<MenuDto>> Delete(int id)
        {
            try
            {
                await _menuService.DeleteById(id);
                return new AjaxResponse<MenuDto>();
            }
            catch (Exception e)
            {
                return new AjaxResponse<MenuDto>(new ErrorInfo(e.Message));
            }
        }
    }
}