using Application.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;
using WebApi.Models.Response;

namespace WebApi.Controllers.admin
{
    public class SetupController : BaseNonAuthAdminApiController

    {
        private readonly ISetupService _setupService;

        public SetupController(ISetupService setupService)
        {
            _setupService = setupService;
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        [HttpPost("setup-admin")]
        public async Task<AjaxResponse<bool>> SetupAdmin()

        {
            try
            {
                var result = await _setupService.SeedDataAdmin();
                return new AjaxResponse<bool>(result);
            }
            catch (Exception e)
            {
                return new AjaxResponse<bool>(new ErrorInfo(e.Message));
            }
        }
    }
}