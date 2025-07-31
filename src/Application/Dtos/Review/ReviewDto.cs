using Application.Dtos.Base;
using Application.Dtos.Product;

namespace Application.Dtos.Review
{
    public class ReviewDto : BaseAuditInfo
    {
        public int Id { get; set; }
        public int OrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public int? Star { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Status { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string CreatedByAvatarUrl { get; set; } = string.Empty;

        public ProductDto Product { get; set; } = new();
    }

    public class CustomerReviewInput
    {
        public int OrderDetailId { get; set; }
        public int Star { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}