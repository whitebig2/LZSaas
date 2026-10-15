using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.Customer.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class CustomerControllerBase : ApiControllerBase
    {
    }
}
