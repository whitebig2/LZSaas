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
    public interface IBudgetApplyRecordService
    {
        Task<List<Live_BudgetApplyRecord>> List(Expression<Func<Live_BudgetApplyRecord, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_BudgetApplyRecord model);
        Task<int> Add(List<Model.Live_BudgetApplyRecord> list);

        Task<bool> Update(Model.Live_BudgetApplyRecord model);
        Task<bool> Update(List<Model.Live_BudgetApplyRecord> list);
        Task<bool> Delete(int id);

        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_BudgetApplyRecord, bool>> exp);

        Task<Model.Live_BudgetApplyRecord> Get(int id);
        Task<Model.Live_BudgetApplyRecord> Get(Expression<Func<Live_BudgetApplyRecord, bool>> exp);
        Task<int> DictionarySet(Dictionary<string, object> dt);
        byte[] CreateDefaultExcel();
        byte[] FinanceExportExcel(List<Live_BudgetApplyRecord> list, List<Live_V_BudgetApplyRecordDetail> details);
        
    }
}
