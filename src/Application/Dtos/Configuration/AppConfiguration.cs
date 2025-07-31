namespace Application.DTOs.Configuration
{
    public class AppConfiguration
    {
        public string Environment { get; set; } = string.Empty;
        public string ServerRootAddress { get; set; } = string.Empty;
        public string ClientRootAddress { get; set; } = string.Empty;
        public bool UseHttps { get; set; }
        public string CorsOrigins { get; set; } = string.Empty;
    }
}