using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.Service.Services;

public sealed class ReimbursementBudgetPoolLocator : IBudgetPoolLocator, IScopedDependency
{
    private readonly IConfiguration? _configuration;

    public ReimbursementBudgetPoolLocator(IConfiguration? configuration = null) => _configuration = configuration;

    public async Task<FIN_Reimb_BudgetManage> LocateAsync(ISqlSugarClient finance, int companyId,
        int userId, int costTypeId, DateTime now, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var startMonth = _configuration?.GetValue<int?>("Reimbursement:Budget:FiscalYearStartMonth") ?? 4;
        var year = startMonth <= 1 ? now.Year : now.AddMonths(-(startMonth - 1)).Year;
        var person = await DbScoped.SugarScope.GetConnection("Main").Queryable<Live_Personnel>()
            .FirstAsync(x => x.CompanyID == companyId && x.UserID == userId && x.Enabled);
        var pools = await finance.Queryable<FIN_Reimb_BudgetManage>()
            .Where(x => x.CompanyID == companyId && x.Year == year && x.CostType == costTypeId && !x.Hidden)
            .ToListAsync();
        var departmentPools = pools.Where(x => x.PartnerGID.HasValue && x.PartnerGID == person?.PartnerGID).ToList();
        var matches = departmentPools.Count > 0 ? departmentPools :
            pools.Where(x => !x.PartnerGID.HasValue).ToList();
        if (matches.Count != 1)
            throw new InvalidOperationException(matches.Count == 0 ? "当前公司、成本类型及预算年度未配置预算池" :
                "当前公司、成本类型及预算年度匹配多个预算池");
        return matches[0];
    }
}
