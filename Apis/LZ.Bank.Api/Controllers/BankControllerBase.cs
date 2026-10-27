using Microsoft.AspNetCore.Mvc;
using LZ.Core;
namespace LZ.Bank.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BankControllerBase : ApiControllerBase
    {
    }
}
