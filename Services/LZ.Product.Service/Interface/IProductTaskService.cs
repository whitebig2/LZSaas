using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface IProductTaskService
    {
        Task<List<Model.Shop_V_ProductTask>> List(Expression<Func<Model.Shop_V_ProductTask, bool>> exp, PagedModelBase query);
        Task<List<Shop_ProductTaskItems>> Items(Expression<Func<Shop_ProductTaskItems, bool>> exp);
        Task<List<Shop_ProductTaskItems>> WeightItems(Expression<Func<Shop_ProductTaskItems, bool>> exp);
        Task<int> AddItems(List<Shop_ProductTaskItems> models);
        Task<int> Add(Shop_ProductTask model);
        Task<bool> AddUseCount(int taskId,int count=1);
        Task<bool> AddItemUseCount(int id, int count = 1);
        Task<bool> UpdateItems(List<Shop_ProductTaskItems> items);
        Task<bool> UpdateAsync(Shop_ProductTask model);

        Task<bool> Delete(int id);
        Task<bool> DeleteItem(int[] ids, int taskId);
        Task<bool> Exist(Expression<Func<Model.Shop_ProductTask, bool>> exp);
        Task<Shop_ProductTask> Get(Expression<Func<Model.Shop_ProductTask, bool>> exp);
        Task<Model.Shop_ProductTask> Get(int id);
        Task<bool> SetStatus(int id, int status);
        Task<Model.Shop_V_ProductTask> GetView(int id);
    }
}
