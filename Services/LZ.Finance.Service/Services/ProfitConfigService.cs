using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;

namespace LZ.Finance.Service.Services
{
    public class ProfitConfigService : ServicesBase<Live_ProfitConfig>, IProfitConfigService, IScopedDependency
    {
        public async Task<List<Live_V_ProfitConfig>> PageList(Expression<Func<Live_V_ProfitConfig, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_ProfitConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public Task<List<Live_V_ProfitConfig>> ListView(Expression<Func<Model.Live_V_ProfitConfig, bool>> exp = null)
        {
            return   base.Context.Queryable<Live_V_ProfitConfig>().Where(exp).ToListAsync();
        }
        public async Task<int> Add(Live_ProfitConfig model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_ProfitConfig> model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_ProfitConfig, bool>> exp)
        {
            return await   base.Context.Queryable<Live_ProfitConfig>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_ProfitConfig model)
        {
            return await   base.Context.Updateable<Live_ProfitConfig>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_ProfitConfig> Get(Expression<Func<Live_ProfitConfig, bool>> exp)
        {
            return await   base.Context.Queryable<Live_ProfitConfig>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_ProfitConfig>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
