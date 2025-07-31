namespace Application.Dtos.UserNotification
{
    public class UpdateNotificationDto
    {
        public bool? SelectAll { get; set; }
        public int NotificationId { get; set; }
        public bool Status { get; set; }
    }
}