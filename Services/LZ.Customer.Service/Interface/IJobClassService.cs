using System.Linq.Expressions;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Interface
{
    public interface IJobClassService
    {
        Task<List<Model.Live_JobClass>> List(Expression<Func<Model.Live_JobClass, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_JobClass model);

        Task<bool> Update(Model.Live_JobClass model);
        Task<bool> Delete(int id);
        Task<bool> Exist(int id);
        Task<bool> Set(int id, bool enabled);
        Task<Model.Live_JobClass> Get(int id);
    }
}
