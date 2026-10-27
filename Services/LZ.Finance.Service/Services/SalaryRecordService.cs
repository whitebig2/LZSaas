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
using LZ.Model.Enums;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static NPOI.HSSF.Util.HSSFColor;

namespace LZ.Finance.Service.Services
{
    public class SalaryRecordService : ServicesBase<Live_SalaryRecord>,  ISalaryRecordService, IScopedDependency
    {

        public async Task<List<Live_V_SalaryRecord>> PageList(Expression<Func<Live_V_SalaryRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_V_SalaryRecord>> ListView(Expression<Func<Live_V_SalaryRecord, bool>> exp)
        {

            return await   base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_SalaryRecord>> List(Expression<Func<Live_SalaryRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }
        public async Task<Live_SalaryRecord> Get(Expression<Func<Live_SalaryRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }

        public async Task<Live_V_SalaryRecord> GetView(Expression<Func<Live_V_SalaryRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_V_SalaryRecord>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).FirstAsync();
        }

      
        public async Task<bool> Update(Live_SalaryRecord model)
        {
            return await   base.Context.Updateable<Live_SalaryRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Update(List<Live_SalaryRecord> models)
        {
            return await   base.Context.Updateable<Live_SalaryRecord>(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public void VerifyPendingSend(Live_SalaryRecord model)
        {
            bool isb = true;
            if (model.IsCashSend && !model.IsCashSignSuc)
            {
                isb = false;
            }
            if (model.IsDelayPayment && !model.IsDelayPaymentSuc)
            {
                isb = false;
            }
            if (model.IsFailSalary && !model.IsFailSalarySuc)
            {
                isb = false;
            }
            if (isb)
            {
                if (model.Status != Model.Enums.SalaryRecordSendStatusEnumType.不发放 && model.Status != Model.Enums.SalaryRecordSendStatusEnumType.已发放)
                {
                    model.Status = Model.Enums.SalaryRecordSendStatusEnumType.待发放;
                }
                // 自动锁定记录
                if (!model.IsLocked&&model.IsSingleSend)
                {
                    model.IsLocked = true;
                    model.LockedTime = DateTime.Now;
                }
            }
        }
        public async Task<bool> Delete(Expression<Func<Live_SalaryRecord, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_SalaryRecord>(exp).ExecuteCommandAsync() > 0;
        }
        /// <summary>
        /// 只针对手动异常
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> SetSendSuccess(Live_SalaryRecord model)
        {
            model.Status = SalaryRecordSendStatusEnumType.已发放;
            model.SendTime = DateTime.Now;
            Live_Settlement settlement = null;
            if (model.IsSingleSend)
            {
                var viewModel = await GetView(f => f.ID == model.ID);
                settlement = new Live_Settlement()
                {
                    ID = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    CustomerGID = viewModel.CustomerGID.Value,
                    Remark = string.Empty,
                    SalaryCustomerID = null,
                    SalaryMonth = viewModel.SalaryMonth,
                    SettlementType = 0,
                    Title = "派遣员工工资",
                    TotalAmount = viewModel.ActualSalarySendCardAmount,
                    ApplyDate = DateTime.Now,
                    BusinessType = SettlementBusinessTypeEnumType.工资结算单,
                    IssuerUnitID = viewModel.CooperationUnitID,
                    IssuerName = viewModel.CooperationUnitCompanyName,
                    IssuerCompanyNo = viewModel.CooperationUnitCompanyNo,
                    IssuerPhone = viewModel.CooperationUnitCompanyPhone,
                    IssuerBank = viewModel.CooperationUnitCompanyBank,
                    IssuerBankNo = viewModel.CooperationUnitCompanyBankNo,
                    IssuerAddress = viewModel.CooperationUnitCompanyAddress,
                    RecipientUnitID = viewModel.SigningUnitID,
                    RecipientName = viewModel.SigningUnitCompanyName,
                    RecipientPhone = viewModel.SigningUnitCompanyPhone,
                    RecipientAddress = viewModel.SigningUnitCompanyAddress,
                    RecipientBank = viewModel.SigningUnitCompanyBank,
                    RecipientBankNo = viewModel.SigningUnitCompanyBankNo,
                    RecipientCompanyNo = viewModel.SigningUnitCompanyBankNo,
                    CustomerAddress = viewModel.CustomerName,
                    CustomerCompanyNo = viewModel.CustomerCompanyNo,
                    CustomerName = viewModel.CustomerName,
                    CustomerBank = viewModel.CustomerCompanyBank,
                    CustomerBankNo = viewModel.CustomerCompanyBankNo,
                    CustomerPhone = viewModel.CustomerCompanyPhone,
                };
                settlement.Description = $"{settlement.CustomerName}-{settlement.SalaryMonth.Value.ToString("yyyy年MM月")}，{viewModel.Name}，共1名员工工资,合计{viewModel.ActualSalarySendCardAmount}元";
                model.SettlementID = settlement.ID;
            }

            try
            {
              itenant.BeginTran();
                await   base.Context.Updateable<Live_SalaryRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                if (settlement != null)
                {
                    await   base.Context.Insertable(settlement).IgnoreColumns().ExecuteCommandAsync();
                }
              itenant.CommitTran();
            }
            catch (Exception ex )
            {

                throw new ErrorCodeException(-1,$"数据保存失败，异常信息：{ex.Message}");
            }
            return true;
        }

        public async Task<bool> SetSendFail(Live_SalaryRecord model, SalaryFailTypeEnumType failType, string failMsg)
        {
            Live_SalaryFailRecord failRecord = new Live_SalaryFailRecord()
            {
                Bank = model.Bank,
                BankNo = model.BankNo,
                BankUserName = model.BankUserName,
                CreateTime = DateTime.Now,
                Description = failMsg ?? "",
                Remark = string.Empty,
                Status = SalaryFailRecordStatusEnumType.待处理,
                SalaryRecordID = model.ID,
                SalaryCustomerID = model.SalaryCustomerID,
                FailType = failType
            };
            model.Status = SalaryRecordSendStatusEnumType.发放异常;
            itenant.BeginTran();
            if (model.SettlementID != null)
            {
                model.SettlementID = null;
                await base.Context.Deleteable<Live_SalaryRecord>(f => f.ID == model.SettlementID).ExecuteCommandAsync();
            }
            model.IsFailSalary = true;
            model.IsFailSalarySuc = false;
            model.IsSingleSend = true;
            model.IsSplitSendSuc = false;
            model.Status = SalaryRecordSendStatusEnumType.发放异常;
            // 将异常信息同步到工资记录上
            model.TransMsg = failMsg;

            await base.Context.Insertable<Live_SalaryFailRecord>(failRecord).ExecuteCommandAsync();
            await base.Context.Updateable<Live_SalaryRecord>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
            itenant.CommitTran();
            return true;
        }



        public async Task<decimal> AvgSalary(Expression<Func<Live_V_SalaryRecord, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_SalaryRecord>().Where(exp).AvgAsync(f => f.UnitSalaryAmount);
        }




    }
}
