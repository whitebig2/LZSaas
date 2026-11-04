using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[PlatformLimit(PlatformType.Admin)]
[PlatformLimit(PlatformType.LZMini)]
[PlatformLimit(PlatformType.MiniAdmin)]
public class ReimbursementBudgetRechargeLogController : ReimbursementControllerBase
{
    private readonly IReimbursementBudgetRechargeLogService _svc;

    public ReimbursementBudgetRechargeLogController(IReimbursementBudgetRechargeLogService s) => _svc = s;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ReimbursementBudgetLogQuery q)
    {
        var (items, count) = await _svc.ListAsync(q);
        return Ok(new { items, count });
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] FIN_Reimb_BudgetRechargeLog m)
    {
        await _svc.AddAsync(m);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int id)
    {
        var res = await _svc.GetAsync(id);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] FIN_Reimb_BudgetRechargeLog m)
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

