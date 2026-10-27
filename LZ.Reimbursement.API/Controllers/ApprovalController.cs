using LZ.Core;
using LZ.Core.Attributes;
using LZ.Finance.Service.Interface;
using LZ.Model.Dtos.Reimbusement;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Reimbursement.API.Controllers;

[Route("api")]
[PlatformLimit(PlatformType.MiniAdmin)]
public sealed class ApprovalController : ReimbursementControllerBase
{
    private readonly IApprovalService _approvalSvc;

    /// <summary>注入分后台报销审批服务。</summary>
    /// <param name="approval">审批配置、查询和决策服务。</param>
    public ApprovalController(IApprovalService approval) => _approvalSvc = approval;

    /// <summary>查询当前企业已发布的审批流程及节点。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>生效流程；无配置时返回 404。</returns>
    [HttpGet("approval-process/active")]
    public Task<IActionResult> GetActive(CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            var process = await _approvalSvc.GetActiveProcessAsync(cancellationToken);
            return process is null ? NotFound("当前企业尚未配置审核流程") : ToCamelCase(Ok(process));
        });

    /// <summary>保存并发布分后台审批流程配置。</summary>
    /// <param name="request">流程和审批节点配置。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>发布后的流程。</returns>
    [HttpPut("approval-process/save")]
    public Task<IActionResult> SaveProcess([FromBody] ApprovalProcessSaveRequest request, CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _approvalSvc.SaveAndPublishAsync(request, cancellationToken))));

    /// <summary>查询分后台审批申请列表。</summary>
    /// <param name="query">状态、搜索和分页条件。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批申请分页。</returns>
    [HttpGet("applications")]
    public Task<IActionResult> GetPaged([FromQuery] ApprovalQuery query, CancellationToken cancellationToken) =>
        ExecuteAsync(async () => ToCamelCase(Ok(await _approvalSvc.GetPagedAsync(query, cancellationToken))));

    /// <summary>查询分后台审批申请详情。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批详情；无数据时返回 404。</returns>
    [HttpGet("applications/{applyGuid:guid}")]
    public Task<IActionResult> GetDetail(Guid applyGuid, CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            var detail = await _approvalSvc.GetDetailAsync(applyGuid, cancellationToken);
            return detail is null ? NotFound("申请不存在") : ToCamelCase(Ok(detail));
        });

    /// <summary>通过当前审批节点并推进申请审批链。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">审批意见。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批操作结果。</returns>
    [HttpPost("applications/{applyGuid:guid}/approve")]
    public Task<IActionResult> Approve(Guid applyGuid, [FromBody] ApprovalApproveRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            await _approvalSvc.ApproveAsync(applyGuid, request, cancellationToken);
            return Ok();
        });

    /// <summary>驳回当前节点并将申请置为审核退回。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">驳回原因。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批操作结果。</returns>
    [HttpPost("applications/{applyGuid:guid}/reject")]
    public Task<IActionResult> Reject(Guid applyGuid, [FromBody] ApprovalRejectRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(async () =>
        {
            await _approvalSvc.RejectAsync(applyGuid, request, cancellationToken);
            return Ok();
        });

    /// <summary>查询指定部门的审批人列表。</summary>
    /// <param name="partnerGid">部门或伙伴 GUID。</param>
    /// <returns>审批人列表。</returns>
    [HttpGet("approvals/{partnerGid:guid}")]
    public async Task<IActionResult> GetApprovalAsync(Guid partnerGid)
    {
        var approvers = await _approvalSvc.GetApproversAsync(partnerGid);
        return ToCamelCase(Ok(approvers));
    }

    /// <summary>执行分后台审批接口并转换预期业务异常。</summary>
    /// <param name="action">异步审批操作。</param>
    /// <returns>接口响应。</returns>
    private static async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (ArgumentException ex) { return new BadRequestObjectResult(ex.Message); }
        catch (UnauthorizedAccessException ex) { return new ObjectResult(ex.Message) { StatusCode = 403 }; }
        catch (InvalidOperationException ex) { return new ConflictObjectResult(ex.Message); }
    }


}
