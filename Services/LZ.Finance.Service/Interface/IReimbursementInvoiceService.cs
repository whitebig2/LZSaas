using LZ.Model.Dtos.Reimbursement;

namespace LZ.Finance.Service.Interface;

public interface IReimbursementInvoiceService
{
    /// <summary>保存发票文件并发起异步识别。</summary>
    /// <param name="request">发票文件、首次上传或补传用途及可选申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>上传 GUID 和初始状态。</returns>
    Task<InvoiceUploadDto> UploadAsync(InvoiceUploadRequest request, CancellationToken cancellationToken = default);
    /// <summary>验证发票供应商回调并更新识别、验真和重复校验结果。</summary>
    /// <param name="request">解析后的回调字段。</param>
    /// <param name="signature">供应商签名。</param>
    /// <param name="rawBody">用于验签的原始请求体。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task HandleCallbackAsync(InvoiceCallbackRequest request, string signature, string rawBody,
        CancellationToken cancellationToken = default);
    /// <summary>从字典扩展服务读取申请表单选项。</summary>
    /// <param name="kind">法人主体、成本类型、费用名称或银行。</param>
    /// <param name="keyword">可选搜索词。</param>
    /// <param name="costTypeId">查询费用名称时可传成本类型 ID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>字典项列表。</returns>
    Task<IReadOnlyList<DictItemDto>> GetDictionaryAsync(string kind, string? keyword,
        int? costTypeId, CancellationToken cancellationToken = default);
}
