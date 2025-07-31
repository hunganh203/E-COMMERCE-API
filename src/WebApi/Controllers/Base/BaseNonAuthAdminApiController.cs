using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Base
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public abstract class BaseNonAuthAdminApiController : ControllerBase
    {
    }
}