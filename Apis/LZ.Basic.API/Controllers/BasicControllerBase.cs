using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.Basic.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BasicControllerBase : ApiControllerBase
    {
    }
}
