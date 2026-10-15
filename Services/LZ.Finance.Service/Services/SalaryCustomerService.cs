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
using LZ.Model.Dtos.SalarySlip;
using Quartz.Simpl;
using LZ.Model.Enums;
using NPOI.SS.Formula.Functions;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData.Salary;
using LZ.Common;
using LZ.Model.Enums.Insurance;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using LZ.Core.EventData;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using LZ.Model.Exceptions;
using LZ.Model.Model;
using LZ.Core.Services;

namespace LZ.Finance.Service.Services
{
    public class SalaryCustomerService : ServicesBase<Live_SalaryCustomer>,  ISalaryCustomerService, IScopedDependency
    {

        private readonly IPlatformNoticeService _platformNoticeService;
        private readonly ISalaryCashSendRecordService  _cashSendRecordService;
        private readonly ISalaryDelaySendRecordService  _delaySendRecordService;
        private readonly IPrepaidApplyRecordService _prepaidApplyRecordService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISendMQService _sendMQService;
        private readonly ICustomerServiceChargeService _chargeService;

        public SalaryCustomerService(IEventPublisher eventPublisher, IPrepaidApplyRecordService prepaidApplyRecordService, ISalaryCashSendRecordService cashSendRecordService, ISalaryDelaySendRecordService delaySendRecordService, IPlatformNoticeService platformNoticeService, ISendMQService sendMQService, ICustomerServiceChargeService chargeService)
        {
            _eventPublisher = eventPublisher;
            _prepaidApplyRecordService = prepaidApplyRecordService;
            _cashSendRecordService = cashSendRecordService;
            _delaySendRecordService = delaySendRecordService;
            _platformNoticeService = platformNoticeService;
            _sendMQService = sendMQService;
            _chargeService = chargeService;
        }

        //public async Task<List<Live_V_SalaryConfig>> List(Expression<Func<Live_V_SalaryConfig, bool>> exp, PagedModelBase query)
        //{
        //    var count = new RefAsync<int>();
        //    if (query.Page == 0)
        //    {
        //        return await   base.Context.Queryable<Live_V_SalaryConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        //    }
        //    var list = await   base.Context.Queryable<Live_V_SalaryConfig>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
        //    query.Count = count.Value;
        //    return list;
        //}


        public async Task<List<Live_V_SalaryCustomer>> PageList(Expression<Func<Live_V_SalaryCustomer, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_V_SalaryCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_V_SalaryCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_SalaryCustomer>> List(Expression<Func<Live_SalaryCustomer, bool>> exp)
        {
            return await   base.Context.Queryable<Live_SalaryCustomer>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        public async Task<List<Live_SalaryCustomer>> QueryList(Expression<Func<Live_SalaryCustomer, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryCustomer>()
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }


        public async Task<bool> Exist(Guid id)
        {
            return await   base.Context.Queryable<Live_SalaryCustomer>().AnyAsync(it => it.ID == id);
        }
        public async Task<bool> Update(Live_SalaryCustomer model)
        {
            return await   base.Context.Updateable<Live_SalaryCustomer>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
     

        public async Task<Live_SalaryCustomer> Get(Guid id)
        {
            return await   base.Context.Queryable<Live_SalaryCustomer>().FirstAsync(it => it.ID == id);
        }

        public async Task<Live_SalaryCustomer> Get(Expression<Func<Live_SalaryCustomer, bool>> exp)
        {
            return await base.Context.Queryable<Live_SalaryCustomer>()
                .OrderByDescending(f=>f.CreateTime)
                .Where(exp).FirstAsync();
        }

        public async Task<Live_V_SalaryCustomer> GetView(Guid id)
        {
            return await   base.Context.Queryable<Live_V_SalaryCustomer>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await   base.Context.Deleteable<Live_SalaryCustomer>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }


        


        public async Task<bool> AddNav(Live_SalaryCustomerExtension model)
        {
            return await   base.Context
                  .InsertNav<Live_SalaryCustomerExtension>(model)
                  .Include(f => f.Records)
                  .ExecuteCommandAsync();
        }

        public async Task<bool> AddNav(List<Live_SalaryCustomerExtension> models)
        {
            return await   base.Context
                  .InsertNav<Live_SalaryCustomerExtension>(models)
                  .Include(f => f.Records)
                  .ExecuteCommandAsync();
            ;
        }
        public async Task<Live_SalaryCustomerExtension> GetNav(Expression<Func<Live_SalaryCustomerExtension, bool>> exp)
        {
            return await   base.Context
                  .Queryable<Live_SalaryCustomerExtension>()
                  .Where(exp)
                  .Includes(f => f.Records)
                  .Includes(f => f.CashSendRecords)
                  .Includes(f => f.DelaySendRecords)
                  .FirstAsync();
        }

        public async Task<Live_V_SalaryCustomerExtension> GetNavView(Expression<Func<Live_V_SalaryCustomerExtension, bool>> exp)
        {
            return await   base.Context
                  .Queryable<Live_V_SalaryCustomerExtension>()
                  .Where(exp)
                  .Includes(f => f.Records)
                  .Includes(f => f.CashSendRecords)
                  .Includes(f => f.DelaySendRecords)
                  .Includes(f => f.Invoices.OrderBy(x=>x.BusinessType).ToList())
                  .Includes(f => f.Settlements)
                  .FirstAsync();
        }





        public async Task<List<Live_SalaryCustomerExtension>> QueryNavList(Expression<Func<Live_SalaryCustomerExtension, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_SalaryCustomerExtension>()
                .Includes(f=>f.Records)
                .Includes(f=>f.DelaySendRecords)
                .Includes(f=>f.CashSendRecords)
                .OrderByIF(!string.IsNullOrWhiteSpace(strOrderByFileds), strOrderByFileds)
                .Where(exp).ToListAsync();
        }


        public async Task<List<Live_V_SalaryRecord>> QuerySalaryRecordViewList(Expression<Func<Live_V_SalaryRecord, bool>> exp, string strOrderByFileds = null)
        {
            return await   base.Context.Queryable<Live_V_SalaryRecord>()
                .OrderByDescending(f=>f.BeforeTaxSalaryTotalAmount)
                .Where(exp).ToListAsync();
        }
        public async Task<bool> UpdateNav(Live_SalaryCustomerExtension model)
        {
            return await   base.Context
                  .UpdateNav<Live_SalaryCustomerExtension>(model)
                  .Include(f => f.Records, new SqlSugar.UpdateNavOptions()
                  {
                      OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                  })
                  .Include(f => f.CashSendRecords, new SqlSugar.UpdateNavOptions()
                  {
                      OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                  }).Include(f => f.DelaySendRecords, new SqlSugar.UpdateNavOptions()
                  {
                      OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                  })
                  .ExecuteCommandAsync();
        }

        public async Task<List<Live_SalaryCustomerExtension>> ListTransNav(Expression<Func<Live_SalaryCustomerExtension, bool>> exp)
        {
            return await   base.Context
                  .Queryable<Live_SalaryCustomerExtension>()
                  .Where(exp)
                  .Includes(f => f.Records)
                  .ToListAsync();
        }
        public async Task<Live_SalaryCustomerExtension> GetTransNav(Expression<Func<Live_SalaryCustomerExtension, bool>> exp)
        {
            return await   base.Context
                  .Queryable<Live_SalaryCustomerExtension>()
                  .Where(exp)
                  .Includes(f => f.Records)
                  .FirstAsync();
        }


        public async Task<bool> UpdateTransNav(List<Live_SalaryCustomerExtension> models)
        {
            return await   base.Context
                  .UpdateNav<Live_SalaryCustomerExtension>(models)
                  .Include(f => f.Records, new SqlSugar.UpdateNavOptions()
                  {
                      OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                  })
                  .ExecuteCommandAsync();
        }
        public async Task<bool> UpdateTransNav(Live_SalaryCustomerExtension model)
        {
            return await   base.Context
                  .UpdateNav<Live_SalaryCustomerExtension>(model)
                  .Include(f => f.Records, new SqlSugar.UpdateNavOptions()
                  {
                      OneToManyInsertOrUpdate = true,//配置启用 插入、更新或删除模式
                  })
                  .ExecuteCommandAsync();
        }

        public async Task<bool> DeleteNav(Live_SalaryCustomerExtension model)
        {
            return await   base.Context
                  .DeleteNav<Live_SalaryCustomerExtension>(model)
                  .Include(f => f.Records)
                  .Include(f => f.CashSendRecords)
                  .Include(f => f.DelaySendRecords)
                  .ExecuteCommandAsync();
        }
        #region 专项扣除
        public async Task<List<Live_V_SalarySpecialDedcution>> QuerySpecialDeductionList(Expression<Func<Live_V_SalarySpecialDedcution, bool>> exp)
        {
            return await   base.Context.Queryable<Live_V_SalarySpecialDedcution>()
                .Where(exp).ToListAsync();
        }
        public async Task<bool> AddSpecialDeduction(List<Live_SalarySpecialDedcution> dedcutions)
        {
            return await   base.Context.Insertable<Live_SalarySpecialDedcution>(dedcutions).ExecuteCommandAsync() > 0;
        }

        #endregion


        #region 工资条
        public async Task<List<Dictionary<int, string>>> ListSalaryRecordSlipRecord(Guid id)
        {
          var records=  await   base.Context.Queryable<Live_V_SalaryRecordSlipRecord>().Where(f => f.SalaryCustomerID == id && f.SalarySlipRecordID != null).ToListAsync();
            if (records==null|| records.Count<=0)
            {
                return null;
            }
            Dictionary<int, string> headerDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(records.First().HeaderKV)));


           var salaryMonth= records.First().SalaryMonth;
            var max = headerDic.Keys.Max();
            headerDic.Add(max + 1, "应发工资合计");
            headerDic.Add(max + 2, "个人社保合计");
            headerDic.Add(max + 3, "个人住房公积金");
            headerDic.Add(max + 4, "个人所得税");
            headerDic.Add(max + 5, "预发工资");
            headerDic.Add(max + 6, "税后扣除");
            headerDic.Add(max + 7, "税后补贴");
            headerDic.Add(max + 8, "实发工资");
            headerDic.Add(max + 9, "银行卡所属人");
            headerDic.Add(max + 10, "银行卡号");
            headerDic.Add(max + 11, "银行卡所属行");
            headerDic.Add(max + 12, "备注");

           var monthKey= headerDic.FirstOrDefault(f => f.Value == "工资所属月");


            List<Dictionary<int, string>> dss = new List<Dictionary<int, string>>();
            dss.Add(headerDic);
            foreach (var item in records)
            {
                Dictionary<int, string> contentDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(item.ContentKV)));
                contentDic[monthKey.Key] = salaryMonth.Value.ToString("yyyy-MM-dd");
                if (contentDic.ContainsKey(max + 1))
                {
                    contentDic[max + 1] = $"{item.BeforeTaxSalaryAmount}";
                }
                else
                {
                    contentDic.Add(max + 1, $"{item.BeforeTaxSalaryAmount}");
                }

                if (contentDic.ContainsKey(max + 2))
                {
                    contentDic[max + 2] = $"{item.SocialEmpSumAmount - item.SocialGongJiJinAmount}";
                }
                else
                {
                    contentDic.Add(max + 2, $"{item.SocialEmpSumAmount - item.SocialGongJiJinAmount}");
                }



                if (contentDic.ContainsKey(max + 3))
                {
                    contentDic[max + 3] = $"{item.SocialGongJiJinAmount}";
                }
                else
                {
                    contentDic.Add(max + 3, $"{item.SocialGongJiJinAmount}");
                }

                if (contentDic.ContainsKey(max + 4))
                {
                    contentDic[max + 4] = $"{item.TaxMonthAmount}";
                }
                else
                {
                    contentDic.Add(max + 4, $"{item.TaxMonthAmount}");
                }


                if (contentDic.ContainsKey(max + 5))
                {
                    contentDic[max + 5] = $"{item.PrepaidRepaymentAmount}";
                }
                else
                {
                    contentDic.Add(max + 5, $"{item.PrepaidRepaymentAmount}");
                }

                if (contentDic.ContainsKey(max + 6))
                {
                    contentDic[max + 6] = $"{item.DeductionNoTaxAmount}";
                }
                else
                {
                    contentDic.Add(max + 6, $"{item.DeductionNoTaxAmount}");
                }


                if (contentDic.ContainsKey(max + 7))
                {
                    contentDic[max + 7] = $"{item.SubsidyNoTaxAmount}";
                }
                else
                {
                    contentDic.Add(max + 7, $"{item.SubsidyNoTaxAmount}");

                }

                if (contentDic.ContainsKey(max + 8))
                {
                    contentDic[max + 8] = $"{item.ActualSalarySendCardAmount}";
                }
                else
                {
                    contentDic.Add(max + 8, $"{item.ActualSalarySendCardAmount}");
                }

                if (contentDic.ContainsKey(max + 9))
                {
                    contentDic[max + 9] = $"{item.BankUserName}";
                }
                else
                {
                    contentDic.Add(max + 9, $"{item.BankUserName}");
                }

                if (contentDic.ContainsKey(max + 10))
                {
                    contentDic[max + 10] = $"{item.BankNo}";
                }
                else
                {
                    contentDic.Add(max + 10, $"{item.BankNo}");
                }

                if (contentDic.ContainsKey(max + 11))
                {
                    contentDic[max + 11] = $"{item.Bank}";
                }
                else
                {
                    contentDic.Add(max + 11, $"{item.Bank}");

                }

                if (contentDic.ContainsKey(max + 12))
                {
                    contentDic[max + 12] = $"{item.Remark}";
                }
                else
                {
                    contentDic.Add(max + 12, $"{item.Remark}");

                }

                if (contentDic.ContainsKey(max + 13))
                {
                    contentDic[max + 13] = $"{item.SalaryBatchType}";
                }
                else
                {
                    contentDic.Add(max + 13, $"{item.SalaryBatchType}");

                }




                dss.Add(contentDic);
            }
            return dss;
        }

     

        public async Task<List<UserSalarySlip>> ListUserSalaryRecordSlipRecord(Expression<Func<Live_V_SalaryRecordSlipRecord, bool>> exp)
        {

            List<UserSalarySlip> userSalarySlips = new List<UserSalarySlip>();
            var records = await   base.Context.Queryable<Live_V_SalaryRecordSlipRecord>().Where(exp).OrderByDescending(f=>new { f.SalaryMonth,f.SalaryBatchType}).ToListAsync();
            if (records == null || records.Count <= 0)
            {
                return null;
            }
            
            UserSalarySlip userSalarySlip = null;

            UserSalarySlipItem slipItem = null;
            foreach (var item in records)
            {

                Dictionary<int, string> headerDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(item.HeaderKV)));
                var monthKey = headerDic.FirstOrDefault(f => f.Value == "工资所属月");

                var max = headerDic.Keys.Max();
                headerDic.Add(max + 1, "应发工资合计");
                headerDic.Add(max + 2, "个人社保合计");
                headerDic.Add(max + 3, "个人住房公积金");
                headerDic.Add(max + 4, "个人所得税");
                headerDic.Add(max + 5, "预发工资");
                headerDic.Add(max + 6, "税后扣除");
                headerDic.Add(max + 7, "税后补贴");
                headerDic.Add(max + 8, "实发工资");
                headerDic.Add(max + 9, "银行卡所属人");
                headerDic.Add(max + 10, "银行卡号");
                headerDic.Add(max + 11, "银行卡所属行");
                headerDic.Add(max + 12, "备注");
                headerDic.Add(max + 13, "工资批次号");

                userSalarySlip = new UserSalarySlip()
                {
                    ID= item.ID,
                    CustomerName = item.CustomerName,
                    SalaryBatchType = item.SalaryBatchType.Value,
                    SalaryMonth = item.SalaryMonth.Value,
                    Records = new List<UserSalarySlipItem>()

                };

                Dictionary<int, string> contentDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(item.ContentKV)));


               
                contentDic[monthKey.Key] = item.SalaryMonth.Value.ToString("yyyy-MM-dd");

                if (contentDic.ContainsKey(max + 1))
                {
                    contentDic[max+1]= $"{item.BeforeTaxSalaryAmount}";
                }
                else {
                    contentDic.Add(max + 1, $"{item.BeforeTaxSalaryAmount}");
                }

                if (contentDic.ContainsKey(max + 2))
                {
                    contentDic[max + 2] = $"{item.SocialEmpSumAmount - item.SocialGongJiJinAmount}";
                }
                else
                {
                    contentDic.Add(max + 2, $"{item.SocialEmpSumAmount - item.SocialGongJiJinAmount}");
                }



                if (contentDic.ContainsKey(max + 3))
                {
                    contentDic[max + 3] = $"{item.SocialGongJiJinAmount}";
                }
                else
                {
                    contentDic.Add(max + 3, $"{item.SocialGongJiJinAmount}");
                }

                if (contentDic.ContainsKey(max + 4))
                {
                    contentDic[max + 4] = $"{item.TaxMonthAmount}";
                }
                else
                {
                    contentDic.Add(max + 4, $"{item.TaxMonthAmount}");
                }


                if (contentDic.ContainsKey(max + 5))
                {
                    contentDic[max + 5] = $"{item.PrepaidRepaymentAmount}";
                }
                else
                {
                    contentDic.Add(max + 5, $"{item.PrepaidRepaymentAmount}");
                }

                if (contentDic.ContainsKey(max + 6))
                {
                    contentDic[max + 6] = $"{item.DeductionNoTaxAmount}";
                }
                else
                {
                    contentDic.Add(max + 6, $"{item.DeductionNoTaxAmount}");
                }


                if (contentDic.ContainsKey(max + 7))
                {
                    contentDic[max + 7] = $"{item.SubsidyNoTaxAmount}";
                }
                else
                {
                    contentDic.Add(max + 7, $"{item.SubsidyNoTaxAmount}");

                }

                if (contentDic.ContainsKey(max + 8))
                {
                    contentDic[max + 8] = $"{item.ActualSalarySendCardAmount}";
                }
                else
                {
                    contentDic.Add(max + 8, $"{item.ActualSalarySendCardAmount}");
                }

                if (contentDic.ContainsKey(max + 9))
                {
                    contentDic[max + 9] = $"{item.BankUserName}";
                }
                else
                {
                    contentDic.Add(max + 9, $"{item.BankUserName}");
                }

                if (contentDic.ContainsKey(max + 10))
                {
                    contentDic[max + 10] = $"{item.BankNo}";
                }
                else
                {
                    contentDic.Add(max + 10, $"{item.BankNo}");
                }

                if (contentDic.ContainsKey(max + 11))
                {
                    contentDic[max + 11] = $"{item.Bank}";
                }
                else
                {
                    contentDic.Add(max + 11, $"{item.Bank}");

                }

                if (contentDic.ContainsKey(max + 12))
                {
                    contentDic[max + 12] = $"{item.Remark}";
                }
                else
                {
                    contentDic.Add(max + 12, $"{item.Remark}");

                }

                if (contentDic.ContainsKey(max + 13))
                {
                    contentDic[max + 13] = $"{item.SalaryBatchType}";
                }
                else
                {
                    contentDic.Add(max + 13, $"{item.SalaryBatchType}");

                }
                foreach (var headerItem in headerDic)
                {

                    if (contentDic.Keys.Contains(headerItem.Key))
                    {
                        slipItem = new UserSalarySlipItem()
                        {
                            Title = headerItem.Value,
                            VauleInfo = contentDic[headerItem.Key]
                        };
                    }
                    else {
                        slipItem = new UserSalarySlipItem()
                        {
                            Title = headerItem.Value,
                            VauleInfo = string.Empty
                        };
                    }
                    
                    userSalarySlip.Records.Add(slipItem);
                }
                userSalarySlips.Add(userSalarySlip);
            }
            return userSalarySlips;
        }





        #endregion



        #region 发放流程


        /// <summary>
        ///  客户工资发放完成
        ///  预发工资还款完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        //public async Task<bool> BatchSendSuccess(Live_SalaryCustomerExtension model)
        //{
        //      base.Context.BeginTran();
        //    model.Status = SalaryStatusEnumType.SucessSend;
        //    foreach (var item in model.Records)
        //    {
        //        if (item.IsBatchSend)
        //        {


        //            item.Status = Model.Enums.SalaryRecordSendStatusEnumType.已发放;
        //            item.SendTime = DateTime.Now;
        //        }
        //        else 
        //        {

        //            if (item.ActualSalarySendCardAmount <= 0 && item.SalaryRecordType == Model.Enums.SalaryRecordTypeEnumType.常规工资)
        //            {
        //                item.Status = Model.Enums.SalaryRecordSendStatusEnumType.已发放;
        //                item.SendTime = DateTime.Now;
        //            }
        //            else if (item.ActualSalarySendCardAmount > 0 && item.SalaryRecordType == Model.Enums.SalaryRecordTypeEnumType.常规工资)
        //            {
        //                // 暂缓且非现金发放 允许民生单笔代发
        //                if (model.IsCanCmbcSend&&item.IsDelayPayment&& !item.IsCashSend)
        //                {
        //                    item.IsSingleSend = true;
        //                    item.CmbcSendStatus = CmbcSendStatusEnumType.待推送;
        //                }
        //            }
        //        }
        //    }

        //    #region 预发工资还款完成标记完成
        //    var prepaidRepaymentRecordList = await   base.Context.Queryable<Live_PrepaidRepaymentRecord>().Where(f => f.SalaryCustomerID == model.ID).ToListAsync();
        //    if (prepaidRepaymentRecordList != null && prepaidRepaymentRecordList.Count > 0)
        //    {
        //        foreach (var item in prepaidRepaymentRecordList)
        //        {
        //            item.IsSuc = true;
        //            item.RepaymentTime = DateTime.Now;
        //            item.Remark = "客户工资批量发放完成";
        //        }
        //        await   base.Context.Updateable(prepaidRepaymentRecordList).ExecuteCommandAsync();
        //        // 获取
        //        var applyIdList = prepaidRepaymentRecordList.Select(f => f.PrepaidApplyRecordID).Distinct().ToList();
        //        var applyList = await   base.Context.Queryable<Live_PrepaidApplyRecord>().Where(f => applyIdList.Contains(f.ID)).ToListAsync();

        //        foreach (var item in applyList)
        //        {
        //            if (item.RepayStatus == Model.Enums.PrepaidRepayStatusEnumType.还款中)
        //            {
        //                item.RepayStatus = Model.Enums.PrepaidRepayStatusEnumType.还款完成;
        //            }
        //        }
        //        await   base.Context.Updateable<Live_PrepaidApplyRecord>(applyList).ExecuteCommandAsync();
        //    }
        //    #endregion

        //    model.SendTime = DateTime.Now;
        //    await   base.Context.UpdateNav<Live_SalaryCustomerExtension>(model).Include(f=>f.Records).ExecuteCommandAsync();
        //      base.Context.CommitTran();


        //    // 如果关联预算申请记录则标记完成
        //    if (model.BudgetApplyRecordID.HasValue&&model.BudgetApplyRecordID.Value>0)
        //    {
        //        SalaryBudgetApplyCompleteEventData eventData = new SalaryBudgetApplyCompleteEventData() { 
        //         BudgetApplyRecordID = model.BudgetApplyRecordID.Value,
        //        };
        //        _eventPublisher.Publish(eventData);
        //    }

        //    return true;
        //}



        public async Task<bool> BatchSendSuccess(Live_SalaryCustomerExtension model)
        {
            
            model.Status = SalaryStatusEnumType.SucessSend;

            List<Live_SalaryFailRecord> failList = new List<Live_SalaryFailRecord>();
            foreach (var item in model.Records)
            {
                if (item.IsBatchSend&&!item.IsSingleSend)
                {
                    // 主要是为民生代发发放异常
                    if (item.Status == Model.Enums.SalaryRecordSendStatusEnumType.发放异常)
                    {
                        // 异常工资也可以民生单笔代发
                        item.IsFailSalary = true;
                        item.IsFailSalarySuc = false;
                        item.IsSendSuccess = false;

                        if (model.IsCanCmbcSend)
                        {
                            item.CmbcSendStatus = CmbcSendStatusEnumType.待推送;
                        }
                        else
                        {
                            item.CmbcSendStatus = CmbcSendStatusEnumType.无;
                        }
                        item.IsSingleSend = true;
                        //item.IsBatchSend = false;
                        item.IsSinglePushSuc = false;
                        Live_SalaryFailRecord failRecord = new Live_SalaryFailRecord()
                        {
                            Bank = item.Bank,
                            BankNo = item.BankNo,
                            BankUserName = item.BankUserName,
                            CreateTime = DateTime.Now,
                            Description = item.TransMsg,
                            Remark = string.Empty,
                            SalaryCustomerID = item.SalaryCustomerID,
                             FailType= SalaryFailTypeEnumType.其他,
                            Status = SalaryFailRecordStatusEnumType.待处理,
                            SalaryRecordID = item.ID,
                        };

                        if (string.IsNullOrWhiteSpace(failRecord.Description))
                        {
                            failRecord.FailType= SalaryFailTypeEnumType.其他;
                            failRecord.Description = "发放异常";
                        }
                        failList.Add(failRecord);

                    }
                    else
                    {
                        item.SendTime = DateTime.Now;
                        item.IsSendSuccess = true;
                        item.Status = Model.Enums.SalaryRecordSendStatusEnumType.已发放;
                    }

                }
                else
                {

                    if (item.ActualSalarySendCardAmount <= 0 && item.SalaryRecordType == Model.Enums.SalaryRecordTypeEnumType.常规工资)
                    {
                        item.Status = Model.Enums.SalaryRecordSendStatusEnumType.已发放;
                        item.SendTime = DateTime.Now;
                        item.IsSendSuccess=true;
                    }
                    else if (item.ActualSalarySendCardAmount > 0 && item.SalaryRecordType == Model.Enums.SalaryRecordTypeEnumType.常规工资)
                    {
                        // 暂缓且非现金发放 允许民生单笔代发
                        if (model.IsCanCmbcSend && item.IsDelayPayment && !item.IsCashSend)
                        {
                            item.IsSingleSend = true;
                            item.CmbcSendStatus = CmbcSendStatusEnumType.待推送;
                        }

                    }

                }
            }
            // 出现异常
            if (failList.Any())
            {
                await   base.Context.Insertable<Live_SalaryFailRecord>(failList).ExecuteCommandAsync();
            }
            var prepaidRepaymentRecordList = await   base.Context.Queryable<Live_PrepaidRepaymentRecord>().Where(f => f.SalaryCustomerID == model.ID).ToListAsync();
            if (prepaidRepaymentRecordList != null && prepaidRepaymentRecordList.Count > 0)
            {
                foreach (var item in prepaidRepaymentRecordList)
                {
                    item.IsSuc = true;
                    item.RepaymentTime = DateTime.Now;
                    item.Remark = "客户工资批量发放完成";
                }
                await   base.Context.Updateable(prepaidRepaymentRecordList).ExecuteCommandAsync();
                // 获取
                var applyIdList = prepaidRepaymentRecordList.Select(f => f.PrepaidApplyRecordID).Distinct().ToList();
                var applyList = await   base.Context.Queryable<Live_PrepaidApplyRecord>().Where(f => applyIdList.Contains(f.ID)).ToListAsync();

                foreach (var item in applyList)
                {
                    if (item.RepayStatus == Model.Enums.PrepaidRepayStatusEnumType.还款中)
                    {
                        item.RepayStatus = Model.Enums.PrepaidRepayStatusEnumType.还款完成;
                    }
                }
                await   base.Context.Updateable<Live_PrepaidApplyRecord>(applyList).ExecuteCommandAsync();
            }
            model.SendTime = DateTime.Now;
            await   base.Context.UpdateNav<Live_SalaryCustomerExtension>(model).Include(f => f.Records).ExecuteCommandAsync();


            // 如果关联预算申请记录则标记完成
            if (model.BudgetApplyRecordID.HasValue && model.BudgetApplyRecordID.Value > 0)
            {
                SalaryBudgetApplyCompleteEventData eventData = new SalaryBudgetApplyCompleteEventData()
                {
                    BudgetApplyRecordID = model.BudgetApplyRecordID.Value,
                };
                _eventPublisher.Publish(eventData);
            }


            #region 大屏统计实时消息-借支还款完成

            if (prepaidRepaymentRecordList != null && prepaidRepaymentRecordList.Count > 0)
            {
                var repayEmpRecord = model.Records.FirstOrDefault(f => f.PrepaidRepaymentAmount > 0);
                if (repayEmpRecord != null)
                {
                    var mqMsg = new
                    {
                        Type = "prepaidSalaryRepaySuccess",
                        Title = "借支还款完成",
                        CustomerName = repayEmpRecord.CustomerName,
                        Name = repayEmpRecord.Name,
                        CardID = repayEmpRecord.IdCardNo,
                        ApplyAmount = repayEmpRecord.PrepaidRepaymentAmount,
                        TotalAmount = prepaidRepaymentRecordList.Sum(f => f.TotalAmount)
                    };
                    _sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
                }
            }


            #endregion
           
            await _chargeService.ChangeSendTime(model.ID);


            return true;
        }


        public async Task<bool> BatchSendFail(Live_SalaryCustomerExtension model)
        {

         
            itenant.BeginTran();
            model.Status = SalaryStatusEnumType.FailSend;
            //foreach (var item in model.Records)
            //{
            //    if (item.IsBatchSend)
            //    {
            //        item.Status = Model.Enums.SalaryRecordSendStatusEnumType.发放异常;
            //    }
            //}
            await   base.Context.UpdateNav<Live_SalaryCustomerExtension>(model).Include(f => f.Records).ExecuteCommandAsync();
            itenant.CommitTran();
            return true;
        }



        /// <summary>
        ///  设置客户已打款
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> SetCustomerPaid(Guid id)
        {
            var model = await GetTransNav(f=>f.ID==id);


            if (model==null)
            {
                return false;
            }


            //if (model.IsCombineSalary)
            //{
            //    return false;
            //}
            if (model.IsCustomerPaid)
            {
                return false;
            }
            model.IsCustomerPaid = true;
            model.CustomerPaidTime = DateTime.Now;

            List<Guid> salaryRecordIDList = new List<Guid>();


            List<Live_PrepaidApplyRecordExtension> updatePrepaidApplyRecordList = new List<Live_PrepaidApplyRecordExtension>();
            // 实发工资大于0 的才能还款，针对新借款的记录
            var idCardNoList = model.Records.Where(f => f.IsCombineSalary == false && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.ActualSalarySendCardAmount > 0).Select(f => f.IdCardNo).ToList();

            #region 预发工资
            // 获取预发工资
            var prepaidApplyRecordList = await _prepaidApplyRecordService.ListNav(f =>
            idCardNoList.Contains(f.IdCardNo)
            && f.CustomerGID == model.CustomerGID
            && f.Status == PrepaidRecordStatusEnumType.已发放
            && (f.RepayStatus == PrepaidRepayStatusEnumType.无 || f.RepayStatus == PrepaidRepayStatusEnumType.待还款));

            // 客户工资未发放
            // 平台未垫付 才能再次扣回预发，否则不再扣回预发
            if (model.Status < SalaryStatusEnumType.SucessSend&&model.IsPlatformPrepaid==false)
            {
                foreach (var item in prepaidApplyRecordList)
                {
                    if (model.Records.Any(f => f.IdCardNo == item.IdCardNo))
                    {
                        var salaryRecord = model.Records.First(f => f.IdCardNo == item.IdCardNo);
                        if (salaryRecord.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && salaryRecord.ActualSalaryAmount > 0)
                        {
                            var repaymentAmount = item.ApplyAmount - item.RepayTotalAmount;
                            if (repaymentAmount > 0)
                            {
                                var canRepaymentAmount = salaryRecord.PrepaidRepayment(repaymentAmount);
                                if (canRepaymentAmount > 0)
                                {
                                    // 进行二次还款的记录
                                    salaryRecordIDList.Add(salaryRecord.ID);
                                    item.AddRepaymentRecord(salaryRecord.SalaryCustomerID, salaryRecord.ID, salaryRecord.UserEntryID, canRepaymentAmount, "工资发放还款");
                                    updatePrepaidApplyRecordList.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            try
            {
              itenant.BeginTran();
                if (salaryRecordIDList.Any())
                {
                    var delayRecords = await _delaySendRecordService.List(f => salaryRecordIDList.Contains(f.SalaryRecordID.Value));
                    if (delayRecords != null && delayRecords.Any())
                    {
                        foreach (var delayRecord in delayRecords)
                        {
                            var salaryRecord = model.Records.First(f => f.ID == delayRecord.SalaryRecordID);
                            delayRecord.TotalAmount = salaryRecord.ActualSalarySendCardAmount;
                        }
                        await _delaySendRecordService.UpdateTotalAmount(delayRecords);
                    }

                    var cashSendRecords = await _cashSendRecordService.List(f => salaryRecordIDList.Contains(f.SalaryRecordID.Value));
                    if (cashSendRecords != null && cashSendRecords.Any())
                    {
                        foreach (var cashSendRecord in cashSendRecords)
                        {
                            var salaryRecord = model.Records.First(f => f.ID == cashSendRecord.SalaryRecordID);
                            cashSendRecord.TotalAmount = salaryRecord.ActualSalarySendCardAmount;
                        }
                        await _cashSendRecordService.UpdateTotalAmount(cashSendRecords);
                    }
                }


                if (updatePrepaidApplyRecordList.Count > 0)
                {
                    await _prepaidApplyRecordService.UpdateNav(updatePrepaidApplyRecordList);
                }
                await UpdateTransNav(model);
              itenant.CommitTran();
            }
            catch (Exception ex)
            {
              itenant.RollbackTran();
                throw ex;
            }
            var modelView = await GetView(id);
            if (modelView.PartnerID != null)
            {
                _platformNoticeService.ManagerDefaultNotice(modelView.PartnerID.Value, "客户已打款通知", $"{model.CustomerName} 已打款");
            }
            return true;
        }





        /// <summary>
        ///  设置工资状态待推送
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> SetPendingPush(Guid id)
        {
            var model = await GetTransNav(f => f.ID == id);
            var recordList = model.Records.Where(f => f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.ActualSalarySendCardAmount > 0).ToList();
            // 工资记录状态==>待处理
            if (recordList.Any())
            {
                foreach (var item in recordList)
                {
                    item.Status = SalaryRecordSendStatusEnumType.待处理;
                }
            }
            // 客户工资状态==>审核退回
            model.Status = SalaryStatusEnumType.PendingPush;

            // 获取待开具发票
            var invoiceList = await   base.Context.Queryable<Live_Invoice>().Where(f => f.SalaryCustomerID != null && f.SalaryCustomerID == model.ID && f.Status <= InvoiceStatusEnumType.待开具).ToListAsync();

            if (invoiceList.Any())
            {
                foreach (var item in invoiceList)
                {
                    item.Status = InvoiceStatusEnumType.待锁定;
                    item.LockedTime = null;
                }
            }


            try
            {
              itenant.BeginTran();
                if (invoiceList.Any())
                {
                    await   base.Context.Updateable(invoiceList).ExecuteCommandAsync();
                }
                bool isSuc = await UpdateTransNav(model);
              itenant.CommitTran();
                return isSuc;

            }
            catch (Exception ex)
            {
              itenant.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存异常，异常信息：{ex.Message}");
            }


        }


        /// <summary>
        ///  工资审核退回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> SetReviewRejected(Guid id)
        {
            var model = await GetTransNav(f => f.ID == id);
            var recordList = model.Records.Where(f => f.IsCashSend == false && f.IsDelayPayment == false && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资 && f.ActualSalarySendCardAmount > 0).ToList();
            // 工资记录状态==>待处理
            if (recordList.Any())
            {
                foreach (var item in recordList)
                {
                    item.Status = SalaryRecordSendStatusEnumType.审核未通过;
                }
            }
            // 客户工资状态==>审核退回
            model.Status = SalaryStatusEnumType.ReviewRejected;

            // 获取待开具发票
            var invoiceList = await   base.Context.Queryable<Live_Invoice>().Where(f => f.SalaryCustomerID != null && f.SalaryCustomerID == model.ID && f.Status <= InvoiceStatusEnumType.待开具).ToListAsync();

            if (invoiceList.Any())
            {
                foreach (var item in invoiceList)
                {
                    item.Status = InvoiceStatusEnumType.审核退回;
                }
            }


            try
            {
              itenant.BeginTran();
                if (invoiceList.Any())
                {
                    await   base.Context.Updateable(invoiceList).ExecuteCommandAsync();
                }
                bool isSuc = await UpdateTransNav(model);
              itenant.CommitTran();
                return isSuc;

            }
            catch (Exception ex)
            {
              itenant.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存异常，异常信息：{ex.Message}");
            }


        }

        /// <summary>
        ///  推送到财务 待审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> SetPendingReview(Guid id)
        {

        

            var model = await GetTransNav(f => f.ID == id);
            
            model.SetBatchRecord();
           
            model.Status = SalaryStatusEnumType.PendingReview;
            var invoiceList = await   base.Context.Queryable<Live_Invoice>().Where(f => f.SalaryCustomerID != null && f.SalaryCustomerID == model.ID && f.Status <= InvoiceStatusEnumType.待开具).ToListAsync();
            try
            {


              itenant.BeginTran();
                if (invoiceList != null && invoiceList.Any())
                {
                    foreach (var item in invoiceList)
                    {
                        item.Status = InvoiceStatusEnumType.待审核;
                        item.LockedTime = DateTime.Now;
                        await   base.Context.Updateable<Live_Invoice>(item).ExecuteCommandAsync();
                    }

                    if (invoiceList.Any(f => f.BusinessType == PrintTemplateEnumType.外部发票))
                    {
                        model.Status = SalaryStatusEnumType.PendingReview;
                    }
                    else
                    {
                        model.Status = SalaryStatusEnumType.PendingSend;
                    }
                }
                else
                {
                    model.Status = SalaryStatusEnumType.PendingSend;
                }
                if (model.Status == SalaryStatusEnumType.PendingSend)
                {
                    model.SetPendingSend();
                }
                await UpdateTransNav(model);

                // 待审核推送
                await _chargeService.Create(id);
                itenant.CommitTran();
            }
            catch (Exception ex)
            {
                itenant.RollbackTran();
                throw new ErrorCodeException(-1, $"数据保存异常，异常信息：{ex.Message}");
            }
            return true;



        }


        /// <summary>
        ///  推送到财务 待审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> SetPendingSend(Guid id)
        {
            var model = await GetTransNav(f=>f.ID==id);
            model.SetPendingSend();   

          return  await UpdateTransNav(model);
        }



        /// <summary>
        ///  锁定数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> SetLocked(Guid id,bool isLocked)
        {
            var model = await GetTransNav(f => f.ID == id);
            try
            {
               itenant.BeginTran();
                model.SetBatchLocked(isLocked);
                #region 发放成功后 则更新结算单
                var sheet = await   base.Context.Queryable<Live_Settlement>().Where(f => f.SalaryCustomerID == model.ID && f.BusinessType == SettlementBusinessTypeEnumType.工资结算单).FirstAsync();
                if (sheet != null)
                {
                    var totalAmount = model.Records.Where(f => f.IsDelayPayment == false && f.IsCashSend == false && f.SalaryRecordType == SalaryRecordTypeEnumType.常规工资).Sum(f => f.ActualSalarySendCardAmount);
                    sheet.Description = sheet.Description.Replace($"{sheet.TotalAmount}", $"{totalAmount}");
                    sheet.TotalAmount = totalAmount;
                    await   base.Context.Updateable<Live_Settlement>(sheet).ExecuteCommandAsync();
                }
                #endregion
                await UpdateTransNav(model);
              itenant.CommitTran();
            }
            catch (Exception ex)
            {

                throw new ErrorCodeException(-1,$"锁定提交异常，异常信息：{ex.Message}");
            }


            return true;
        }


        #endregion


        public async Task<int> DictionarySet(Dictionary<string, object> dt)
        {
            return await base.Context.Updateable<Model.Live_SalaryCustomer>(dt).With(SqlWith.UpdLock).ExecuteCommandAsync();
        }



        #region 导出报表
        public byte[] Export(List<Model.ExcelTemplateModel.SalaryRecordExportExcelTemplate> list)
        {

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("工资结算单");
            var font = workbook.CreateFont();
            font.FontHeightInPoints = 9;
            font.FontName = "微软雅黑";
            font.IsBold = true;
            double width = 20 * 256;
            short height = 20 * 20;

            sheet.DefaultRowHeight = height;
            //for (int x = 0; x < 6; x++)
            //{
            //    sheet.SetColumnWidth(x, width);
            //}
            //sheet.SetColumnWidth(6, 25 * 256);
            //for (int y = 0; y < 27; y++)
            //{
            //    sheet.CreateRow(y);
            //}
            #region excel 表头
            string[] heads = {
                "批次",
                "客户单位",
                "年度累计次数",
                "工资所属月",
                "入职日期",
                "离职日期",
                "姓名",
                "身份证号码",
                "联系方式",
                "招聘渠道",
                "是否现金发放",
                "是否暂缓发放",
                "企业应发工资",
                "企业扣款",
                "实收企业工资合计",
                "税前补贴",
                "税后补贴",
                "税后扣除",
                "提成(平台专用)",
                "福利(平台专用)",
                "应发工资合计",
                "个人养老保险",
                "个人医疗保险",
                "个人失业保险","个人住房公积金","个人重大疾病险","个人所得税","预发工资","实发工资",
                "子女教育",
                "个人教育","住房租金","住房贷款","赡养老人","婴幼儿照护","银行卡所属人","银行卡所属行","银行卡号",
                "累计收入","社保个人部分合计","累计专项扣除","累计专项附加扣除","累计减除费用","累计应纳税所得额","累计应缴","暂缓发放",
                "备注","商务合作单位",
                "实发工资单位",
                "社保企业部分","管理费","合计金额"
            };

            
            ExcelHeler.SetHeaderSingle(workbook, sheet, heads, font, width, height);
            #endregion
            #region 表格主体
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.IsHidden = true;
            
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 9;
            font1.FontName = "微软雅黑";
            int index = 1;
            foreach (var x in list)
            {
                int row = 0;
                sheet.CreateRow(index + 1);//index=1,从第三行开始,前2行是表头
                List<object> arr = new List<object>();
               
                arr.Add(x.SalaryBatchType);
                arr.Add(x.CustomerName);
                arr.Add(x.TaxTimes);
                arr.Add(x.SalaryMonth);
                arr.Add(x.EntryDate);
                arr.Add(x.LeaveDate);
                arr.Add(x.Name);
                arr.Add(x.IdCardNo);
                arr.Add(x.Phone);
                arr.Add(x.EntryChannel);
                arr.Add(x.IsCashSend);
                arr.Add(x.IsDelayPayment);
                arr.Add(x.UnitSalaryAmount);
                arr.Add(x.UnitDeductionAmount);
                arr.Add(x.CompanySalarySumAmount);
                arr.Add(x.SubsidyAmount);
                arr.Add(x.SubsidyNoTaxAmount);
                arr.Add(x.DeductionNoTaxAmount);
                arr.Add(x.PlatformCommissionAmount);
                arr.Add(x.PlatformWelfareAmount);
                arr.Add(x.BeforeTaxSalaryAmount);
                arr.Add(x.SocialYangLaoAmount);
                arr.Add(x.SocialYiLiaoAmount);
                arr.Add(x.SocialShiYeAmount);
                arr.Add(x.SocialGongJiJinAmount);
                arr.Add(x.SocialDaBingAmount);
                arr.Add(x.TaxMonthAmount);
                arr.Add(x.PrepaidRepaymentAmount);
                arr.Add(x.ActualSalarySendCardAmount);
                arr.Add(x.SpZiNvAmount);
                arr.Add(x.SpJiaoYuAmount);
                arr.Add(x.SpZuJinAmount);
                arr.Add(x.SpDaiKuanAmount);
                arr.Add(x.SpShanYangAmount);
                arr.Add(x.SpYingErAmount);
                arr.Add(x.BankUserName);
                arr.Add(x.Bank);
                arr.Add(x.BankNo);
                arr.Add(x.BeforeTaxSalaryTotalAmount);
                arr.Add(x.SocialEmpSumAmount);
                arr.Add(x.TaxSocialTotalAmount);
                arr.Add(x.TaxSpTotalAmount);
                arr.Add(x.TaxSpBasicTotalAmount);
                arr.Add(x.TaxSalaryTotalAmount);
                arr.Add(x.TaxTotalAmount);
                arr.Add(x.DelaySendAmount);
                arr.Add(x.Remark);
                arr.Add(x.CooperationUnitCompanyName);
                arr.Add(x.SigningUnitCompanyName);
                arr.Add(x.SocialUnitSumAmount);
                arr.Add(x.UnitManageAmount);
                arr.Add(x.CompanyTotalAmount);
                

                foreach (var value in arr)
                {
                    ExcelHeler.SetCellValueNoStyle(workbook, sheet, style, font1, index + 1, row, value);
                    row++;
                }

                index++;
            }
            #endregion





            byte[] buffer = new byte[1024 * 5];
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                buffer = stream.ToArray();
            }
            return buffer;
        }

        #endregion


        public SalayCustomerCalculateTimes SalaryCustomerCalculateTimes()
        {
            string sql = @"select next value for SalaryCalculateTimesToday as TodayTimes, next value for SalaryCalculateTimesMonth as MonthTimes,next value for SalaryCalculateTimesYear as YearTimes";
            var sqlRes =   base.Context.Ado.SqlQuerySingle<dynamic>(sql);

            var res = new SalayCustomerCalculateTimes()
            {

                MonthTimes = sqlRes.MonthTimes,
                TodayTimes = sqlRes.TodayTimes,
                YearTimes = sqlRes.YearTimes,
            };

            return res;
        }



        public bool ResetSalaryCustomerCalculateTimes()
        {

            string sql1 = @"
ALTER SEQUENCE SalaryCalculateTimesToday
RESTART WITH 0;
";
            string sql2 = @"
ALTER SEQUENCE SalaryCalculateTimesMonth
RESTART WITH 0;
";

            string sql3 = @"
ALTER SEQUENCE SalaryCalculateTimesYear
RESTART WITH 0;
";

            var today = DateTime.Today;
            var lastDay= today.AddDays(-1);

            var todaySeq =   base.Context.Ado.ExecuteCommand(sql1);

            if (today.Month!=lastDay.Month)
            {
                var monthSeq =   base.Context.Ado.ExecuteCommand(sql2);
            }


            if (today.Year != lastDay.Year)
            {
                var monthSeq =   base.Context.Ado.ExecuteCommand(sql3);
            }




            return true;
        }
        #region 代发推送取消 或者完全发放失败


        /// <summary>
        ///  重置代发推送状态
        ///  完全发放失败或者取消发放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> RollbackToPendingSend(Guid id)
        {
            var model = await GetNav(f => f.ID == id);
            model.SetPendingSend();
            return await UpdateTransNav(model);
        }
        #endregion


    }
}
