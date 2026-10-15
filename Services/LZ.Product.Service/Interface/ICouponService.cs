using System.Linq.Expressions;
using LZ.Model;
using LZ.Model.PagedModel;

namespace LZ.Product.Service.Interface
{
    public interface ICouponService
    {
        Task<List<Model.Shop_Coupon>> List(Expression<Func<Model.Shop_Coupon, bool>> exp, PagedModelBase query);
        Task<List<Shop_CouponProduct>> Products(Expression<Func<Shop_CouponProduct, bool>> exp);
        Task<List<Shop_V_CouponProduct>> ProductsView(Expression<Func<Shop_V_CouponProduct, bool>> exp);
        Task<int> Add(Model.Shop_Coupon model);
        Task<int> AddProducts(List<Shop_CouponProduct> models);
        Task<bool> UpdateProducts(List<Shop_CouponProduct> models, int couponId, Guid couponGid);
        Task<bool> UpdateAsync(Model.Shop_Coupon model);
        Task<bool> Delete(int id);
        Task<bool> Exist(Expression<Func<Model.Shop_Coupon, bool>> exp);
        Task<Shop_Coupon> Get(Expression<Func<Model.Shop_Coupon, bool>> exp);
        Task<Model.Shop_Coupon> Get(int id);
    }
}
