using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.Clients.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class ClientsControllerBase : ApiControllerBase
    {
    }
}
