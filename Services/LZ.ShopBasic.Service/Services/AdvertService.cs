using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.ShopBasic.Service.Interface;
using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Services
{
    public class AdvertService : ServicesBase<Model.Shop_Advert>, IAdvertService, IScopedDependency
    {
        public async Task<List<Shop_Advert>> List(Expression<Func<Shop_Advert, bool>> exp, bool orderBySort = false)
        {
            var query = base.Context.Queryable<Shop_Advert>().Where(exp);
            if (orderBySort)
            {
                return await query.OrderBy(it => it.Sort).OrderBy(it => it.ID).ToListAsync();
            }
            return await query.OrderByDescending(it => it.ID).ToListAsync();
        }

        public async Task<int> Add(Shop_Advert model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<bool> Exist(Expression<Func<Shop_Advert, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Advert>().AnyAsync(exp);
        }

        public async Task<bool> Update(Shop_Advert model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_Advert> Get(int id)
        {
            return await base.Context.Queryable<Shop_Advert>().FirstAsync(it => it.ID == id);
        }
        public async Task<Shop_Advert> Get(Expression<Func<Shop_Advert, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Advert>().FirstAsync(exp);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Shop_Advert>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
