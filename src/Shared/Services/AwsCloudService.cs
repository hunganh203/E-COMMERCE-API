using Application.Dtos;
using Application.DTOs.Cloud.AWS;
using Application.Interfaces.CloudService.AWS;
using Application.Interfaces.Service.AWS;
using Microsoft.AspNetCore.Http;

namespace Shared.Services
{
    public class AwsCloudService : IAwsCloudService
    {
        private readonly IAwsS3Service _awsS3Service;

        public AwsCloudService(IAwsS3Service awsS3Service)
        {
            _awsS3Service = awsS3Service;
        }

        public async Task<FileStorageDto> UploadFileAsync(IFormFile fileData, AwsInput input, int creatorId)
        {
            return await _awsS3Service.UploadFileAsync(fileData, input, creatorId);
        }
    }
}