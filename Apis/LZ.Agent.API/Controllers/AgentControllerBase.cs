using LZ.Core;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Agent.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class AgentControllerBase : ApiControllerBase
    {
    }
}
