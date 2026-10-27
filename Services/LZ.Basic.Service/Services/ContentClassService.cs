using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;


namespace LZ.Basic.Service.Services
{
    public class ContentClassService : ServicesBase<SYS_ContentClass>, IContentClassService, IScopedDependency
    {
        public async Task<List<SYS_ContentClass>> List(Expression<Func<SYS_ContentClass, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_ContentClass>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_ContentClass>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(SYS_ContentClass model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(SYS_ContentClass model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<SYS_ContentClass> Get(int id)
        {
            return await base.Context.Queryable<SYS_ContentClass>().FirstAsync(it=>it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<SYS_ContentClass>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
