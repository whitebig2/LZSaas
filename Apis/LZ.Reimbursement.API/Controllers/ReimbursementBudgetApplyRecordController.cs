using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[PlatformLimit(PlatformType.Admin)]
[PlatformLimit(PlatformType.LZMini)]
[PlatformLimit(PlatformType.MiniAdmin)]
public class ReimbursementBudgetApplyRecordController : ReimbursementControllerBase
{
    private readonly IReimbursementBudgetApplyRecordService _svc;

    public ReimbursementBudgetApplyRecordController(IReimbursementBudgetApplyRecordService s) => _svc = s;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ReimbursementBudgetApplyQuery q, [FromQuery] int? userId = null)
    {
        var (items, count) = await _svc.ListAsync(q, userId);
        return Ok(new { items, count });
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromForm] FIN_Reimb_BudgetApplyRecord m, [FromForm] List<FIN_Reimb_BudgetApplyRecordDetail>? details = null, [FromForm] List<FileRef>? files = null)
    {
        var res = await _svc.AddAsync(m, details, files);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int id)
    {
        var res = await _svc.GetAsync(id);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> GetSimple([FromQuery] int id)
    {
        var res = await _svc.GetSimpleAsync(id);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] FIN_Reimb_BudgetApplyRecord m)
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
    public async Task<IActionResult> Auditing([FromForm] int id, [FromForm] int method, [FromForm] int status)
    {
        await _svc.AuditingAsync(id, method, status);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int status, [FromForm] string message)
    {
        await _svc.UpdateStatusAsync(id, status, message);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetPartner()
    {
        var res = await _svc.GetPartnerAsync();
        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> SalaryCustomerAutoComplete([FromBody] SalaryAutoInput m)
    {
        var id = await _svc.SalaryCustomerAutoCompleteAsync(m);
        return Ok(id);
    }

    [HttpPost]
    public async Task<IActionResult> ImportBudgetDetailExcel([FromForm] int id, IFormFile file)
    {
        var res = await _svc.ImportBudgetDetailExcelAsync(id, file);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> CreateDefaultExcel()
    {
        var res = await _svc.CreateDefaultExcelAsync();
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> FinanceExportExcel([FromQuery] ReimbursementBudgetApplyQuery q, [FromQuery] string? payer = null)
    {
        var res = await _svc.FinanceExportExcelAsync(q, payer);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> SaveFile([FromQuery] int id)
    {
        var res = await _svc.SaveFileAsync(id);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> TiaoZheng()
    {
        var res = await _svc.TiaoZhengAsync();
        return Ok(res);
    }
}

