using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IContentService
    {
        Task<List<Model.SYS_V_Content>> List(Expression<Func<Model.SYS_V_Content, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.SYS_Content model);
        Task<bool> Update(Model.SYS_Content model);
        Task<bool> Delete(int id);
        Task<Model.SYS_Content> Get(int id);
    }
}
