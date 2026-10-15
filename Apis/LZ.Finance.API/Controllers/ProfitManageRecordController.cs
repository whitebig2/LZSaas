using LZ.Core.Attributes;
using LZ.Clients.API;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class ProfitManageRecordController(IProfitManageRecordService profitManageRecordService) : FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query)
        {
            var exp = Expressionable.Create<Model.Live_ProfitManageRecord>();
            if (query.Year.HasValue)
            {
                exp.And(t => t.ProfitMonth.HasValue && t.ProfitMonth.Value.Year==query.Year);
            }
            var models = await profitManageRecordService.List(exp.ToExpression());
            return Ok(new { items = models, count = query.Count });
        }
    }
}
