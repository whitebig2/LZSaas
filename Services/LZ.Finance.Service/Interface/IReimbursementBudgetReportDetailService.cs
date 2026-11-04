using LZ.Model;
using LZ.Model.Budget;
using LZ.Model.PagedModel;

namespace LZ.Finance.Service.Interface;

public class ReimbursementBudgetReportDetailQuery : FinancePageModel
{
    public int? QuarterType { get; set; }
    public int? PartnerID { get; set; }
    public Guid? PartnerGID { get; set; }
}

public interface IReimbursementBudgetReportDetailService
{
    Task<(IList<FIN_Reimb_BudgetReportDetail> items, int count)> ListAsync(ReimbursementBudgetReportDetailQuery q, CancellationToken t = default);
    Task AddAsync(FIN_Reimb_BudgetReportDetail m, CancellationToken t = default);
    Task<FIN_Reimb_BudgetReportDetail> GetAsync(int id, CancellationToken t = default);
    Task UpdateAsync(FIN_Reimb_BudgetReportDetail m, CancellationToken t = default);
    Task DeleteAsync(int id, CancellationToken t = default);
    Task<object> CreateBillAsync(RecordMonthModel m, CancellationToken t = default);
    Task DeleteMonthBillAsync(RecordMonthModel m, CancellationToken t = default);
    Task<IList<RecordMonthModel>> ReportMonthListAsync(RecordMonthModel m, CancellationToken t = default);
}
