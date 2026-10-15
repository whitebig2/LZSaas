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
using LZ.Core.Interface;
using LZ.Users.Service.Interface;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class AttendanceConfigService : ServicesBase<Live_AttendanceConfig>, IAttendanceConfigService, IScopedDependency
    {
        public async Task<List<Model.Live_AttendanceConfig>> List(Expression<Func<Model.Live_AttendanceConfig, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_AttendanceConfig>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_AttendanceConfig>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public long GetSequence()
        {
            var sqeuareNo =  base.Context.Ado.SqlQuery<long>("select  next value for DayWorkJobID").First();
            return sqeuareNo;
            //var nextValue = await base.Context.SqlQueryable<CountModel>("select next value for DayWorkJobID as Count").FirstAsync();
            //return nextValue.Count;
        }
        public async Task<List<Model.Live_V_AttendanceConfig>> ListV(Expression<Func<Model.Live_V_AttendanceConfig, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_AttendanceConfig>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_AttendanceConfig>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceConfig model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceConfig model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceConfig>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceConfig> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceConfig>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceConfig>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendanceConfig, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceConfig>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceConfig>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
