using LZ.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusinessInsuranceOrderService
    {
        Task<bool> AddNav(Live_BusinessInsuranceOrderExtension model);
        Task<bool> DeleteNav(Expression<Func<Live_BusinessInsuranceOrderExtension, bool>> exp);
        Task<Live_BusinessInsuranceOrder> Get(Expression<Func<Live_BusinessInsuranceOrder, bool>> exp);
        Task<Live_BusinessInsuranceOrderExtension> GetNav(Expression<Func<Live_BusinessInsuranceOrderExtension, bool>> exp);
        Task<Live_V_BusinessInsuranceOrderPlan> GetOrderPlanView(Expression<Func<Live_V_BusinessInsuranceOrderPlan, bool>> exp);
        Task<List<Live_BusinessInsuranceOrder>> List(Expression<Func<Live_BusinessInsuranceOrder, bool>> exp);
        Task<List<Live_V_BusinessInsuranceOrderPlan>> ListOrderPlanView(Expression<Func<Live_V_BusinessInsuranceOrderPlan, bool>> exp);
        Task<List<Live_V_BusinessInsuranceOrder>> ListView(Expression<Func<Live_V_BusinessInsuranceOrder, bool>> exp);
        Task<List<Live_V_BusinessInsuranceOrder>> PageList(Expression<Func<Live_V_BusinessInsuranceOrder, bool>> exp, PagedModelBase query);
        Task<bool> Update(Live_BusinessInsuranceOrder model);
        Task<bool> Update(List<Live_BusinessInsuranceOrder> models);
        Task<bool> UpdateNav(Live_BusinessInsuranceOrderExtension model);
    }
}
