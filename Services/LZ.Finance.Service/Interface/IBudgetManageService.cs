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
    public interface IBudgetManageService
    {
        Task<List<Model.Live_V_BudgetManage>> List(Expression<Func<Model.Live_V_BudgetManage, bool>> exp, PagedModelBase query);
        Task<List<Live_BudgetManage>> ListModel(Expression<Func<Live_BudgetManage, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_BudgetManage model);
        Task<int> Add(List<Model.Live_BudgetManage> list);

        Task<bool> Update(Model.Live_BudgetManage model);
        Task<bool> Update(List<Model.Live_BudgetManage> list);
        Task<bool> Delete(int id);

        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Live_BudgetManage, bool>> exp);
        Task<bool> UpdateMoney(Live_V_BudgetManage model);
        Task<bool> UpdateHidden(int id, bool isHidden);

        Task<Model.Live_BudgetManage> Get(int id);
        Task<Model.Live_V_BudgetManage> GetView(int id);
        Task<Live_V_BudgetManage> GetV(Expression<Func<Live_V_BudgetManage, bool>> exp);
        Task<Live_BudgetManage> Get(Expression<Func<Live_BudgetManage, bool>> exp);
        Task<int> DictionarySet(Dictionary<string, object> dt);

		byte[] ExportExcel(List<Live_V_BudgetManage> list, List<Live_BudgetRechargeLog> logs);
	}
}
