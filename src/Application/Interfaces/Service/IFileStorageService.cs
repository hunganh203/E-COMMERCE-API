using Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Service
{
    public interface IFileStorageService
    {
        Task<FileStorageDto> UploadFileAsync(IFormFile file, string module);

        Task<FileStorageDto> CreateFileStorageFromUrlAsync(string linkUrl, string module);
    }
}