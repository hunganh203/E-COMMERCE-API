namespace Application.Dtos
{
    public class GalleryDto
    {
        public int Id { get; set; }

        public string Image { get; set; } = string.Empty;
        public int Type { get; set; }

        public string Metadata { get; set; } = string.Empty;
    }
}