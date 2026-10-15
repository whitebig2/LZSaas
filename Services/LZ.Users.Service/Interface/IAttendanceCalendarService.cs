using LZ.Model.PagedModel;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IAttendanceCalendarService
    {
        Task<int> Add(Live_AttendanceCalendar model);
        Task<int> Add(List<Live_AttendanceCalendar> list);
        Task<bool> Delete(int id);
        Task<bool> ExistID(int id);
        Task<bool> ExistDate(DateTime date, int configID, bool holiday);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceCalendar, bool>> exp);
        Task<Live_AttendanceCalendar> Get(int id);
        Task<List<Live_AttendanceCalendar>> List(Expression<Func<Live_AttendanceCalendar, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendanceCalendar model);
        Task<int> DictionarySet(Dictionary<string, object> dt);
        Task<bool> IsWorkDaty(DateTime date);
    }
}
