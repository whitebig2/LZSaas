using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Polly;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;


namespace LZ.Product.Service.Services
{
    public class CouponService(ICouponItemsService couponItemsService) : ServicesBase<Model.Shop_Coupon>, ICouponService, IScopedDependency
    {
        public async Task<List<Shop_Coupon>> List(Expression<Func<Shop_Coupon, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_Coupon>().Where(exp).OrderByDescending(it => it.CouponID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_Coupon>().Where(exp).OrderByDescending(it => it.CouponID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;

        }
        public async Task<List<Shop_CouponProduct>> Products(Expression<Func<Shop_CouponProduct, bool>> exp)
        {
            return await base.Context.Queryable<Shop_CouponProduct>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();

        }
        public async Task<List<Shop_V_CouponProduct>> ProductsView(Expression<Func<Shop_V_CouponProduct, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_CouponProduct>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();

        }
        public async Task<int> AddProducts(List<Shop_CouponProduct> models)
        {
            return await base.Context.Insertable(models).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(Shop_Coupon model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> UpdateProducts(List<Shop_CouponProduct> models, int couponId, Guid couponGid)
        {
            var updateItems = models.Where(it => it.ID > 0).ToList();
            var addItems = models.Where(it => it.ID == 0).ToList();
            foreach (var item in addItems)
            {
                item.CouponGID = couponGid;
                item.CouponID = couponId;
            }
            await base.Context.Updateable(updateItems).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            var ids = updateItems.Select(it => it.ID).ToList();
            await base.Context.Deleteable<Shop_CouponProduct>().Where(it => !ids.Contains(it.ID) && it.CouponID == couponId).ExecuteCommandAsync();
            await base.Context.Insertable(addItems).ExecuteCommandAsync();
            return true;
        }
        public async Task<bool> Exist(Expression<Func<Shop_Coupon, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Coupon>().AnyAsync(exp);
        }
        public async Task<Shop_Coupon> Get(Expression<Func<Shop_Coupon, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Coupon>().FirstAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_Coupon model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_Coupon> Get(int id)
        {
            return await base.Context.Queryable<Shop_Coupon>().FirstAsync(it => it.CouponID == id);
        }
        public async Task<bool> Delete(int id)
        {
            if(await couponItemsService.Exist(it => it.CouponID == id))
            {
                throw new ErrorCodeException(-1, "删除失败，已关联优惠券！");
            }
            var has = await base.Context.Deleteable<Shop_Coupon>().Where(it => it.CouponID == id).ExecuteCommandAsync() > 0;
            if (has)
            {
                await base.Context.Deleteable<Shop_CouponProduct>().Where(it => it.CouponID == id).ExecuteCommandAsync();
            }
            return has;
        }
    }
}
