namespace Domain.Entities
{
    public class UserDeviceToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DeviceToken { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
        public int DeviceGmt { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}