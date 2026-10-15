using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Interface
{
    public interface IEntryJobService
    {
        Task<List<Model.Live_EntryJob>> List(Expression<Func<Model.Live_EntryJob, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_EntryJob model);


        Task<bool> Update(Model.Live_EntryJob model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Exist(Expression<Func<Model.Live_EntryJob, bool>> exp);
        Task<bool> Set(int id, bool enabled);
        Task<Model.Live_EntryJob> Get(int id);
    }
}
