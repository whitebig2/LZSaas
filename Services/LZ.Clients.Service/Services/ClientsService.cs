using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.PagedModel;
using Polly;
using SqlSugar;
using SqlSugar.IOC;
using System.ComponentModel.Design;
using System.Linq.Expressions;


namespace LZ.Clients.Service.Services
{
    public class ClientsService(IRedisService redisService) : ServicesBase<Model.Live_Clients>, IClientsService, IScopedDependency
    {
        public async Task<List<Live_V_Clients>> List(Expression<Func<Live_V_Clients, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_Clients>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_Clients>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
            
        }
        public async Task<int> Add(Live_Clients model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id)
        {
            var expable = Expressionable.Create<Live_Clients>();
            expable.And(it => it.ID == id);
            var exp = expable.ToExpression();
            return await Exist(exp);
        }
        public async Task<bool> Exist(string appid, int id = 0)
        {
            var expable = Expressionable.Create<Live_Clients>();
            expable.And(it => it.AppID == appid);
            if (id > 0)
            {
                expable.And(it => it.ID != id);
            }
            var exp = expable.ToExpression();
            return await Exist(exp);
        }
        public async Task<bool> ChangeStatus(int id, int status, DateTime? time = null)
        {
            var db = base.Context.Updateable<Live_Clients>();
            db.SetColumns(it => it.ReleaseStatus == status);
            if (time.HasValue)
            {
                db.SetColumns(it => it.ReleaseTime == time);
            }
            return await db.Where(it=>it.ID==id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Exist(Expression<Func<Live_Clients, bool>> exp)
        {
            return await base.Context.Queryable<Live_Clients>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_Clients model)
        {
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.AppID, model.AppID));
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id, bool enabled)
        {
            return await base.Context.Updateable<Live_Clients>().SetColumns(it => it.Enabled == enabled).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_Clients> Get(int id)
        {
            return await base.Context.Queryable<Live_Clients>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_Clients> Get(Guid id)
        {
            return await base.Context.Queryable<Live_Clients>().FirstAsync(it => it.GID == id);
        }
        public async Task<Live_Clients> Get(int companyId, string type,string appType)
        {
            return await base.Context.Queryable<Live_Clients>().FirstAsync(it => it.CompanyID == companyId && it.Enabled && it.IsDefault && it.Type == type && it.AppType == appType);
        }
        public async Task<Live_ClientPage> GetPage(int id, string type)
        {
            var appobj = await redisService.GetAsync<Live_ClientPage>(string.Format(CoreRedisConstants.AppPage, id, type));
            if (appobj == null)
            {
                appobj = await base.Context.Queryable<Live_ClientPage>().FirstAsync(it => it.ClientID == id && it.PageType == type);
                await redisService.SetAsync(string.Format(CoreRedisConstants.AppPage, id, type), appobj, TimeSpan.FromHours(10));
            }
            return appobj;
        }
        public async Task<Live_Clients> Get(string appid)
        {
            var appobj = await redisService.GetAsync<Live_Clients>(string.Format(CoreRedisConstants.AppID, appid));
            if (appobj == null)
            {
                appobj = await base.Context.Queryable<Live_Clients>().FirstAsync(it => it.AppID == appid);
                await redisService.SetAsync(string.Format(CoreRedisConstants.AppID, appid), appobj, TimeSpan.FromHours(10));
            }
            return appobj;
        }
        public Live_Clients GetSyn(string appid)
        {
            return base.Context.Queryable<Live_Clients>().First(it => it.AppID == appid);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_Clients>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
