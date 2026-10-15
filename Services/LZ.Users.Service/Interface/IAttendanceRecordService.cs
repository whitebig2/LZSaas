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
    public interface IAttendanceRecordService
    {
        Task<int> Add(Live_AttendanceRecord model);
        Task<int> Add(List<Live_AttendanceRecord> list);
        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<Live_AttendanceRecord, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceRecord, bool>> exp);
        Task<Live_AttendanceRecord> Get(int id);
        Task<Live_AttendanceRecord> Get(Expression<Func<Live_AttendanceRecord, bool>> exp);
        Task<List<Live_AttendanceRecord>> List(Expression<Func<Live_AttendanceRecord, bool>> exp, UserPagedModel query);
        Task<List<Live_V_AttendanceRecord>> ListView(Expression<Func<Live_V_AttendanceRecord, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendanceRecord model);
        Task<bool> Update(List<Live_AttendanceRecord> list);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
