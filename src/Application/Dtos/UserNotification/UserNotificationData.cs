namespace Application.DTOs.UserNotification
{
    public class UserNotificationData
    {
        public int OrderId { get; set; }
        public int AssigneesId { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}