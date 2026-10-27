using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.Statistics.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class StatisticsControllerBase : ApiControllerBase
    {
    }
}
