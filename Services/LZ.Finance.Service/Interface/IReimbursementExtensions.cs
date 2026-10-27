using LZ.Model.Dtos.Reimbursement;

namespace LZ.Finance.Service.Interface;

// 外部能力尚未确定实现方；接入时实现接口并按项目的依赖注入规则注册即可。
public interface IReimbursementBudgetProvider
{
    /// <summary>检查预算并在预算内预留金额；外部请求号须幂等。</summary>
    /// <param name="companyId">企业 ID。</param>
    /// <param name="userId">申请人 ID。</param>
    /// <param name="applyNo">申请单号，作为外部幂等请求号。</param>
    /// <param name="costTypeId">成本类型 ID。</param>
    /// <param name="amount">申请金额。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>是否预算内、是否预留及预算快照。</returns>
    Task<BudgetDecision> CheckAndReserveAsync(int companyId, int userId, string applyNo,
        int costTypeId, decimal amount, CancellationToken cancellationToken);
    /// <summary>打款确认后提交先前的预算预留；实现方须幂等。</summary>
    /// <param name="externalRequestNo">预留时使用的外部请求号。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task CommitAsync(string externalRequestNo, CancellationToken cancellationToken);
    /// <summary>本地提交失败时补偿释放预算预留；实现方须幂等。</summary>
    /// <param name="externalRequestNo">预留时使用的外部请求号。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task ReleaseAsync(string externalRequestNo, CancellationToken cancellationToken);
}

public sealed class BudgetDecision
{
    public bool WithinBudget { get; set; }
    public bool Reserved { get; set; }
    public string? ExternalBudgetId { get; set; }
    public decimal? AvailableBefore { get; set; }
    public string? ResponseSnapshot { get; set; }
}

public interface IReimbursementDictionaryProvider
{
    /// <summary>提供申请表单所需的字典项。</summary>
    /// <param name="companyId">企业 ID。</param>
    /// <param name="kind">字典种类。</param>
    /// <param name="keyword">可选搜索词。</param>
    /// <param name="costTypeId">费用名称筛选使用的成本类型 ID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>字典项列表。</returns>
    Task<IReadOnlyList<DictItemDto>> GetAsync(int companyId, string kind, string? keyword,
        int? costTypeId, CancellationToken cancellationToken);
}

public interface IReimbursementInvoiceProvider
{
    /// <summary>向发票供应商发起 OCR 和验真；结果由异步回调返回。</summary>
    /// <param name="companyId">企业 ID。</param>
    /// <param name="uploadGuid">本地上传 GUID。</param>
    /// <param name="providerRequestId">回调匹配所用的供应商请求号。</param>
    /// <param name="fileUrl">OSS 发票文件地址。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task StartRecognitionAsync(int companyId, Guid uploadGuid, string providerRequestId, string fileUrl,
        CancellationToken cancellationToken);
    /// <summary>验证原始回调请求体的签名，失败时不得写识别结果。</summary>
    /// <param name="signature">请求头中的签名。</param>
    /// <param name="body">原始请求体。</param>
    /// <returns>签名是否有效。</returns>
    bool VerifyCallback(string signature, string body);
}

public interface IReimbursementPaymentAuthorizer
{
    /// <summary>检查分后台用户是否拥有独立的财务打款确认权限。</summary>
    /// <param name="companyId">企业 ID。</param>
    /// <param name="userId">当前分后台用户 ID。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>是否允许确认打款。</returns>
    Task<bool> CanCompleteAsync(int companyId, int userId, CancellationToken cancellationToken);
}

public interface IReimbursementCardProtector
{
    /// <summary>加密收款银行卡号，返回值只写入数据库密文列。</summary>
    /// <param name="cardNumber">银行卡原号。</param>
    /// <returns>加密后的字节。</returns>
    byte[] Encrypt(string cardNumber);
}
