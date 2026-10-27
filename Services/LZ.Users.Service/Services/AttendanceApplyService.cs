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
    public class AttendanceApplyService : ServicesBase<Live_AttendanceApply>, IAttendanceApplyService, IScopedDependency
    {
        public async Task<List<Model.Live_V_AttendanceApply>> List(Expression<Func<Model.Live_V_AttendanceApply, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_V_AttendanceApply>().Where(exp);
            if (!string.IsNullOrEmpty(query.Order))
            {
                sco.OrderBy(query.Order);
            }
            else
            {
                sco.OrderByDescending(t => t.CreateTime);
            }
            if (query.Page == 0)
            {
                return await sco.Where(exp).ToListAsync();
            }
            var list = await sco.Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceApply model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_AttendanceApply> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceApply model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceApply>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_AttendanceApply, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceApply>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceApply> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceApply>().Where(t => t.ID == id).FirstAsync();
        }

        public async Task<Live_V_AttendanceApply> GetV(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_V_AttendanceApply>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceApply>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendanceApply, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceApply>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceApply>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
        public async Task<int> Count(Expression<Func<Model.Live_AttendanceApply, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceApply>().Where(exp).CountAsync();
        }
    }
}
