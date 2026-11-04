using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using SqlSugar;

namespace LZ.Finance.Service.Services;

public class ReimbursementBudgetRechargeLogService : ServicesBase<FIN_Reimb_BudgetRechargeLog>,
    IReimbursementBudgetRechargeLogService, IScopedDependency
{
    private readonly ICurrentUser _user;

    public ReimbursementBudgetRechargeLogService(ICurrentUser user) => _user = user;

    public async Task<(IList<FIN_Reimb_BudgetRechargeLog> items, int count)> ListAsync(ReimbursementBudgetLogQuery q, CancellationToken t = default)
    {
        var exp = Expressionable.Create<FIN_Reimb_BudgetRechargeLog>();
        if (_user.PlatformType == PlatformType.Admin || _user.PlatformType == PlatformType.Api)
        {
            exp.AndIF(q.CompanyID.HasValue && q.CompanyID > 0, it => it.CompanyID == q.CompanyID);
        }
        else
        {
            exp.And(it => it.CompanyID == _user.CompanyID);
        }
        exp.AndIF(q.CID.HasValue, it => it.CID == q.CID);
        exp.AndIF(q.Year.HasValue, it => it.Year == q.Year);

        var count = new RefAsync<int>();
        var page = q.Page <= 0 ? 1 : q.Page;
        var pageSize = q.PageSize <= 0 ? 15 : q.PageSize;

        var list = await Context.Queryable<FIN_Reimb_BudgetRechargeLog>()
            .Where(exp.ToExpression())
            .OrderByDescending(it => it.CreateTime)
            .ToOffsetPageAsync(page, pageSize, count);

        return (list, count.Value);
    }

    public async Task<int> AddAsync(FIN_Reimb_BudgetRechargeLog m, CancellationToken t = default)
    {
        m.CompanyID = _user.CompanyID;
        m.CompanyGID = _user.CompanyGID;
        m.CreateTime = DateTime.Now;

        if (m.CID.HasValue && m.CID > 0)
        {
            var manage = await Context.Queryable<FIN_Reimb_BudgetManage>().InSingleAsync(m.CID.Value);
            if (manage != null)
            {
                m.Year = manage.Year;
            }
        }
        return await Context.Insertable(m).ExecuteReturnIdentityAsync();
    }

    public async Task<FIN_Reimb_BudgetRechargeLog> GetAsync(int id, CancellationToken t = default)
    {
        var model = await Context.Queryable<FIN_Reimb_BudgetRechargeLog>().InSingleAsync(id);
        if (model == null) throw new ErrorCodeException(-1, "你要读取的数据不存在");
        return model;
    }

    public async Task UpdateAsync(FIN_Reimb_BudgetRechargeLog m, CancellationToken t = default)
    {
        await Context.Updateable(m).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(int id, CancellationToken t = default)
    {
        await Context.Deleteable<FIN_Reimb_BudgetRechargeLog>().Where(it => it.ID == id).ExecuteCommandAsync();
    }

    public async Task WritePoolFlowAsync(int poolId, byte actionType, decimal money, decimal add, decimal dec, string content, CancellationToken t = default)
    {
        var manage = await Context.Queryable<FIN_Reimb_BudgetManage>().InSingleAsync(poolId);
        if (manage == null) return;

        var log = new FIN_Reimb_BudgetRechargeLog
        {
            CompanyID = manage.CompanyID,
            CompanyGID = manage.CompanyGID,
            CID = poolId,
            Year = manage.Year,
            UserGID = Guid.Empty,
            UserName = _user.FullName,
            Content = content,
            CreateTime = DateTime.Now,
            Money = money,
            PartnerName = manage.PartnerName,
            Title = manage.ExpensesTitle,
            Type = actionType,
            TotalAmount = manage.TotalAmount,
            LockAmount = manage.LockAmount,
            UsedAmount = manage.UsedAmount,
            AvailableAmount = manage.AvailableAmount,
            HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount,
            AddAmount = add,
            DecreaseAmount = dec
        };
        await Context.Insertable(log).ExecuteCommandAsync();
    }
}
