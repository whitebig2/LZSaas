using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface ICommentService
    {
        Task<List<Model.Shop_Comment>> List(Expression<Func<Model.Shop_Comment, bool>> exp, PagedModelBase query);
        Task<int> Add(Model.Shop_Comment model);
        Task<int> Add(List<Model.Shop_Comment> models);
        Task<bool> UpdateAsync(Model.Shop_Comment model);
        Task<bool> Delete(int id, int userId = 0);
        Task<bool> Exist(Expression<Func<Model.Shop_Comment, bool>> exp);
        Task<Model.Shop_Comment> Get(int id);
        Task<Model.Shop_Comment> Get(Expression<Func<Model.Shop_Comment, bool>> exp);
        Task<int> Count(Expression<Func<Model.Shop_Comment, bool>> exp);
        Task<bool> Like(int id);
        Task<CommentSummary> Summary(int productId);
    }
}
