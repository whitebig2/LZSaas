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
    public class ClientTemplateService : ServicesBase<Model.Live_ClientTemplate>, IClientTemplateService, IScopedDependency
    {
        public async Task<List<Live_ClientTemplate>> List(Expression<Func<Live_ClientTemplate, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_ClientTemplate>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_ClientTemplate>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_ClientTemplate model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_ClientTemplate>().AnyAsync(it=>it.ID==id);
        }
        public async Task<bool> Update(Live_ClientTemplate model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_ClientTemplate> Get(int id)
        {
            return await base.Context.Queryable<Live_ClientTemplate>().FirstAsync(it => it.ID == id);
        }
        public async Task<Live_ClientTemplate> Get(Expression<Func<Live_ClientTemplate, bool>> exp)
        {
            return await base.Context.Queryable<Live_ClientTemplate>().FirstAsync(exp);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_ClientTemplate>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
