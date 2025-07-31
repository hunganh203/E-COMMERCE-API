namespace Application.Dtos.Product
{
    public class ProductRelatedDto
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductRelatedId { get; set; }

        public ProductDto ProductRelated { get; set; } = new();
    }
}