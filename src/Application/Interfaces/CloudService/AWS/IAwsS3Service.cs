using Application.Dtos;
using Application.DTOs.Cloud.AWS;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.CloudService.AWS
{
    public interface IAwsS3Service
    {
        Task<FileStorageDto> UploadFileAsync(IFormFile fileData, AwsInput input, int creatorId);

        Task<FileStorageDto> UploadFileAsync(byte[] fileData, AwsInput input, int creatorId, string folder);
    }
}