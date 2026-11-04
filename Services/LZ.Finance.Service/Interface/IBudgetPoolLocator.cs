using LZ.Model;
using SqlSugar;

namespace LZ.Finance.Service.Interface;

public interface IBudgetPoolLocator
{
    Task<FIN_Reimb_BudgetManage> LocateAsync(ISqlSugarClient finance, int companyId,
        int userId, int costTypeId, DateTime now, CancellationToken cancellationToken);
}
