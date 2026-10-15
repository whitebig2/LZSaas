using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Statistics.Service.Interface;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Services
{
    public class AttendanceStatistics : ServicesBase<Model.Live_AttendancePrepaidApplyCustomer>, IAttendanceStatistics, IScopedDependency
    {
        public async Task<List<Model.Live_AttendancePrepaidApplyCustomer>> PrepaidApplyCustomerList(Expression<Func<Model.Live_AttendancePrepaidApplyCustomer, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_AttendancePrepaidApplyCustomer>();
            return await qb.Where(exp).ToListAsync();
        }
        public async Task<int> PrepaidApplyCustomerCount(Expression<Func<Model.Live_AttendancePrepaidApplyCustomer, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_AttendancePrepaidApplyCustomer>();
            return await qb.CountAsync(exp);
        }
        public async Task<Model.Live_AttendancePrepaidApplyCustomer> GetPrepaidApplyCustomer(Expression<Func<Model.Live_AttendancePrepaidApplyCustomer, bool>> exp, string order)
        {
            var qb = base.Context.Queryable<Live_AttendancePrepaidApplyCustomer>();
            return await qb.Where(exp).OrderBy(order).FirstAsync();
        }

        public async Task<List<Model.Live_V_AttendanceRecord>> AttendanceRecordList(Expression<Func<Model.Live_V_AttendanceRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_AttendanceRecord>();
            return await qb.Where(exp).ToListAsync();
        }

        public async Task<int> AttendanceRecordCount(Expression<Func<Model.Live_V_AttendanceRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_AttendanceRecord>();
            return await qb.CountAsync(exp);
        }

        public async Task<Model.Live_V_AttendanceRecord> GetAttendanceRecord(Expression<Func<Model.Live_V_AttendanceRecord, bool>> exp, string order)
        {
            var qb = base.Context.Queryable<Live_V_AttendanceRecord>();
            return await qb.Where(exp).OrderBy(order).FirstAsync();
        }

        public async Task<Model.Live_V_PrepaidApplyRecord> GetPrepaidApplyRecord(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp, string order)
        {
            var qb = base.Context.Queryable<Live_V_PrepaidApplyRecord>();
            return await qb.Where(exp).OrderBy(order).FirstAsync();
        }

        public async Task<int> PrepaidApplyRecordCount(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_PrepaidApplyRecord>();
            return await qb.CountAsync(exp);
        }

        public async Task<List<Model.Live_V_PrepaidApplyRecord>> PrepaidApplyRecordList(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_PrepaidApplyRecord>();
            return await qb.Where(exp).ToListAsync();
        }

        public async Task<int> AttendancePrepaidApplyCustomerCount(Expression<Func<Model.Live_V_AttendancePrepaidApplyCustomer, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_AttendancePrepaidApplyCustomer>();
            return await qb.CountAsync(exp);
        }
        public async Task<int> AttendanceApplyCount(Expression<Func<Model.Live_V_AttendanceApply, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_AttendanceApply>();
            return await qb.CountAsync(exp);
        }
    }
}
