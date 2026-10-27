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
    public class PrepaidApplyRecordService : ServicesBase<Model.Live_PrepaidApplyRecord>, IPrepaidApplyRecordService, IScopedDependency
    {

        public async Task<List<Live_V_PrepaidApplyRecord>> PrepaidApplyRecordList(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_PrepaidApplyRecord>();
            qb.Where(exp);
            return await qb.ToListAsync();
        }

        public async Task<decimal> PrepaidApplyRecordSumAmount(Expression<Func<Live_PrepaidApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_PrepaidApplyRecord>();
            qb.Where(exp);
            return await qb.SumAsync(f=>f.ApplyAmount);
        }

        /// <summary>
        ///  金融技术服务费
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<decimal> PrepaidInterestReportSumAmount(Expression<Func<Live_PrepaidInterestReport, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_PrepaidInterestReport>();
            qb.Where(exp);
            return await qb.SumAsync(f => f.InterestMonthAmount);
        }

        
    }
}
