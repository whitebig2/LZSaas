using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface ISKUsService
    {
        Task<List<Model.Shop_SKUs>> List(Expression<Func<Model.Shop_SKUs, bool>> exp);

        Task<int> Add(Model.Shop_SKUs model);
        Task<int> Adds(List<Model.Shop_SKUs> models);

        Task<bool> UpdateAsync(Model.Shop_SKUs model);
        /// <summary>
        /// 同步库存
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="skuId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<bool> ChangeInventory(int productId,string skuId, int count);
        Task<bool> Delete(string id);
        Task<bool> ProductDelete(int productId);
        Task<bool> Exist(Expression<Func<Model.Shop_SKUs, bool>> exp);
        Task<Model.Shop_SKUs> Get(string id);
    }
}
