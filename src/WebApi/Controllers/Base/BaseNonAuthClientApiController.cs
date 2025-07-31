using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers.Base
{
    [ApiController]
    [OpenApiIgnore]
    [Route("api/[controller]")]
    public abstract class BaseNonAuthClientApiController : ControllerBase
    {
    }
}