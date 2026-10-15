using Microsoft.AspNetCore.Mvc;
using LZ.Core;

namespace LZ.Company.API
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class CompanyControllerBase : ApiControllerBase
    {
    }
}
