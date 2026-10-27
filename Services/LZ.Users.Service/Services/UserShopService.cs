using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using System.Linq.Expressions;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class UserShopService : ServicesBase<Shop_UserShop>, IUserShopService, IScopedDependency
    {
        public async Task<List<Shop_UserShop>> List(Expression<Func<Shop_UserShop, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_UserShop>().Where(exp).OrderByDescending(it=>it.ShopID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_UserShop>().Where(exp).OrderByDescending(it => it.ShopID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Shop_UserShop model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_UserShop, bool>> exp)
        {
            return await base.Context.Queryable<Shop_UserShop>().AnyAsync(exp);
        }
        public async Task<bool> Update(Shop_UserShop model)
        {
            return await base.Context.Updateable(model).IgnoreNullColumns(true).ExecuteCommandAsync() > 0;
        }
        public async Task<Shop_UserShop> Get(int id)
        {
            return await base.Context.Queryable<Shop_UserShop>().FirstAsync(it=>it.ShopID==id);
        }

        public async Task<Shop_UserShop> Get(Expression<Func<Shop_UserShop, bool>> exp)
        {
            return await base.Context.Queryable<Shop_UserShop>().FirstAsync(exp);
        }
    }
}
