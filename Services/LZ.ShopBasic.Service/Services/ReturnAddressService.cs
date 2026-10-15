using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.ShopBasic.Service.Interface;
using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Services
{
    public class ReturnAddressService : ServicesBase<Model.Shop_ReturnAddress>, IReturnAddressService, IScopedDependency
    {
        public async Task<List<Shop_ReturnAddress>> List(Expression<Func<Shop_ReturnAddress, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ReturnAddress>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
        }
        public async Task<int> Add(Shop_ReturnAddress model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_ReturnAddress, bool>> exp)
        {
            return await base.Context.Queryable<Shop_ReturnAddress>().AnyAsync(exp);
        }
        public async Task<bool> Update(Shop_ReturnAddress model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_ReturnAddress> Get(int id)
        {
            return await base.Context.Queryable<Shop_ReturnAddress>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Shop_ReturnAddress>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
