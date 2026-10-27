using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model.Dtos.Reimbusement;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[Authorize(AuthenticationSchemes = "Authorization-Token")]
[PlatformLimit(PlatformType.XFZMini)]
public sealed class MiniApprovalController : ReimbursementControllerBase
{
    private readonly IApprovalService _approval;

    /// <summary>注入小程序和分后台共用的审批服务。</summary>
    /// <param name="approval">审批查询及决策服务。</param>
    public MiniApprovalController(IApprovalService approval) => _approval = approval;

    /// <summary>查询当前审批人的待办、已办或全部审批单。</summary>
    /// <param name="request">列表状态、筛选及分页条件。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批单分页。</returns>
    [HttpGet]
    public Task<IActionResult> GetApprovals([FromQuery] MiniApprovalQuery request,
        CancellationToken cancellationToken) => ExecuteAsync(async () =>
    {
        var status = request?.Status?.Trim().ToLowerInvariant();
        if (status is not ("todo" or "done" or "all"))
            throw new ArgumentException("审批列表状态无效");
        var query = new ApprovalQuery
        {
            PendingOnly = status == "all" ? null : status == "todo",
            ExpenseName = request!.ExpenseName,
            ApplyNo = request.ApplyNo,
            ApplyTimeStart = request.ApplyTimeStart,
            ApplyTimeEnd = request.ApplyTimeEnd,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
        return ToCamelCase(Ok(await _approval.GetPagedAsync(query, cancellationToken)));
    });

    /// <summary>查询指定申请的审批详情和当前节点。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批详情；不存在时返回 404。</returns>
    [HttpGet]
    public Task<IActionResult> GetDetail(Guid applyGuid, CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            var detail = await _approval.GetDetailAsync(applyGuid, cancellationToken);
            return detail is null ? Error(404, "审批单不存在") : ToCamelCase(Ok(detail));
        });

    /// <summary>通过当前审批节点，幂等键用于防止重复操作。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">审批意见和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>操作结果及申请 GUID。</returns>
    [HttpPost]
    public Task<IActionResult> Approve(Guid applyGuid, [FromBody] MiniApproveRequest request,
        CancellationToken cancellationToken) => ExecuteAsync(async () =>
    {
        if (request is null) throw new ArgumentException("审批请求不能为空");
        await _approval.ApproveWithKeyAsync(applyGuid,
            new ApprovalApproveRequest { Comment = request.Comment }, request.IdempotencyKey,
            cancellationToken);
        return ToCamelCase(Ok(new { ApplyGuid = applyGuid }));
    });

    /// <summary>驳回当前审批节点并将申请退回给申请人。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">驳回原因和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>操作结果及申请 GUID。</returns>
    [HttpPost]
    public Task<IActionResult> Reject(Guid applyGuid, [FromBody] MiniRejectRequest request,
        CancellationToken cancellationToken) => ExecuteAsync(async () =>
    {
        if (request is null) throw new ArgumentException("驳回请求不能为空");
        await _approval.RejectWithKeyAsync(applyGuid,
            new ApprovalRejectRequest { RejectReason = request.RejectReason }, request.IdempotencyKey,
            cancellationToken);
        return ToCamelCase(Ok(new { ApplyGuid = applyGuid }));
    });

    /// <summary>执行审批接口并将预期的业务异常转成 HTTP 错误。</summary>
    /// <param name="action">异步审批操作。</param>
    /// <returns>接口响应。</returns>
    private async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (ArgumentException ex) { return Error(400, ex.Message); }
        catch (UnauthorizedAccessException ex) { return Error(403, ex.Message); }
        catch (InvalidOperationException ex) { return Error(409, ex.Message); }
    }

    /// <summary>保留审批错误的 HTTP 状态码和原有响应内容。</summary>
    /// <param name="status">HTTP 状态码。</param>
    /// <param name="message">错误说明。</param>
    /// <returns>审批错误响应。</returns>
    private IActionResult Error(int status, string message) => new JsonResult(new ApiResponse<object>
    {
        Code = status, Message = message, TraceId = HttpContext.TraceIdentifier
    }) { StatusCode = status };
}
