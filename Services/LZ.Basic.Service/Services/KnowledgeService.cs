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
    public class KnowledgeService : ServicesBase<AI_Knowledge>, IKnowledgeService, IScopedDependency
    {
        public async Task<List<AI_Knowledge>> List(Expression<Func<AI_Knowledge, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<AI_Knowledge>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<AI_Knowledge>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(AI_Knowledge model)
        {
          return  await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Update(AI_Knowledge model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<AI_Knowledge> Get(int id)
        {
            return await base.Context.Queryable<AI_Knowledge>().FirstAsync(it=>it.ID == id);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<AI_Knowledge>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
