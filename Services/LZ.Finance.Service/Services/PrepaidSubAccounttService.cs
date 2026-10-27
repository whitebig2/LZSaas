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
    public class PrepaidSubAccounttService : ServicesBase<Live_PrepaidSubAccount>, IPrepaidSubAccounttService, IScopedDependency
    {

        public async Task<List<Live_PrepaidSubAccount>> PageList(Expression<Func<Live_PrepaidSubAccount, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_PrepaidSubAccount>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PrepaidSubAccount>> List(Expression<Func<Live_PrepaidSubAccount, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_PrepaidSubAccount>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            return list;
        }
        public async Task<bool> Add(Live_PrepaidSubAccount model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }
     
    
        public async Task<bool> Update(Live_PrepaidSubAccount model)
        {
            return await   base.Context.Updateable<Live_PrepaidSubAccount>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_PrepaidSubAccount> Get(Expression<Func<Live_PrepaidSubAccount, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidSubAccount>().FirstAsync(exp);
        }
        public async Task<bool> Delete(int id)
        {
            return await   base.Context.Deleteable<Live_PrepaidSubAccount>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_PrepaidSubAccount, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_PrepaidSubAccount>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
