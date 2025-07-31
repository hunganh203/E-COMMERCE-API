namespace Application.Dtos.Base
{
    public class BaseAuditInfo
    {
        public int? CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
    }
}