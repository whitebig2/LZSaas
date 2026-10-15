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
    public class BusinessInsuranceRecordService : ServicesBase<Model.Live_BusinessInsuranceRecord>, IBusinessInsuranceRecordService, IScopedDependency
    {

        public async Task<int> Count(Expression<Func<Live_BusinessInsuranceRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_BusinessInsuranceRecord>()
                .Where(exp).CountAsync();
        }

        //public async Task<decimal> PrepaidApplyRecordSumAmount(Expression<Func<Live_PrepaidApplyRecord, bool>> exp)
        //{
        //    var qb = base.Context.Queryable<Live_PrepaidApplyRecord>();
        //    qb.Where(exp);
        //    return await qb.SumAsync(f=>f.ApplyAmount);
        //}
    }
}
