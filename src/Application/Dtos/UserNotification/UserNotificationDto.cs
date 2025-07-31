namespace Application.DTOs.UserNotification
{
    public class UserNotificationDto
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
        public string ExecutorFirstName { get; set; } = string.Empty;
        public string ExecutorLastName { get; set; } = string.Empty;
        public string ExecutorAvatar { get; set; } = string.Empty;
        public string ExecutorAvatarUrl { get; set; } = string.Empty;
        public string ExecutorAvatarThumbnail { get; set; } = string.Empty;
    }
}