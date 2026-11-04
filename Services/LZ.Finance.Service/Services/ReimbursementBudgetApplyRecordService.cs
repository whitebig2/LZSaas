using System.Data;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Enums.Budget;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.Service.Services;

public class ReimbursementBudgetApplyRecordService : ServicesBase<FIN_Reimb_BudgetApplyRecord>,
    IReimbursementBudgetApplyRecordService, IScopedDependency
{
    private readonly ICurrentUser _user;
    private ISqlSugarClient Main => DbScoped.SugarScope.GetConnection("Main");

    public ReimbursementBudgetApplyRecordService(ICurrentUser user) => _user = user;

    public async Task<(IList<FIN_Reimb_BudgetApplyRecord> items, int count)> ListAsync(ReimbursementBudgetApplyQuery q, int? userId, CancellationToken t = default)
    {
        var exp = Expressionable.Create<FIN_Reimb_BudgetApplyRecord>();
        if (_user.PlatformType == PlatformType.Admin || _user.PlatformType == PlatformType.Api)
        {
            exp.AndIF(q.CompanyID.HasValue && q.CompanyID > 0, it => it.CompanyID == q.CompanyID);
        }
        else
        {
            exp.And(it => it.CompanyID == _user.CompanyID);
            exp.AndIF(userId.HasValue, it => it.UserGID.HasValue);
        }

        if (!string.IsNullOrEmpty(q.Types))
        {
            var arr = q.Types.Split(",").Select(s => int.TryParse(s, out var v) ? (int?)v : null).Where(v => v.HasValue).Select(v => v!.Value).ToList();
            exp.And(t => arr.Contains(t.ProcessStatus ?? 0));
        }

        exp.AndIF(!string.IsNullOrEmpty(q.Name), t => t.ExpensesTitle.Contains(q.Name));
        exp.AndIF(!string.IsNullOrEmpty(q.Payer), t => t.Payer.Contains(q.Payer));
        exp.AndIF(!string.IsNullOrEmpty(q.UserName), t => t.UserName.Contains(q.UserName));
        exp.AndIF(!string.IsNullOrEmpty(q.PartnerName), t => t.PartnerName.Contains(q.PartnerName));
        exp.AndIF(!string.IsNullOrEmpty(q.Description), t => t.Description.Contains(q.Description));
        exp.AndIF(q.Money.HasValue, t => t.ApplyAmount == q.Money);
        exp.AndIF(q.StartTime.HasValue, t => t.CreateTime >= q.StartTime);
        exp.AndIF(q.EndTime.HasValue, t => t.CreateTime <= q.EndTime);
        exp.AndIF(q.ProcessStatus.HasValue, t => t.ProcessStatus == q.ProcessStatus);
        exp.AndIF(q.Year.HasValue, t => t.Year == q.Year);

        var count = new RefAsync<int>();
        var page = q.Page <= 0 ? 1 : q.Page;
        var pageSize = q.PageSize <= 0 ? 15 : q.PageSize;

        var list = await Context.Queryable<FIN_Reimb_BudgetApplyRecord>()
            .Where(exp.ToExpression())
            .OrderByDescending(it => it.CreateTime)
            .ToOffsetPageAsync(page, pageSize, count);

        return (list, count.Value);
    }

    public async Task<object> AddAsync(FIN_Reimb_BudgetApplyRecord model, List<FIN_Reimb_BudgetApplyRecordDetail>? details, List<FileRef>? files, CancellationToken t = default)
    {
        if (!model.BudgetManageID.HasValue) throw new ErrorCodeException(-1, "预算池未指定");

        await Context.Ado.BeginTranAsync();
        try
        {
            var manage = await Context.Queryable<FIN_Reimb_BudgetManage>()
                .InSingleAsync(model.BudgetManageID.Value);
            if (manage == null) throw new ErrorCodeException(-1, "没有匹配的预算池");

            decimal applyMoney = model.ApplyAmount;
            decimal availableAmount = manage.AvailableAmount - applyMoney;
            if ((manage.AvailableAmount <= 0 || availableAmount < 0) && !manage.DirectlyPass)
            {
                throw new ErrorCodeException(-1, "剩余可用金额不足, 申请失败");
            }

            var log = new FIN_Reimb_BudgetRechargeLog
            {
                CompanyID = manage.CompanyID,
                CompanyGID = manage.CompanyGID,
                CID = manage.ID,
                CreateTime = DateTime.Now,
                UserName = model.UserName,
                UserGID = model.UserGID ?? Guid.Empty,
                Money = applyMoney,
                PartnerName = manage.PartnerName,
                CostName = ((CostType?)manage.CostType)?.ToString() ?? "",
                Title = model.ExpensesTitle,
                GenerateName = ((GenerationType?)manage.GenerateType)?.ToString() ?? "",
                Type = (int)BudgetManageRecordActionEnum.客服申请扣款,
                Year = manage.Year
            };

            if (!manage.DirectlyPass)
            {
                log.TotalAmount = manage.TotalAmount;
                log.AvailableAmount = availableAmount;
                log.LockAmount = manage.LockAmount + applyMoney;
                log.UsedAmount = manage.UsedAmount;
                log.HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount;
            }

            var logId = await Context.Insertable(log).ExecuteReturnIdentityAsync();

            model.RechargeLogID = logId;
            model.CreateTime = DateTime.Now;
            model.CompanyID = manage.CompanyID;
            model.CompanyGID = manage.CompanyGID;
            model.ProcessStatus = (int)BudgetApplyRecordProcessStatus.待审核;
            var recordId = await Context.Insertable(model).ExecuteReturnIdentityAsync();

            if (!manage.DirectlyPass)
            {
                manage.LockAmount += applyMoney;
                manage.AvailableAmount = availableAmount;
                await Context.Updateable(manage).ExecuteCommandAsync();
            }

            if (details != null && details.Count > 0)
            {
                foreach (var detail in details)
                {
                    detail.CompanyID = manage.CompanyID;
                    detail.BudgetApplyRecordID = recordId;
                    detail.CreateTime = DateTime.Now;
                }
                await Context.Insertable(details).ExecuteCommandAsync();
            }

            if (files != null && files.Count > 0)
            {
                var fileEntities = files.Select(f => new Live_FileUpload
                {
                    CompanyID = manage.CompanyID,
                    CompanyGID = manage.CompanyGID,
                    UserID = _user.UserID,
                    FID = recordId,
                    CreateTime = DateTime.Now,
                    FileName = f.FileName ?? "",
                    Path = f.Path ?? "",
                    TypeKey = f.TypeKey ?? 10
                }).ToList();
                await Context.Insertable(fileEntities).ExecuteCommandAsync();
            }

            await Context.Ado.CommitTranAsync();
            return new { items = model, list = details, files };
        }
        catch
        {
            await Context.Ado.RollbackTranAsync();
            throw;
        }
    }

    public async Task<ApplyRecordDetailDto> GetAsync(int id, CancellationToken t = default)
    {
        var model = await Context.Queryable<FIN_Reimb_BudgetApplyRecord>().InSingleAsync(id);
        if (model == null) throw new ErrorCodeException(-1, "你要读取的数据不存在");

        var details = await Context.Queryable<FIN_Reimb_BudgetApplyRecordDetail>()
            .Where(d => d.BudgetApplyRecordID == id)
            .ToListAsync();

        var files = await Context.Queryable<Live_FileUpload>()
            .Where(f => f.FID == id && f.TypeKey == 10)
            .ToListAsync();

        return new ApplyRecordDetailDto { Model = model, Details = details, Files = files };
    }

    public async Task<FIN_Reimb_BudgetApplyRecord> GetSimpleAsync(int id, CancellationToken t = default)
    {
        var model = await Context.Queryable<FIN_Reimb_BudgetApplyRecord>().InSingleAsync(id);
        if (model == null) throw new ErrorCodeException(-1, "你要读取的数据不存在");
        return model;
    }

    public async Task UpdateAsync(FIN_Reimb_BudgetApplyRecord m, CancellationToken t = default)
    {
        await Context.Updateable(m).ExecuteCommandAsync();
    }

    public async Task DeleteAsync(int id, CancellationToken t = default)
    {
        await Context.Deleteable<FIN_Reimb_BudgetApplyRecord>().Where(it => it.ID == id).ExecuteCommandAsync();
    }

    public async Task AuditingAsync(int id, int method, int status, CancellationToken t = default)
    {
        var model = await Context.Queryable<FIN_Reimb_BudgetApplyRecord>().InSingleAsync(id);
        if (model == null) throw new ErrorCodeException(-1, "记录不存在");

        if (model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.作废 ||
            model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.审核退回)
        {
            throw new ErrorCodeException(-1, "该预算已经作废或者审核退回，请刷新页面");
        }

        if (!model.BudgetManageID.HasValue) throw new ErrorCodeException(-1, "预算池未关联");

        await Context.Ado.BeginTranAsync();
        try
        {
            var manage = await Context.Queryable<FIN_Reimb_BudgetManage>().InSingleAsync(model.BudgetManageID.Value);
            var log = new FIN_Reimb_BudgetRechargeLog
            {
                CompanyID = model.CompanyID,
                CompanyGID = model.CompanyGID,
                UserName = model.UserName,
                UserGID = model.UserGID,
                CreateTime = DateTime.Now,
                Money = model.ApplyAmount,
                PartnerName = manage.PartnerName,
                CostName = ((CostType?)manage.CostType)?.ToString() ?? "",
                Title = model.ExpensesTitle,
                GenerateName = ((GenerationType?)manage.GenerateType)?.ToString() ?? "",
                CID = manage.ID,
                TotalAmount = manage.TotalAmount,
                AvailableAmount = manage.AvailableAmount,
                HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount,
                Year = manage.Year
            };

            if (method == (int)BudgetAutdingMethod.打款成功)
            {
                log.Type = (int)BudgetManageRecordActionEnum.已打款;
                log.LockAmount = manage.LockAmount - model.ApplyAmount;
                log.UsedAmount = manage.UsedAmount + model.ApplyAmount;
                log.DecreaseAmount = model.ApplyAmount;

                manage.LockAmount -= model.ApplyAmount;
                manage.UsedAmount += model.ApplyAmount;
                model.PayTime = DateTime.Now;
            }
            else if (method == (int)BudgetAutdingMethod.打款后回退)
            {
                log.Type = (int)BudgetManageRecordActionEnum.回滚打款前;
                log.LockAmount = manage.LockAmount + model.ApplyAmount;
                log.UsedAmount = manage.UsedAmount - model.ApplyAmount;
                log.DecreaseAmount = -model.ApplyAmount;

                manage.LockAmount += model.ApplyAmount;
                manage.UsedAmount -= model.ApplyAmount;
            }
            else if (method == (int)BudgetAutdingMethod.打款后作废)
            {
                log.Type = (int)BudgetManageRecordActionEnum.回滚作废;
                log.LockAmount = manage.LockAmount;
                log.AvailableAmount = manage.AvailableAmount + model.ApplyAmount;
                log.UsedAmount = manage.UsedAmount - model.ApplyAmount;
                log.DecreaseAmount = -model.ApplyAmount;

                manage.AvailableAmount += model.ApplyAmount;
                manage.UsedAmount -= model.ApplyAmount;
            }

            model.ProcessStatus = status;

            await Context.Insertable(log).ExecuteCommandAsync();
            if (!manage.DirectlyPass)
            {
                await Context.Updateable(manage).ExecuteCommandAsync();
            }
            await Context.Updateable(model).ExecuteCommandAsync();

            await Context.Ado.CommitTranAsync();
        }
        catch
        {
            await Context.Ado.RollbackTranAsync();
            throw;
        }
    }

    public async Task UpdateStatusAsync(int id, int status, string message, CancellationToken t = default)
    {
        var model = await Context.Queryable<FIN_Reimb_BudgetApplyRecord>().InSingleAsync(id);
        if (model == null) throw new ErrorCodeException(-1, "预算申请不存在");

        if (status == (int)BudgetApplyRecordProcessStatus.作废 || status == (int)BudgetApplyRecordProcessStatus.审核退回)
        {
            if (model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.待打款 || model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.已打款)
            {
                throw new ErrorCodeException(-1, "财务已审核通过或者已打款,请联系财务");
            }

            if (model.BudgetManageID.HasValue)
            {
                var manage = await Context.Queryable<FIN_Reimb_BudgetManage>().InSingleAsync(model.BudgetManageID.Value);
                if (manage != null && !manage.DirectlyPass)
                {
                    manage.AvailableAmount += model.ApplyAmount;
                    manage.LockAmount -= model.ApplyAmount;
                    await Context.Updateable(manage).ExecuteCommandAsync();

                    var log = new FIN_Reimb_BudgetRechargeLog
                    {
                        CompanyID = model.CompanyID,
                        CompanyGID = model.CompanyGID,
                        CID = manage.ID,
                        UserName = model.UserName,
                        UserGID = model.UserGID,
                        CreateTime = DateTime.Now,
                        Money = model.ApplyAmount,
                        PartnerName = model.PartnerName,
                        CostName = ((CostType?)manage.CostType)?.ToString() ?? "",
                        Title = model.ExpensesTitle,
                        GenerateName = ((GenerationType?)manage.GenerateType)?.ToString() ?? "",
                        Type = (int)BudgetManageRecordActionEnum.作废,
                        TotalAmount = manage.TotalAmount,
                        AvailableAmount = manage.AvailableAmount,
                        LockAmount = manage.LockAmount,
                        UsedAmount = manage.UsedAmount,
                        Year = manage.Year,
                        HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount
                    };
                    await Context.Insertable(log).ExecuteCommandAsync();
                }
            }
        }

        model.ProcessStatus = status;
        if (!string.IsNullOrEmpty(message)) model.Message = message;
        await Context.Updateable(model).ExecuteCommandAsync();
    }

    public async Task<object> GetPartnerAsync(CancellationToken t = default)
    {
        var partner = await Main.Queryable<Live_Partner>()
            .FirstAsync(p => p.CompanyID == _user.CompanyID && p.Enabled && p.IsBudget);
        if (partner == null) throw new ErrorCodeException(-1, "服务商获取失败");
        return new { items = partner };
    }

    public async Task<int> SalaryCustomerAutoCompleteAsync(SalaryAutoInput m, CancellationToken t = default)
    {
        if (!m.Gid.HasValue) throw new ErrorCodeException(-1, "id没有传值");

        var record = await Context.Queryable<FIN_Reimb_BudgetApplyRecord>()
            .FirstAsync(it => it.SalaryGID == m.Gid &&
                it.ProcessStatus != (int)BudgetApplyRecordProcessStatus.审核退回 &&
                it.ProcessStatus != (int)BudgetApplyRecordProcessStatus.作废);
        if (record != null) throw new ErrorCodeException(-1, "已经创建过预算申请，id：" + record.ID);

        int year = DateTime.Now.AddMonths(-3).Year;
        var manage = await Context.Queryable<FIN_Reimb_BudgetManage>()
            .FirstAsync(it => it.ExpensesTitle == "内部员工应发工资" && it.Year == year);
        if (manage == null) throw new ErrorCodeException(-1, "未找到内部员工应发工资预算池");

        var model = new FIN_Reimb_BudgetApplyRecord
        {
            CompanyID = m.CompanyID > 0 ? m.CompanyID : _user.CompanyID,
            CompanyGID = m.CompanyGID != Guid.Empty ? m.CompanyGID : _user.CompanyGID,
            Year = year,
            ExpensesTitle = manage.ExpensesTitle,
            CostType = manage.CostType,
            UserGID = m.UserGID ?? Guid.Empty,
            SalaryGID = m.Gid,
            UserName = m.UserName ?? _user.FullName,
            TotalAmount = manage.TotalAmount,
            ApplyAmount = 0,
            AvailableAmount = manage.AvailableAmount,
            PayType = "银行卡",
            ProcessStatus = (int)BudgetApplyRecordProcessStatus.待审核,
            BudgetManageID = manage.ID,
            CreateTime = DateTime.Now
        };
        return await Context.Insertable(model).ExecuteReturnIdentityAsync();
    }

    public async Task<ImportResult> ImportBudgetDetailExcelAsync(int id, IFormFile file, CancellationToken t = default)
    {
        var result = new ImportResult { Id = id };
        if (file == null || file.Length == 0) return result;

        var manages = await Context.Queryable<FIN_Reimb_BudgetManage>()
            .Where(m => m.CompanyID == _user.CompanyID)
            .ToListAsync();

        var filePath = "/";
        var fn = await Common.UploadFile.UpFile(file, filePath, true);
        if (string.IsNullOrEmpty(fn) || fn.Contains("error")) throw new ErrorCodeException(-1, "文件上传错误");

        var ds = Common.ExcelHeler.ReadExcelToDataSet(fn, "", 1);
        if (ds != null && ds.Tables.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row[0] == null || string.IsNullOrWhiteSpace(row[0].ToString())) continue;
                var partnerName = row[0].ToString()!.Trim();
                var item = manages.FirstOrDefault(m => m.PartnerName == partnerName);
                if (item == null)
                {
                    result.SkippedReasons.Add($"未匹配到服务商: {partnerName}");
                    continue;
                }

                var detail = new FIN_Reimb_BudgetApplyRecordDetail
                {
                    PartnerGID = item.PartnerGID,
                    PartnerName = partnerName,
                    Money = row[1] != null && decimal.TryParse(row[1].ToString(), out var mVal) ? mVal : 0,
                    Remark = row.ItemArray.Length > 2 && row[2] != null ? row[2].ToString() : ""
                };
                result.Items.Add(detail);
            }
        }
        return result;
    }

    public Task<ExcelResult> CreateDefaultExcelAsync(CancellationToken t = default)
    {
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("Sheet1");
        var row = sheet.CreateRow(0);
        row.CreateCell(0).SetCellValue("服务商名称");
        row.CreateCell(1).SetCellValue("拆分金额");
        row.CreateCell(2).SetCellValue("备注");

        byte[] buffer;
        using (var stream = new MemoryStream())
        {
            workbook.Write(stream);
            buffer = stream.ToArray();
        }
        return Task.FromResult(new ExcelResult
        {
            Base64String = Convert.ToBase64String(buffer),
            FileName = "预算总部拆分模板.xlsx"
        });
    }

    public async Task<ExcelResult> FinanceExportExcelAsync(ReimbursementBudgetApplyQuery q, string? payer, CancellationToken t = default)
    {
        var (list, _) = await ListAsync(q, null);
        var details = await Context.Queryable<FIN_Reimb_BudgetApplyRecordDetail>()
            .Where(d => d.CompanyID == _user.CompanyID)
            .ToListAsync();

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("预算审核明细");
        var headRow = sheet.CreateRow(0);
        string[] headers = { "ID", "费用名称", "申请金额", "创建时间", "状态", "付款人", "备注" };
        for (int i = 0; i < headers.Length; i++) headRow.CreateCell(i).SetCellValue(headers[i]);

        int r = 1;
        foreach (var item in list)
        {
            var row = sheet.CreateRow(r++);
            row.CreateCell(0).SetCellValue(item.ID);
            row.CreateCell(1).SetCellValue(item.ExpensesTitle ?? "");
            row.CreateCell(2).SetCellValue((double)item.ApplyAmount);
            row.CreateCell(3).SetCellValue(item.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
            row.CreateCell(4).SetCellValue(item.ProcessStatus?.ToString() ?? "");
            row.CreateCell(5).SetCellValue(item.Payer ?? "");
            row.CreateCell(6).SetCellValue(item.Remark ?? "");
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
            FileName = "预算审核导出明细.xlsx"
        };
    }

    public async Task<FileContent> SaveFileAsync(int id, CancellationToken t = default)
    {
        var file = await Context.Queryable<Live_FileUpload>().InSingleAsync(id);
        if (file == null) throw new ErrorCodeException(-1, "文件不存在");

        byte[] buffer = Array.Empty<byte>();
        if (!string.IsNullOrEmpty(file.Path))
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            buffer = await client.GetByteArrayAsync(file.Path, t);
        }

        return new FileContent
        {
            Base64String = Convert.ToBase64String(buffer),
            FileName = file.FileName ?? "download.xlsx"
        };
    }

    public async Task<object> TiaoZhengAsync(CancellationToken t = default)
    {
        var companyId = _user.CompanyID;
        await Context.Ado.BeginTranAsync();
        try
        {
            var manages = await Context.Queryable<FIN_Reimb_BudgetManage>()
                .Where(t => t.CompanyID == companyId && !t.DirectlyPass)
                .ToListAsync();

            var records = await Context.Queryable<FIN_Reimb_BudgetApplyRecord>()
                .Where(t => t.CompanyID == companyId)
                .ToListAsync();

            var manageList = new List<FIN_Reimb_BudgetManage>();
            foreach (var x in manages)
            {
                if (x.TotalAmount > 0)
                {
                    x.LockAmount = records.Where(t => t.PartnerGID == x.PartnerGID && t.CostType == x.CostType && t.ExpensesTitle == x.ExpensesTitle && t.Year == x.Year && (t.ProcessStatus == 2 || t.ProcessStatus == 3)).Sum(t => t.ApplyAmount);
                    x.UsedAmount = records.Where(t => t.PartnerGID == x.PartnerGID && t.CostType == x.CostType && t.ExpensesTitle == x.ExpensesTitle && t.Year == x.Year && t.ProcessStatus == 4).Sum(t => t.ApplyAmount);
                    x.AvailableAmount = x.TotalAmount - x.LockAmount - x.UsedAmount;
                    manageList.Add(x);
                }
            }

            if (manageList.Count > 0)
            {
                await Context.Updateable(manageList).ExecuteCommandAsync();
            }

            await Context.Ado.CommitTranAsync();
            return new { items = manages, list = manageList };
        }
        catch
        {
            await Context.Ado.RollbackTranAsync();
            throw;
        }
    }
}
