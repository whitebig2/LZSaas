using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Finance.Service.Interface;

public class ReimbursementBudgetReportConfigQuery : FinancePageModel
{
}

public interface IReimbursementBudgetReportConfigService
{
    Task<(IList<FIN_Reimb_BudgetReportConfig> items, int count)> ListAsync(ReimbursementBudgetReportConfigQuery q, CancellationToken t = default);
    Task AddAsync(List<FIN_Reimb_BudgetReportConfig> list, CancellationToken t = default);
    Task<FIN_Reimb_BudgetReportConfig> GetAsync(int id, CancellationToken t = default);
    Task UpdateAsync(FIN_Reimb_BudgetReportConfig m, CancellationToken t = default);
    Task DeleteAsync(int id, CancellationToken t = default);
}

