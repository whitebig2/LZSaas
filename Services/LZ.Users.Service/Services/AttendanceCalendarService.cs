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
using System.Globalization;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class AttendanceCalendarService : ServicesBase<Live_AttendanceCalendar>, IAttendanceCalendarService, IScopedDependency
    {
        public async Task<List<Model.Live_AttendanceCalendar>> List(Expression<Func<Model.Live_AttendanceCalendar, bool>> exp, UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_AttendanceCalendar>().Where(exp).OrderByDescending(t=>t.Year).OrderBy(t=>t.Calendar).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_AttendanceCalendar>().Where(exp).OrderByDescending(t => t.Year).OrderBy(t => t.Calendar).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_AttendanceCalendar model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<int> Add(List<Live_AttendanceCalendar> list)
        {
            var db = base.Context;
            return await db.Insertable(list).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_AttendanceCalendar model)
        {
            var db = base.Context;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_AttendanceCalendar>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<Model.Live_AttendanceCalendar> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_AttendanceCalendar>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_AttendanceCalendar>().AnyAsync(it => it.ID == id);
        }

        public async Task<bool> ExistDate(DateTime date,int configID,bool holiday)
        {
            var dq =  base.Context.Queryable<Live_AttendanceCalendar>().Where(t => t.Calendar.HasValue && t.Calendar.Value.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd"));
            if (holiday)
            {
                dq.Where(t => t.Status == 1 || t.Status == 3);
            }
            var calendar = await dq.FirstAsync();
          
            if (calendar==null)
            {
                return false;
            }
            var config = await base.Context.Queryable<Model.Live_AttendanceConfig>().Where(t => t.ID == configID).FirstAsync();
            bool isWork = true;
            if (calendar.Status == 3)
            {
                isWork = true;
            }
            else if (!string.IsNullOrEmpty(config.Days))
            {
                string[] arr = config.Days.Split(",");
                string d = ((int)date.DayOfWeek).ToString();
                isWork = arr.Contains(d);
            }
            else
            {
                isWork = false;
            }
            return isWork;
        }

        public async Task<bool> Exist(Expression<Func<Live_AttendanceCalendar, bool>> exp)
        {
            return await base.Context.Queryable<Live_AttendanceCalendar>().AnyAsync(exp);
        }

        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_AttendanceCalendar>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }

        public async Task<bool> IsWorkDaty(DateTime date)
        {
            List<int> days= new List<int>() { 1,2,3,4,5};
            var isWork = await base.Context.Queryable<Live_AttendanceCalendar>().Where(t => t.Calendar.HasValue && t.Calendar.Value.ToString("yyyy-MM-dd") == date.ToString("yyyy-MM-dd") && ((t.Status == 1 && days.Contains(t.DayEnum.Value)) || t.Status == 3)).AnyAsync();
            return isWork;
        }

    }
}
