using Application.Dtos;
using Application.DTOs.Cloud.AWS;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Service.AWS
{
    public interface IAwsCloudService
    {
        Task<FileStorageDto> UploadFileAsync(IFormFile fileData, AwsInput input, int creatorId);
    }
}