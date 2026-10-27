using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IBusInsurancePlanNameService
    {
        Task<bool> Add(List<Live_BusInsurancePlanName> models);
        Task<bool> Add(Live_BusInsurancePlanName model);
        Task Delete(Expression<Func<Live_BusInsurancePlanName, bool>> exp);
        Task<Live_BusInsurancePlanName> Get(Expression<Func<Live_BusInsurancePlanName, bool>> exp);
        Task<List<Live_BusInsurancePlanName>> List(Expression<Func<Live_BusInsurancePlanName, bool>> exp);
        Task<List<Live_BusInsurancePlanName>> PageList(Expression<Func<Live_BusInsurancePlanName, bool>> exp, PagedModelBase query);
        Task<bool> Update(List<Live_BusInsurancePlanName> models);
        Task<bool> Update(Live_BusInsurancePlanName model);
    }
}
