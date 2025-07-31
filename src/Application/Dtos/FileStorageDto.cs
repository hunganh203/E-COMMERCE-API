namespace Application.Dtos
{
    public class FileStorageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public decimal Capacity { get; set; }
        public string Extension { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public int? CreatorId { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int LastModifierId { get; set; }
        public string PathUrl { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public string Bucket { get; set; } = string.Empty;
    }
}