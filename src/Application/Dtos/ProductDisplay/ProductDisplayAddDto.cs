namespace Application.Dtos.ProductDisplay
{
    public class ProductDisplayAddDto
    {
        public List<Guid> ProductIds { get; set; } = new List<Guid>();
        public int Type { get; set; }
        public string? Metadata { get; set; } = string.Empty;
    }
}