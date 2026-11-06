using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.Reimbursement;
using LZ.Model.Enums;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.Service.Services;

public sealed class ReimbursementBudgetAdministrationService : ServicesBase<FIN_Reimb_BudgetConfig>,
    IReimbursementBudgetAdministrationService, IScopedDependency
{
    private readonly ICurrentUser _user;
    private readonly IConfiguration? _configuration;
    private ISqlSugarClient Main => DbScoped.SugarScope.GetConnection("Main");

    public ReimbursementBudgetAdministrationService(ICurrentUser user, IConfiguration? configuration = null)
    {
        _user = user;
        _configuration = configuration;
    }

    /// <summary>
    /// 获取预算年度。默认规则：当年4月到次年3月归为当年年度(DateTime.Now.AddMonths(-3).Year)；
    /// 若配置 Reimbursement:Budget:FiscalYearStartMonth 为 1，则为自然年(1月到12月)。
    /// </summary>
    private int GetDefaultBudgetYear(DateTime? date = null)
    {
        var targetDate = date ?? DateTime.Now;
        var startMonth = _configuration?.GetValue<int?>("Reimbursement:Budget:FiscalYearStartMonth") ?? 4;
        if (startMonth <= 1)
        {
            return targetDate.Year;
        }
        return targetDate.AddMonths(-(startMonth - 1)).Year;
    }

    private int CompanyId => _user.IsAuthenticated && _user.CompanyID > 0 && _user.UserID > 0 &&
        (_user.PlatformType == PlatformType.MiniAdmin || _user.PlatformType == PlatformType.Admin || _user.PlatformType == PlatformType.LZMini) ? _user.CompanyID :
        throw new UnauthorizedAccessException("仅当前企业人员可管理报销预算");

    public async Task<ApplicationPageDto<FIN_Reimb_BudgetConfig>> ListConfigsAsync(ReimbursementBudgetQuery query,
        CancellationToken token = default)
    {
        var companyId = CompanyId;
        ValidatePage(query, token);
        var count = new RefAsync<int>();
        var name = query.Name?.Trim();
        var items = await Context.Queryable<FIN_Reimb_BudgetConfig>()
            .Where(x => x.CompanyID == companyId)
            .WhereIF(query.CostType.HasValue, x => x.CostType == query.CostType)
            .WhereIF(!string.IsNullOrEmpty(name), x => x.Title.Contains(name!))
            .OrderBy(x => x.ID, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, count);
        return new ApplicationPageDto<FIN_Reimb_BudgetConfig>
        { PageIndex = query.PageIndex, PageSize = query.PageSize, TotalCount = count.Value, Items = items };
    }

    public async Task<FIN_Reimb_BudgetConfig?> GetConfigAsync(int id, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        return await Context.Queryable<FIN_Reimb_BudgetConfig>()
            .FirstAsync(x => x.ID == id && x.CompanyID == companyId);
    }

    public async Task<FIN_Reimb_BudgetConfig> CreateConfigAsync(ReimbursementBudgetConfigInput input,
        CancellationToken token = default)
    {
        var companyId = CompanyId;
        await Context.Ado.BeginTranAsync();
        try
        {
            var partners = await ValidateConfigAsync(input, null, companyId, token);
            var config = new FIN_Reimb_BudgetConfig
            {
                GID = Guid.NewGuid(),
                CompanyID = companyId,
                CompanyGID = _user.CompanyGID,
                CreateTime = DateTime.Now
            };
            CopyConfig(input, partners, config);
            config.ID = await Context.Insertable(config).ExecuteReturnIdentityAsync();
            await Context.Ado.CommitTranAsync();
            return config;
        }
        catch { await Context.Ado.RollbackTranAsync(); throw; }
    }

    public async Task<FIN_Reimb_BudgetConfig> UpdateConfigAsync(int id,
        ReimbursementBudgetConfigInput input, CancellationToken token = default)
    {
        var companyId = CompanyId;
        Context.Ado.BeginTran();
        try
        {
            var config = await GetConfigAsync(id, token) ?? throw new KeyNotFoundException("预算配置不存在");
            var partners = await ValidateConfigAsync(input, id, companyId, token);
            var hasPools = await Context.Queryable<FIN_Reimb_BudgetManage>()
                .AnyAsync(x => x.CompanyID == companyId && x.CostType == config.CostType &&
                               x.ExpensesTitle == config.Title);
            if (hasPools && (config.CostType != input.CostType || config.Title != input.Title.Trim() ||
                config.IsRelate != input.IsRelate ||
                (config.PartnerIDS ?? string.Empty) != string.Join(',', partners.Select(x => x.PartnerGID))))
                throw new InvalidOperationException("已生成预算池，不能修改配置的定位字段");
            CopyConfig(input, partners, config);
            var changed = await Context.Updateable(config)
                .UpdateColumns(x => new
                {
                    x.Title,
                    x.CostType,
                    x.GenerateType,
                    x.Percentage,
                    x.Enable,
                    x.DirectlyPass,
                    x.PublicUse,
                    x.IsRelate,
                    x.PartnerIDS,
                    x.PartnerNames
                })
                .Where(x => x.ID == id && x.CompanyID == companyId).ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("预算配置已变化，请刷新后重试");
            Context.Ado.CommitTran();
            return config;
        }
        catch { Context.Ado.RollbackTran(); throw; }
    }

    public async Task DeleteConfigAsync(int id, CancellationToken token = default)
    {
        var companyId = CompanyId;
        var config = await GetConfigAsync(id, token) ?? throw new KeyNotFoundException("预算配置不存在");
        var hasPools = await Context.Queryable<FIN_Reimb_BudgetManage>()
            .AnyAsync(x => x.CompanyID == companyId && x.CostType == config.CostType &&
                           x.ExpensesTitle == config.Title);
        if (hasPools) throw new InvalidOperationException("配置已生成预算池，不能删除");
        await Context.Deleteable<FIN_Reimb_BudgetConfig>()
            .Where(x => x.ID == id && x.CompanyID == companyId).ExecuteCommandAsync();
    }

    public async Task<ApplicationPageDto<FIN_Reimb_BudgetManage>> ListPoolsAsync(ReimbursementBudgetQuery query,
        CancellationToken token = default)
    {
        var companyId = CompanyId;
        ValidatePage(query, token);
        var count = new RefAsync<int>();
        var name = query.Name?.Trim();
        var items = await Context.Queryable<FIN_Reimb_BudgetManage>()
            .Where(x => x.CompanyID == companyId)
            .WhereIF(query.Year.HasValue, x => x.Year == query.Year)
            .WhereIF(query.CostType.HasValue, x => x.CostType == query.CostType)
            .WhereIF(query.PartnerGid.HasValue, x => x.PartnerGID == query.PartnerGid)
            .WhereIF(!string.IsNullOrEmpty(name), x => x.ExpensesTitle.Contains(name!))
            .OrderBy(x => x.ID, OrderByType.Desc)
            .ToPageListAsync(query.PageIndex, query.PageSize, count);
        return new ApplicationPageDto<FIN_Reimb_BudgetManage>
        { PageIndex = query.PageIndex, PageSize = query.PageSize, TotalCount = count.Value, Items = items };
    }

    public async Task<FIN_Reimb_BudgetManage?> GetPoolAsync(int id, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        return await Context.Queryable<FIN_Reimb_BudgetManage>()
            .FirstAsync(x => x.ID == id && x.CompanyID == companyId);
    }

    public async Task<FIN_Reimb_BudgetManage?> GetViewAsync(int id, CancellationToken token = default)
    {
        return await GetPoolAsync(id, token);
    }

    public async Task<FIN_Reimb_BudgetManage?> GetFuncAsync(Guid partnerGid, string expensesTitle, int costType, int year, CancellationToken token = default)
    {
        var companyId = CompanyId;
        return await Context.Queryable<FIN_Reimb_BudgetManage>()
            .FirstAsync(x => x.CompanyID == companyId && x.PartnerGID == partnerGid &&
                             x.ExpensesTitle == expensesTitle && x.CostType == costType && x.Year == year);
    }

    public async Task AddPoolAsync(FIN_Reimb_BudgetManage model, CancellationToken token = default)
    {
        model.CompanyID = CompanyId;
        model.CompanyGID = _user.CompanyGID;
        model.CreateTime = DateTime.Now;
        await Context.Insertable(model).ExecuteCommandAsync();
    }

    public async Task<int> BatchCreatePoolsAsync(int? year, CancellationToken token = default)
    {
        var companyId = CompanyId;
        token.ThrowIfCancellationRequested();
        var budgetYear = year ?? GetDefaultBudgetYear();
        if (budgetYear is < 2000 or > 2100) throw new ArgumentException("预算年度无效");
        var partners = await Main.Queryable<Live_Partner>()
            .Where(x => x.CompanyID == companyId && x.Enabled && x.IsBudget && x.PartnerGID != null)
            .ToListAsync();
        var configs = await Context.Queryable<FIN_Reimb_BudgetConfig>()
            .Where(x => x.CompanyID == companyId && x.Enable && x.IsRelate).ToListAsync();
        if (configs.Count > 0 && partners.Count == 0)
            throw new InvalidOperationException("当前公司没有启用预算的部门，无法生成预算池");
        var desired = new List<FIN_Reimb_BudgetManage>();
        foreach (var config in configs)
        {
            var allowed = ParsePartnerIds(config.PartnerIDS);
            foreach (var partner in partners.Where(p => allowed.Contains(p.PartnerGID!.Value)))
            {
                desired.Add(new FIN_Reimb_BudgetManage
                {
                    CompanyID = companyId,
                    CompanyGID = _user.CompanyGID,
                    Year = budgetYear,
                    PartnerID = partner.PartnerID,
                    PartnerGID = partner.PartnerGID,
                    PartnerName = partner.PartnerName,
                    CostType = config.CostType,
                    GenerateType = config.GenerateType,
                    ExpensesTitle = config.Title,
                    Percentage = config.Percentage,
                    DirectlyPass = config.DirectlyPass,
                    PublicUse = config.PublicUse,
                    CreateTime = DateTime.Now
                });
            }
        }
        if (desired.GroupBy(x => (x.CostType, x.PartnerGID)).Any(g => g.Count() > 1))
            throw new InvalidOperationException("同一部门、成本类型匹配多个启用配置，无法生成唯一预算池");
        await Context.Ado.BeginTranAsync();
        try
        {
            var existing = await Context.Queryable<FIN_Reimb_BudgetManage>()
                .With(SqlWith.TabLockX + "," + SqlWith.HoldLock)
                .Where(x => x.CompanyID == companyId && x.Year == budgetYear).ToListAsync();
            var count = 0;
            foreach (var pool in desired)
            {
                if (existing.Any(x => x.CostType == pool.CostType && x.PartnerGID == pool.PartnerGID))
                    continue;
                await Context.Insertable(pool).ExecuteCommandAsync();
                count++;
            }
            await Context.Ado.CommitTranAsync();
            return count;
        }
        catch { await Context.Ado.RollbackTranAsync(); throw; }
    }

    public async Task<FIN_Reimb_BudgetManage> UpdatePoolAsync(int id, ReimbursementBudgetPoolInput input,
        CancellationToken token = default)
    {
        var companyId = CompanyId;
        token.ThrowIfCancellationRequested();
        if (input is null || input.Percentage is < 0 or > 100)
            throw new ArgumentException("预算池参数无效");
        var pool = await GetPoolAsync(id, token) ?? throw new KeyNotFoundException("预算池不存在");
        var changed = await Context.Updateable<FIN_Reimb_BudgetManage>()
            .SetColumns(x => x.Percentage == input.Percentage)
            .SetColumns(x => x.DirectlyPass == input.DirectlyPass)
            .SetColumns(x => x.PublicUse == input.PublicUse)
            .SetColumns(x => x.Hidden == input.Hidden)
            .Where(x => x.ID == id && x.CompanyID == companyId && (!input.Hidden || x.LockAmount == 0))
            .ExecuteCommandAsync();
        if (changed != 1) throw new InvalidOperationException("预算池已变化或存在未结清预留");
        pool.Percentage = input.Percentage;
        pool.DirectlyPass = input.DirectlyPass;
        pool.PublicUse = input.PublicUse;
        pool.Hidden = input.Hidden;
        return pool;
    }

    public async Task RechargeAsync(int id, decimal amount, CancellationToken token = default)
    {
        var companyId = CompanyId;
        ValidateAmount(amount, token);
        var changed = await Context.Updateable<FIN_Reimb_BudgetManage>()
            .SetColumns(x => x.TotalAmount == x.TotalAmount + amount)
            .SetColumns(x => x.AvailableAmount == x.AvailableAmount + amount)
            .Where(x => x.ID == id && x.CompanyID == companyId && !x.Hidden).ExecuteCommandAsync();
        if (changed != 1) throw new KeyNotFoundException("可用预算池不存在");
    }

    public async Task WithdrawAsync(int id, decimal amount, CancellationToken token = default)
    {
        var companyId = CompanyId;
        ValidateAmount(amount, token);
        var changed = await Context.Updateable<FIN_Reimb_BudgetManage>()
            .SetColumns(x => x.TotalAmount == x.TotalAmount - amount)
            .SetColumns(x => x.AvailableAmount == x.AvailableAmount - amount)
            .Where(x => x.ID == id && x.CompanyID == companyId && !x.Hidden && x.AvailableAmount >= amount)
            .ExecuteCommandAsync();
        if (changed != 1) throw new InvalidOperationException("预算池不存在或可用额度不足");
    }

    public async Task TransferAsync(int id, ReimbursementBudgetTransferInput input, CancellationToken token = default)
    {
        var companyId = CompanyId;
        var sourceId = input.SourceId > 0 ? input.SourceId : id;
        if (input is null || sourceId == input.TargetPoolId) throw new ArgumentException("划拨目标无效");
        ValidateAmount(input.Amount, token);
        Context.Ado.BeginTran();
        try
        {
            var pools = await Context.Queryable<FIN_Reimb_BudgetManage>()
                .With(SqlWith.UpdLock + "," + SqlWith.HoldLock)
                .Where(x => x.CompanyID == companyId && (x.ID == sourceId || x.ID == input.TargetPoolId))
                .OrderBy(x => x.ID).ToListAsync();
            if (pools.Count != 2 || pools.Any(x => x.Hidden) || pools[0].Year != pools[1].Year)
                throw new InvalidOperationException("预算池不存在、已隐藏或不在同一年度");
            var removed = await Context.Updateable<FIN_Reimb_BudgetManage>()
                .SetColumns(x => x.TotalAmount == x.TotalAmount - input.Amount)
                .SetColumns(x => x.AvailableAmount == x.AvailableAmount - input.Amount)
                .Where(x => x.ID == sourceId && x.CompanyID == companyId && x.AvailableAmount >= input.Amount)
                .ExecuteCommandAsync();
            if (removed != 1) throw new InvalidOperationException("划出预算池可用额度不足");
            var added = await Context.Updateable<FIN_Reimb_BudgetManage>()
                .SetColumns(x => x.TotalAmount == x.TotalAmount + input.Amount)
                .SetColumns(x => x.AvailableAmount == x.AvailableAmount + input.Amount)
                .Where(x => x.ID == input.TargetPoolId && x.CompanyID == companyId).ExecuteCommandAsync();
            if (added != 1) throw new InvalidOperationException("划入预算池已变化");
            Context.Ado.CommitTran();
        }
        catch { Context.Ado.RollbackTran(); throw; }
    }

    public async Task SetPoolHiddenAsync(int id, bool hidden, CancellationToken token = default)
    {
        var companyId = CompanyId;
        token.ThrowIfCancellationRequested();
        var pool = await GetPoolAsync(id, token) ?? throw new KeyNotFoundException("预算池不存在");
        if (hidden && pool.LockAmount != 0) throw new InvalidOperationException("存在未结清的预留，不能隐藏预算池");
        var changed = await Context.Updateable<FIN_Reimb_BudgetManage>()
            .SetColumns(x => x.Hidden == hidden)
            .Where(x => x.ID == id && x.CompanyID == companyId && (!hidden || x.LockAmount == 0))
            .ExecuteCommandAsync();
        if (changed != 1) throw new InvalidOperationException("预算池已变化，请刷新后重试");
    }

    public async Task DeletePoolAsync(int id, CancellationToken token = default)
    {
        var companyId = CompanyId;
        token.ThrowIfCancellationRequested();
        var hasReservation = await Context.Queryable<FIN_Reimb_BudgetReservation>()
            .AnyAsync(x => x.CompanyId == companyId && x.ExternalBudgetId == id.ToString());
        if (hasReservation) throw new InvalidOperationException("预算池已有报销预留流水，不能删除");
        var changed = await Context.Deleteable<FIN_Reimb_BudgetManage>()
            .Where(x => x.ID == id && x.CompanyID == companyId && x.TotalAmount == 0 &&
                x.LockAmount == 0 && x.UsedAmount == 0 && x.AvailableAmount == 0)
            .ExecuteCommandAsync();
        if (changed != 1) throw new InvalidOperationException("仅能删除没有金额变动的预算池");
    }

    public async Task<ExcelResult> ExportPoolsAsync(ReimbursementBudgetQuery q, CancellationToken token = default)
    {
        var companyId = CompanyId;
        int year = q.Year ?? GetDefaultBudgetYear();
        var pools = await Context.Queryable<FIN_Reimb_BudgetManage>()
            .Where(t => t.CompanyID == companyId && t.Year == year)
            .OrderBy(t => t.PartnerName)
            .ToListAsync();

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("总预算管理");
        var headRow = sheet.CreateRow(0);
        string[] headers = { "ID", "部门/服务商", "费用名称", "总金额", "锁定金额", "已用金额", "可用金额" };
        for (int i = 0; i < headers.Length; i++) headRow.CreateCell(i).SetCellValue(headers[i]);

        int r = 1;
        foreach (var p in pools)
        {
            var row = sheet.CreateRow(r++);
            row.CreateCell(0).SetCellValue(p.ID);
            row.CreateCell(1).SetCellValue(p.PartnerName ?? "");
            row.CreateCell(2).SetCellValue(p.ExpensesTitle ?? "");
            row.CreateCell(3).SetCellValue((double)p.TotalAmount);
            row.CreateCell(4).SetCellValue((double)p.LockAmount);
            row.CreateCell(5).SetCellValue((double)p.UsedAmount);
            row.CreateCell(6).SetCellValue((double)p.AvailableAmount);
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
            FileName = "总预算管理.xlsx"
        };
    }

    public async Task DeleteNotBudgetAsync(CancellationToken token = default)
    {
        var companyId = CompanyId;
        var partners = await Main.Queryable<Live_Partner>()
            .Where(p => p.CompanyID == companyId && p.Enabled && p.IsBudget)
            .ToListAsync();
        var validGids = partners.Select(p => p.PartnerGID).Where(g => g.HasValue).Select(g => g!.Value).ToHashSet();

        var pools = await Context.Queryable<FIN_Reimb_BudgetManage>()
            .Where(m => m.CompanyID == companyId)
            .ToListAsync();

        var toDelete = pools.Where(p => p.PartnerGID.HasValue && !validGids.Contains(p.PartnerGID.Value) &&
                                        p.TotalAmount == 0 && p.LockAmount == 0 && p.UsedAmount == 0).Select(p => p.ID).ToList();
        if (toDelete.Count > 0)
        {
            await Context.Deleteable<FIN_Reimb_BudgetManage>().Where(it => toDelete.Contains(it.ID)).ExecuteCommandAsync();
        }
    }

    public async Task<FIN_Reimb_BudgetReservation?> GetReservationAsync(Guid id, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        return await Context.Queryable<FIN_Reimb_BudgetReservation>()
            .FirstAsync(x => x.CompanyId == companyId && x.ReservationGuid == id);
    }

    public async Task<FIN_Reimb_BudgetReservation?> GetReservationByApplyAsync(Guid applyGuid,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var companyId = CompanyId;
        return await Context.Queryable<FIN_Reimb_BudgetReservation>()
            .FirstAsync(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid);
    }

    private async Task<List<Live_Partner>> ValidateConfigAsync(ReimbursementBudgetConfigInput input, int? id,
        int companyId, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (input is null || string.IsNullOrWhiteSpace(input.Title) || input.CostType <= 0 ||
            input.Percentage is < 0 or > 100)
            throw new ArgumentException("预算配置参数无效");
        var ids = input.PartnerGids?.Distinct().ToList() ?? new List<Guid>();
        if (input.IsRelate && ids.Count == 0)
            throw new ArgumentException("关联服务商模式必须选择至少一个部门");
        var partners = ids.Count == 0 ? new List<Live_Partner>() :
            await Main.Queryable<Live_Partner>()
                .Where(x => x.CompanyID == companyId && x.Enabled && x.IsBudget && ids.Contains(x.PartnerGID!.Value))
                .ToListAsync();
        if (partners.Count != ids.Count)
            throw new ArgumentException("选中的部分部门不存在或未启用预算");
        var others = await Context.Queryable<FIN_Reimb_BudgetConfig>()
            .Where(x => x.CompanyID == companyId && x.CostType == input.CostType)
            .WhereIF(id.HasValue, x => x.ID != id!.Value)
            .ToListAsync();
        if (others.Any(x => x.Title == input.Title.Trim()))
            throw new InvalidOperationException("当前成本类型下预算名称已存在");
        if (input.Enable && input.IsRelate && others.Any(x => x.Enable && x.IsRelate &&
            ParsePartnerIds(x.PartnerIDS).Overlaps(ids)))
            throw new InvalidOperationException("同一成本类型的启用关联配置覆盖了相同部门");
        return partners.OrderBy(x => ids.IndexOf(x.PartnerGID!.Value)).ToList();
    }

    private static void CopyConfig(ReimbursementBudgetConfigInput input, List<Live_Partner> partners,
        FIN_Reimb_BudgetConfig config)
    {
        config.Title = input.Title.Trim();
        config.CostType = input.CostType;
        config.GenerateType = input.GenerateType;
        config.Percentage = input.Percentage;
        config.Enable = input.Enable;
        config.DirectlyPass = input.DirectlyPass;
        config.PublicUse = input.PublicUse;
        config.IsRelate = input.IsRelate;
        config.PartnerIDS = string.Join(',', partners.Select(x => x.PartnerGID));
        config.PartnerNames = string.Join(',', partners.Select(x => x.PartnerName));
    }

    private static HashSet<Guid> ParsePartnerIds(string? value) =>
        (value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(x => Guid.TryParse(x, out _)).Select(Guid.Parse).ToHashSet();

    private static void ValidatePage(ReimbursementBudgetQuery query, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (query is null || query.PageIndex < 1 || query.PageSize is < 1 or > 100)
            throw new ArgumentException("分页参数无效");
    }

    private static void ValidateAmount(decimal amount, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (amount <= 0 || decimal.Round(amount, 2) != amount)
            throw new ArgumentException("金额须为正数且最多两位小数");
    }
}
