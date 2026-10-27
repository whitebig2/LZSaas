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
    public class AttendanceDetailService : ServicesBase<Live_AttendanceDetail>, IAttendanceDetailService, IScopedDependency
    {
        public async Task<List<Model.Live_AttendanceDetail>> List(Expression<Func<Model.Live_AttendanceDetail, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_AttendanceDetail>().Where(exp).OrderByDescending(t=>t.ConfigID).OrderBy(t=>t.Sort).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_AttendanceDetail>().Where(exp).OrderByDescending(t => t.ConfigID).OrderBy(t => t.Sort).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceDetail model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_AttendanceDetail> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceDetail model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceDetail>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_AttendanceDetail, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceDetail>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceDetail> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceDetail>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<Live_AttendanceDetail> Get(Expression<Func<Model.Live_AttendanceDetail, bool>> exp)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceDetail>().Where(exp).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceDetail>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendanceDetail, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceDetail>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceDetail>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
