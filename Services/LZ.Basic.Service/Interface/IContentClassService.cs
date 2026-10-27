using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IContentClassService
    {
        Task<List<Model.SYS_ContentClass>> List(Expression<Func<Model.SYS_ContentClass, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.SYS_ContentClass model);
        Task<bool> Update(Model.SYS_ContentClass model);
        Task<bool> Delete(int id);
        Task<Model.SYS_ContentClass> Get(int id);
    }
}
