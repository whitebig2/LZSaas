using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.Service.Services;

public class ReimbursementBudgetReportService : ServicesBase<FIN_Reimb_BudgetReport>,
    IReimbursementBudgetReportService, IScopedDependency
{
    private readonly ICurrentUser _user;
    private ISqlSugarClient Main => DbScoped.SugarScope.GetConnection("Main");

    public ReimbursementBudgetReportService(ICurrentUser user) => _user = user;

    public async Task<(IList<FIN_Reimb_BudgetReport> items, int count)> ListAsync(ReimbursementBudgetReportQuery q, CancellationToken t = default)
    {
        var exp = Expressionable.Create<FIN_Reimb_BudgetReport>();
        if (_user.PlatformType == PlatformType.Admin || _user.PlatformType == PlatformType.Api)
        {
            exp.AndIF(q.CompanyID.HasValue && q.CompanyID > 0, it => it.CompanyID == q.CompanyID);
        }
        else
        {
            exp.And(it => it.CompanyID == _user.CompanyID);
        }
        exp.AndIF(q.Year.HasValue, it => it.Year == q.Year);
        exp.AndIF(q.PartnerID.HasValue, it => it.PartnerID == q.PartnerID);
        exp.AndIF(q.PartnerGID.HasValue, it => it.PartnerGID == q.PartnerGID);

        var count = new RefAsync<int>();
        var page = q.Page <= 0 ? 1 : q.Page;
        var pageSize = q.PageSize <= 0 ? 15 : q.PageSize;

        var list = await Context.Queryable<FIN_Reimb_BudgetReport>()
            .Where(exp.ToExpression())
            .OrderByDescending(it => it.Year)
            .ToOffsetPageAsync(page, pageSize, count);

        return (list, count.Value);
    }

    public async Task AddAsync(FIN_Reimb_BudgetReport m, CancellationToken t = default)
    {
        var cid = _user.CompanyID;
        var cgid = _user.CompanyGID;

        var exists = await Context.Queryable<FIN_Reimb_BudgetReport>()
            .AnyAsync(t => t.CompanyID == cid && t.PartnerID == m.PartnerID && t.Year == m.Year);
        if (exists) throw new ErrorCodeException(-1, "该记录已存在!");

        m.CompanyID = cid;
        m.CompanyGID = cgid;
        m.CreateTime = DateTime.Now;
        await Context.Insertable(m).ExecuteCommandAsync();
    }

    public async Task<FIN_Reimb_BudgetReport> GetAsync(int id, CancellationToken t = default)
    {
        var model = await Context.Queryable<FIN_Reimb_BudgetReport>().InSingleAsync(id);
        if (model == null) throw new ErrorCodeException(-1, "你要读取的数据不存在");
        return model;
    }

    public async Task UpdateAsync(FIN_Reimb_BudgetReport m, CancellationToken t = default)
    {
        await Context.Updateable(m).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(int id, CancellationToken t = default)
    {
        await Context.Deleteable<FIN_Reimb_BudgetReport>().Where(it => it.ID == id).ExecuteCommandAsync();
    }

    public async Task<ExcelResult> CreateExcelAsync(FIN_Reimb_BudgetReport m, CancellationToken t = default)
    {
        var companyId = _user.CompanyID;
        var date = DateTime.Now.AddMonths(-1);
        string filename = date.ToString("yyyyMM") + "_" + m.PartnerName + "_经营报表";

        var configs = await Context.Queryable<FIN_Reimb_BudgetReportConfig>()
            .Where(t => t.CompanyID == companyId)
            .ToListAsync();

        var details = await Context.Queryable<FIN_Reimb_BudgetReportDetail>()
            .Where(t => t.CompanyID == companyId && t.Year == m.Year)
            .WhereIF(m.PartnerID.HasValue, t => t.PartnerID == m.PartnerID)
            .ToListAsync();

        int? month;
        if (details.Any(d => d.Month == 1))
        {
            month = details.Where(d => d.Month < 4).Max(d => d.Month);
        }
        else
        {
            month = details.Count > 0 ? details.Max(d => d.Month) : (int?)null;
        }

        if (month.HasValue)
        {
            int? y = month > 3 ? m.Year : m.Year + 1;
            string mm = month < 10 ? "0" + month : month.Value.ToString();
            filename = $"{y}{mm}_{m.PartnerName}_经营报表";
        }

        var profits = await Context.Queryable<Live_ProfitManage>()
            .Where(it => (it.ProfitMonth.Value.Year == m.Year && it.ProfitMonth.Value.Month > 3) ||
                         (it.ProfitMonth.Value.Year == m.Year + 1 && it.ProfitMonth.Value.Month < 4))
            .ToListAsync();

        // NPOI 生成 Excel 骨架与报表
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("Sheet1");
        var font = workbook.CreateFont();
        font.FontHeightInPoints = 9;
        font.FontName = "微软雅黑";
        font.IsBold = true;
        double width = 11 * 256;
        short height = 20 * 20;
        sheet.DefaultRowHeight = height;

        string[] heads1 = {
            "项目", "一季度","一季度","一季度","一季度", "二季度","二季度","二季度","二季度",
            "三季度","三季度","三季度","三季度", "四季度","四季度","四季度","四季度", "合计"
        };
        string[] heads2 = {
            "项目", "4月","5月","6月","小计", "7月","8月","9月","小计",
            "10月","11月","12月","小计", "1月","2月","3月","小计", "合计"
        };
        sheet.SetColumnWidth(0, 35 * 256);
        for (int x = 1; x < heads1.Length; x++) sheet.SetColumnWidth(x, (int)width);
        ExcelHeler.SetHeaderNoSetWidth(workbook, sheet, font, heads1, heads2, width, height);

        int rowIdx = 2;
        foreach (var c in configs)
        {
            var row = sheet.CreateRow(rowIdx++);
            var cell0 = row.CreateCell(0);
            cell0.SetCellValue(c.CostName);

            var cDetails = details.Where(d => d.BudgetReportConfigID == c.ID).ToList();
            int col = 1;
            int[] months = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3 };
            decimal rowTotal = 0;
            for (int q = 0; q < 4; q++)
            {
                decimal qSubtotal = 0;
                for (int mi = 0; mi < 3; mi++)
                {
                    var mon = months[q * 3 + mi];
                    var dVal = cDetails.FirstOrDefault(d => d.Month == mon)?.Money ?? 0;
                    var cell = row.CreateCell(col++);
                    cell.SetCellValue((double)dVal);
                    qSubtotal += dVal;
                }
                var subCell = row.CreateCell(col++);
                subCell.SetCellValue((double)qSubtotal);
                rowTotal += qSubtotal;
            }
            var totCell = row.CreateCell(col);
            totCell.SetCellValue((double)rowTotal);
        }

        byte[] buffer;
        using (var stream = new MemoryStream())
        {
            workbook.Write(stream);
            buffer = stream.ToArray();
        }

        return new ExcelResult
        {
            Base64String = Convert.ToBase64String(buffer),
            FileName = filename + ".xlsx"
        };
    }

    public async Task<object> CreatePartnerAsync(CancellationToken t = default)
    {
        var companyId = _user.CompanyID;
        var year = DateTime.Now.AddMonths(-3).Year;

        // 直查 Main 库 Live_Partner (Enabled && IsBudget)
        var partners = await Main.Queryable<Live_Partner>()
            .Where(p => p.CompanyID == companyId && p.Enabled && p.IsBudget)
            .ToListAsync();

        var existing = await Context.Queryable<FIN_Reimb_BudgetReport>()
            .Where(t => t.CompanyID == companyId && t.Year == year)
            .ToListAsync();

        var list = new List<FIN_Reimb_BudgetReport>();
        foreach (var x in partners)
        {
            if (existing.Any(e => e.PartnerID == x.PartnerID)) continue;

            list.Add(new FIN_Reimb_BudgetReport
            {
                Year = year,
                PartnerID = x.PartnerID,
                PartnerGID = x.PartnerGID,
                PartnerName = x.PartnerName,
                CompanyID = x.CompanyID,
                CompanyGID = x.CompanyGID,
                CreateTime = DateTime.Now
            });
        }
        if (list.Count > 0)
        {
            await Context.Insertable(list).ExecuteCommandAsync();
        }
        return new { items = partners };
    }
}

