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
    public interface IBudgetConfigService
    {
        Task<List<Model.Live_V_BudgetConfig>> List(Expression<Func<Model.Live_V_BudgetConfig, bool>> exp, PagedModelBase query);
        Task<List<Model.Live_BudgetConfig>> ListModel(Expression<Func<Model.Live_BudgetConfig, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_BudgetConfig model);

        Task<bool> Update(Model.Live_BudgetConfig model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Exist(Expression<Func<Live_BudgetConfig, bool>> exp);

        Task<Model.Live_BudgetConfig> Get(Guid id);
        Task<Live_BudgetConfig> Get(Expression<Func<Live_BudgetConfig, bool>> exp);
    }
}
