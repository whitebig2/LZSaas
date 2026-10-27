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
    public interface IAttendanceAskForLeaveRecordService
    {
        Task<int> Add(Live_AttendanceAskForLeaveRecord model);
        Task<int> Add(List<Live_AttendanceAskForLeaveRecord> list);
        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<Live_AttendanceAskForLeaveRecord, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendanceAskForLeaveRecord, bool>> exp);
        Task<Live_AttendanceAskForLeaveRecord> Get(int id);
        Task<Live_AttendanceAskForLeaveRecord> Get(Expression<Func<Model.Live_AttendanceAskForLeaveRecord, bool>> exp);
        Task<Live_V_AttendanceAskForLeaveRecord> GetV(int id);
        Task<List<Live_V_AttendanceAskForLeaveRecord>> List(Expression<Func<Live_V_AttendanceAskForLeaveRecord, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendanceAskForLeaveRecord model);
    }
}
