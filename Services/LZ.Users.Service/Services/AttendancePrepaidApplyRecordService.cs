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
    public class AttendancePrepaidApplyRecordService : ServicesBase<Live_AttendancePrepaidApplyRecord>, IAttendancePrepaidApplyRecordService, IScopedDependency
    {
        public async Task<List<Model.Live_AttendancePrepaidApplyRecord>> List(Expression<Func<Model.Live_AttendancePrepaidApplyRecord, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_AttendancePrepaidApplyRecord>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_AttendancePrepaidApplyRecord>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendancePrepaidApplyRecord model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_AttendancePrepaidApplyRecord> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendancePrepaidApplyRecord model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendancePrepaidApplyRecord>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_AttendancePrepaidApplyRecord, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendancePrepaidApplyRecord>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendancePrepaidApplyRecord> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendancePrepaidApplyRecord>().Where(t => t.ID == id).FirstAsync();
        }

        public async Task<Live_AttendancePrepaidApplyRecord> Get(Expression<Func<Live_AttendancePrepaidApplyRecord, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendancePrepaidApplyRecord>().Where(exp).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendancePrepaidApplyRecord>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendancePrepaidApplyRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendancePrepaidApplyRecord>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendancePrepaidApplyRecord>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
