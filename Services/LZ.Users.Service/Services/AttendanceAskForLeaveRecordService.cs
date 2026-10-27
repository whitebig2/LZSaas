using LZ.Core.Interface;
using LZ.Core;
using LZ.Model;
using LZ.Users.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Users.Service.Services
{
    public class AttendanceAskForLeaveRecordService : ServicesBase<Live_AttendanceApply>, IAttendanceAskForLeaveRecordService, IScopedDependency
    {
        public async Task<List<Model.Live_V_AttendanceAskForLeaveRecord>> List(Expression<Func<Model.Live_V_AttendanceAskForLeaveRecord, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_AttendanceAskForLeaveRecord>().Where(exp);
            if (!string.IsNullOrEmpty(query.Order))
            {
                sco.OrderBy(query.Order);
            }
            else
            {
                sco.OrderBy(t=>t.Status).OrderBy(t => t.CreateTime);
            }
            if (query.Page == 0)
            {
                return await sco.Where(exp).ToListAsync();
            }
            var list = await sco.Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceAskForLeaveRecord model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_AttendanceAskForLeaveRecord> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceAskForLeaveRecord model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceAskForLeaveRecord>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_AttendanceAskForLeaveRecord, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceAskForLeaveRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceAskForLeaveRecord> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceAskForLeaveRecord>().Where(t => t.ID == id).FirstAsync();
        }

        public async Task<Live_AttendanceAskForLeaveRecord> Get(Expression<Func<Model.Live_AttendanceAskForLeaveRecord, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceAskForLeaveRecord>().Where(exp).FirstAsync();
        }

        public async Task<Live_V_AttendanceAskForLeaveRecord> GetV(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_V_AttendanceAskForLeaveRecord>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceAskForLeaveRecord>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendanceAskForLeaveRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceAskForLeaveRecord>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceAskForLeaveRecord>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
