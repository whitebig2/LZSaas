using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.Product.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class ProductControllerBase : ApiControllerBase
    {
    }
}
