using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.ShopBasic.Service.Interface;
using System.Linq.Expressions;


namespace LZ.ShopBasic.Service.Services
{
    public class BrandService : ServicesBase<Model.Shop_Attribute>, IBrandService, IScopedDependency
    {
        public async Task<List<Shop_Brand>> List(Expression<Func<Shop_Brand, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Brand>().Where(exp).OrderByDescending(it => it.BrandID).ToListAsync();

        }
        public async Task<int> Add(Shop_Brand model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_Brand, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Brand>().AnyAsync(exp);
        }
        public async Task<bool> Update(Shop_Brand model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_Brand> Get(int id)
        {
            return await base.Context.Queryable<Shop_Brand>().FirstAsync(it => it.BrandID == id);
        }
        public async Task<bool> Delete(int id)
        {
            var has = await base.Context.Deleteable<Shop_Brand>().Where(it => it.BrandID == id).ExecuteCommandAsync() > 0;
            return has;
        }
    }
}
