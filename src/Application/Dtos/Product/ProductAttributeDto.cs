namespace Application.Dtos.Product
{
    public class ProductAttributeDto
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; } = string.Empty;

        public bool Checked { get; set; }

        public AttributeDto Attribute { get; set; } = new();
    }
}