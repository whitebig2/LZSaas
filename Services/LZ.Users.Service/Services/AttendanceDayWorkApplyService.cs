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
    public class AttendanceDayWorkApplyService : ServicesBase<Live_AttendanceDayWorkApply>, IAttendanceDayWorkApplyService, IScopedDependency
    {
        public async Task<List<Model.Live_AttendanceDayWorkApply>> List(Expression<Func<Model.Live_AttendanceDayWorkApply, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_AttendanceDayWorkApply>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_AttendanceDayWorkApply>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceDayWorkApply model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<int> Add(List<Live_AttendanceDayWorkApply> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceDayWorkApply model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceDayWorkApply>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Delete(Expression<Func<Live_AttendanceDayWorkApply, bool>> exp)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceDayWorkApply>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceDayWorkApply> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceDayWorkApply>().Where(t => t.ID == id).FirstAsync();
        }

        public async Task<Live_AttendanceDayWorkApply> GetV(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceDayWorkApply>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceDayWorkApply>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> UpdateStatus(int id, int status, string msg = "")
        {

            var db = base.Context;
            var ub = db.Updateable<Live_AttendanceDayWorkApply>();
            if (status > 0)
            {
                ub.SetColumns(it => it.Status == status);
            }
            if (!string.IsNullOrEmpty(msg))
            {
                ub.SetColumns(it => it.Message == msg);
            }
            ub.Where(t => t.ID == id);
            return await ub.ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendanceDayWorkApply, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceDayWorkApply>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceDayWorkApply>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }
    }
}
