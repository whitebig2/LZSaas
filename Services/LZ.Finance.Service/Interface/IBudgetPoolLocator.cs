using LZ.Model;
using SqlSugar;

namespace LZ.Finance.Service.Interface;

public interface IBudgetPoolLocator
{
    Task<Live_BudgetManage> LocateAsync(ISqlSugarClient main, int companyId,
        int userId, int costTypeId, string expenseName, DateTime now, CancellationToken cancellationToken);
}
