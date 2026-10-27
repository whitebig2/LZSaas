using Microsoft.AspNetCore.Mvc;
using Dapr.Client;

namespace LZ.Company.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {

        private readonly ILogger<ProductController> _logger;
        private readonly DaprClient _daprClient;
        public ProductController(ILogger<ProductController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpGet]
        public string Index()
        {
            return "go";
        }
        [HttpGet]
        public string List()
        {
            return "list";
        }
    }
}
