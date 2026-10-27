using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface IPrepaidApplyRecordService
    {
        Task<List<Live_V_PrepaidApplyRecord>> PrepaidApplyRecordList(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp);
        Task<decimal> PrepaidInterestReportSumAmount(Expression<Func<Live_PrepaidInterestReport, bool>> exp);
    }
}
