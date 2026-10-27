using LZ.Model.Dtos.Reimbursement;

namespace LZ.Finance.Service.Interface;

public interface IReimbursementWriteService
{
    /// <summary>根据申请人部门配置确定审核版或预算版流程。</summary>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>当前流程版本及来源。</returns>
    Task<FlowVersionDto> GetFlowVersionAsync(CancellationToken cancellationToken = default);
    /// <summary>提交报销或付款申请，按申请类型执行不同发票门槛。</summary>
    /// <param name="request">申请表单、类型、发票上传 GUID 和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>申请标识、单号、状态和流程版本。</returns>
    Task<ApplySubmitResultDto> SubmitAsync(ApplicationCreateRequest request, CancellationToken cancellationToken = default);
    /// <summary>更新审核退回的原申请并重新进入预算或审批流程。</summary>
    /// <param name="applyGuid">原申请 GUID。</param>
    /// <param name="request">修改后的表单及可追加发票。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>原申请的最新状态。</returns>
    Task<ApplySubmitResultDto> ResubmitAsync(Guid applyGuid, ApplicationResubmitRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>再次申请：退回单重提，已打款单沿用旧票新建。</summary>
    /// <param name="request">源申请 GUID、修改后的表单和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>原单或新单的标识及状态。</returns>
    Task<ApplySubmitResultDto> CopyAsync(ApplicationCopyRequest request, CancellationToken cancellationToken = default);
    /// <summary>给待补票付款单关联已验真的补传发票并重算缺口。</summary>
    /// <param name="applyGuid">付款申请 GUID。</param>
    /// <param name="request">补传上传 GUID 和幂等键。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>累计有效票额及是否补齐。</returns>
    Task<InvoiceValidationDto> SupplementAsync(Guid applyGuid, SupplementInvoiceRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>核验财务权限并记录已完成的打款。</summary>
    /// <param name="applyGuid">待打款申请 GUID。</param>
    /// <param name="request">银行流水号、幂等键和回单地址。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>打款单。</returns>
    Task<PaymentOrderDto> CompletePaymentAsync(Guid applyGuid, CompletePaymentRequest request,
        CancellationToken cancellationToken = default);
}
