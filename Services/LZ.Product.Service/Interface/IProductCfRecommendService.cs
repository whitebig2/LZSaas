using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Product.Service.Interface
{
    /// <summary>
    /// 基于用户加购 + 访问记录的 Item-CF 协同过滤推荐服务。
    /// </summary>
    public interface IProductCfRecommendService
    {
        Task<List<Shop_Product>> List(Expression<Func<Shop_Product, bool>> exp, ProductPagedModel query, int userId, int companyId);
    }
}
