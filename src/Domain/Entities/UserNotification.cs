namespace Domain.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string Data { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int? ExecutorId { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}