using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface IInjuryStatisticService
    {
        Task<List<Model.Live_V_InjuryInfo>> InfoList(Expression<Func<Model.Live_V_InjuryInfo, bool>> exp);
        Task<int> InfoCount(Expression<Func<Model.Live_V_InjuryInfo, bool>> exp);
        Task<Model.Live_V_InjuryInfo> GetInfo(Expression<Func<Model.Live_V_InjuryInfo, bool>> exp, string order);

        Task<List<Model.Live_V_InjuryApplyRecord>> ApplyRecordList(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp);
        Task<int> ApplyRecordCount(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp);

        Task<Model.Live_V_InjuryApplyRecord> GetApplyRecord(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp, string order);
        Task<int> PaymentCount(Expression<Func<Model.Live_V_InjuryPayment, bool>> exp);

        Task<decimal> ApplyRecorSumAmount(Expression<Func<Model.Live_V_InjuryApplyRecord, bool>> exp);

        Task<List<Model.Live_V_InjuryPayment>> PaymentList(Expression<Func<Model.Live_V_InjuryPayment, bool>> exp);

    }
}
