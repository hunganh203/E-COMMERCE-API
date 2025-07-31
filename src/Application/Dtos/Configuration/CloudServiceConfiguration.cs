namespace Application.DTOs.Configuration
{
    public class CloudServiceConfiguration
    {
        public AppFireBase AppFireBase { get; set; } = new();
    }

    public class AppFireBase
    {
        public string LocalUri { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}