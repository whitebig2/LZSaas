using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class BudgetReportConfigService : ServicesBase<Live_BudgetReportConfig>, IBudgetReportConfigService, IScopedDependency
    {
        public async Task<List<Live_BudgetReportConfig>> List(Expression<Func<Live_BudgetReportConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BudgetReportConfig>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BudgetReportConfig>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_BudgetReportConfig model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Live_BudgetReportConfig> list)
        {
            return await base.Context.Insertable(list).ExecuteCommandAsync();
        }

        public async Task<bool> Exist(int id)
        {
            return await base.Context.Queryable<Live_BudgetReportConfig>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BudgetReportConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetReportConfig>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BudgetReportConfig model)
        {
            return await base.Context.Updateable<Live_BudgetReportConfig>(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Model.Live_BudgetReportConfig> list)
        {
            return await base.Context.Updateable<Live_BudgetReportConfig>(list).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BudgetReportConfig> Get(int id)
        {
            return await base.Context.Queryable<Live_BudgetReportConfig>().FirstAsync(it => it.ID == id);
        }
        public async Task<Model.Live_BudgetReportConfig> Get(Expression<Func<Live_BudgetReportConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetReportConfig>().Where(exp).FirstAsync();
        }


        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_BudgetReportConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(Expression<Func<Live_BudgetReportConfig, bool>> exp)
        {
            return await base.Context.Deleteable<Live_BudgetReportConfig>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_BudgetReportConfig>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
