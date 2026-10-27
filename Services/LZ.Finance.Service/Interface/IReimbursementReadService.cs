using LZ.Model;
using LZ.Model.Dtos.Reimbursement;

namespace LZ.Finance.Service.Interface;

public interface IReimbursementReadService
{
    /// <summary>按条件分页查询当前小程序用户的申请。</summary>
    /// <param name="query">状态、时间、搜索词和分页条件。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>本人申请分页。</returns>
    Task<ApplicationPageDto<ApplicationListItemDto>> GetApplicationsAsync(ApplicationQuery query, CancellationToken cancellationToken = default);
    /// <summary>读取本人申请的表单、发票、审批历史和打款信息。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>申请详情；不存在时为 null。</returns>
    Task<ApplicationDetailDto?> GetApplicationAsync(Guid applyGuid, CancellationToken cancellationToken = default);
    /// <summary>读取审核退回或已打款申请的再次申请预填数据。</summary>
    /// <param name="applyGuid">源申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>预填表单和只读发票；不存在时为 null。</returns>
    Task<ResubmitPreloadDto?> GetResubmitPreloadAsync(Guid applyGuid, CancellationToken cancellationToken = default);
    /// <summary>查询本人申请的打款单。</summary>
    /// <param name="applyGuid">申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>打款单；不存在时为 null。</returns>
    Task<PaymentOrderDto?> GetPaymentOrderAsync(Guid applyGuid, CancellationToken cancellationToken = default);
    /// <summary>查询本人发票上传的识别、验真及查重状态。</summary>
    /// <param name="uploadGuid">发票上传 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>上传状态；不存在时为 null。</returns>
    Task<InvoiceUploadDto?> GetInvoiceUploadAsync(Guid uploadGuid, CancellationToken cancellationToken = default);
    /// <summary>只读校验所选发票是否有效及其金额，不建立申请关联。</summary>
    /// <param name="request">上传 GUID、申请类型、申请金额和流程版本。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>有效票额、有效和无效上传 GUID。</returns>
    Task<InvoiceValidationDto> ValidateInvoicesAsync(InvoiceValidationRequest request, CancellationToken cancellationToken = default);
    /// <summary>
    /// 根据当前登录部门获取部门信息
    /// </summary>
    /// <param name="companyGid"></param>
    /// <param name="partnerGid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Live_Partner?> GetCurrentPartnerAsync(Guid companyGId, int userId, CancellationToken cancellationToken = default);
}
