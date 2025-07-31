namespace Domain.Entities.Base
{
    public class BaseAuditAndSeoInfo
    {
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}