using LZ.Clients.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Clients.Service.Services
{
    public class ClientNoticeTemplateService : ServicesBase<Model.Live_V_ClientNoticeTemplate>, IClientNoticeTemplateService, IScopedDependency
    {
        public async Task<List<Live_V_ClientNoticeTemplate>> List(Expression<Func<Live_V_ClientNoticeTemplate, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_ClientNoticeTemplate>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_ClientNoticeTemplate>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(List<Live_ClientNoticeTemplate> models)
        {
          return  await base.Context.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<int> Add(Live_ClientNoticeTemplate model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Live_ClientNoticeTemplate model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Exist(int clientId)
        {
            return await base.Context.Queryable<Model.Live_ClientNoticeTemplate>().AnyAsync(it=>it.ClientID==clientId);
        }
        public async Task<Live_ClientNoticeTemplate> Get(int id)
        {
            return await base.Context.Queryable<Live_ClientNoticeTemplate>().FirstAsync(it=>it.ID==id);
        }
        public async Task<Live_V_ClientNoticeTemplate> Get(int clientId,int noticeId)
        {
            return await base.Context.Queryable<Live_V_ClientNoticeTemplate>().FirstAsync(it => it.ClientID == clientId && it.NoticeID== noticeId);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_ClientNoticeTemplate>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
