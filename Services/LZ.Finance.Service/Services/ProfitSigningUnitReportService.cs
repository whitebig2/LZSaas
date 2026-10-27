using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Model.ModelExtensions;

namespace LZ.Finance.Service.Services
{
    public class ProfitSigningUnitReportService : ServicesBase<Live_ProfitSigningUnitReport>, IProfitSigningUnitReportService, IScopedDependency
    {
        public async Task<List<Live_ProfitSigningUnitReport>> PageList(Expression<Func<Live_ProfitSigningUnitReport, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_ProfitSigningUnitReport>().Where(exp).OrderByDescending(it =>new { it.BillMonth,it.SigningUnitName,it.CreateTime}).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }


        public async Task<List<Live_ProfitSigningUnitReport>> List(Expression<Func<Live_ProfitSigningUnitReport, bool>> exp)
        {
            return await   base.Context.Queryable<Live_ProfitSigningUnitReport>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_ProfitSigningUnitReport model)
        {
            return await   base.Context.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(List<Live_ProfitSigningUnitReport> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }
       
        public async Task<bool> Update(Live_ProfitSigningUnitReport model)
        {
            return await   base.Context.Updateable<Live_ProfitSigningUnitReport>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
       
        public async Task<Live_ProfitSigningUnitReport> Get(Expression<Func<Live_ProfitSigningUnitReport, bool>> exp)
        {
            return await   base.Context.Queryable<Live_ProfitSigningUnitReport>().Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }

        public async Task<Live_ProfitSigningUnitReportExtension> GetDomain(Expression<Func<Live_ProfitSigningUnitReportExtension, bool>> exp)
        {
            return await   base.Context.Queryable<Live_ProfitSigningUnitReportExtension>()
                .Includes(f=>f.RiskInvoice)
                .Includes(f=>f.BusinessInvoice)
                .Includes(f=>f.BusinessSheet)
                .Includes(f=>f.BusInsuranceSheet)
                .Includes(f=>f.RiskSheet)
                .Where(exp).OrderByDescending(it => it.CreateTime).FirstAsync();
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_ProfitSigningUnitReport>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }


        

        public async Task<List<Live_ProfitSigningUnitReport>> GroupBySigningUnit(DateTime billMonth)
        {
            return await   base.Context.Queryable<Live_ProfitManageRecord>().GroupBy(f => new { f.SigningUnitName, f.SigningUnitID }).Where(it => it.ProfitMonth == billMonth).Select(f => new Live_ProfitSigningUnitReport
            {
                SigningUnitID = f.SigningUnitID,
                SigningUnitName = f.SigningUnitName,
                BusinessAmount = SqlFunc.AggregateSum(f.BusinessAmount),// 业务派单费
                RiskAmount = SqlFunc.AggregateSum(f.RiskCurrentAmount),// 风险金
                BusInsuranceAmount = SqlFunc.AggregateSum(f.BusInsuranceSumAmount),//保费实收
                BillMonth = billMonth,
                BusinessInvoiceID = null,
                BusinessSheetID = null,
                BusInsuranceBlanceAmount = 0,
                BusInsuranceSheetID = null,
                CreateTime = DateTime.Now,
                IsBusinessPaid = false,
                IsBusinessPrint = false,
                IsBusInsurancePaid = false,
                IsRiskPaid = false,
                IsRiskPrint = false,
                RiskInvoiceID = null,
                RiskSheetID = null,
                ID = Guid.NewGuid(),
                IsPush = false,

            }).ToListAsync();
        }
    }
}
