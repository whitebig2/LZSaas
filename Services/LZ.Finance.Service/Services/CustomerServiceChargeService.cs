using LZ.Core;
using LZ.Core.CommonHandle;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Services
{
    public class CustomerServiceChargeService : ServicesBase<Live_CustomerServiceCharge>, ICustomerServiceChargeService, IScopedDependency
    {

        public async Task<List<Live_V_CustomerServiceCharge>> List(Expression<Func<Live_V_CustomerServiceCharge, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_CustomerServiceCharge>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_CustomerServiceCharge>().Where(exp).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<Live_CustomerServiceCharge> Statistics(Expression<Func<Live_V_CustomerServiceCharge, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_CustomerServiceCharge>().Where(exp).Select(it=>new Live_CustomerServiceCharge() { SalaryVAT= SqlFunc.AggregateSum(it.SalaryVAT), SalarySurtax = SqlFunc.AggregateSum(it.SalarySurtax), ServiceVAT = SqlFunc.AggregateSum(it.ServiceVAT), ServiceSurtax = SqlFunc.AggregateSum(it.ServiceSurtax), ServiceCharge = SqlFunc.AggregateSum(it.ServiceCharge), PeopleCount = SqlFunc.AggregateSum(it.PeopleCount), TotalAmount = SqlFunc.AggregateSum(it.TotalAmount) }).FirstAsync();
        }

        public async Task<Live_V_CustomerServiceCharge> Get(Expression<Func<Live_V_CustomerServiceCharge, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_CustomerServiceCharge>().Where(exp).OrderByDescending(it => it.ID).FirstAsync();
        }
        public async Task<bool> Create(Guid id)
        {
            if(await base.Context.Queryable<Live_CustomerServiceCharge>().AnyAsync(it => it.SalaryCustomerID == id))
            {
                return false;
            }
            var model = await base.Context.Queryable<Live_V_SalaryCustomer>().FirstAsync(it=>it.ID==id);
            var invoice = await base.Context.Queryable<Live_Invoice>().FirstAsync(it => it.SalaryCustomerID == id && it.BusinessType == PrintTemplateEnumType.外部发票);
            if (model != null && invoice !=null)
            {
                var invoiceTaxs= await base.Context.Queryable<Live_InvoiceTaxRecord>().Where(it => it.InvoiceID == invoice.ID).ToListAsync();
                var salaryTax = invoiceTaxs.FirstOrDefault(it => it.DataType == Model.Enums.InvoiceTaxDataTypeEnumType.企业工资);
                var serviceTax = invoiceTaxs.FirstOrDefault(it => it.DataType == Model.Enums.InvoiceTaxDataTypeEnumType.服务费);
                var customerServiceCharge = new Live_CustomerServiceCharge()
                {
                    CustomerID = model.CustomerID,
                    CustomerGID = model.CustomerGID,
                    IsPredict = false,
                    CreateTime = DateTime.Now,
                    SalaryMonth = model.SalaryMonth,
                    ReceiptCustomerGID = model.CustomerGID,
                    ReceiptCustomerID = model.CustomerID,
                    InvoiceCustomerGID = model.CooperationUnitID,
                    SendDate = model.SendTime,
                    PeopleCount = model.RecordCount,
                    SalaryCustomerID = id,
                    InvoiceAmount= invoice.InvoiceAmount,
                    InvoiceDate= invoice.CreateTime,
                    ServiceCharge= model.ServiceFeeSumAmount,
                    SalaryVAT=0,
                    SalarySurtax=0,
                    ServiceVAT=0,
                    ServiceSurtax=0
                };
                if (salaryTax != null)
                {
                    customerServiceCharge.SalaryVAT = salaryTax.ValueAddedTaxAmount;
                    customerServiceCharge.SalarySurtax = salaryTax.AdditionalTaxAmount;
                }
                if (serviceTax != null)
                {
                    customerServiceCharge.ServiceVAT = serviceTax.ValueAddedTaxAmount;
                    customerServiceCharge.ServiceSurtax = serviceTax.AdditionalTaxAmount;
                }
                customerServiceCharge.TotalAmount = customerServiceCharge.SalaryVAT + customerServiceCharge.SalarySurtax + customerServiceCharge.ServiceVAT + customerServiceCharge.ServiceSurtax + customerServiceCharge.ServiceCharge;
                await base.Context.Insertable(customerServiceCharge).ExecuteCommandAsync();
                await base.Context.Deleteable<Live_CustomerServiceCharge>().Where(it => it.IsPredict && it.CustomerGID == model.CustomerGID && it.ReceiptCustomerGID==model.CustomerGID).ExecuteCommandAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> ChangeSendTime(Guid id)
        {
            var model = await base.Context.Queryable<Live_V_SalaryCustomer>().FirstAsync(it => it.ID == id);
            if (model != null)
            {
                await base.Context.Updateable<Live_CustomerServiceCharge>().SetColumns(it=>it.SendDate==model.SendTime).Where(it=>it.SalaryCustomerID==id).ExecuteCommandAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> Batch(DateTime date)
        {
            var nowMonth=date.AddMonths(-1);
            var now= DateTime.Now;
            var userStatus = new int[] { (int)Model.Enums.UserStatus.Leave, (int)Model.Enums.UserStatus.Entry };
            var cmodels = await base.Context.Queryable<Live_V_SalaryCustomer>().Where(it=>it.CreateTime.Value.ToString("yyyy-MM") == nowMonth.ToString("yyyy-MM") && it.SalaryType==0).ToListAsync();
            var ids=cmodels.Select(it=>it.CustomerID).Distinct().ToList();
            var entryList = await base.Context.Queryable<Live_V_UserEntry>().Where(it=>ids.Contains(it.CustomerID) && userStatus.Contains(it.Status.Value)).ToListAsync();
            var list = new List<Model.Live_CustomerServiceCharge>();
            var invoiceConfigs = await base.Context.Queryable<Live_V_InvoiceConfig>().Where(it=>ids.Contains(it.CustomerID) && it.PrintTemplate== Model.PrintTemplateEnumType.外部发票).ToListAsync();
            foreach (var id in ids)
            {
                var model = new Model.Live_CustomerServiceCharge();
                //var recordCount = cmodels.Where(it => it.CustomerID == id).Sum(it=>it.RecordCount);
                var unitSalarySumAmount = cmodels.Where(it => it.CustomerID == id).Sum(it => it.UnitSalarySumAmount);
                var serviceSumAmount = cmodels.Where(it => it.CustomerID == id).Sum(it => it.ServiceFeeSumAmount);
                var cmodel = cmodels.Find(it=>it.CustomerID==id);
                //var users= entryList.Where(it=>it.CustomerID==id).ToList();
                var invoiceConfig = invoiceConfigs.Find(it=>it.CustomerID==id);
                if (invoiceConfig == null)
                {
                    continue;
                }
                var nowMonthOneDay= Convert.ToDateTime(nowMonth.ToString("yyyy-MM-01"));
                model.PerMoneyDayCount = 0;
                model.NowMoneyDayCount = 0;
                var nowDate = date.AddDays(-1);
                //var perDate =Convert.ToDateTime(cmodel.SalaryMonth.Value.ToString("yyyy-MM-01")).AddDays(-1);
                var nowUsers = entryList.Where(it => it.CustomerID == id && ((it.Status == (int)Model.Enums.UserStatus.Entry && it.EntryTime <= nowDate) || (it.Status == (int)Model.Enums.UserStatus.Leave && it.LeaveTime <= nowDate && it.LeaveTime >= nowMonthOneDay))).ToList();
                var perDate = nowDate.AddMonths(-1);
                var perMonthOneDay = Convert.ToDateTime(perDate.ToString("yyyy-MM-01"));
                var perUsers = entryList.Where(it => it.CustomerID == id && ((it.Status == (int)Model.Enums.UserStatus.Entry && it.EntryTime <= perDate) || (it.Status == (int)Model.Enums.UserStatus.Leave && it.LeaveTime <= perDate && it.LeaveTime >= perMonthOneDay))).ToList();
                model.PeopleCount = nowUsers.Count();
                if (id == 2641)
                {
                    Common.StaticLog.Debug($"{id};perDate={perDate};perMonthOneDay={perMonthOneDay};perUsers={perUsers.Count}");
                }
                    
                foreach (var user in nowUsers)
                {
                    model.NowMoneyDayCount += UserEntryHandle.NowMonthInDay(user, nowDate);
                }
                foreach (var user in perUsers)
                {
                    var d= UserEntryHandle.NowMonthInDay(user, perDate);
                    if (id == 2641)
                    {
                        Common.StaticLog.Debug($"{user.Name};id={user.ID};d={d};");
                    }
                    model.PerMoneyDayCount += d;
                }
                if (model.PerMoneyDayCount > 0)
                {
                    model.AverageSalary = unitSalarySumAmount / (model.PerMoneyDayCount * 1.0M);
                    model.AverageService = serviceSumAmount / (model.PerMoneyDayCount * 1.0M);
                }
                else
                {
                    model.AverageSalary = 0;
                    model.AverageService = 0;
                }
                model.ServiceCharge = model.AverageService * model.NowMoneyDayCount;
                model.SalaryVAT = model.AverageSalary * model.NowMoneyDayCount * invoiceConfig.TaxPointSalaryValueAdded / 100M;
                model.SalarySurtax = model.AverageSalary * model.NowMoneyDayCount * invoiceConfig.TaxPointSalaryAdditional / 100M;
                model.ServiceVAT = model.ServiceCharge * invoiceConfig.TaxPointServiceFeeValueAdded / 100M;
                model.ServiceSurtax = model.ServiceCharge * invoiceConfig.TaxPointServiceFeeAdditional / 100M;
                model.CustomerID = id;
                model.CustomerGID = cmodel.CustomerGID;
                model.IsPredict = true;
                model.CreateTime = now;
                model.SalaryMonth = cmodel.SalaryMonth.Value.AddMonths(1);
                model.ReceiptCustomerGID = cmodel.CustomerGID;
                model.ReceiptCustomerID = id;
                model.InvoiceCustomerGID = cmodel.CooperationUnitID;
                model.SendDate = cmodel.SendTime.HasValue ? cmodel.SendTime.Value.AddMonths(+1) : null;
                model.TotalAmount = model.SalaryVAT + model.SalarySurtax+ model.ServiceVAT+ model.ServiceSurtax + model.ServiceCharge;
                list.Add(model);
            }
            await base.Context.Deleteable<Live_CustomerServiceCharge>().Where(it => it.IsPredict && it.CreateTime.Value.ToString("yyyy-MM-01")==now.ToString("yyyy-MM-01")).ExecuteCommandAsync();
            await base.Context.Insertable(list).ExecuteCommandAsync();
            return true;
        }
    }
}
