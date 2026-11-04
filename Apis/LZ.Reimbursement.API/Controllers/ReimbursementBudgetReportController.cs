using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[PlatformLimit(PlatformType.Admin)]
[PlatformLimit(PlatformType.LZMini)]
[PlatformLimit(PlatformType.MiniAdmin)]
public class ReimbursementBudgetReportController : ReimbursementControllerBase
{
    private readonly IReimbursementBudgetReportService _svc;

    public ReimbursementBudgetReportController(IReimbursementBudgetReportService s) => _svc = s;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ReimbursementBudgetReportQuery q)
    {
        var (items, count) = await _svc.ListAsync(q);
        return Ok(new { items, count });
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] FIN_Reimb_BudgetReport m)
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
    public async Task<IActionResult> Update([FromBody] FIN_Reimb_BudgetReport m)
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

    [HttpPost]
    public async Task<IActionResult> CreateExcel([FromBody] FIN_Reimb_BudgetReport m)
    {
        var res = await _svc.CreateExcelAsync(m);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> CreatePartner()
    {
        var res = await _svc.CreatePartnerAsync();
        return Ok(res);
    }
}

