using Application.DTOs.Configuration;
using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    public class MediaController : BaseAuthAdminApiController
    {
        private readonly AwsConfiguration _awsConfiguration;

        public MediaController(IUserService userService, AwsConfiguration awsConfiguration) : base(userService)
        {
            _awsConfiguration = awsConfiguration;
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
    }
}