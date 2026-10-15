using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface IProductUserTaskService
    {
        Task<List<Model.Shop_V_ProductUserTask>> List(Expression<Func<Model.Shop_V_ProductUserTask, bool>> exp, PagedModelBase query);

        Task<int> Add(Shop_ProductUserTask model);
        Task<int> AddItem(Shop_ProductUserTaskItems model);

        Task<bool> UpdateAsync(Shop_ProductUserTask model);

        Task<bool> Delete(int id,int userId=0);
        Task<bool> Cancel(int id, int userId=0);
        Task<bool> SetStatus(int id, UserTaskStatus status);
        Task<bool> Exist(Expression<Func<Model.Shop_V_ProductUserTask, bool>> exp);
        Task<int> CountSync(Expression<Func<Shop_V_ProductUserTask, bool>> exp);
        Task<Model.Shop_ProductUserTask> Get(int id);
        Task<Model.Shop_V_ProductUserTask> GetView(int id);
        Task<Shop_V_ProductUserTask> GetView(Expression<Func<Shop_V_ProductUserTask, bool>> exp);
    }
}
