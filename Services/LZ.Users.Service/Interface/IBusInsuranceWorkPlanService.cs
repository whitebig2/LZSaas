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
   

    public interface IBusInsuranceWorkPlanService
    {
        Task<bool> Add(List<Live_BusInsuranceWorkPlan> models);
        Task<bool> Add(Live_BusInsuranceWorkPlan model);
        Task Delete(Expression<Func<Live_BusInsuranceWorkPlan, bool>> exp);
        Task<Live_BusInsuranceWorkPlan> Get(Expression<Func<Live_BusInsuranceWorkPlan, bool>> exp);
        Task<List<Live_BusInsuranceWorkPlan>> List(Expression<Func<Live_BusInsuranceWorkPlan, bool>> exp);
        Task<List<Live_BusInsuranceWorkPlan>> PageList(Expression<Func<Live_BusInsuranceWorkPlan, bool>> exp, PagedModelBase query);
        Task<bool> Update(List<Live_BusInsuranceWorkPlan> models);
        Task<bool> Update(Live_BusInsuranceWorkPlan model);
        Task<List<string>> GetCustomerNames();
    }
}
