using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Customer.Service.Interface
{
    public interface IInsuranceOrderService
    {
        Task<bool> AddNav(Live_InsuranceOrderExtension model);
        Task<bool> Delete(Guid id);
        Task<bool> DeleteNav(Expression<Func<Live_InsuranceOrderExtension, bool>> exp);
        Task<bool> Exist(Guid id);
        Task<Live_InsuranceOrder> Get(Expression<Func<Live_InsuranceOrder, bool>> exp);
        Task<Live_InsuranceOrderExtension> GetNav(Expression<Func<Live_InsuranceOrderExtension, bool>> exp);
        Task<Live_V_InsuranceOrderPlan> GetOrderPlanView(Expression<Func<Live_V_InsuranceOrderPlan, bool>> exp);
        Task<List<Live_V_InsuranceOrderPlan>> GetOrderPlanViewList(Expression<Func<Live_V_InsuranceOrderPlan, bool>> exp);
        Task<Live_V_InsuranceOrder> GetView(Expression<Func<Live_V_InsuranceOrder, bool>> exp);
        Task<List<Live_InsuranceOrder>> List(Expression<Func<Live_InsuranceOrder, bool>> exp);
        Task<List<Live_V_InsuranceOrderExtension>> PageList(Expression<Func<Live_V_InsuranceOrderExtension, bool>> exp, PagedModelBase query);
        Task<bool> Update(List<Live_InsuranceOrder> models);
        Task<bool> Update(Live_InsuranceOrder model);
        Task<bool> UpdateNav(Live_InsuranceOrderExtension model);
    }
}
