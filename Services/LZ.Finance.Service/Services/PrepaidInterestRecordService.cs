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
    public class PrepaidInterestRecordService : ServicesBase<Live_PrepaidInterestRecord>, IPrepaidInterestRecordService, IScopedDependency
    {

        public async Task<List<Live_PrepaidInterestRecord>> PageList(Expression<Func<Live_PrepaidInterestRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_PrepaidInterestRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PrepaidInterestRecord>> List(Expression<Func<Live_PrepaidInterestRecord, bool>> exp)
        {
            var list = await   base.Context.Queryable<Live_PrepaidInterestRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            return list;
        }
        public async Task<bool> Add(Live_PrepaidInterestRecord model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
        }
        public async Task<bool> Add(List<Live_PrepaidInterestRecord> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandIdentityIntoEntityAsync();
        }

      

        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_PrepaidInterestRecord>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_PrepaidInterestRecord model)
        {
            return await   base.Context.Updateable<Live_PrepaidInterestRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_PrepaidInterestRecord> Get(Expression<Func<Live_PrepaidInterestRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidInterestRecord>().FirstAsync(exp);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_PrepaidInterestRecord>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_PrepaidInterestRecord, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_PrepaidInterestRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }
    }
}
