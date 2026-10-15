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
    public class EntryStatisticsService : ServicesBase<Model.Live_UserEntry>, IEntryStatisticsService, IScopedDependency
    {

        public async Task<List<int>> ListUserEntryId(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            return await qb.Select(it =>it.ID).ToListAsync();
        }

        public async Task<int> Count(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
           var counts = await qb.Select(it => SqlFunc.AggregateCount(it.ID)).ToListAsync();
            return counts.Count > 0 ? counts[0] : 0;
        }
        public async Task<List<Model.Statistics.EntryModel>> StatusCount(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.Status });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Status = it.Status, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> ContractStatusCount(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.ContractStatus });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Status = it.ContractStatus, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> CustomerList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it=>new  { it.CustomerID,it.CustomerName});
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.CustomerName, ID = it.CustomerID, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> CustomerStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.CustomerID, it.CustomerName,it.Status });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.CustomerName, ID = it.CustomerID,Status=it.Status, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> CompanyList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.CompanyID, it.CompanyName });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.CompanyName, ID = it.CompanyID, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> CompanyStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.CompanyID, it.CompanyName,it.Status });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.CompanyName, ID = it.CompanyID,Status=it.Status, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> WorkClassList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.WorkClass });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.WorkClass, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> PartnerList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.PartnerID, it.PartnerName,it.PartnerLevel,it.PartnerType });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.PartnerName, ID = it.PartnerID,PartnerType=it.PartnerType,PartnerLevel=it.PartnerLevel, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> PartnerStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.PartnerID, it.PartnerName,it.Status, it.PartnerLevel, it.PartnerType });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.PartnerName, ID = it.PartnerID, PartnerType = it.PartnerType, PartnerLevel = it.PartnerLevel, Status=it.Status, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> PartnerTypeList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.PartnerType });
            return await qb.Select(it => new Model.Statistics.EntryModel() { PartnerType = it.PartnerType, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> PartnerTypeStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.PartnerType, it.Status });
            return await qb.Select(it => new Model.Statistics.EntryModel() { PartnerType = it.PartnerType, Status = it.Status, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> PartnerJointList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.JointPartnerID });
            return await qb.Select(it => new Model.Statistics.EntryModel() { ID = it.JointPartnerID, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> PartnerJointStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.JointPartnerID, it.Status });
            return await qb.Select(it => new Model.Statistics.EntryModel() { ID = it.JointPartnerID, Status = it.Status, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> LeaveReason(Expression<Func<Model.Live_UserLeaveReason, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_UserLeaveReason>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.LeaveName,it.LeaveID });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.LeaveName, ID = it.LeaveID, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Live_UserLeaveReason>> LeaveReasonList(Expression<Func<Model.Live_UserLeaveReason, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_UserLeaveReason>();
            qb.Where(exp);
            return await qb.ToListAsync();
        }
        public async Task<Live_V_UserEntry> GetView(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
          return await qb.Where(exp).FirstAsync();
            
        }

        public async Task<List<Model.Statistics.EntryModel>> FromTypeList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.FromType });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.FromType, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }
        public async Task<List<Model.Statistics.EntryModel>> FromTypeStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_V_UserEntry>();
            qb.Where(exp);
            qb.GroupBy(it => new { it.FromType, it.Status });
            return await qb.Select(it => new Model.Statistics.EntryModel() { Name = it.FromType, Status = it.Status, Count = SqlFunc.AggregateCount(it.ID) }).ToListAsync();
        }



    }
}
