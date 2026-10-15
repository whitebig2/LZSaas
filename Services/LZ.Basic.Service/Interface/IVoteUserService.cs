using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Basic.Service.Interface
{
    public interface IVoteUserService
    {
        Task<List<Model.Live_VoteUser>> List(Expression<Func<Model.Live_VoteUser, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_VoteUser model);
        Task<int> AddItems(List<Live_VoteItems> items);
        Task<bool> Exist(Expression<Func<Live_VoteUser, bool>> exp);

        Task<bool> Update(Model.Live_VoteUser model);
        Task<bool> Delete(int id);
        Task<Model.Live_VoteUser> Get(int id);
    }
}
