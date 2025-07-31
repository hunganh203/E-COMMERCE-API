using Application.Dtos.Product;

namespace Application.Dtos
{
    public class AttributeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ProductAttributeDto> ProductAttributes { get; set; } = new();
    }
}