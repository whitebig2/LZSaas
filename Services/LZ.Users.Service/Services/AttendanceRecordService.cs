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
using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class AttendanceRecordService : ServicesBase<Live_AttendanceRecord>, IAttendanceRecordService , IScopedDependency
    {
        public async Task<List<Live_V_AttendanceRecord>> ListView(Expression<Func<Live_V_AttendanceRecord, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_AttendanceRecord>().Where(exp);
            if(!string.IsNullOrEmpty(query.Order))
            {
                sco.OrderBy(query.Order);
            }
            else
            {
                sco.OrderBy(t => t.AttendanceDate);
            }
            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Model.Live_AttendanceRecord>> List(Expression<Func<Model.Live_AttendanceRecord, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_AttendanceRecord>().Where(exp).OrderBy(t=>t.AttendanceDate).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_AttendanceRecord>().Where(exp).OrderBy(t => t.AttendanceDate).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceRecord model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_AttendanceRecord> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceRecord model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_AttendanceRecord> list)
        {
            var db = base.Context;
            return await db.Updateable(list).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceRecord>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_AttendanceRecord, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceRecord> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceRecord>().Where(t => t.ID == id).FirstAsync();
        }

        public async Task<Live_AttendanceRecord> Get(Expression<Func<Live_AttendanceRecord, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceRecord>().Where(exp).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceRecord>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendanceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceRecord>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceRecord>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
