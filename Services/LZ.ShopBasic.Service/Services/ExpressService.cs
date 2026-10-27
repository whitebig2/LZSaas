using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.ShopBasic.Service.Interface;
using System.Linq.Expressions;


namespace LZ.ShopBasic.Service.Services
{
    public class AttributeService : ServicesBase<Model.Shop_Attribute>, IExpressService, IScopedDependency
    {
        public async Task<List<Shop_Express>> List(Expression<Func<Shop_Express, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Express>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();

        }
        public async Task<int> Add(Shop_Express model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(Expression<Func<Shop_Express, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Express>().AnyAsync(exp);
        }
        public async Task<bool> Update(Shop_Express model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_Express> Get(int id)
        {
            return await base.Context.Queryable<Shop_Express>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            var has = await base.Context.Deleteable<Shop_Express>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
            return has;
        }
        public async Task<List<Shop_ExpressInfo>> InfoList(string expressNo)
        {
            return await base.Context.Queryable<Shop_ExpressInfo>().Where(it => it.ExpressNo == expressNo).OrderBy(it => it.CreateTime).ToListAsync();
        }
    }
}
