using LZ.Model;
using LZ.Model.Model;
using LZ.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Interface
{
    public interface ISalaryStatisticsService
    {
        Task<List<Live_V_SalaryCashSendRecord>> ListSalaryCashRecord(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp = null);
        Task<List<Live_V_SalaryDelaySendRecord>> ListSalaryDelayRecord(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp = null);
        Task<int> ListSalaryDelayCount(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp = null);
        Task<List<Live_V_SalaryFailRecord>> ListSalaryFailRecord(Expression<Func<Live_V_SalaryFailRecord, bool>> exp = null);
        Task<int> SalaryCashCount(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp = null);
        Task<int> SalaryFailCount(Expression<Func<Live_V_SalaryFailRecord, bool>> exp = null);
        Task<List<int>> SalaryCashUserEntryIdList(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp = null);
        Task<List<int>> ListSalaryFailRecordUserEntryIdList(Expression<Func<Live_V_SalaryFailRecord, bool>> exp = null);
        Task<int> InvoiceCount(Expression<Func<Live_Invoice, bool>> exp = null);
        Task<int> SalaryRecordViewCount(Expression<Func<Live_V_SalaryRecord, bool>> exp = null);
        Task<int> PrepaidApplyRecordCount(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null);
        Task<decimal> PrepaidApplyRecordSumAmount(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null);
        Task<decimal> PrepaidApplyRecordPendingRepaySumAmount(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null);
        Task<decimal> TransBatchSumAmount(Expression<Func<Live_TransBatch, bool>> exp = null);
        Task<int> SalaryRecordSlipCount(Expression<Func<Live_SalarySlipRecord, bool>> exp = null);
        Task<int> TransBatchRecordCount(Expression<Func<Live_TransBatchRecord, bool>> exp = null);
        Task<int> SalarySplitSendRecordCount(Expression<Func<live_V_SalarySplitSendRecord, bool>> exp = null);
        Task<int> TransBatchRecordEmployeeCount(Expression<Func<Live_TransBatch, bool>> exp = null);
        Task<List<PrepaidPartnerGroupItem>> PrepaidPartnerGroup();
        Task<int> SalaryCustomerCount(Expression<Func<Live_SalaryCustomer, bool>> exp = null);
        Task<List<Live_SalaryCustomer>> ListSalaryCustomer(Expression<Func<Live_SalaryCustomer, bool>> exp = null);
        List<CountReport> SalaryRangeReport();
        SalayCustomerCalculateTimes SalaryCustomerCalculateTimes();
        List<CountReport> SpecialDeductionReport();
        Task<decimal> PrepaidRepaymentRecordRepaySumAmount(Expression<Func<Live_V_PrepaidRepaymentRecord, bool>> exp = null);
        Task<Live_V_PrepaidApplyRecord> GetPrepaidApplyRecord(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null);
        Task<List<PrepaidCustomerGroupItem>> PrepaidCustomerGroup();
        Task<decimal> InvoiceSumTax(Expression<Func<Live_Invoice, bool>> exp = null);
        Task<decimal> ListSalaryGjjTotalAmount(Expression<Func<Live_V_SalaryRecord, bool>> exp = null);
        Task<decimal> ListSalarySocialEmpTotalAmount(Expression<Func<Live_V_SalaryRecord, bool>> exp = null);
        Task<int> SalaryCashRecordCount(Expression<Func<Live_SalaryCashSendRecord, bool>> exp = null);
        Task<decimal> ListEmpSocialTotalAmount(Expression<Func<Live_InsuranceEmployeeBill, bool>> exp = null);
        Task<int> PrepaidSubAccountCounts(Expression<Func<Live_PrepaidSubAccount, bool>> exp = null);
    }
}
