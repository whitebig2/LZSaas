using LZ.Core;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Invoice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class InvoiceControllerBase : ApiControllerBase
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Ok();
        }
    }
}
