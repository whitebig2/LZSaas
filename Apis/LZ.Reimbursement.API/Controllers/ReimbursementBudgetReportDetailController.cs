using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Budget;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[PlatformLimit(PlatformType.Admin)]
[PlatformLimit(PlatformType.LZMini)]
[PlatformLimit(PlatformType.MiniAdmin)]
public class ReimbursementBudgetReportDetailController : ReimbursementControllerBase
{
    private readonly IReimbursementBudgetReportDetailService _svc;

    public ReimbursementBudgetReportDetailController(IReimbursementBudgetReportDetailService s) => _svc = s;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ReimbursementBudgetReportDetailQuery q)
    {
        var (items, count) = await _svc.ListAsync(q);
        return Ok(new { items, count });
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] FIN_Reimb_BudgetReportDetail m)
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
    public async Task<IActionResult> Update([FromBody] FIN_Reimb_BudgetReportDetail m)
    {
        await _svc.UpdateAsync(m);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBill([FromBody] RecordMonthModel m)
    {
        var res = await _svc.CreateBillAsync(m);
        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMonthBill([FromBody] RecordMonthModel m)
    {
        await _svc.DeleteMonthBillAsync(m);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ReportMonthList([FromBody] RecordMonthModel m)
    {
        var items = await _svc.ReportMonthListAsync(m);
        return Ok(new { items });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromForm] int id)
    {
        await _svc.DeleteAsync(id);
        return Ok();
    }
}

