using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface ISKUItemsService
    {
        Task<List<Model.Shop_V_SKUItems>> List(Expression<Func<Model.Shop_V_SKUItems, bool>> exp);

        Task<int> Add(Model.Shop_SKUItems model);
        Task<int> Adds(List<Model.Shop_SKUItems> models);
        Task<bool> UpdateAsync(Model.Shop_SKUItems model);
        Task<bool> ProductDelete(int productId);
        Task<bool> Exist(Expression<Func<Model.Shop_SKUItems, bool>> exp);
    }
}
