using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Statistics.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Services
{
    public class PrepaidApplyStatisticsService : ServicesBase<Model.Live_PrepaidApplyRecord>, IPrepaidApplyStatisticsService, IScopedDependency
    {
        public async Task<int> Count(Expression<Func<Model.Live_PrepaidApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_PrepaidApplyRecord>();
            return await qb.CountAsync(exp);
        }
        public async Task<List<Model.Statistics.EntryModel>> TypeCount(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_PrepaidApplyRecord>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.ApplyType });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Status = (int)it.ApplyType, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }

        public async Task<decimal> ApplySumAmount(Expression<Func<Model.Live_PrepaidApplyRecord, bool>> exp)
        {
           return await base.Context.Queryable<Live_PrepaidApplyRecord>().Where(exp).SumAsync(f=>f.ApplyAmount);
        }

        public async Task<int> ViewCount(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_V_PrepaidApplyRecord>();
            return await qb.CountAsync(exp);
        }

        public async Task<decimal> ViewApplySumAmount(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).SumAsync(f => f.ApplyAmount);
        }
        public async Task<List<Model.Statistics.EntryModel>> TypeCount(Expression<Func<Model.Live_PrepaidApplyRecord, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_PrepaidApplyRecord>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.ApplyType });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Status = (int)it.ApplyType, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }


        public async Task<List<Live_V_PrepaidPartnerAccount>> PrepaidPartnerAccountListView(Expression<Func<Live_V_PrepaidPartnerAccount, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_PrepaidPartnerAccount>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_V_PrepaidApplyRecord>> PrepaidApplyRecordListView(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

    


        public async Task<List<Live_V_PrepaidRepaymentRecord>> PrepaidRepaymentRecorListView(Expression<Func<Live_V_PrepaidRepaymentRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_PrepaidRepaymentRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<Live_PrepaidInterestReport> PrepaidInterestReportGet(Expression<Func<Live_PrepaidInterestReport, bool>> exp)
        {
            return await base.Context.Queryable<Live_PrepaidInterestReport>().Where(exp).FirstAsync();
        }

        public async Task<List<Live_PrepaidInterestReport>> PrepaidInterestReportList(Expression<Func<Live_PrepaidInterestReport, bool>> exp)
        {
            return await base.Context.Queryable<Live_PrepaidInterestReport>().Where(exp).ToListAsync();
        }



    }
}
