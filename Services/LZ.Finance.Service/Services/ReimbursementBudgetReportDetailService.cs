using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Budget;
using LZ.Model.Enums;
using LZ.Model.Enums.Budget;
using LZ.Model.Exceptions;
using SqlSugar;

namespace LZ.Finance.Service.Services;

public class ReimbursementBudgetReportDetailService : ServicesBase<FIN_Reimb_BudgetReportDetail>,
    IReimbursementBudgetReportDetailService, IScopedDependency
{
    private readonly ICurrentUser _user;

    public ReimbursementBudgetReportDetailService(ICurrentUser user) => _user = user;

    public async Task<(IList<FIN_Reimb_BudgetReportDetail> items, int count)> ListAsync(ReimbursementBudgetReportDetailQuery q, CancellationToken t = default)
    {
        var exp = Expressionable.Create<FIN_Reimb_BudgetReportDetail>();
        if (_user.PlatformType == PlatformType.Admin || _user.PlatformType == PlatformType.Api)
        {
            exp.AndIF(q.CompanyID.HasValue && q.CompanyID > 0, it => it.CompanyID == q.CompanyID);
        }
        else
        {
            exp.And(it => it.CompanyID == _user.CompanyID);
        }
        exp.AndIF(q.Year.HasValue, it => it.Year == q.Year);
        exp.AndIF(q.Month.HasValue, it => it.Month == q.Month);
        exp.AndIF(q.QuarterType.HasValue, it => it.QuarterType == q.QuarterType);
        exp.AndIF(q.PartnerID.HasValue, it => it.PartnerID == q.PartnerID);
        exp.AndIF(q.PartnerGID.HasValue, it => it.PartnerGID == q.PartnerGID);

        var count = new RefAsync<int>();
        var page = q.Page <= 0 ? 1 : q.Page;
        var pageSize = q.PageSize <= 0 ? 15 : q.PageSize;

        var list = await Context.Queryable<FIN_Reimb_BudgetReportDetail>()
            .Where(exp.ToExpression())
            .ToOffsetPageAsync(page, pageSize, count);

        return (list, count.Value);
    }

    public async Task AddAsync(FIN_Reimb_BudgetReportDetail m, CancellationToken t = default)
    {
        m.CompanyID = _user.CompanyID;
        m.CompanyGID = _user.CompanyGID;
        m.CreateTime = DateTime.Now;
        await Context.Insertable(m).ExecuteCommandAsync();
    }

    public async Task<FIN_Reimb_BudgetReportDetail> GetAsync(int id, CancellationToken t = default)
    {
        var model = await Context.Queryable<FIN_Reimb_BudgetReportDetail>().InSingleAsync(id);
        if (model == null) throw new ErrorCodeException(-1, "你要读取的数据不存在");
        return model;
    }

    public async Task UpdateAsync(FIN_Reimb_BudgetReportDetail m, CancellationToken t = default)
    {
        await Context.Updateable(m).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(int id, CancellationToken t = default)
    {
        await Context.Deleteable<FIN_Reimb_BudgetReportDetail>().Where(it => it.ID == id).ExecuteCommandAsync();
    }

    public async Task<object> CreateBillAsync(RecordMonthModel model, CancellationToken t = default)
    {
        var companyId = _user.CompanyID;
        int year = model.Year;
        if (model.Month == 1 || model.Month == 2 || model.Month == 3)
        {
            year = model.Year + 1;
        }

        int quarterType = model.Month switch
        {
            4 or 5 or 6 => (int)BudgetQuarterTypeEnum.第一季度,
            7 or 8 or 9 => (int)BudgetQuarterTypeEnum.第二季度,
            10 or 11 or 12 => (int)BudgetQuarterTypeEnum.第三季度,
            _ => (int)BudgetQuarterTypeEnum.第四季度
        };

        var applyDetails = await Context.Queryable<FIN_Reimb_BudgetApplyRecordDetail>()
            .Where(d => d.CompanyID == companyId)
            .ToListAsync();

        var configs = await Context.Queryable<FIN_Reimb_BudgetReportConfig>()
            .Where(t => t.CompanyID == companyId)
            .ToListAsync();

        var partners = await Context.Queryable<FIN_Reimb_BudgetReport>()
            .Where(t => t.CompanyID == companyId && t.Year == model.Year)
            .WhereIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID)
            .ToListAsync();

        var existingDetails = await Context.Queryable<FIN_Reimb_BudgetReportDetail>()
            .Where(t => t.CompanyID == companyId && t.Year == model.Year && t.QuarterType == quarterType)
            .WhereIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID)
            .ToListAsync();

        var detailsAdd = new List<FIN_Reimb_BudgetReportDetail>();
        var detailsUpdate = new List<FIN_Reimb_BudgetReportDetail>();

        foreach (var partner in partners)
        {
            foreach (var item in configs)
            {
                var exists = existingDetails.Any(x => x.BudgetReportID == partner.ID && x.BudgetReportConfigID == item.ID && x.Month == model.Month);
                if (exists) continue;

                var detail = new FIN_Reimb_BudgetReportDetail
                {
                    CreateTime = DateTime.Now,
                    CompanyID = partner.CompanyID,
                    CompanyGID = partner.CompanyGID,
                    BudgetReportID = partner.ID,
                    BudgetReportConfigID = item.ID,
                    PartnerID = partner.PartnerID,
                    PartnerGID = partner.PartnerGID,
                    Title = item.CostName,
                    Money = 0,
                    Year = model.Year,
                    Month = model.Month,
                    MonthType = (model.Month + 2) % 3,
                    QuarterType = quarterType,
                    QuarterName = ((BudgetQuarterTypeEnum)quarterType).ToString()
                };
                detail.MonthName = ((BudgetMonthTypeEnum)detail.MonthType).ToString();

                var matched = applyDetails.Where(d => d.PartnerGID == partner.PartnerGID).ToList();
                if (matched.Count > 0)
                {
                    detail.Money = matched.Sum(m => m.Money);
                }

                // 月度小计
                var xiaoji = existingDetails.Find(x => x.PartnerID == partner.PartnerID && x.BudgetReportConfigID == item.ID && x.MonthType == 3);
                if (xiaoji != null)
                {
                    var arr = existingDetails.Where(x => x.PartnerID == partner.PartnerID && x.BudgetReportConfigID == item.ID && x.MonthType != 3);
                    xiaoji.Money = arr.Sum(x => x.Money) + detail.Money;
                    detailsUpdate.Add(xiaoji);
                }
                else
                {
                    var detailNew = new FIN_Reimb_BudgetReportDetail
                    {
                        CreateTime = DateTime.Now,
                        CompanyID = detail.CompanyID,
                        CompanyGID = detail.CompanyGID,
                        BudgetReportID = detail.BudgetReportID,
                        BudgetReportConfigID = detail.BudgetReportConfigID,
                        PartnerID = detail.PartnerID,
                        PartnerGID = detail.PartnerGID,
                        Title = "小计",
                        Money = detail.Money,
                        Year = detail.Year,
                        Month = 0,
                        MonthType = 3,
                        QuarterType = detail.QuarterType,
                        QuarterName = detail.QuarterName,
                        MonthName = ((BudgetMonthTypeEnum)3).ToString()
                    };
                    detailsAdd.Add(detailNew);
                }
                detailsAdd.Add(detail);
            }
        }

        if (detailsUpdate.Count > 0) await Context.Updateable(detailsUpdate).ExecuteCommandAsync();
        if (detailsAdd.Count > 0) await Context.Insertable(detailsAdd).ExecuteCommandAsync();

        return new { detailsUpdate, detailsAdd };
    }

    public async Task DeleteMonthBillAsync(RecordMonthModel model, CancellationToken t = default)
    {
        var companyId = _user.CompanyID;
        int quarterType = model.Month switch
        {
            4 or 5 or 6 => (int)BudgetQuarterTypeEnum.第一季度,
            7 or 8 or 9 => (int)BudgetQuarterTypeEnum.第二季度,
            10 or 11 or 12 => (int)BudgetQuarterTypeEnum.第三季度,
            _ => (int)BudgetQuarterTypeEnum.第四季度
        };

        await Context.Deleteable<FIN_Reimb_BudgetReportDetail>()
            .Where(t => t.CompanyID == companyId && t.Year == model.Year && t.Month == model.Month)
            .WhereIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID)
            .ExecuteCommandAsync();

        var details = await Context.Queryable<FIN_Reimb_BudgetReportDetail>()
            .Where(t => t.CompanyID == companyId && t.Year == model.Year && t.QuarterType == quarterType)
            .WhereIF(model.PartnerID.HasValue, t => t.PartnerID == model.PartnerID)
            .ToListAsync();

        var xiaojiList = details.Where(t => t.MonthType == (int)BudgetMonthTypeEnum.季度小计).ToList();
        var detailsUpdate = new List<FIN_Reimb_BudgetReportDetail>();
        foreach (var x in xiaojiList)
        {
            var arr = details.Where(t => t.PartnerID == x.PartnerID && t.BudgetReportConfigID == x.BudgetReportConfigID && t.MonthType != 3);
            x.Money = arr.Sum(d => d.Money);
            detailsUpdate.Add(x);
        }
        if (detailsUpdate.Count > 0)
        {
            await Context.Updateable(detailsUpdate).ExecuteCommandAsync();
        }
    }

    public async Task<IList<RecordMonthModel>> ReportMonthListAsync(RecordMonthModel info, CancellationToken t = default)
    {
        var companyId = _user.CompanyID;
        var now = DateTime.Now;
        now = new DateTime(now.Year, now.Month, 1);
        var models = new List<RecordMonthModel>();

        var date = new DateTime(info.Year, 4, 1);
        int month = date.Month;
        int syear = date.AddMonths(-3).Year;

        var list = await Context.Queryable<FIN_Reimb_BudgetReportDetail>()
            .Where(t => t.CompanyID == companyId && t.Year == info.Year)
            .WhereIF(info.PartnerID.HasValue, t => t.PartnerID == info.PartnerID)
            .ToListAsync();

        while (info.Year == syear)
        {
            var model = new RecordMonthModel
            {
                Year = info.Year,
                Month = month,
                PartnerName = info.PartnerName,
                PartnerID = info.PartnerID,
                PartnerGID = info.PartnerGID
            };
            var items = list.Where(t => t.Month == month).ToList();
            model.Exist = items.Count > 0;
            model.IsTimeCheck = now >= date;
            models.Add(model);

            date = date.AddMonths(1);
            month = date.Month;
            syear = date.AddMonths(-3).Year;
        }

        return models;
    }
}

