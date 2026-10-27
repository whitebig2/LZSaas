using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.Order.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class OrderControllerBase : ApiControllerBase
    {
    }
}
