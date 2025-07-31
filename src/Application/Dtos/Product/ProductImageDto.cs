namespace Application.Dtos.Product
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Image { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}