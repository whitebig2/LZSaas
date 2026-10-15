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
    public class ClientTemplatePageService : ServicesBase<Model.Live_ClientTemplatePage>, IClientTemplatePageService, IScopedDependency
    {
        public async Task<List<Live_ClientTemplatePage>> List(Expression<Func<Live_ClientTemplatePage, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_ClientTemplatePage>().Where(exp).OrderByDescending(it=>it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_ClientTemplatePage>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_ClientTemplatePage model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_ClientTemplatePage>().AnyAsync(it=>it.ID==id);
        }
        public async Task<bool> Update(Live_ClientTemplatePage model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_ClientTemplatePage> Get(int id)
        {
            return await base.Context.Queryable<Live_ClientTemplatePage>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_ClientTemplatePage>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
