using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
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
    public class CouponItemsService : ServicesBase<Model.Shop_CouponItems>, ICouponItemsService, IScopedDependency
    {
        public async Task<List<Shop_V_CouponItems>> List(Expression<Func<Shop_V_CouponItems, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_V_CouponItems>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_V_CouponItems>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;

        }
        public async Task<int> Add(Shop_CouponItems model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Shop_CouponItems> models)
        {
            return await base.Context.Insertable(models).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_CouponItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_CouponItems>().AnyAsync(exp);
        }
        public override async Task<bool> UpdateAsync(Shop_CouponItems model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Shop_V_CouponItems> GetView(Expression<Func<Shop_V_CouponItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_CouponItems>().FirstAsync(exp);
        }
        public async Task<Shop_CouponItems> Get(Expression<Func<Shop_CouponItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_CouponItems>().FirstAsync(exp);
        }
        public async Task<int> Count(Expression<Func<Shop_V_CouponItems, bool>> exp)
        {
            return await base.Context.Queryable<Shop_V_CouponItems>().CountAsync(exp);
        }
        public async Task<Shop_CouponItems> Get(int id)
        {
            return await base.Context.Queryable<Shop_CouponItems>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(Guid id)
        {
            var has = await base.Context.Deleteable<Shop_CouponItems>().Where(it => it.GID == id).Where(it=>it.Status == (int)Model.Enums.CouponItemStatus.Void || it.Status == (int)Model.Enums.CouponItemStatus.WaitExchange).ExecuteCommandAsync() > 0;
            if (!has)
            {
                throw new ErrorCodeException(-1, "删除失败，只有作废或者待绑定优惠券才可删除！");
            }
            return has;
        }
    }
}
