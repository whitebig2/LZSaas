using LZ.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Finance.Service.Interface;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System.Linq.Expressions;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class BudgetConfigService : ServicesBase<Live_BudgetConfig>, IBudgetConfigService, IScopedDependency
    {
        public async Task<List<Live_V_BudgetConfig>> List(Expression<Func<Live_V_BudgetConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_BudgetConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_BudgetConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Model.Live_BudgetConfig>> ListModel(Expression<Func<Model.Live_BudgetConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_BudgetConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_BudgetConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_BudgetConfig model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_BudgetConfig>().AnyAsync(it => it.GID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_BudgetConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetConfig>().AnyAsync(exp);
        }

        public async Task<bool> Update(Live_BudgetConfig model)
        {
            return await base.Context.Updateable<Live_BudgetConfig>(model).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_BudgetConfig> Get(Guid id)
        {
            return await base.Context.Queryable<Live_BudgetConfig>().FirstAsync(it => it.GID == id);
        }
        public async Task<Live_BudgetConfig> Get(Expression<Func<Live_BudgetConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_BudgetConfig>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_BudgetConfig>().Where(it => it.GID == id).ExecuteCommandAsync() > 0;
        }
    }
}
