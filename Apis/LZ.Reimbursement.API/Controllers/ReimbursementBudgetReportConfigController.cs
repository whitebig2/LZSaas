using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[PlatformLimit(PlatformType.Admin)]
[PlatformLimit(PlatformType.LZMini)]
[PlatformLimit(PlatformType.MiniAdmin)]
public class ReimbursementBudgetReportConfigController : ReimbursementControllerBase
{
    private readonly IReimbursementBudgetReportConfigService _svc;

    public ReimbursementBudgetReportConfigController(IReimbursementBudgetReportConfigService s) => _svc = s;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ReimbursementBudgetReportConfigQuery q)
    {
        var (items, count) = await _svc.ListAsync(q);
        return Ok(new { items, count });
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] List<FIN_Reimb_BudgetReportConfig> list)
    {
        await _svc.AddAsync(list);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int id)
    {
        var res = await _svc.GetAsync(id);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] FIN_Reimb_BudgetReportConfig m)
    {
        await _svc.UpdateAsync(m);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromForm] int id)
    {
        await _svc.DeleteAsync(id);
        return Ok();
    }
}

