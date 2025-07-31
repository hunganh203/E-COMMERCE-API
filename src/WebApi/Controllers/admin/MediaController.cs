using Application.DTOs.Configuration;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    public class MediaController : BaseAuthAdminApiController
    {
        private readonly AwsConfiguration _awsConfiguration;
        private readonly IFileStorageService _fileStorageService;

        public MediaController(IUserService userService,
            AwsConfiguration awsConfiguration,
            IFileStorageService fileStorageService) : base(userService)
        {
            _awsConfiguration = awsConfiguration;
            _fileStorageService = fileStorageService;
        }

        [HttpGet("product-bucket-url")]
        public async Task<AjaxResponse> ProductBucketUrl()
        {
            try
            {
                return new AjaxResponse(await Task.Run(() => this._awsConfiguration.Uri.Product));
            }
            catch (Exception e)
            {
                return new AjaxResponse(new ErrorInfo(e.Message));
            }
        }

        [HttpPost("upload-file")]
        public async Task<AjaxResponse> UploadFile([FromForm] IFormFile file)
        {
            try
            {
                return new AjaxResponse(await Task.Run(() => true));
            }
            catch (Exception e)
            {
                return new AjaxResponse(new ErrorInfo(e.Message));
            }
        }

        [OpenApiOperation("Upload single file to local storage")]
        [HttpPost("upload")]
        public async Task<AjaxResponse> Upload(IFormFile file, [FromForm] string module)
        {
            try
            {
                var fileResult = await _fileStorageService.UploadFileAsync(file, module);
                return new AjaxResponse(fileResult);
            }
            catch (Exception e)
            {
                return new AjaxResponse(new ErrorInfo(e.Message));
            }
        }
    }
}