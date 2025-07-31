using Amazon.S3;
using Amazon.S3.Model;
using Application.Enums;

namespace Application.DTOs.Cloud.AWS
{
    public class AwsBaseInput
    {
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public S3CannedACL CannedAcl { get; set; } = string.Empty;
        public BucketType BucketType { get; set; }
    }

    public class AwsInput : AwsBaseInput
    {
        public List<Tag> TagSet { get; set; } = new();
        public bool IsExpires { get; set; }
        public bool IsThumbs { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}