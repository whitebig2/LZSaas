using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Finance.Service.Interface;

public class ReimbursementBudgetReportQuery : FinancePageModel
{
    public int? PartnerID { get; set; }
    public Guid? PartnerGID { get; set; }
}

public interface IReimbursementBudgetReportService
{
    Task<(IList<FIN_Reimb_BudgetReport> items, int count)> ListAsync(ReimbursementBudgetReportQuery q, CancellationToken t = default);
    Task AddAsync(FIN_Reimb_BudgetReport m, CancellationToken t = default);
    Task<FIN_Reimb_BudgetReport> GetAsync(int id, CancellationToken t = default);
    Task UpdateAsync(FIN_Reimb_BudgetReport m, CancellationToken t = default);
    Task DeleteAsync(int id, CancellationToken t = default);
    Task<ExcelResult> CreateExcelAsync(FIN_Reimb_BudgetReport m, CancellationToken t = default);
    Task<object> CreatePartnerAsync(CancellationToken t = default);
}
