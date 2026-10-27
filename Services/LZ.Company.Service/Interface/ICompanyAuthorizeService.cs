using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Company.Service.Interface
{
    public interface ICompanyAuthorizeService
    {
        Task<List<Model.Live_V_CompanyAuthorize>> List(Expression<Func<Model.Live_V_CompanyAuthorize, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_CompanyAuthorize model);
        Task<Model.Live_CompanyAuthorize> Get(Expression<Func<Model.Live_CompanyAuthorize, bool>> exp);

        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Live_CompanyAuthorize, bool>> exp);
    }
}
