using LZ.Model.Dtos.Reimbursement;

namespace LZ.Finance.Service.Interface;

public interface IReimbursementInvoiceService
{
    /// <summary>保存并校验发票；有效补传发票同时关联付款申请并更新累计票额。</summary>
    /// <param name="request">发票文件、首次上传或补传用途及可选申请 GUID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>上传校验结果；补传时还包括申请累计票额、缺口和状态。</returns>
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
