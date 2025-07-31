using Application.Enums;
using Microsoft.Extensions.Configuration;

namespace Application.Utility.AWS
{
    public class S3Path
    {
        public static string GetS3Url(IConfiguration configuration, string filePath, BucketType bucketType = BucketType.Product, bool isThumbnail = false)
        {
            if (configuration == null)
            {
                throw new NullReferenceException();
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            switch (bucketType)
            {
                case BucketType.Product:
                    return $"{configuration["AWSConfig:Uri:Product"]}{filePath}";

                case BucketType.SourceDefault:
                    return $"{configuration["AWSConfig:Uri:SourceDefault"]}{filePath}";

                case BucketType.UserAvatar:
                    return $"{configuration["AWSConfig:Uri:UserAvatar"]}{filePath}";

                default:
                    return "";
            }
        }
    }
}