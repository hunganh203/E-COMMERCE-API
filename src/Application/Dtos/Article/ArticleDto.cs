using Application.Dtos.Base;
using Application.Dtos.Menu;

namespace Application.Dtos.Article
{
    public class ArticleDto : BaseAuditAndSeoInfo
    {
        public int Id { get; set; }
        public int? MenuId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int? Index { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTimeOffset Created { get; set; }

        public MenuDto Menu { get; set; } = new();
    }
}