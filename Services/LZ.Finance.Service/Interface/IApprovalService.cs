using LZ.Model;
using LZ.Model.Dtos.Reimbusement;

namespace LZ.Finance.Service.Interface;

public interface IApprovalService
{
    /// <summary>查询当前企业已发布的审批流程。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>生效流程；未配置时为 null。</returns>
    Task<ApprovalProcessDto?> GetActiveProcessAsync(CancellationToken cancellationToken = default);
    /// <summary>保存并发布审批流程及动态节点。</summary>
    /// <param name="request">流程与节点配置。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>发布后的流程。</returns>
    Task<ApprovalProcessDto> SaveAndPublishAsync(ApprovalProcessSaveRequest request, CancellationToken cancellationToken = default);
    /// <summary>按当前用户权限查询审批申请分页。</summary>
    /// <param name="query">筛选和分页条件。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批申请分页。</returns>
    Task<ApprovalPageDto<ApprovalListItemDto>> GetPagedAsync(ApprovalQuery query, CancellationToken cancellationToken = default);
    /// <summary>查询指定申请的审批详情和审批记录。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批详情；不存在时为 null。</returns>
    Task<ApprovalDetailDto?> GetDetailAsync(Guid applyGuid, CancellationToken cancellationToken = default);
    /// <summary>分后台通过当前审批节点。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">审批意见。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task ApproveAsync(Guid applyGuid, ApprovalApproveRequest request, CancellationToken cancellationToken = default);
    /// <summary>分后台驳回当前审批节点。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">驳回原因。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task RejectAsync(Guid applyGuid, ApprovalRejectRequest request, CancellationToken cancellationToken = default);
    /// <summary>小程序幂等通过当前审批节点。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">审批意见。</param>
    /// <param name="idempotencyKey">防止同一审批操作重复执行的键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task ApproveWithKeyAsync(Guid applyGuid, ApprovalApproveRequest request, string idempotencyKey,
        CancellationToken cancellationToken = default);
    /// <summary>小程序幂等驳回当前审批节点。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="request">驳回原因。</param>
    /// <param name="idempotencyKey">防止同一审批操作重复执行的键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task RejectWithKeyAsync(Guid applyGuid, ApprovalRejectRequest request, string idempotencyKey,
        CancellationToken cancellationToken = default);

    /// <summary>在调用方的 Finance 事务内为提交或重提申请启动审批实例。</summary>
    /// <param name="applyGuid">已写入的申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>新审批实例 GUID；调用方负责事务提交或回滚。</returns>
    Task<Guid> StartAsync(Guid applyGuid, CancellationToken cancellationToken = default);

    /// <summary>查询指定部门的审批人。</summary>
    /// <param name="partnerGid">部门或伙伴 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>审批人列表。</returns>
    Task<List<ApprovalDto>> GetApproversAsync(Guid partnerGid, CancellationToken cancellationToken = default);

}
