using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;


namespace LZ.Basic.Service.Services
{
    public class ContentService : ServicesBase<SYS_Content>, IContentService, IScopedDependency
    {
        public async Task<List<SYS_V_Content>> List(Expression<Func<SYS_V_Content, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<SYS_V_Content>().IgnoreColumns(it=>it.Contents).Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<SYS_V_Content>().IgnoreColumns(it => it.Contents).Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(SYS_Content model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(SYS_Content model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<SYS_Content> Get(int id)
        {
            return await base.Context.Queryable<SYS_Content>().FirstAsync(it=>it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<SYS_Content>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
