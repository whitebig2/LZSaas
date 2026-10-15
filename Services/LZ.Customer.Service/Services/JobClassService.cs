using LZ.Customer.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;


namespace LZ.Customer.Service.Services
{
    public class JobClassService : ServicesBase<Live_JobClass>, IJobClassService, IScopedDependency
    {
        public async Task<List<Live_JobClass>> List(Expression<Func<Live_JobClass, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_JobClass>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_JobClass>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_JobClass model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_JobClass>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Set(int id, bool enabled)
        {
            return await base.Context.Updateable<Live_JobClass>().SetColumns(it => it.Enabled == enabled).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(Live_JobClass model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_JobClass> Get(int id)
        {
            return await base.Context.Queryable<Live_JobClass>().FirstAsync(it=>it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_JobClass>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
