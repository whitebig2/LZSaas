using LZ.Model.PagedModel;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IBudgetReportService
    {
        Task<List<Live_BudgetReport>> List(Expression<Func<Live_BudgetReport, bool>> exp, PagedModelBase query, string order="");

        Task<int> Add(Model.Live_BudgetReport model);
        Task<int> Add(List<Model.Live_BudgetReport> list);

        Task<bool> Update(Model.Live_BudgetReport model);
        Task<bool> Update(List<Model.Live_BudgetReport> list);
        Task<bool> Delete(int id);

        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_BudgetReport, bool>> exp);

        Task<Model.Live_BudgetReport> Get(int id);
        Task<Model.Live_BudgetReport> Get(Expression<Func<Live_BudgetReport, bool>> exp);
        Task<int> DictionarySet(Dictionary<string, object> dt);

        Task<byte[]> ReportExcel(List<Model.Live_BudgetReportConfig> configs, List<Model.Live_BudgetReportDetail> details, List<Model.Live_ProfitManage> profit);
    }
}
