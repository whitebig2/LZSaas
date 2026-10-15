using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IDictionaryService
    {
        Task<List<Model.SYS_Dictionary>> List(Expression<Func<Model.SYS_Dictionary, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.SYS_Dictionary model);

        Task<bool> Update(Model.SYS_Dictionary model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(int id);
        Task<bool> Set(int id, bool enabled);
        Task<Model.SYS_Dictionary> Get(int id);
        Task<SYS_Dictionary> Get(Expression<Func<SYS_Dictionary, bool>> exp);
    }
}
