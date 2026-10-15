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
    public interface IBudgetApplyRecordDetailService
    {
        Task<List<Live_BudgetApplyRecordDetail>> List(Expression<Func<Live_BudgetApplyRecordDetail, bool>> exp, PagedModelBase query);

        Task<List<Live_V_BudgetApplyRecordDetail>> ListV(Expression<Func<Live_V_BudgetApplyRecordDetail, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_BudgetApplyRecordDetail model);
        Task<int> Add(List<Model.Live_BudgetApplyRecordDetail> list);

        Task<bool> Update(Model.Live_BudgetApplyRecordDetail model);
        Task<bool> Delete(int id);

        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_BudgetApplyRecordDetail, bool>> exp);

        Task<Model.Live_BudgetApplyRecordDetail> Get(int id);
        Task<List<Live_BudgetApplyRecordDetail>> GetByApply(int id);
        
    }
}
