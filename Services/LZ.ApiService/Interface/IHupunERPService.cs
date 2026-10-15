using LZ.Model.ApiModel;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace LZ.ApiService.Interface
{
    public interface IHupunERPService
    {
        /// <summary>
        /// 商品分类推送
        /// </summary>
        /// <returns></returns>
        Task<HupunResultModel?> Categories(List<Model.Shop_Categories> items, int status=1);
        /// <summary>
        /// 商品推送(批量)
        /// </summary>
        /// <returns></returns>
        Task<HupunResultModel?> Product(List<Model.Shop_Product> items, List<Model.Shop_SKUs> skus = null, List<Model.Shop_V_SKUItems> skuItems = null, int status = 1);
        /// <summary>
        /// 商品推送
        /// </summary>
        /// <returns></returns>
        Task<HupunResultModel?> Product(Model.Shop_Product model, List<Model.Shop_SKUs> skus=null, List<Model.Shop_V_SKUItems> skuItems=null, int status = 1);
        /// <summary>
        /// 订单推送
        /// </summary>
        /// <returns></returns>
        Task<HupunResultModel?> Order(Model.Shop_Order order, List<Model.Shop_V_OrderItems> items, int status = 1, int has_refund = 0);
        /// <summary>
        /// 退单推送
        /// </summary>
        /// <returns></returns>
        Task<HupunResultModel?> OrderReturn(Model.Shop_OrderReturn model, List<Model.Shop_V_OrderReturnItems> items);
    }
}
