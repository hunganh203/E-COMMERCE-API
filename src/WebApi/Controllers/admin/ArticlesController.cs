using Application.Constants;
using Application.Dtos.Article;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    [Authorize(Roles = $"{UserRoleKey.RoleConst.Admin},{UserRoleKey.RoleConst.Sale}")]
    public class ArticlesController : BaseAuthAdminApiController
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IUserService userService, IArticleService articleService) : base(userService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<AjaxResponse<PagedResultDto<ArticleDto>>> Get([FromQuery] GetArticleInput input)
        {
            try
            {
                var articles = await this._articleService.GetAll(input);
                return new AjaxResponse<PagedResultDto<ArticleDto>>(articles);
            }
            catch (Exception e)
            {
                return new AjaxResponse<PagedResultDto<ArticleDto>>(new ErrorInfo(e.Message));
            }
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<AjaxResponse<ArticleDto>> GetById(int id)
        {
            try
            {
                var website = await this._articleService.GetById(id);
                return new AjaxResponse<ArticleDto>(website);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ArticleDto>(new ErrorInfo(e.Message));
            }
        }

        [Route("get-by-alias/{alias}")]
        [HttpGet]
        public async Task<AjaxResponse<ArticleDto>> GetByAlias(string alias)
        {
            try
            {
                var website = await this._articleService.GetByAlias(alias);
                return new AjaxResponse<ArticleDto>(website);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ArticleDto>(new ErrorInfo(e.Message));
            }
        }

        [Route("get-highlight")]
        [HttpGet]
        public async Task<AjaxResponse<List<ArticleDto>>> GetHighlight()
        {
            try
            {
                var website = await this._articleService.GetHighlight();
                return new AjaxResponse<List<ArticleDto>>(website);
            }
            catch (Exception e)
            {
                return new AjaxResponse<List<ArticleDto>>(new ErrorInfo(e.Message));
            }
        }

        [HttpPost]
        public async Task<AjaxResponse<ArticleDto>> Post(ArticleDto article)
        {
            try
            {
                await this._articleService.Insert(article, UserId.GetValueOrDefault());
                return new AjaxResponse<ArticleDto>(true);
            }
            catch (Exception e)
            {
                return new AjaxResponse<ArticleDto>(new ErrorInfo(e.Message));
            }
        }

        [Route("{id}")]
        [HttpPost]
        public async Task<AjaxResponse> Put(int id, ArticleDto article)
        {
            try
            {
                await this._articleService.Update(id, article, UserId.GetValueOrDefault());
                return new AjaxResponse(true);
            }
            catch (Exception e)
            {
                return new AjaxResponse(new ErrorInfo(e.Message));
            }
        }

        [Route("delete/{id}")]
        [HttpPost]
        public async Task<AjaxResponse> Delete(int id)
        {
            try
            {
                await _articleService.DeleteById(id);
                return new AjaxResponse(true);
            }
            catch (Exception e)
            {
                return new AjaxResponse(new ErrorInfo(e.Message));
            }
        }
    }
}