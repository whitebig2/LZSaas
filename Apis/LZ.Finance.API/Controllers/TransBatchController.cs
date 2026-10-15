using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransBatchController : ApiControllerBase
    {

        private readonly ITransBatchService   _transBatchService;

        public TransBatchController(ITransBatchService transBatchService)
        {
            _transBatchService = transBatchService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_TransBatch>();
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            var models = await _transBatchService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpDelete]
        public async Task<IActionResult> GetNav(Guid id)
        {
            await _transBatchService.GetNav(f => f.ID == id);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> TransSinglePush(Guid id)
        {
            await _transBatchService.GetNav(f => f.ID == id);
            return Ok();
        }
    }
}
