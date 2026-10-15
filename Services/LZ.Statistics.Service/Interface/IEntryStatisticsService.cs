using LZ.Core.EventData;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Interface
{
    public interface IEntryStatisticsService
    {
        Task<int> Count(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> ContractStatusCount(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> StatusCount(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> CustomerList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> CustomerStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> CompanyList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> CompanyStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> WorkClassList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> PartnerList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> PartnerStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> PartnerTypeList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> PartnerTypeStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> PartnerJointList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> PartnerJointStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> LeaveReason(Expression<Func<Model.Live_UserLeaveReason, bool>> exp);
        Task<List<Model.Live_UserLeaveReason>> LeaveReasonList(Expression<Func<Model.Live_UserLeaveReason, bool>> exp);
        Task<List<int>> ListUserEntryId(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<Live_V_UserEntry> GetView(Expression<Func<Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> FromTypeList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
        Task<List<Model.Statistics.EntryModel>> FromTypeStatusList(Expression<Func<Model.Live_V_UserEntry, bool>> exp);
    }
}
