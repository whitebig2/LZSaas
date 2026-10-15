using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.ShopBasic.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class ShopBasicControllerBase : ApiControllerBase
    {
    }
}
