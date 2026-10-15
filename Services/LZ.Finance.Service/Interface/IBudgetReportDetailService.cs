using LZ.Model.PagedModel;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.Budget;

namespace LZ.Finance.Service.Interface
{
    public interface IBudgetReportDetailService
    {
        Task<List<Live_BudgetReportDetail>> List(Expression<Func<Live_BudgetReportDetail, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_BudgetReportDetail model);
        Task<int> Add(List<Model.Live_BudgetReportDetail> list);

        Task<bool> Update(Model.Live_BudgetReportDetail model);
        Task<bool> Update(List<Model.Live_BudgetReportDetail> list);
        Task<bool> Delete(int id);
        Task<bool> Delete(Expression<Func<Live_BudgetReportDetail, bool>> exp);

        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_BudgetReportDetail, bool>> exp);

        Task<Model.Live_BudgetReportDetail> Get(int id);
        Task<Model.Live_BudgetReportDetail> Get(Expression<Func<Live_BudgetReportDetail, bool>> exp);
        Task<int> DictionarySet(Dictionary<string, object> dt);

        List<Model.Budget.RecordMonthModel> ReportMonthList(List<Live_BudgetReportDetail> list, RecordMonthModel info);
    }
}
