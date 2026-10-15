using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Company.Service.Interface
{
    public interface ICompanyFunctionMenuService
    {
        Task<List<Model.SYS_CompanyFunctionMenu>> List(Expression<Func<Model.SYS_CompanyFunctionMenu, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.SYS_CompanyFunctionMenu model);

        Task<bool> Update(Model.SYS_CompanyFunctionMenu model);
        Task<bool> Delete(int id);
        Task<bool> Set(int id, bool enabled);
        Task<bool> Exist(int id);
        Task<Model.SYS_CompanyFunctionMenu> Get(int id);
        Task<List<Model.SYS_CompanyFunctionMenu>> UserMenu(int uid);
    }
}
