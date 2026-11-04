using LZ.Model;
using LZ.Model.Budget;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Http;

namespace LZ.Finance.Service.Interface;

public class ReimbursementBudgetApplyQuery : FinancePageModel
{
    public string? Payer { get; set; }
}

public class ApplyRecordDetailDto
{
    public FIN_Reimb_BudgetApplyRecord? Model { get; set; }
    public List<FIN_Reimb_BudgetApplyRecordDetail> Details { get; set; } = new();
    public List<Live_FileUpload> Files { get; set; } = new();
}

public class SalaryAutoInput
{
    public Guid? Gid { get; set; }
    public Guid? UserGID { get; set; }
    public string? UserName { get; set; }
    public int CompanyID { get; set; }
    public Guid CompanyGID { get; set; }
}

public class FileRef
{
    public string? FileName { get; set; }
    public string? Path { get; set; }
    public int? FileSize { get; set; }
    public int? TypeKey { get; set; }
}

public class ImportResult
{
    public List<FIN_Reimb_BudgetApplyRecordDetail> Items { get; set; } = new();
    public int Id { get; set; }
    public List<string> SkippedReasons { get; set; } = new();
}

public class ExcelResult
{
    public string Base64String { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}

public class FileContent
{
    public string Base64String { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}

public interface IReimbursementBudgetApplyRecordService
{
    Task<(IList<FIN_Reimb_BudgetApplyRecord> items, int count)> ListAsync(ReimbursementBudgetApplyQuery q, int? userId, CancellationToken t = default);
    Task<object> AddAsync(FIN_Reimb_BudgetApplyRecord m, List<FIN_Reimb_BudgetApplyRecordDetail>? details, List<FileRef>? files, CancellationToken t = default);
    Task<ApplyRecordDetailDto> GetAsync(int id, CancellationToken t = default);
    Task<FIN_Reimb_BudgetApplyRecord> GetSimpleAsync(int id, CancellationToken t = default);
    Task UpdateAsync(FIN_Reimb_BudgetApplyRecord m, CancellationToken t = default);
    Task DeleteAsync(int id, CancellationToken t = default);
    Task AuditingAsync(int id, int method, int status, CancellationToken t = default);
    Task UpdateStatusAsync(int id, int status, string message, CancellationToken t = default);
    Task<object> GetPartnerAsync(CancellationToken t = default);
    Task<int> SalaryCustomerAutoCompleteAsync(SalaryAutoInput m, CancellationToken t = default);
    Task<ImportResult> ImportBudgetDetailExcelAsync(int id, IFormFile file, CancellationToken t = default);
    Task<ExcelResult> CreateDefaultExcelAsync(CancellationToken t = default);
    Task<ExcelResult> FinanceExportExcelAsync(ReimbursementBudgetApplyQuery q, string? payer, CancellationToken t = default);
    Task<FileContent> SaveFileAsync(int id, CancellationToken t = default);
    Task<object> TiaoZhengAsync(CancellationToken t = default);
}

