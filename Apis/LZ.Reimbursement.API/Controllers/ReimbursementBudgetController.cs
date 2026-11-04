using LZ.Core;
using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[PlatformLimit(PlatformType.Admin)]
[PlatformLimit(PlatformType.LZMini)]
[PlatformLimit(PlatformType.MiniAdmin)]
public sealed class ReimbursementBudgetController : ReimbursementControllerBase
{
    private readonly IReimbursementBudgetAdministrationService _budgets;

    public ReimbursementBudgetController(IReimbursementBudgetAdministrationService budgets) => _budgets = budgets;

    // ── 预算配置（FIN_Reimb_BudgetConfig） ─────────────────────
    [HttpGet]
    public Task<IActionResult> ListConfigs([FromQuery] ReimbursementBudgetQuery query, CancellationToken token) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _budgets.ListConfigsAsync(query, token))));

    [HttpGet]
    public Task<IActionResult> GetConfig([FromQuery] int id, CancellationToken token) =>
        ExecuteAsync(async () => Result(await _budgets.GetConfigAsync(id, token)));

    [HttpPost]
    public Task<IActionResult> CreateConfig([FromBody] ReimbursementBudgetConfigInput input, CancellationToken token) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _budgets.CreateConfigAsync(input, token))));

    [HttpPut]
    public Task<IActionResult> UpdateConfig(int id, [FromBody] ReimbursementBudgetConfigInput input, CancellationToken token) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _budgets.UpdateConfigAsync(id, input, token))));

    [HttpDelete]
    public Task<IActionResult> DeleteConfig([FromQuery] int id, CancellationToken token) =>
        ExecuteAsync(async () => { await _budgets.DeleteConfigAsync(id, token); return Ok(); });

    // ── 预算池管理（FIN_Reimb_BudgetManage） ───────────────────
    [HttpGet]
    public Task<IActionResult> ListPools([FromQuery] ReimbursementBudgetQuery query, CancellationToken token) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _budgets.ListPoolsAsync(query, token))));

    [HttpGet]
    public Task<IActionResult> GetPool([FromQuery] int id, CancellationToken token) =>
        ExecuteAsync(async () => Result(await _budgets.GetPoolAsync(id, token)));

    [HttpPost]
    public Task<IActionResult> BatchCreate([FromQuery] int? year, CancellationToken token) =>
        ExecuteAsync(async () => ToCamelCase(Ok(new { Created = await _budgets.BatchCreatePoolsAsync(year, token) })));

    [HttpPut]
    public Task<IActionResult> UpdatePool(int id, [FromBody] ReimbursementBudgetPoolInput input, CancellationToken token) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _budgets.UpdatePoolAsync(id, input, token))));

    [HttpPost]
    public Task<IActionResult> Recharge(int id, [FromBody] ReimbursementBudgetAmountInput input, CancellationToken token) =>
        ExecuteAsync(async () => { await _budgets.RechargeAsync(id, input.Amount, token); return Ok(); });

    [HttpPost]
    public Task<IActionResult> Withdraw(int id, [FromBody] ReimbursementBudgetAmountInput input, CancellationToken token) =>
        ExecuteAsync(async () => { await _budgets.WithdrawAsync(id, input.Amount, token); return Ok(); });

    [HttpPost]
    public Task<IActionResult> Transfer(int id, [FromBody] ReimbursementBudgetTransferInput input, CancellationToken token) =>
        ExecuteAsync(async () => { await _budgets.TransferAsync(id, input, token); return Ok(); });

    [HttpPost]
    public Task<IActionResult> SetHidden(int id, [FromBody] bool hidden, CancellationToken token) =>
        ExecuteAsync(async () => { await _budgets.SetPoolHiddenAsync(id, hidden, token); return Ok(); });

    [HttpDelete]
    public Task<IActionResult> DeletePool([FromQuery] int id, CancellationToken token) =>
        ExecuteAsync(async () => { await _budgets.DeletePoolAsync(id, token); return Ok(); });

    // ── 预留（FIN_Reimb_BudgetReservation，报销特有） ───────────
    [HttpGet]
    public Task<IActionResult> GetReservation([FromQuery] Guid id, CancellationToken token) =>
        ExecuteAsync(async () => Result(await _budgets.GetReservationAsync(id, token)));

    [HttpGet]
    public Task<IActionResult> GetReservationByApply([FromQuery] Guid applyGuid, CancellationToken token) =>
        ExecuteAsync(async () => Result(await _budgets.GetReservationByApplyAsync(applyGuid, token)));

    private IActionResult Result<T>(T? value) where T : class =>
        value is null ? Error(404, "数据不存在") : ToCamelCase(Ok(value));

    private async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (ArgumentException ex) { return Error(400, ex.Message); }
        catch (UnauthorizedAccessException ex) { return Error(403, ex.Message); }
        catch (KeyNotFoundException ex) { return Error(404, ex.Message); }
        catch (InvalidOperationException ex) { return Error(409, ex.Message); }
    }

    private IActionResult Error(int status, string message) => new JsonResult(new ApiResponse<object>
    { Code = status, Message = message, TraceId = HttpContext.TraceIdentifier }) { StatusCode = status };
}
