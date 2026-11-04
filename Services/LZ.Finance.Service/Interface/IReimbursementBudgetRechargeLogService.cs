using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Finance.Service.Interface;

public class ReimbursementBudgetLogQuery : FinancePageModel
{
}

public interface IReimbursementBudgetRechargeLogService
{
    Task<(IList<FIN_Reimb_BudgetRechargeLog> items, int count)> ListAsync(ReimbursementBudgetLogQuery q, CancellationToken t = default);
    Task<int> AddAsync(FIN_Reimb_BudgetRechargeLog m, CancellationToken t = default);
    Task<FIN_Reimb_BudgetRechargeLog> GetAsync(int id, CancellationToken t = default);
    Task UpdateAsync(FIN_Reimb_BudgetRechargeLog m, CancellationToken t = default);
    Task DeleteAsync(int id, CancellationToken t = default);
    Task WritePoolFlowAsync(int poolId, byte actionType, decimal money, decimal add, decimal dec, string content, CancellationToken t = default);
}

