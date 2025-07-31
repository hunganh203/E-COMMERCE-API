namespace Application.DTOs.UserDeviceToken
{
    public class UserDeviceTokenInput
    {
        public string DeviceToken { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}