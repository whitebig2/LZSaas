using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;


namespace LZ.Clients.Service.Services
{
    public class WeiXinMenuService : ServicesBase<Model.Live_WeiXinMenu>, IWeiXinMenuService, IScopedDependency
    {
        public async Task<List<Live_WeiXinMenu>> List(Expression<Func<Live_WeiXinMenu, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_WeiXinMenu>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_WeiXinMenu>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_WeiXinMenu model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            var expable = Expressionable.Create<Model.Live_WeiXinMenu>();
            expable.And(it => it.ID == id);
            var exp = expable.ToExpression();
            return await Exist(exp);
        }
        public async Task<bool> Exist(Expression<Func<Live_WeiXinMenu, bool>> exp)
        {
            return await base.Context.Queryable<Live_WeiXinMenu>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_WeiXinMenu model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_WeiXinMenu> Get(int id)
        {
            return await base.Context.Queryable<Live_WeiXinMenu>().FirstAsync(it=>it.ID==id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_WeiXinMenu>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
