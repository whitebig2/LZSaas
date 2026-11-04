using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using SqlSugar;

namespace LZ.Finance.Service.Services;

public class ReimbursementBudgetReportConfigService : ServicesBase<FIN_Reimb_BudgetReportConfig>,
    IReimbursementBudgetReportConfigService, IScopedDependency
{
    private readonly ICurrentUser _user;

    public ReimbursementBudgetReportConfigService(ICurrentUser user) => _user = user;

    public async Task<(IList<FIN_Reimb_BudgetReportConfig> items, int count)> ListAsync(ReimbursementBudgetReportConfigQuery q, CancellationToken t = default)
    {
        var exp = Expressionable.Create<FIN_Reimb_BudgetReportConfig>();
        if (_user.PlatformType == PlatformType.Admin || _user.PlatformType == PlatformType.Api)
        {
            exp.AndIF(q.CompanyID.HasValue && q.CompanyID > 0, it => it.CompanyID == q.CompanyID);
        }
        else
        {
            exp.And(it => it.CompanyID == _user.CompanyID);
        }
        exp.AndIF(q.CID.HasValue, it => it.CID == q.CID);

        var count = new RefAsync<int>();
        var page = q.Page <= 0 ? 1 : q.Page;
        var pageSize = q.PageSize <= 0 ? 15 : q.PageSize;

        var list = await Context.Queryable<FIN_Reimb_BudgetReportConfig>()
            .Where(exp.ToExpression())
            .ToOffsetPageAsync(page, pageSize, count);

        return (list, count.Value);
    }

    public async Task AddAsync(List<FIN_Reimb_BudgetReportConfig> list, CancellationToken t = default)
    {
        if (list == null || list.Count == 0) throw new ErrorCodeException(-1, "没有数据");

        var cid = _user.CompanyID;
        var cgid = _user.CompanyGID;
        var firstCid = list[0].CID;

        await Context.Deleteable<FIN_Reimb_BudgetReportConfig>()
            .Where(t => t.CompanyID == cid && t.CID == firstCid)
            .ExecuteCommandAsync();

        var models = new List<FIN_Reimb_BudgetReportConfig>();
        foreach (var x in list)
        {
            if (string.IsNullOrEmpty(x.CostName)) continue;
            x.CompanyID = cid;
            x.CompanyGID = cgid;
            models.Add(x);
        }
        if (models.Count == 0) throw new ErrorCodeException(-1, "没有数据");
        await Context.Insertable(models).ExecuteCommandAsync();
    }

    public async Task<FIN_Reimb_BudgetReportConfig> GetAsync(int id, CancellationToken t = default)
    {
        var model = await Context.Queryable<FIN_Reimb_BudgetReportConfig>().InSingleAsync(id);
        if (model == null) throw new ErrorCodeException(-1, "你要读取的数据不存在");
        return model;
    }

    public async Task UpdateAsync(FIN_Reimb_BudgetReportConfig m, CancellationToken t = default)
    {
        await Context.Updateable(m).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(int id, CancellationToken t = default)
    {
        await Context.Deleteable<FIN_Reimb_BudgetReportConfig>().Where(it => it.ID == id).ExecuteCommandAsync();
    }
}

