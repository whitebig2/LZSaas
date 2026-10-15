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
    public interface IAttendanceApplyLogService
    {
        Task<int> Add(Live_AttendanceApplyLog model);
        Task<int> Add(List<Live_AttendanceApplyLog> list);
        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<Live_AttendanceApplyLog, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceApplyLog, bool>> exp);
        Task<Live_AttendanceApplyLog> Get(int id);
        Task<List<Live_AttendanceApplyLog>> List(Expression<Func<Live_AttendanceApplyLog, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendanceApplyLog model);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
