using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using SqlSugar;

namespace LZ.Finance.Service.Services;

public sealed class ReimbursementBudgetPoolLocator : IBudgetPoolLocator, IScopedDependency
{
    public async Task<Live_BudgetManage> LocateAsync(ISqlSugarClient main, int companyId,
        int userId, int costTypeId, string expenseName, DateTime now, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var year = now.AddMonths(-3).Year;
        var person = await main.Queryable<Live_Personnel>()
            .FirstAsync(x => x.CompanyID == companyId && x.UserID == userId && x.Enabled);
        if (person?.PartnerGID is not Guid partnerGid)
            throw new InvalidOperationException("当前用户未绑定老预算服务商");
        var pools = await main.Queryable<Live_BudgetManage>()
            .Where(x => x.CompanyID == companyId && x.Year == year && x.PartnerGID == partnerGid &&
                        x.CostType == costTypeId && x.ExpensesTitle == expenseName && !x.Hidden)
            .ToListAsync();
        if (pools.Count != 1)
            throw new InvalidOperationException(pools.Count == 0 ? "当前企业、服务商、费用及预算年度未配置老预算池" :
                "当前企业、服务商、费用及预算年度匹配多个老预算池");
        return pools[0];
    }
}
