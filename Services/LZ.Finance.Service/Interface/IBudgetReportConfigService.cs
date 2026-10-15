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
    public interface IBudgetReportConfigService
    {
        Task<List<Live_BudgetReportConfig>> List(Expression<Func<Live_BudgetReportConfig, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_BudgetReportConfig model);
        Task<int> Add(List<Model.Live_BudgetReportConfig> list);

        Task<bool> Update(Model.Live_BudgetReportConfig model);
        Task<bool> Update(List<Model.Live_BudgetReportConfig> list);
        Task<bool> Delete(int id);

        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_BudgetReportConfig, bool>> exp);

        Task<Model.Live_BudgetReportConfig> Get(int id);
        Task<Model.Live_BudgetReportConfig> Get(Expression<Func<Live_BudgetReportConfig, bool>> exp);
        Task<bool> Delete(Expression<Func<Live_BudgetReportConfig, bool>> exp);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
