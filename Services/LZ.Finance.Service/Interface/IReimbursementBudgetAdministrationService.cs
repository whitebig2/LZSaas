using LZ.Model;
using LZ.Model.Dtos.Reimbursement;

namespace LZ.Finance.Service.Interface;

public sealed class ReimbursementBudgetQuery
{
    public int? Year { get; set; }
    public int? CostType { get; set; }
    public Guid? PartnerGid { get; set; }
    public string? Name { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public sealed class ReimbursementBudgetConfigInput
{
    public string Title { get; set; } = string.Empty;
    public int CostType { get; set; }
    public int GenerateType { get; set; }
    public decimal Percentage { get; set; }
    public bool Enable { get; set; }
    public bool DirectlyPass { get; set; }
    public bool PublicUse { get; set; }
    public bool IsRelate { get; set; }
    public List<Guid> PartnerGids { get; set; } = new();
}

public sealed class ReimbursementBudgetAmountInput
{
    public decimal Amount { get; set; }
}

public sealed class ReimbursementBudgetPoolInput
{
    public decimal Percentage { get; set; }
    public bool DirectlyPass { get; set; }
    public bool PublicUse { get; set; }
    public bool Hidden { get; set; }
}

public sealed class ReimbursementBudgetTransferInput
{
    public int SourceId { get; set; }
    public int TargetPoolId { get; set; }
    public decimal Amount { get; set; }
}

public interface IReimbursementBudgetAdministrationService
{
    Task<ApplicationPageDto<FIN_Reimb_BudgetConfig>> ListConfigsAsync(ReimbursementBudgetQuery query, CancellationToken token = default);
    Task<FIN_Reimb_BudgetConfig?> GetConfigAsync(int id, CancellationToken token = default);
    Task<FIN_Reimb_BudgetConfig> CreateConfigAsync(ReimbursementBudgetConfigInput input, CancellationToken token = default);
    Task<FIN_Reimb_BudgetConfig> UpdateConfigAsync(int id, ReimbursementBudgetConfigInput input, CancellationToken token = default);
    Task DeleteConfigAsync(int id, CancellationToken token = default);
    Task<ApplicationPageDto<FIN_Reimb_BudgetManage>> ListPoolsAsync(ReimbursementBudgetQuery query, CancellationToken token = default);
    Task<FIN_Reimb_BudgetManage?> GetPoolAsync(int id, CancellationToken token = default);
    Task<FIN_Reimb_BudgetManage?> GetViewAsync(int id, CancellationToken token = default);
    Task<FIN_Reimb_BudgetManage?> GetFuncAsync(Guid partnerGid, string expensesTitle, int costType, int year, CancellationToken token = default);
    Task AddPoolAsync(FIN_Reimb_BudgetManage model, CancellationToken token = default);
    Task<int> BatchCreatePoolsAsync(int? year, CancellationToken token = default);
    Task<FIN_Reimb_BudgetManage> UpdatePoolAsync(int id, ReimbursementBudgetPoolInput input, CancellationToken token = default);
    Task RechargeAsync(int id, decimal amount, CancellationToken token = default);
    Task WithdrawAsync(int id, decimal amount, CancellationToken token = default);
    Task TransferAsync(int id, ReimbursementBudgetTransferInput input, CancellationToken token = default);
    Task SetPoolHiddenAsync(int id, bool hidden, CancellationToken token = default);
    Task DeletePoolAsync(int id, CancellationToken token = default);
    Task<ExcelResult> ExportPoolsAsync(ReimbursementBudgetQuery q, CancellationToken token = default);
    Task DeleteNotBudgetAsync(CancellationToken token = default);
    Task<FIN_Reimb_BudgetReservation?> GetReservationAsync(Guid id, CancellationToken token = default);
    Task<FIN_Reimb_BudgetReservation?> GetReservationByApplyAsync(Guid applyGuid, CancellationToken token = default);
}
