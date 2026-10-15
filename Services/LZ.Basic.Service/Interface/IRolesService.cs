using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IRolesService
    {
        Task<List<Model.SYS_Roles>> List(Expression<Func<Model.SYS_Roles, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.SYS_Roles model);

        Task<List<Model.SYS_RolesFunciton>> Menu(Guid id);

        Task<bool> Update(Model.SYS_Roles model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Guid id);
        Task<bool> Set(Guid id, bool enabled);
        Task<Model.SYS_Roles> Get(Guid id);
        Task<bool> SetMenu(List<Model.SYS_RolesFunciton> models);
    }
}
