using LZ.Model;
using LZ.Model.ApiModel;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Product.Service.Interface
{
    public interface IProductService
    {
        Task<List<Model.Shop_Product>> List(Expression<Func<Model.Shop_Product, bool>> exp, PagedModelBase query, Expression<Func<Shop_Product, object>>? order = null);

        Task<int> Add(Shop_Product model, List<Model.Shop_SKUs> skus, List<Model.Shop_Attribute> attrs, List<Model.Shop_AttributeValues> values);

        Task<bool> UpdateAsync(Shop_Product model, List<Model.Shop_SKUs> skus, List<Model.Shop_Attribute> attrs, List<Model.Shop_AttributeValues> values);

        Task<bool> Delete(int id);
        Task<bool> SetStatus(int id, int status);
        Task<bool> Exist(Expression<Func<Model.Shop_Product, bool>> exp);
        Task<Model.Shop_Product> Get(int id);
        Task<Model.Shop_V_Product> GetView(int id);
        /// <summary>
        /// 批量修改状态
        /// </summary>
        /// <param name="productIds"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<bool> ChangeStatus(List<int> productIds, ProductStatus status);
        /// <summary>
        /// 同步库存
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<bool> ChangeInventory(int productId, int count);
        /// <summary>
        /// 同步库存
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<bool> ChangeInventory(int productId);
        /// <summary>
        /// 推送商品给erp
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<HupunResultModel?> SendProduct(Shop_Product model, int id = 0, int status = 1);
        /// <summary>
        /// 推送商品给erp(批量)
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<HupunResultModel?> SendProducts(Expression<Func<Shop_Product, bool>> exp, int status = 1);
    }
}
