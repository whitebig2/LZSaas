using Aliyun.OSS;
using Consul;
using LZ.ApiService.Interface;
using LZ.Core.Attributes;
using LZ.Core.CommonHandle;
using LZ.Core.EventData;
using LZ.Core.EventData.Insurance;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Enums.Injury;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using Senparc.Weixin.WxOpen.Entities;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryInfoController(IInjuryInfoService injuryInfoService,IAuthService authService, IEntryService entryService, IHttpService httpService,IInjuryApplyRecordService injuryApplyRecordService, IInjuryFinancialWriteOffService injuryFinancialWriteOffService,IInjuryPaymentService injuryPaymentService,IAliSendInfo aliSendInfo,ISendMQService sendMQService,IBusinessGuaranteeService businessGuaranteeService,IBusinessInsuranceOrderService businessInsuranceOrderService,IInjuryAdvanceAccountService injuryAdvanceAccountService,IEventPublisher eventPublisher) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InjuryInfo>();
            
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            //else
            //{
            //    if(CurrentUser.CompanyID!=1)
            //    {
            //        var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            //        expable.And(t => t.PartnerID == info.PartnerID);
            //    }
                
            //}
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(it => it.CustomerName.Contains(query.CustomerName));
            }
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(it => it.UserName.Contains(query.UserName));
            }
            if (query.Status!=null)
            {
                expable.And(it => it.ProcessStatus== query.Status);
            }
            if(query.From== "lzmini" || query.From == "spmini")//蓝招小程序和第三方小程序,工伤完成和工伤撤销不可视
            {
                expable.And(it => it.ProcessStatus != 8 && it.ProcessStatus != 9);
            }
            else if (query.From == "loan")
            {
                expable.And(it => it.ProcessStatus != 9 && it.ProcessStatus != 2);
            }
            else
            {
                expable.And(it => it.ProcessStatus != 0);
            }
            if (query.UserID.HasValue)
            {
                expable.And(it => it.UserID == query.UserID);
            }
            if (user.CompanyType == CompanyType.Party.ToString().ToLower())//人力公司授权甲方公司
            {
                expable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else if (user.PlatformType != Model.Enums.PlatformType.Admin && user.CompanyType != CompanyType.Self.ToString().ToLower())//非自营人力公司只能看到自己
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var puser = await authService.UserInfo<Model.Live_Personnel>();
                expable.And(UserEntryHandle.SetPersonnelOperateAuth<Model.Live_V_InjuryInfo>(puser));
            }
            var exp = expable.ToExpression();
            var models = await injuryInfoService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InjuryInfo model)
        {
            var user = CurrentUser;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = user.CompanyGID;
                cid = user.CompanyID;
            }
            var expable = Expressionable.Create<Model.Live_InjuryInfo>();
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.InjuryID == model.InjuryID);
            if (await injuryInfoService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该标签已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            model.EditTime = DateTime.Now;
            await injuryInfoService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await injuryInfoService.Get(id);
            Live_UserEntry entry = new Live_UserEntry();
            if (model.UserEntryID.HasValue)
            {
                entry = await entryService.Get((int)model.UserEntryID);
            }
            
            var expable = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
            if (!string.IsNullOrEmpty(model.UserCardNo))
            {
                expable.And(it => it.IdCardNo == model.UserCardNo);
            }
            expable.AndIF(model.InjuryTime.HasValue, t => t.EndDate.HasValue && t.StartDate < model.InjuryTime && t.EndDate.Value.AddDays(1) > model.InjuryTime);
            var exp = expable.ToExpression();
            if (!string.IsNullOrEmpty(model.InsuranceId))
            {
                var insuranceOrder = await businessInsuranceOrderService.Get(t => t.InsuranceId == model.InsuranceId);
                if(insuranceOrder != null)
                {
                    var guarantee = await businessGuaranteeService.Get(t => t.InsuranceId == model.InsuranceId);
                    return Ok(new { model, entry, insuranceOrder, guarantee });
                }
            }
            return Ok(new { model, entry });
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryInfo model)
        {
            if (!await injuryInfoService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            
            if (model.ProcessStatus == (int)InjuryProcessStatus.待审核)
            {
                if (model.IsFracture)
                {
                    if (string.IsNullOrEmpty(model.InjuryLevel))
                    {
                        throw new ErrorCodeException(-1, "骨折情况下没有选择工伤等级");
                    }
                    var now = DateTime.Now;
                    var start = Convert.ToDateTime(now.ToString("yyyy-MM") + "-01").AddYears(-1);//计算一年员工应发工资
                    var sparas = new Dictionary<string, string>();
                    sparas.Add("idCardNo", model.UserCardNo);
                    sparas.Add("customerID", model.CustomerID.ToString());
                    sparas.Add("salaryMonth", start.ToString("yyyy-MM-dd"));
                    var sobj = await httpService.GetAsync(ServiceNames.FinanceService, "/api/SalaryRecord/InjurySalaryList", sparas);
                    try
                    {
                        model.AverageWage = Convert.ToDecimal(sobj.Data.ToString());
                    }
                    catch (Exception e)
                    {
                        return Ok(new { msg = "计算平均工资出错", obj = sobj.Data });
                    }
                    if (model.AverageWage == 0M) model.AverageWage = 5000;
                    model.ProvincialAverageWage = 8433;//省平均工资固定，一年换一次
                    if (model.InjuryLevel == "十级")
                    {
                        model.EstimatedClaim = (7 * model.AverageWage) + (4 * model.ProvincialAverageWage);
                    }
                    else if (model.InjuryLevel == "九级")
                    {
                        model.EstimatedClaim = (9 * model.AverageWage) + (8 * model.ProvincialAverageWage);
                    }
                    else
                    {
                        model.EstimatedClaim = 0M;
                    }
                }
                model.ShowTime = DateTime.Now;
                var time = model.InjuryTime.HasValue ? model.InjuryTime.Value.ToString("yyyy-MM-dd HH:mm:ss"):"未知";
                string msg = model.UserName + "已提交事故报告,公司单位：" + model.CustomerName+",工伤时间:"+ time;
				aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "工伤员工提醒", msg, "", "");
				aliSendInfo.SendAliEmail("lbyl@cn-labor.cn", "工伤员工提醒", msg, "", "");
                aliSendInfo.SendAliEmail("hjh@cn-labor.cn", "工伤员工提醒", msg, "", "");

                var item = new {
                    type = "injury-waitAuditModel",
                    customerName = model.CustomerName,
                    agentName = model.AgentName,
                    userName = model.UserName,
                    partnerName = model.PartnerName,
                    userCardNo = model.UserCardNo,
                    userType = model.UserType
                };
                var json = JsonConvert.SerializeObject(item);
                sendMQService.SendStatisticsMessage(json);


            }
            //else if(model.ProcessStatus==(int)InjuryProcessStatus.工伤理赔期)
            //{
            //    var item = new
            //    {
            //        type = "injury-lipeiModel",
            //        customerName = model.CustomerName,
            //        agentName = model.AgentName,
            //        userName = model.UserName,
            //        partnerName = model.PartnerName,
            //        userCardNo = model.UserCardNo,
            //        userType = model.UserType
            //    };
            //    var json = JsonConvert.SerializeObject(item);
            //    sendMQService.SendStatisticsMessage(json);
            //    if (model.IsPerson)
            //    {
            //        var account = await injuryAdvanceAccountService.Get(t => t.PartnerID == model.PartnerID);
            //        if (account != null)
            //        {
            //            //个人借款在流程进入工伤理赔期时候自动生成销账记录，功能与还款一样
            //            var off = await injuryFinancialWriteOffService.GetByInjuryID(model.ID);
            //            if (off == null)
            //            {
            //                off = new Model.Live_InjuryFinancialWriteOff();
            //                off.CompanyID = model.CompanyID;
            //                off.CompanyGID = model.CompanyGID;
            //                off.CreateTime = DateTime.Now;
            //                off.InjuryID = model.ID;
            //                off.UserEntryID = model.UserEntryID;
            //                off.UserName = model.UserName;
            //                off.UserCardNo = model.UserCardNo;
            //                off.Bank = model.SealCompanyBank;
            //                off.BankAccount = model.SealCompanyBankNo;
            //                off.Status = 1;
            //                await injuryFinancialWriteOffService.Add(off);
            //            }
            //        }

            //    }
            //}
            else if (model.ProcessStatus == (int)InjuryProcessStatus.工伤完成)
            {
                var item = new
                {
                    type = "injury-completeModel",
                    customerName = model.CustomerName,
                    agentName = model.AgentName,
                    userName = model.UserName,
                    partnerName = model.PartnerName,
                    userCardNo = model.UserCardNo,
                    userType = model.UserType,
                    InsuranceTotalAmount = model.InsuranceTotalAmount
                };
                var json = JsonConvert.SerializeObject(item);
                sendMQService.SendStatisticsMessage(json);
                if (!model.IsPerson)
                {

                    var record = await injuryApplyRecordService.Get(t => t.InjuryID == model.ID && t.MoneyType == (int)InjuryApplyType.还款单);
                    if (record == null)
                    {
                        record = new Model.Live_InjuryApplyRecord();
                        record.CompanyID = model.CompanyID;
                        record.CompanyGID = model.CompanyGID;
                        record.CreateTime = DateTime.Now;
                        record.InjuryID = model.ID;
                        record.Status = 1;
                        record.MoneyType = (int)InjuryApplyType.还款单;
                        record.Bank = model.SealCompanyBank;
                        record.BankAccount = model.SealCompanyBankNo;
                        record.Remark = "自动生成还款单";
                        record.CustomerID = model.CustomerID;
                        await injuryApplyRecordService.Add(record);
                    }
                }
                
            }
            model.EditTime = DateTime.Now;
            await injuryInfoService.Update(model);
            if (model.ProcessStatus == (int)InjuryProcessStatus.工伤完成)
            {
                //更新后在判断是否还有工伤
                Model.PagedModel.UserPagedModel query = new Model.PagedModel.UserPagedModel();
                query.Page = 0;
                var expable_injuryinfo = Expressionable.Create<Model.Live_V_InjuryInfo>();
                expable_injuryinfo.And(t => t.UserEntryID == model.UserEntryID);
                expable_injuryinfo.And(t => t.ProcessStatus != 9 && t.ProcessStatus != 8);
                var list = await injuryInfoService.List(expable_injuryinfo.ToExpression(), query);
                if (list.Count == 0)
                {
                    var expable = Expressionable.Create<Model.Live_UserEntry>();
                    expable.And(t => t.ID == model.UserEntryID);
                    await entryService.UpdateInjuryStatus(2, expable.ToExpression());
                }
            }





            return Ok();
        }

		[HttpPost]
        public async Task<IActionResult> InjuryDelete([FromBody] Model.Live_InjuryInfo model)
        {
            Model.PagedModel.UserPagedModel query = new Model.PagedModel.UserPagedModel();
            query.Page = 0;
            var expable = Expressionable.Create<Model.Live_UserEntry>();
            var expable_injuryinfo = Expressionable.Create<Model.Live_V_InjuryInfo>();
            if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(t => t.ID == model.UserEntryID);
                expable_injuryinfo.And(t => t.UserEntryID == model.UserEntryID);
                expable_injuryinfo.And(t => t.ProcessStatus != 9 && t.ProcessStatus != 8);
            }
            if (!await injuryInfoService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            string emailMsg = model.UserName + "已小程序取消工伤,公司单位：" + model.CustomerName;
            aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "工伤员工提醒", emailMsg, "", "");
            aliSendInfo.SendAliEmail("lbyl@cn-labor.cn", "工伤员工提醒", emailMsg, "", "");
            aliSendInfo.SendAliEmail("hjh@cn-labor.cn", "工伤员工提醒", emailMsg, "", "");
            await injuryInfoService.UpdateStatus(model.ID,9, "小程序取消标记");
            var list = await injuryInfoService.List(expable_injuryinfo.ToExpression(), query);
            if(list.Count==0)
            {
                await entryService.UpdateInjuryStatus(0, expable.ToExpression());
            }
            

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMoney([FromForm] int id, [FromForm] int type, [FromForm] decimal money, [FromForm] string edit)
        {

            var info = await injuryInfoService.Get(id);
            var objs = new Dictionary<string, object>();
            objs.Add("ID", info.ID);
            if (type == 1)
            {
                //发票
                var m = edit == "add" ? (info.ReturnBackMoneyTotalAmount + money) : (info.ReturnBackMoneyTotalAmount - money);
                objs.Add("ReturnBackMoneyTotalAmount", m);
            }
            var count = await injuryInfoService.DictionarySet(objs);
            if (count == 0)
            {
                throw new ErrorCodeException(-1, "更新失败");
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int status, [FromForm] string msg, [FromForm] string edit)
        {
            if (!await injuryInfoService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            if (edit == "back")
            {
                await injuryInfoService.UpdateStatus(id, status, msg);
            } 
            else
            {
                if (status == (int)InjuryProcessStatus.工伤理赔期)
                {
                    var model = await injuryInfoService.Get(id);
                    var item = new
                    {
                        type = "injury-lipeiModel",
                        customerName = model.CustomerName,
                        agentName = model.AgentName,
                        userName = model.UserName,
                        partnerName = model.PartnerName,
                        userCardNo = model.UserCardNo,
                        userType = model.UserType
                    };
                    var json = JsonConvert.SerializeObject(item);
                    sendMQService.SendStatisticsMessage(json);
                    if (model.IsPerson && model.LoanTotalAmount > 0)
                    {
                        var account = await injuryAdvanceAccountService.Get(t => t.PartnerID == model.PartnerID);
                        if (account != null)
                        {
                            //个人借款在流程进入工伤理赔期时候自动生成销账记录，功能与还款一样
                            var off = await injuryFinancialWriteOffService.GetByInjuryID(model.ID);
                            if (off == null)
                            {
                                off = new Model.Live_InjuryFinancialWriteOff();
                                off.CompanyID = model.CompanyID;
                                off.CompanyGID = model.CompanyGID;
                                off.CreateTime = DateTime.Now;
                                off.InjuryID = model.ID;
                                off.UserEntryID = model.UserEntryID;
                                off.UserName = model.UserName;
                                off.UserCardNo = model.UserCardNo;
                                off.Bank = model.SealCompanyBank;
                                off.BankAccount = model.SealCompanyBankNo;
                                off.Status = 1;
                                await injuryFinancialWriteOffService.Add(off);
                            }
                        }

                    }
                    await injuryInfoService.UpdateStatus(id, status, msg);
                }
                else if (status == (int)InjuryProcessStatus.工伤完成)
                {
                    var model = await injuryInfoService.Get(id);
                    var item = new
                    {
                        type = "injury-completeModel",
                        customerName = model.CustomerName,
                        agentName = model.AgentName,
                        userName = model.UserName,
                        partnerName = model.PartnerName,
                        userCardNo = model.UserCardNo,
                        userType = model.UserType,
                        InsuranceTotalAmount = model.InsuranceTotalAmount
                    };
                    var json = JsonConvert.SerializeObject(item);
                    sendMQService.SendStatisticsMessage(json);
                    //自动生成还款单
                    if (!model.IsPerson && model.LoanTotalAmount > 0)
                    {
                        var record = await injuryApplyRecordService.Get(t => t.InjuryID == model.ID && t.MoneyType == (int)InjuryApplyType.还款单 && t.Status != 3);
                        if (record == null)
                        {
                            record = new Model.Live_InjuryApplyRecord();
                            record.CompanyID = model.CompanyID;
                            record.CompanyGID = model.CompanyGID;
                            record.CreateTime = DateTime.Now;
                            record.InjuryID = model.ID;
                            record.Status = 1;
                            record.MoneyType = (int)InjuryApplyType.还款单;
                            record.Bank = model.SealCompanyBank;
                            record.BankAccount = model.SealCompanyBankNo;
                            record.Remark = "自动生成还款单";
                            record.CustomerID = model.CustomerID;
                            await injuryApplyRecordService.Add(record);
                        }
                    }
                    await injuryInfoService.UpdateStatus(id, status, msg);
                }
                else if (status == (int)InjuryProcessStatus.仲裁中)
                {
                    var model = await injuryInfoService.Get(id);
                    model.ProcessStatus = status;
                    model.IsArbitration = true;
                    await injuryInfoService.Update(model);
                }
                else if (status == (int)InjuryProcessStatus.工伤撤回)
                {
                    var model = await injuryInfoService.Get(id);
                    string emailMsg = model.UserName + "已工伤撤案,公司单位：" + model.CustomerName;
                    //aliSendInfo.SendAliEmail("yyy@cn-labor.cn", "工伤员工提醒", emailMsg, "", "");
                    //aliSendInfo.SendAliEmail("lbyl@cn-labor.cn", "工伤员工提醒", emailMsg, "", "");
                    
                    await injuryInfoService.UpdateStatus(id, status, msg);
                }
                else if (status == (int)InjuryProcessStatus.审核失败)
                {
                    var model = await injuryInfoService.Get(id);
                    NotificationEventData eventData = new NotificationEventData()
                    {
                        CompanyID = model.CompanyID,
                        CompanyGID = model.CompanyGID,
                        PartnerID = model.PartnerID,
                        PartnerGID = model.PartnerGID,
                        Content = msg,
                        Status = 1,
                        Type = (int)NotificationTypeEnum.工伤通知,
                        TypeName = NotificationTypeEnum.工伤通知.ToString(),
                        Title = "工伤审核退回",
                        CustomerID = model.CustomerID,
                        CustomerName = model.CustomerName,
                    };
                    eventPublisher.Publish(eventData);
                    await injuryInfoService.UpdateStatus(id, status, msg);
                    return Ok(new {model = eventData });
                }
                else
                {
                    await injuryInfoService.UpdateStatus(id, status, msg);
                }
                if (status == 8 || status == 9)
                {
                    var model = await injuryInfoService.Get(id);
                    Model.PagedModel.UserPagedModel query = new Model.PagedModel.UserPagedModel();
                    query.Page = 0;
                    var expable_injuryinfo = Expressionable.Create<Model.Live_V_InjuryInfo>();
                    expable_injuryinfo.And(t => t.UserEntryID == model.UserEntryID);
                    expable_injuryinfo.And(t => t.ProcessStatus != 9 && t.ProcessStatus != 8);
                    var list = await injuryInfoService.List(expable_injuryinfo.ToExpression(), query);
                    if (list.Count == 0)
                    {
                        int entryStatus = status == 8 ? 2 : 0;
                        var expable = Expressionable.Create<Model.Live_UserEntry>();
                        expable.And(t => t.ID == model.UserEntryID);
                        await entryService.UpdateInjuryStatus(entryStatus, expable.ToExpression());
                    }

                }
            }
            
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryInfoService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var exp_ApplyRecord = Expressionable.Create<Model.Live_InjuryApplyRecord>();
            exp_ApplyRecord.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_ApplyRecord.And(t => t.InjuryID == id);
            await injuryApplyRecordService.Delete(exp_ApplyRecord.ToExpression());

            var exp_FinancialWriteOff = Expressionable.Create<Model.Live_InjuryFinancialWriteOff>();
            exp_FinancialWriteOff.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_FinancialWriteOff.And(t => t.InjuryID == id);
            await injuryFinancialWriteOffService.Delete(exp_FinancialWriteOff.ToExpression());

            var exp_Payment = Expressionable.Create<Model.Live_InjuryPayment>();
            exp_Payment.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp_Payment.And(t => t.InjuryID == id);
            await injuryPaymentService.Delete(exp_Payment.ToExpression());

            await injuryInfoService.Delete(id);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Test(int id)
        {
            var model = await injuryInfoService.Test(f => f.ID == id);
            model.cal();
            return Ok(new { items = model });
        }


        [HttpGet]
        public async Task<IActionResult> ApplyCompensationExportExcel(int id)
        {
            if (!await injuryInfoService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var expable = Expressionable.Create<Model.Live_InjuryInfo>();
            expable.And(t => t.ID == id);
            var model = await injuryInfoService.View(expable.ToExpression());
            var result = injuryInfoService.ApplyCompensationExportExcel(model);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "工伤结算表.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpGet]
        public async Task<IActionResult> StatisticsExportExcel([FromQuery] Model.PagedModel.UserPagedModel query)
        {

            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_InjuryInfo>();
            
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(t => t.CustomerName.Contains(query.CustomerName));
            }
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(t => t.UserName.Contains(query.UserName));
            }
            if (query.Status != null)
            {
                expable.And(t => t.ProcessStatus == query.Status);
            }
            expable.And(t => t.ProcessStatus != 0 && t.ProcessStatus != 9);
            var list = await injuryInfoService.List(expable.ToExpression(),query);
            var result = await injuryInfoService.StatisticsExportExcel(list);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "工伤统计表.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
    }
}
