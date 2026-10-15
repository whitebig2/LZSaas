using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Customer.Service.Interface
{
    public interface IEnterpriseService
    {
        Task<List<Model.Live_Enterprise>> List(Expression<Func<Model.Live_Enterprise, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_Enterprise model);

        Task<bool> Update(Model.Live_Enterprise model);
        Task<bool> Exist(Guid id);

        Task<Model.Live_Enterprise> Get(Guid id);

        Task<bool> Delete(Guid id);
        Task<List<Live_Enterprise>> List(Expression<Func<Live_Enterprise, bool>> exp);
        Task<Live_Enterprise> Get(Expression<Func<Live_Enterprise, bool>> exp);
    }
}
