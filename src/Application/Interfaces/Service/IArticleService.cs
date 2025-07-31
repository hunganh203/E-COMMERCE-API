using Application.Dtos.Article;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Service
{
    public interface IArticleService
    {
        Task DeleteById(int key);

        Task<PagedResultDto<ArticleDto>> GetAll(GetArticleInput input);

        Task<List<ArticleDto>> GetHighlight();

        Task<ArticleDto> GetById(int key);

        Task<ArticleDto> GetByAlias(string alias);

        Task<ArticleDto> Insert(ArticleDto entity, int userId);

        Task Update(int key, ArticleDto entity, int userId);
    }
}