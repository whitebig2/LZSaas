using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface IAttendanceStatistics
    {
        Task<List<Model.Live_AttendancePrepaidApplyCustomer>> PrepaidApplyCustomerList(Expression<Func<Model.Live_AttendancePrepaidApplyCustomer, bool>> exp);
        Task<int> PrepaidApplyCustomerCount(Expression<Func<Model.Live_AttendancePrepaidApplyCustomer, bool>> exp);
        Task<Model.Live_AttendancePrepaidApplyCustomer> GetPrepaidApplyCustomer(Expression<Func<Model.Live_AttendancePrepaidApplyCustomer, bool>> exp, string order);

        Task<List<Model.Live_V_AttendanceRecord>> AttendanceRecordList(Expression<Func<Model.Live_V_AttendanceRecord, bool>> exp);
        Task<int> AttendanceRecordCount(Expression<Func<Model.Live_V_AttendanceRecord, bool>> exp);

        Task<Model.Live_V_AttendanceRecord> GetAttendanceRecord(Expression<Func<Model.Live_V_AttendanceRecord, bool>> exp, string order);
        Task<int> PrepaidApplyRecordCount(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp);

        Task<Model.Live_V_PrepaidApplyRecord> GetPrepaidApplyRecord(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp, string order);

        Task<List<Model.Live_V_PrepaidApplyRecord>> PrepaidApplyRecordList(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp);
        Task<int> AttendancePrepaidApplyCustomerCount(Expression<Func<Model.Live_V_AttendancePrepaidApplyCustomer, bool>> exp);
        Task<int> AttendanceApplyCount(Expression<Func<Model.Live_V_AttendanceApply, bool>> exp);
    }
}
