using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Model.PagedModel;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class EntryRecordService : ServicesBase<Live_UserEntryRecord>, IEntryRecordService, IScopedDependency
    {
        public async Task<List<Live_V_UserEntryRecord>> List(Expression<Func<Live_V_UserEntryRecord, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_V_UserEntryRecord>();
            qb.Where(exp);
            qb.OrderByDescending(it => it.ID);
            if (query.Page == 0)
            {
                return await qb.ToListAsync();
            }
            var list = await qb.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_UserEntryRecord model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_UserEntryRecord>().Where(it => it.UserID == id).ExecuteCommandAsync() > 0;
        }
    }
}
