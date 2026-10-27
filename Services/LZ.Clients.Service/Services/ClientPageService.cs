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
    public class ClientPageService(IRedisService redisService) : ServicesBase<Model.Live_ClientPage>, IClientPageService, IScopedDependency
    {
        public async Task<List<Live_V_ClientPage>> List(Expression<Func<Live_V_ClientPage, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_ClientPage>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_ClientPage>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_ClientPage model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<int> Add(List<Live_ClientPage> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_ClientPage>().AnyAsync(it=>it.ID==id);
        }
        public async Task<bool> Update(Live_ClientPage model)
        {
            await redisService.SetAsync(string.Format(CoreRedisConstants.AppPage, model.ClientID, model.PageType), model, TimeSpan.FromHours(10));
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_ClientPage> Get(int id)
        {
            return await base.Context.Queryable<Live_ClientPage>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_V_ClientPage> GetView(int id)
        {
            return await base.Context.Queryable<Live_V_ClientPage>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_ClientPage> GetPage(int id, string type)
        {
            var appobj = await redisService.GetAsync<Live_ClientPage>(string.Format(CoreRedisConstants.AppPage, id, type));
            if (appobj == null)
            {
                appobj = await base.Context.Queryable<Live_ClientPage>().FirstAsync(it => it.ClientID == id && it.PageType == type && SqlFunc.Subqueryable<Model.Live_ClientMyTemplate>().Where(s => s.ClientID== id && s.ID==it.TempID && s.IsDefault).Any());
                await redisService.SetAsync(string.Format(CoreRedisConstants.AppPage, id, type), appobj, TimeSpan.FromHours(10));
            }
            return appobj;
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_ClientPage>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
