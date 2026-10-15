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
    public class DictionaryService : ServicesBase<SYS_Dictionary>,  IDictionaryService, IScopedDependency
    {
        public async Task<List<SYS_Dictionary>> List(Expression<Func<SYS_Dictionary, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_Dictionary>().Where(exp).OrderBy(it=>it.Sort).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_Dictionary>().Where(exp).OrderBy(it => it.Sort).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(SYS_Dictionary model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<SYS_Dictionary>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(SYS_Dictionary model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id,bool enabled)
        {
            return await base.Context.Updateable<SYS_Dictionary>().SetColumns(it => it.Enabled == enabled).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<SYS_Dictionary> Get(int id)
        {
            return await base.Context.Queryable<SYS_Dictionary>().FirstAsync(it=>it.ID==id);
        }


        public async Task<SYS_Dictionary> Get(Expression<Func<SYS_Dictionary, bool>> exp)
        {
            return await base.Context.Queryable<SYS_Dictionary>().Where(exp).OrderBy(it => it.Sort).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<SYS_Dictionary>().Where(it => it.GUID == id).ExecuteCommandAsync() > 0;
        }
    }
}
