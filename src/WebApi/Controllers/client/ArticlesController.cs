using Application.Dtos.Article;
using Application.DTOs.Pagination;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.client
{
    [Route("api/articles/")]
    public class ArticlesController : BaseNonAuthClientApiController
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IUserService userService, IArticleService articleService)
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
    }
}