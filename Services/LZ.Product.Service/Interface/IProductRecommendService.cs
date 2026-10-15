using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Product.Service.Interface
{
    public interface IProductRecommendService
    {
        Task<List<Shop_Product>> List(Expression<Func<Shop_Product, bool>> exp, ProductPagedModel query, int userId, int companyId);
    }
}
