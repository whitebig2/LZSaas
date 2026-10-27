using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface ICouponItemsService
    {
        Task<List<Model.Shop_V_CouponItems>> List(Expression<Func<Model.Shop_V_CouponItems, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Shop_CouponItems model);
        Task<int> Add(List<Shop_CouponItems> models);
        Task<Shop_V_CouponItems> GetView(Expression<Func<Shop_V_CouponItems, bool>> exp);
        Task<int> Count(Expression<Func<Shop_V_CouponItems, bool>> exp);
        Task<Shop_CouponItems> Get(Expression<Func<Shop_CouponItems, bool>> exp);
        Task<bool> UpdateAsync(Model.Shop_CouponItems model);
        Task<bool> Delete(Guid id);
        Task<bool> Exist(Expression<Func<Model.Shop_CouponItems, bool>> exp);
        Task<Model.Shop_CouponItems> Get(int id);
    }
}
