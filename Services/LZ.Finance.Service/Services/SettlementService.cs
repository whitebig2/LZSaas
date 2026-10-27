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
    public class SettlementService : ServicesBase<Live_Settlement>, ISettlementService, IScopedDependency
    {

        public async Task<List<Live_V_Settlement>> PageList(Expression<Func<Live_V_Settlement, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_Settlement>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_V_Settlement>> List(Expression<Func<Live_V_Settlement, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_V_Settlement>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }
        public async Task<bool> Add(Live_Settlement model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }

        public async Task<bool> Add(List<Live_Settlement> model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_Settlement>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_Settlement model)
        {
            return await   base.Context.Updateable<Live_Settlement>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_Settlement> Get(Guid id)
        {
            return await   base.Context.Queryable<Live_Settlement>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_Settlement>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_Settlement, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_Settlement>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
