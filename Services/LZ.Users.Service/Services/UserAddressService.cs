using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using System.Linq.Expressions;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class UserAddressService : ServicesBase<Shop_UserAddress>, IUserAddressService, IScopedDependency
    {
        public async Task<List<Shop_UserAddress>> List(Expression<Func<Shop_UserAddress, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_UserAddress>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_UserAddress>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Shop_UserAddress model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_UserAddress, bool>> exp)
        {
            return await base.Context.Queryable<Shop_UserAddress>().AnyAsync(exp);
        }
        public async Task<bool> Update(Shop_UserAddress model)
        {
            return await base.Context.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<Shop_UserAddress> Get(int id)
        {
            return await base.Context.Queryable<Shop_UserAddress>().FirstAsync(it=>it.ID==id);
        }

        public async Task<Shop_UserAddress> Get(Expression<Func<Shop_UserAddress, bool>> exp)
        {
            return await base.Context.Queryable<Shop_UserAddress>().FirstAsync(exp);
        }
    }
}
