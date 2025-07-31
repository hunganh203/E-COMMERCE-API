using Application.Constants;
using Application.Dtos;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale}")]
    public class AttributesController : BaseAuthAdminApiController
    {
        private readonly IAttributeService _attributeService;

        public AttributesController(IUserService userService, IAttributeService attributeService) : base(userService)
        {
            _attributeService = attributeService;
        }

        [HttpGet("get-list")]
        public async Task<AjaxResponse<List<AttributeDto>>> GetList(string? keySearch)
        {
            try
            {
                var attributes = await _attributeService.Get(keySearch);
                return new AjaxResponse<List<AttributeDto>>(attributes);
            }
            catch (Exception e)
            {
                return new AjaxResponse<List<AttributeDto>>(new ErrorInfo(e.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<AjaxResponse<AttributeDto>> GetById(int id)
        {
            try
            {
                var attribute = await _attributeService.GetById(id);
                return new AjaxResponse<AttributeDto>(attribute);
            }
            catch (Exception e)
            {
                return new AjaxResponse<AttributeDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("add")]
        public async Task<AjaxResponse<AttributeDto>> Add(AttributeDto attributeInput)
        {
            try
            {
                var attribute = await _attributeService.Insert(attributeInput);
                return new AjaxResponse<AttributeDto>(attribute);
            }
            catch (Exception e)
            {
                return new AjaxResponse<AttributeDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("update")]
        public async Task<AjaxResponse<AttributeDto>> Update(int id, AttributeDto attributeInput)
        {
            try
            {
                await _attributeService.Update(id, attributeInput);
                return new AjaxResponse<AttributeDto>(attributeInput);
            }
            catch (Exception e)
            {
                return new AjaxResponse<AttributeDto>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("delete")]
        public async Task<AjaxResponse<AttributeDto>> Delete(int id)
        {
            try
            {
                await _attributeService.DeleteById(id);
                return new AjaxResponse<AttributeDto>();
            }
            catch (Exception e)
            {
                return new AjaxResponse<AttributeDto>(new ErrorInfo(e.Message));
            }
        }
    }
}