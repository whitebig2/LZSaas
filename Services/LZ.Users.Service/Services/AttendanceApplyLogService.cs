using LZ.Core.Interface;
using LZ.Model.PagedModel;
using LZ.Model;
using LZ.Users.Service.Interface;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class AttendanceApplyLogService : ServicesBase<Model.Live_AttendanceApplyLog>,  IAttendanceApplyLogService, IScopedDependency
    {
        public async Task<List<Model.Live_AttendanceApplyLog>> List(Expression<Func<Model.Live_AttendanceApplyLog, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_AttendanceApplyLog>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_AttendanceApplyLog>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceApplyLog model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_AttendanceApplyLog> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceApplyLog model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceApplyLog>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_AttendanceApplyLog, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceApplyLog>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceApplyLog> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceApplyLog>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceApplyLog>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendanceApplyLog, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceApplyLog>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceApplyLog>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
