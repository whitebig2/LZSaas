using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Basic.Service.Interface
{
    public interface IKnowledgeService
    {
        Task<List<Model.AI_Knowledge>> List(Expression<Func<Model.AI_Knowledge, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.AI_Knowledge model);
        Task<bool> Update(Model.AI_Knowledge model);
        Task<AI_Knowledge> Get(int id);
        Task<bool> Delete(int id);
    }
}
