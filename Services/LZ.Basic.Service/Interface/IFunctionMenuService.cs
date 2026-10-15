using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IFunctionMenuService
    {
        Task<List<Model.SYS_FunctionMenu>> List(Expression<Func<Model.SYS_FunctionMenu, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.SYS_FunctionMenu model);

        Task<bool> Update(Model.SYS_FunctionMenu model);
        Task<bool> Delete(int id);
        Task<bool> Set(int id, bool enabled);
        Task<bool> Exist(int id);
        Task<Model.SYS_FunctionMenu> Get(int id);
        Task<List<Model.SYS_FunctionMenu>> UserMenu(int uid);
        Task<List<SYS_FunctionMenu>> CompanyUserMenu(int uid, int companyId);
    }
}
