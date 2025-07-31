namespace Application.DTOs.Configuration
{
    public class AwsConfiguration
    {
        public string AwsS3AccessKeyId { get; set; } = string.Empty;
        public string AwsS3SecretAccessKey { get; set; } = string.Empty;
        public string AwsS3Region { get; set; } = string.Empty;
        public int AwsS3ExpireDays { get; set; }
        public int AwsS3MaxHoursToResign { get; set; }
        public int IntervalSigningCheck { get; set; }
        public Uri Uri { get; set; } = new();
        public Bucket Bucket { get; set; } = new();
    }

    public class Uri
    {
        public string Product { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public string SourceDefault { get; set; } = string.Empty;
    }

    public class Bucket
    {
        public string Product { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public string SourceDefault { get; set; } = string.Empty;
    }
}