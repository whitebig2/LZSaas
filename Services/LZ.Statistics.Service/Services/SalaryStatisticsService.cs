using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.Model;
using LZ.Model.Response;
using LZ.Statistics.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Services
{
    public class SalaryStatisticsService : ServicesBase<Model.Live_V_SalaryCashSendRecord>, ISalaryStatisticsService, IScopedDependency
    {




        /// <summary>
        ///  现金签收单
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<int> SalaryCashCount(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp=null)
        {
            var qb = base.Context.Queryable<Live_V_SalaryCashSendRecord>();
            return await qb.CountAsync(exp);
        }

        public async Task<List<int>> SalaryCashUserEntryIdList(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryCashSendRecord>().Select(f=>f.UserEntryID.Value).ToListAsync();
        }

        public async Task<List<Live_V_SalaryCashSendRecord>> ListSalaryCashRecord(Expression<Func<Live_V_SalaryCashSendRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryCashSendRecord>().Where(exp).ToListAsync();
        }
        public async Task<int> SalaryFailCount(Expression<Func<Live_V_SalaryFailRecord, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_V_SalaryFailRecord>();
            return await qb.CountAsync(exp);
        }


        public async Task<List<int>> ListSalaryFailRecordUserEntryIdList(Expression<Func<Live_V_SalaryFailRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryFailRecord>().Select(f => f.UserEntryID.Value).ToListAsync();
        }

        public async Task<List<Live_V_SalaryFailRecord>> ListSalaryFailRecord(Expression<Func<Live_V_SalaryFailRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryFailRecord>().Where(exp).ToListAsync();
        }

        public async Task<List<Live_V_SalaryDelaySendRecord>> ListSalaryDelayRecord(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryDelaySendRecord>().Where(exp).ToListAsync();
        }
        public async Task<int> ListSalaryDelayCount(Expression<Func<Live_V_SalaryDelaySendRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryDelaySendRecord>().Where(exp).CountAsync();
        }
        public async Task<decimal> ListSalaryGjjTotalAmount(Expression<Func<Live_V_SalaryRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).SumAsync(f=>f.SocialGongJiJinAmount);
        }
        public async Task<decimal> ListSalarySocialEmpTotalAmount(Expression<Func<Live_V_SalaryRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).SumAsync(f=>f.SocialEmpSumAmount);
        }

        public async Task<decimal> ListEmpSocialTotalAmount(Expression<Func<Live_InsuranceEmployeeBill, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_InsuranceEmployeeBill>().Where(exp).SumAsync(f => f.TotalAmount);
        }

        public async Task<int> InvoiceCount(Expression<Func<Live_Invoice, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_Invoice>().Where(exp).CountAsync();
        }

        public async Task<decimal> InvoiceSumTax(Expression<Func<Live_Invoice, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_Invoice>().Where(exp).SumAsync(f=> f.AddedTaxAmount + f.AddedTaxDiscountAmount+ f.AdditionalTaxAmount + f.AdditionalTaxDiscountAmount + f.SfAddedTaxAmount+f.SfPersonalTaxAmount+f.SfTaxRefundAmount);
        }

        public async Task<int> SalaryRecordViewCount(Expression<Func<Live_V_SalaryRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).CountAsync();
        }


        public async Task<int> PrepaidApplyRecordCount(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).CountAsync();
        }

        public async Task<decimal> PrepaidApplyRecordSumAmount(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).SumAsync(f=>f.ApplyAmount);
        }

        public async Task<Live_V_PrepaidApplyRecord> GetPrepaidApplyRecord(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).FirstAsync();
        }

        public async Task<decimal> PrepaidApplyRecordPendingRepaySumAmount(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).SumAsync(f => f.ApplyAmount-f.RepayTotalAmount);
        }

        public async Task<decimal> PrepaidRepaymentRecordRepaySumAmount(Expression<Func<Live_V_PrepaidRepaymentRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_V_PrepaidRepaymentRecord>().Where(exp).SumAsync(f =>f.TotalAmount);
        }

        public async Task<int> PrepaidSubAccountCounts(Expression<Func<Live_PrepaidSubAccount, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_PrepaidSubAccount>().Where(exp).CountAsync();
        }


        public async Task<List<PrepaidPartnerGroupItem>> PrepaidPartnerGroup()
        {

            string sql = @"

SELECT 
a.ApplyType,
a.PartnerGID,
   a.PartnerName,
    SUM(CASE WHEN CreateTime>=DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) THEN a.ApplyAmount ELSE 0 END) AS MonthTotalAmount,
    SUM(a.ApplyAmount) AS YearTotalAmount
FROM 
    Live_V_PrepaidApplyRecord a

WHERE 
    YEAR(a.CreateTime) = YEAR(GETDATE()) and a.PartnerName is not null 
GROUP BY 
    a.PartnerName,a.ApplyType ,a.PartnerGID

";

            return await base.Context.Ado.SqlQueryAsync<PrepaidPartnerGroupItem>(sql);

        }


        public async Task<List<PrepaidCustomerGroupItem>> PrepaidCustomerGroup()
        {

            string sql = @"

SELECT 
a.ApplyType,
a.CustomerGID,
   a.CustomerName,
    SUM(CASE WHEN CreateTime>=DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) THEN a.ApplyAmount ELSE 0 END) AS MonthTotalAmount,
    SUM(a.ApplyAmount) AS YearTotalAmount
FROM 
    Live_V_PrepaidApplyRecord a

WHERE 
    YEAR(a.CreateTime) = YEAR(GETDATE()) and a.CustomerName is not null 
GROUP BY 
    a.CustomerName,a.ApplyType ,a.CustomerGID

";

            return await base.Context.Ado.SqlQueryAsync<PrepaidCustomerGroupItem>(sql);

        }


        public async Task<decimal> TransBatchSumAmount(Expression<Func<Live_TransBatch, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_TransBatch>().Where(exp).SumAsync(f =>f.TotalAmount);
        }

        public async Task<int> TransBatchRecordCount(Expression<Func<Live_TransBatchRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_TransBatchRecord>().Where(exp).CountAsync();
        }

        // 批量发放 人数统计
        public async Task<int> TransBatchRecordEmployeeCount(Expression<Func<Live_TransBatch, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_TransBatch>().Where(exp).SumAsync(f=>f.TotalNum);
        }

        public async Task<int> SalaryRecordSlipCount(Expression<Func<Live_SalarySlipRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_SalarySlipRecord>().Where(exp).CountAsync();
        }


        public async Task<int> SalarySplitSendRecordCount(Expression<Func<live_V_SalarySplitSendRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<live_V_SalarySplitSendRecord>().Where(exp).CountAsync();
        }

        public async Task<int> SalaryCustomerCount(Expression<Func<Live_SalaryCustomer, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_SalaryCustomer>().Where(exp).CountAsync();
        }

        public async Task<int> SalaryCashRecordCount(Expression<Func<Live_SalaryCashSendRecord, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_SalaryCashSendRecord>().Where(exp).CountAsync();
        }


        public async Task<List<Live_SalaryCustomer>> ListSalaryCustomer(Expression<Func<Live_SalaryCustomer, bool>> exp = null)
        {
            return await base.Context.Queryable<Live_SalaryCustomer>().Where(exp).OrderByDescending(f=>f.CreateTime).Take(20).ToListAsync();
        }


        public SalayCustomerCalculateTimes SalaryCustomerCalculateTimes()
        {
            string sql = @"
select 
(SELECT current_value  VALUE FROM sys.sequences WHERE name = 'SalaryCalculateTimesToday') as TodayTimes,
(SELECT current_value  VALUE FROM sys.sequences WHERE name = 'SalaryCalculateTimesMonth') as MonthTimes,
(SELECT current_value  VALUE FROM sys.sequences WHERE name = 'SalaryCalculateTimesYear') as YearTimes
";
            var sqlRes= base.Context.Ado.SqlQuerySingle<dynamic>(sql);

            var res= new SalayCustomerCalculateTimes() { 
            
             MonthTimes= sqlRes.MonthTimes,
              TodayTimes= sqlRes.TodayTimes,
              YearTimes= sqlRes.YearTimes,
            };

            return res;
        }

        public List<CountReport> SalaryRangeReport()
        {

            string sql = @"
SELECT 
    CASE 
        WHEN UnitSalaryAmount < 3000 THEN '3000-'
        WHEN UnitSalaryAmount >= 3000 AND UnitSalaryAmount < 5000 THEN '3000-5000'
        WHEN UnitSalaryAmount >= 5000 AND UnitSalaryAmount < 8000 THEN '5000-8000'
        WHEN UnitSalaryAmount >= 8000 THEN '8000+'
        ELSE '未知区间'
    END AS Title,
    COUNT(*) AS TotalCount
FROM 
    Live_SalaryRecord
GROUP BY 
    CASE 
        WHEN UnitSalaryAmount < 3000 THEN '3000-'
        WHEN UnitSalaryAmount >= 3000 AND UnitSalaryAmount < 5000 THEN '3000-5000'
        WHEN UnitSalaryAmount >= 5000 AND UnitSalaryAmount < 8000 THEN '5000-8000'
        WHEN UnitSalaryAmount >= 8000 THEN '8000+'
        ELSE '未知区间'
    END
ORDER BY 
    Title;
";

            return base.Context.Ado.SqlQuery<CountReport>(sql);

        }


        public List<CountReport>SpecialDeductionReport()
        {

            string sql = @"

select 
case DeductionType 
	when 0 then '子女教育支出'
	when 1 then '继续教育支出'
	when 2 then '住房贷款利息支出'
	when 3 then '住房租金支出'
	when 4 then '赡养老人支出'
	when 5 then '婴幼儿照护支出'
	end as Title
	,count(1)as TotalCount 
	from Live_SalarySpecialDedcution  where TaxCountMonth>=DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) 
 group by 
 
 case DeductionType 
	when 0 then '子女教育支出'
	when 1 then '继续教育支出'
	when 2 then '住房贷款利息支出'
	when 3 then '住房租金支出'
	when 4 then '赡养老人支出'
	when 5 then '婴幼儿照护支出'
	end 
 order by Title
";

            return base.Context.Ado.SqlQuery<CountReport>(sql);

        }
    }
}
