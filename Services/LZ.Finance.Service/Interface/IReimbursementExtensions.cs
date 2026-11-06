using LZ.Model.Dtos.Reimbursement;

namespace LZ.Finance.Service.Interface;

public interface IReimbursementBudgetProvider
{
    /// <summary>按老预算池校验并预留；同一请求号重试不得重复锁款。失败抛异常，余额不足返回预算外。</summary>
    /// <param name="companyId">企业 ID。</param>
    /// <param name="userId">申请人 ID。</param>
    /// <param name="externalRequestNo">由申请 GUID 生成的稳定幂等请求号。</param>
    /// <param name="costTypeId">成本类型 ID。</param>
    /// <param name="expenseName">老预算池的费用名称。</param>
    /// <param name="amount">申请金额。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    /// <returns>是否预算内、是否预留及预算快照。</returns>
    Task<BudgetDecision> CheckAndReserveAsync(int companyId, int userId, string externalRequestNo,
        int costTypeId, string expenseName, decimal amount, CancellationToken cancellationToken);
    /// <summary>将已预留金额转为已用；重复确认不重复扣减，未预留或已释放时抛异常。</summary>
    /// <param name="externalRequestNo">预留时使用的外部请求号。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task CommitAsync(string externalRequestNo, CancellationToken cancellationToken);
    /// <summary>释放尚未确认的预留；重复释放不重复加回可用金额，已确认时抛异常。</summary>
    /// <param name="externalRequestNo">预留时使用的外部请求号。</param>
    /// <param name="cancellationToken">请求取消令牌。</param>
    Task ReleaseAsync(string externalRequestNo, CancellationToken cancellationToken);
}

public sealed class BudgetDecision
{
    public bool WithinBudget { get; set; }
    public bool Reserved { get; set; }
    public bool DirectlyPass { get; set; }
    public bool ReservationChanged { get; set; }
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
