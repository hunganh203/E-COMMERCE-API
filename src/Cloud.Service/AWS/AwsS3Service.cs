using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Application.Dtos;
using Application.DTOs.Cloud.AWS;
using Application.DTOs.Configuration;
using Application.Enums;
using Application.Interfaces.CloudService.AWS;
using Application.Utility.AWS;
using Microsoft.AspNetCore.Http;

namespace Cloud.Service.AWS
{
    internal class AwsS3Service : IAwsS3Service
    {
        private readonly AwsConfiguration _awsConfiguration;
        private readonly IAmazonS3 _s3Client;

        public AwsS3Service(AwsConfiguration awsConfiguration)
        {
            _awsConfiguration = awsConfiguration;

            var credentials = new BasicAWSCredentials(_awsConfiguration.AwsS3AccessKeyId, _awsConfiguration.AwsS3SecretAccessKey);
            var fileRegion = RegionEndpoint.GetBySystemName(_awsConfiguration.AwsS3Region);

            _s3Client = new AmazonS3Client(credentials, fileRegion);
        }

        /// <summary>
        /// Upload file to AWS
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="input"></param>
        /// <param name="creatorId"></param>
        public async Task<FileStorageDto> UploadFileAsync(IFormFile fileData, AwsInput input, int creatorId)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_s3Client);
                //handle fileName
                var extension = System.IO.Path.GetExtension(input.FileName);

                var uniqueFileName = AwsS3PathExtensions.GenerateFileName(input.Id);

                var request = CreateRequest(fileData.OpenReadStream(), input, extension, extension);

                // AWS file upload origin file
                await fileTransferUtility.UploadAsync(request);

                var fileStorage = new FileStorageDto
                {
                    Path = $"{uniqueFileName}{extension}",
                    CreationTime = DateTime.Now,
                    CreatorId = creatorId,
                    Extension = extension,
                    Name = fileData.FileName,
                    LastModificationTime = DateTime.Now,
                    LastModifierId = 0,
                    Capacity = fileData.Length,
                    ContentType = fileData.ContentType
                };

                return fileStorage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Upload file to AWS
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="input"></param>
        /// <param name="creatorId"></param>
        /// <param name="folder"></param>
        public async Task<FileStorageDto> UploadFileAsync(byte[] fileData, AwsInput input, int creatorId, string folder)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_s3Client);
                //handle fileName
                var extension = System.IO.Path.GetExtension(input.FileName);

                var uniqueFileName = AwsS3PathExtensions.GenerateFileName(input.Id);

                var stream = new MemoryStream(fileData);

                var request = CreateRequest(stream, input, extension, folder);

                // AWS file upload origin file
                await fileTransferUtility.UploadAsync(request);

                var fileStorage = new FileStorageDto
                {
                    Path = $"{folder}/{uniqueFileName}{extension}",
                    CreationTime = DateTime.Now,
                    CreatorId = creatorId,
                    Extension = extension,
                    Name = input.FileName,
                    LastModificationTime = DateTime.Now,
                    LastModifierId = 0,
                    Capacity = fileData.Length,
                    ContentType = input.ContentType
                };

                return fileStorage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CopyObjectAsync(string srcBucket, string srcKey, string destBucket, string destKey)
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = srcBucket,
                SourceKey = srcKey,
                DestinationBucket = destBucket,
                DestinationKey = destKey,
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.CopyObjectAsync(request);
        }

        public async Task MoveObjectAsync(string srcBucket, string srcKey, string destBucket, string destKey)
        {
            await CopyObjectAsync(srcBucket, srcKey, destBucket, destKey);

            var requestRemove = new DeleteObjectRequest
            {
                BucketName = srcBucket,
                Key = srcKey
            };
            await _s3Client.DeleteObjectAsync(requestRemove);
        }

        public async Task DeleteObject(string bucket, string path)
        {
            var requestRemove = new DeleteObjectRequest
            {
                BucketName = bucket,
                Key = path
            };
            await _s3Client.DeleteObjectAsync(requestRemove);
        }

        public async Task<DeleteObjectResponse> DeleteObjectByUrl(string url)
        {
            var awsUri = new AmazonS3Uri(url);

            var requestRemove = new DeleteObjectRequest
            {
                BucketName = awsUri.Bucket,
                Key = awsUri.Key
            };

            return await _s3Client.DeleteObjectAsync(requestRemove);
        }

        private TransferUtilityUploadRequest CreateRequest(Stream inputStream, AwsInput input, string extension, string folder)
        {
            return new TransferUtilityUploadRequest
            {
                BucketName = GetBucketName(input.BucketType),
                Key = AwsS3PathExtensions.GenerateAwsS3Key($"{input.Id}{extension}", folder),
                CannedACL = S3CannedACL.PublicRead,
                InputStream = inputStream,
                Headers =
                {
                    ContentDisposition =
                        $"inline; filename*=UTF-8''" + System.Uri.EscapeDataString(input.Id)
                },
                ContentType = input.ContentType
            };
        }

        private string GetBucketName(BucketType bucketType)
        {
            return bucketType switch
            {
                BucketType.SourceDefault => _awsConfiguration.Bucket.SourceDefault,
                BucketType.UserAvatar => _awsConfiguration.Bucket.UserAvatar,
                BucketType.Product => _awsConfiguration.Bucket.Product,
                _ => ""
            };
        }
    }
}