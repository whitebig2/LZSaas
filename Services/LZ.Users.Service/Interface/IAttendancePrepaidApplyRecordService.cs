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
    public interface IAttendancePrepaidApplyRecordService
    {
        Task<int> Add(Live_AttendancePrepaidApplyRecord model);
        Task<int> Add(List<Live_AttendancePrepaidApplyRecord> list);
        Task<bool> Delete(int id);

        Task<bool> Delete(Expression<Func<Live_AttendancePrepaidApplyRecord, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Exist(Expression<Func<Model.Live_AttendancePrepaidApplyRecord, bool>> exp);
        Task<Live_AttendancePrepaidApplyRecord> Get(int id);
        Task<Live_AttendancePrepaidApplyRecord> Get(Expression<Func<Live_AttendancePrepaidApplyRecord, bool>> exp);
        Task<List<Live_AttendancePrepaidApplyRecord>> List(Expression<Func<Live_AttendancePrepaidApplyRecord, bool>> exp, UserPagedModel query);

        Task<bool> Update(Live_AttendancePrepaidApplyRecord model);
        Task<int> DictionarySet(Dictionary<string, object> dt);
    }
}
