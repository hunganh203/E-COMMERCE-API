using Application.Dtos.Product;

namespace Application.Dtos.ProductDisplay
{
    public class ProductDisplayDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Type { get; set; }
        public string Metadata { get; set; } = string.Empty;

        public ProductDto Product { get; set; } = new();
    }
}