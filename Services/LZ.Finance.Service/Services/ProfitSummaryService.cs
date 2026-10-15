using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Finance.Service.Services
{
    public class ProfitSummaryService : ServicesBase<Live_ProfitSummary>, IProfitSummaryService, IScopedDependency
    {
        public async Task<List<Live_ProfitSummary>> List(Expression<Func<Live_ProfitSummary, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_ProfitSummary>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_ProfitSummary>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_ProfitSummary model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_ProfitSummary> models)
        {
            return await base.Context.Insertable(models).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_ProfitSummary>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_ProfitSummary, bool>> exp)
        {
            return await base.Context.Queryable<Live_ProfitSummary>().Where(exp).AnyAsync();
        }

        public async Task<bool> Update(Live_ProfitSummary model)
        {
            return await base.Context.Updateable<Live_ProfitSummary>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_ProfitSummary> Get(int id)
        {
            return await base.Context.Queryable<Live_ProfitSummary>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_ProfitSummary> Get(Expression<Func<Live_ProfitSummary, bool>> exp)
        {
            return await base.Context.Queryable<Live_ProfitSummary>().FirstAsync(exp);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_ProfitSummary>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_ProfitSummary, bool>> exp)
        {
            return await base.Context.Deleteable<Live_ProfitSummary>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
