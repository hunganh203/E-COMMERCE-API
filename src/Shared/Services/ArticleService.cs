using Application.Constants;
using Application.Dtos.Article;
using Application.Dtos.Menu;
using Application.DTOs.Pagination;
using Application.Interfaces.Repositories.EfCore;
using Application.Interfaces.Service;
using Application.Utility.AWS;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMenuRepository _menuRepository;

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository articleRepository, IMenuRepository menuRepository, IMapper mapper, IConfiguration configuration)
        {
            _articleRepository = articleRepository;
            _menuRepository = menuRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task DeleteById(int key)
        {
            var entity = await _articleRepository.FirstOrDefaultAsync(x => x.Id == key);
            if (entity == null)
                throw new Exception(CustomResponseMessage.ArticleDoesNotExist);
            await _articleRepository.DeleteAsync(entity);
        }

        /// <summary>
        /// Get tất cả bài viết
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<ArticleDto>> GetAll(GetArticleInput input)
        {
            var productsQuery = _articleRepository
                .AsQueryable()
                .OrderBy(x => x.Index)
                .Where(x => !input.CategoryId.HasValue || x.MenuId == input.CategoryId)

                .Where(x => string.IsNullOrEmpty(input.KeySearch) || (x.Title.Contains(input.KeySearch.Trim())));

            var articles = await productsQuery.Skip(input.SkipCount)
               .Take(input.MaxResultCount)
               .Include(x => x.Menu)
               .ToListAsync();

            if (articles is not { Count: > 0 })
            {
                var items = _mapper.Map<List<ArticleDto>>(articles);
                return new PagedResultDto<ArticleDto>
                {
                    Items = items,
                    TotalCount = items.Count
                };
            }

            var articleDtos = _mapper.Map<List<ArticleDto>>(articles);

            foreach (var product in articleDtos)
            {
                product.ImageUrl = !string.IsNullOrEmpty(product.Image)
                    ? S3Path.GetS3Url(_configuration, product.Image)
                    : "";
            }
            var articlesResult = _mapper.Map<List<ArticleDto>>(articleDtos);

            return new PagedResultDto<ArticleDto>
            {
                Items = articlesResult,
                TotalCount = string.IsNullOrEmpty(input.KeySearch) && !input.CategoryId.HasValue
                    ? _articleRepository.Count()
                    : _articleRepository
                        .Count(x => (!input.CategoryId.HasValue || x.MenuId == input.CategoryId)
                            && (string.IsNullOrEmpty(input.KeySearch) || x.Title.Contains(input.KeySearch.Trim()))
                           )
            };
        }

        /// <summary>
        /// Get các bài viết nỗi bật để hiển thị trên trang chủ
        /// </summary>
        /// <returns></returns>
        public async Task<List<ArticleDto>> GetHighlight()
        {
            return (await _articleRepository.GetAllAsync())
                .Where(x => x.Active)
                .Where(x => x.Menu?.Type == "bai-viet")
                .OrderBy(x => x.Index)
                .Select(x => new ArticleDto()
                {
                    Title = x.Title,
                    Alias = x.Alias,
                    Image = x.Image
                })
                .Take(5)
                .ToList();
        }

        public async Task<ArticleDto> GetById(int key)
        {
            var entity = await _articleRepository
                .AsQueryable().Include(x=>x.Menu)
                .FirstOrDefaultAsync(x => x.Id == key);
            if (entity == null)
                throw new Exception(CustomResponseMessage.ArticleDoesNotExist);
            var articleDto = _mapper.Map<ArticleDto>(entity);
            articleDto.ImageUrl = !string.IsNullOrEmpty(articleDto.Image)
                  ? S3Path.GetS3Url(_configuration, articleDto.Image)
                  : "";
            return articleDto;
        }

        public async Task<ArticleDto> GetByAlias(string alias)
        {
            return (await _articleRepository.GetAllAsync())
               .Where(x => x.Alias == alias)
               .Select(x => new ArticleDto()
               {
                   Menu = new MenuDto()
                   {
                       Alias = x.Menu?.Alias ?? ""
                   },
                   Title = x.Title,
                   Alias = x.Alias,
                   ShortDescription = x.ShortDescription,
                   Description = x.Description,
                   Image = x.Image,
                   Created = x.CreatedDate
               })
               .FirstOrDefault() ?? new ArticleDto();
        }

        public async Task<ArticleDto> Insert(ArticleDto entity, int userId)
        {
            var article = new Article()
            {
                Active = entity.Active,
                Alias = entity.Alias,
                Description = entity.Description,
                Image = entity.Image,
                Index = entity.Index,
                MenuId = entity.MenuId == 0 ? null : entity.MenuId,
                ShortDescription = entity.ShortDescription,
                Title = entity.Title,

                MetaTitle = entity.MetaTitle,
                MetaDescription = entity.MetaDescription,

                CreatedBy = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedBy = userId,
                UpdatedDate = DateTimeOffset.UtcNow,
            };

            await _articleRepository.AddAsync(article);

            return entity;
        }

        public async Task Update(int key, ArticleDto entity, int userId)
        {
            var article = await _articleRepository
                .FirstOrDefaultAsync(x => x.Id == key);

            article.Title = entity.Title;

            if (article.Alias != entity.Alias)
                article.Alias = entity.Alias;

            article.Active = entity.Active;
            article.ShortDescription = entity.ShortDescription;
            article.Description = entity.Description;
            article.Image = entity.Image;
            article.Index = entity.Index;
            article.MenuId = entity.MenuId == 0 ? null : entity.MenuId;

            article.MetaTitle = entity.MetaTitle;
            article.MetaDescription = entity.MetaDescription;

            article.UpdatedBy = userId;
            article.UpdatedDate = DateTimeOffset.Now;

            await _articleRepository.UpdateAsync(article);
        }
    }
}