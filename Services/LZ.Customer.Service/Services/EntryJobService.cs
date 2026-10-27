using LZ.Customer.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Customer.Service.Services
{
    public class EntryJobService : ServicesBase<Live_EntryJob>, IEntryJobService, IScopedDependency
    {
        public async Task<List<Live_EntryJob>> List(Expression<Func<Live_EntryJob, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_EntryJob>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_EntryJob>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_EntryJob model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_EntryJob>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Exist(Expression<Func<Model.Live_EntryJob, bool>> exp)
        {
            return await base.Context.Queryable<Live_EntryJob>().Where(exp).AnyAsync();
        }
        public async Task<bool> Update(Live_EntryJob model)
        {
            return await base.Context.Updateable(model).Where(it=>it.ID==model.ID).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id,bool enabled)
        {
            return await base.Context.Updateable<Live_EntryJob>().SetColumns(it => it.Enabled == enabled).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_EntryJob> Get(int id)
        {
            return await base.Context.Queryable<Live_EntryJob>().FirstAsync(it=>it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_EntryJob>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
