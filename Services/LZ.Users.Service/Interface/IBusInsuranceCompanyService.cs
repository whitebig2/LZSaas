using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Users.Service.Interface
{
    public interface IBusInsuranceCompanyService
    {
        Task<bool> Add(List<Live_BusInsuranceCompany> models);
        Task<bool> Add(Live_BusInsuranceCompany model);
        Task Delete(Expression<Func<Live_BusInsuranceCompany, bool>> exp);
        Task<Live_BusInsuranceCompany> Get(Expression<Func<Live_BusInsuranceCompany, bool>> exp);
        Task<List<Live_BusInsuranceCompany>> List(Expression<Func<Live_BusInsuranceCompany, bool>> exp);
        Task<List<Live_BusInsuranceCompany>> PageList(Expression<Func<Live_BusInsuranceCompany, bool>> exp, PagedModelBase query);
        Task<bool> Update(List<Live_BusInsuranceCompany> models);
        Task<bool> Update(Live_BusInsuranceCompany model);
    }
}
