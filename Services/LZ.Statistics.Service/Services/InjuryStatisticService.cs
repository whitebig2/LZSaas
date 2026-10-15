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
    public class InjuryStatisticService : ServicesBase<Model.Live_InjuryInfo>, IInjuryStatisticService, IScopedDependency
    {
        public async Task<List<Model.Live_V_InjuryInfo>> InfoList(Expression<Func<Model.Live_V_InjuryInfo, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_V_InjuryInfo>();
            return await qb.Where(exp).ToListAsync();
        }
        public async Task<int> InfoCount(Expression<Func<Model.Live_V_InjuryInfo, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_V_InjuryInfo>();
            return await qb.CountAsync(exp);
        }
        public async Task<Model.Live_V_InjuryInfo> GetInfo(Expression<Func<Model.Live_V_InjuryInfo, bool>> exp, string order)
        {
            var qb = base.Context.Queryable<Live_V_InjuryInfo>();
            return await qb.Where(exp).OrderBy(order).FirstAsync();
        }

        public async Task<List<Model.Live_V_InjuryApplyRecord>> ApplyRecordList(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_InjuryApplyRecord>();
            return await qb.Where(exp).ToListAsync();
        }

        public async Task<int> ApplyRecordCount(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_InjuryApplyRecord>();
            return await qb.CountAsync(exp);
        }

        public async Task<Model.Live_V_InjuryApplyRecord> GetApplyRecord(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp, string order)
        {
            var qb = base.Context.Queryable<Live_V_InjuryApplyRecord>();
            return await qb.Where(exp).OrderBy(order).FirstAsync();
        }

        public async Task<decimal> ApplyRecorSumAmount(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_InjuryApplyRecord>();
            qb.Where(exp);
            return await qb.SumAsync(f => f.Money);
        }

        public async Task<int> PaymentCount(Expression<Func<Model.Live_V_InjuryPayment, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_InjuryPayment>();
            return await qb.CountAsync(exp);
        }

        public async Task<List<Model.Live_V_InjuryPayment>> PaymentList(Expression<Func<Model.Live_V_InjuryPayment, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_InjuryPayment>();
            return await qb.Where(exp).ToListAsync();
        }


    }
}
