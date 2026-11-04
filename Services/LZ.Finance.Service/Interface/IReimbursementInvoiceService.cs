using LZ.Model.Dtos.Reimbursement;

namespace LZ.Finance.Service.Interface;

public interface IReimbursementInvoiceService
{
    /// <summary>保存发票文件并同步完成识别、验真及查重。</summary>
    /// <param name="request">发票文件、首次上传或补传用途及可选申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>上传 GUID、三项校验状态、有效性及验真金额。</returns>
    Task<InvoiceUploadDto> UploadAsync(InvoiceUploadRequest request, CancellationToken cancellationToken = default);
    /// <summary>从字典扩展服务读取申请表单选项。</summary>
    /// <param name="kind">法人主体、成本类型、费用名称或银行。</param>
    /// <param name="keyword">可选搜索词。</param>
    /// <param name="costTypeId">查询费用名称时可传成本类型 ID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>字典项列表。</returns>
    Task<IReadOnlyList<DictItemDto>> GetDictionaryAsync(string kind, string? keyword,
        int? costTypeId, CancellationToken cancellationToken = default);
}
