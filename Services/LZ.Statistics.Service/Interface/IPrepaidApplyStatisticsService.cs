using LZ.Core.EventData;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Interface
{
    public interface IPrepaidApplyStatisticsService
    {
        Task<decimal> ApplySumAmount(Expression<Func<Live_PrepaidApplyRecord, bool>> exp);
        Task<int> Count(Expression<Func<Model.Live_PrepaidApplyRecord, bool>> exp);
        Task<decimal> ViewApplySumAmount(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp);
        Task<int> ViewCount(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null);
      
        Task<List<Model.Statistics.EntryModel>> TypeCount(Expression<Func<Model.Live_V_PrepaidApplyRecord, bool>> exp);
        Task<List<Live_V_PrepaidPartnerAccount>> PrepaidPartnerAccountListView(Expression<Func<Live_V_PrepaidPartnerAccount, bool>> exp);
        Task<List<Live_V_PrepaidRepaymentRecord>> PrepaidRepaymentRecorListView(Expression<Func<Live_V_PrepaidRepaymentRecord, bool>> exp);
        Task<Live_PrepaidInterestReport> PrepaidInterestReportGet(Expression<Func<Live_PrepaidInterestReport, bool>> exp);
        Task<List<Live_V_PrepaidApplyRecord>> PrepaidApplyRecordListView(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp);
        Task<List<Live_PrepaidInterestReport>> PrepaidInterestReportList(Expression<Func<Live_PrepaidInterestReport, bool>> exp);
    }
}
