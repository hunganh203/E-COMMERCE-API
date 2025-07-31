namespace Application.DTOs.UserDeviceToken
{
    public class RemoveUserDeviceTokenInput
    {
        public string DeviceToken { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}