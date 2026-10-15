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
    public interface IBudgetRechargeLogService
    {
        Task<List<Model.Live_BudgetRechargeLog>> List(Expression<Func<Model.Live_BudgetRechargeLog, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_BudgetRechargeLog model);
        Task<int> Add(List<Model.Live_BudgetRechargeLog> list);

        Task<bool> Update(Model.Live_BudgetRechargeLog model);

        Task<bool> Update(List<Model.Live_BudgetRechargeLog> list);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_BudgetRechargeLog, bool>> exp);

        Task<Model.Live_BudgetRechargeLog> Get(int id);
        Task<Live_BudgetRechargeLog> Get(Expression<Func<Live_BudgetRechargeLog, bool>> exp);
    }
}
