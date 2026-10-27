using LZ.Clients.API;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Enums.Budget;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs.MerChant;
using Senparc.Weixin.WxOpen.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BudgetApplyRecordController(IBudgetApplyRecordService budgetApplyRecordService,IBudgetManageService budgetManageService,IBudgetRechargeLogService budgetRechargeLogService, IBudgetApplyRecordDetailService budgetApplyRecordDetailService, IFileUploadService fileUploadService,IAuthService authService,IHttpService httpService,ISalaryCustomerService salaryCustomerService) : FinanceControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query,string payer)
        {
            var exp = Expressionable.Create<Model.Live_BudgetApplyRecord>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
                if(CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin && query.From =="limit")
                {
                    var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                    exp.And(t => t.UserGID == info.UserGID);
                }
                
            }
            if(!string.IsNullOrEmpty(query.Types))  
            {
                var arr = query.Types.Split(",");
                exp.And(t => arr.Contains(t.ProcessStatus.ToString()));
            }
            exp.AndIF(!string.IsNullOrEmpty(query.Name), t => t.ExpensesTitle.Contains(query.Name));
            exp.AndIF(!string.IsNullOrEmpty(payer), t => t.Payer.Contains(payer));
            exp.AndIF(!string.IsNullOrEmpty(query.UserName), t => t.UserName.Contains(query.UserName));
            exp.AndIF(!string.IsNullOrEmpty(query.PartnerName), t => t.PartnerName.Contains(query.PartnerName));
            exp.AndIF(!string.IsNullOrEmpty(query.Description), t => t.Description.Contains(query.Description));
            exp.AndIF(query.Money.HasValue, t => t.ApplyAmount == query.Money);
            exp.AndIF(query.StartTime.HasValue, t => t.CreateTime >= query.StartTime);
            exp.AndIF(query.EndTime.HasValue, t => t.CreateTime <= query.EndTime);
            exp.AndIF(query.ProcessStatus.HasValue, t => t.ProcessStatus==query.ProcessStatus);
            exp.AndIF(query.Year.HasValue, t => t.Year == query.Year);
            var models = await budgetApplyRecordService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Model.Live_BudgetApplyRecord model, [FromForm] List<Model.Live_BudgetApplyRecordDetail> details, [FromForm] List<Model.Live_FileUpload> files)
        {
            if(CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var user = await authService.UserInfo<Model.SYS_CompanyUser>();
                model.UserGID = user.UserGID;
            }
            

            var manage = await budgetManageService.GetView(model.BudgetManageID.Value);
            decimal applyMony = model.ApplyAmount;
            decimal availableAmount = manage.AvailableAmount - applyMony;
            if ((manage.AvailableAmount <= 0 || availableAmount < 0) && !manage.DirectlyPass)
            {
                //防止申请金额时，已被别人借款导致金额不够
                throw new ErrorCodeException(-1, "剩余金额不够,申请失败");
            }
            Model.Live_BudgetRechargeLog log = new Model.Live_BudgetRechargeLog();
            log.CompanyID = manage.CompanyID;
            log.CompanyGID = manage.CompanyGID;
            log.CID = manage.ID;
            log.CreateTime = DateTime.Now;
            log.UserName = model.UserName;
            log.UserGID = model.UserGID;
            log.Money = applyMony;
            log.PartnerName = manage.PartnerName;
            log.CostName = ((CostType)manage.CostType).ToString();
            log.Title = model.ExpensesTitle;
            log.GenerateName = ((GenerationType)manage.GenerateType).ToString();
            log.Type = (int)BudgetManageRecordActionEnum.客服申请扣款;
            if(!manage.DirectlyPass)
            {
                log.TotalAmount = model.TotalAmount;
                log.AvailableAmount = availableAmount;
                log.LockAmount = manage.LockAmount + applyMony;
                log.UsedAmount = manage.UsedAmount;
                log.HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount;
            }
           
            log.Year = manage.Year;
            var logID = await budgetRechargeLogService.Add(log);






            model.RechargeLogID = logID;
            model.CreateTime = DateTime.Now;
            model.CompanyID = manage.CompanyID;
            model.CompanyGID = manage.CompanyGID;
            model.ProcessStatus = (int)BudgetApplyRecordProcessStatus.待审核;
            var recordID = await budgetApplyRecordService.Add(model);
            var objs = new Dictionary<string, object>();
            decimal lockmoney = manage.LockAmount;
            if (!manage.DirectlyPass)
            {
                lockmoney = manage.LockAmount + applyMony;//申请的金额添加到锁定金额中，等待财务审核
                objs.Add("ID", manage.ID);
                objs.Add("LockAmount", lockmoney);
                objs.Add("AvailableAmount", availableAmount);
                var count = await budgetManageService.DictionarySet(objs);
            }


            
            foreach (var detail in details)
            {
                detail.CompanyID = manage.CompanyID;
                detail.BudgetApplyRecordID = recordID;
                detail.CreateTime = DateTime.Now;
            }
            await budgetApplyRecordDetailService.Add(details);
            foreach(var file in files)
            {
                file.CompanyID = manage.CompanyID;
                file.CompanyGID = manage.CompanyGID;
                file.UserID = CurrentUser.UserID;
                file.FID = recordID;
                file.CreateTime = DateTime.Now;
            }
            await fileUploadService.Add(files);

            #region 总部申请添加记录 并更新manage的总部申请金额
            #endregion



            return Ok( new { items=model,list = details, files });
        }

        

        [HttpGet]
        public async Task<IActionResult> GetPartner()
        {
            if (CurrentUser.PlatformType != Model.Enums.PlatformType.MiniAdmin)
            {
                throw new ErrorCodeException(-1, "无法获取账号服务商");
            }

            var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            if (!info.PartnerID.HasValue)
            {
                throw new ErrorCodeException(-1, "该账号没有分配服务商");
            }

            var insuracneDic = new Dictionary<string, string>();
            insuracneDic.Add("id", info.PartnerID.Value.ToString());
            var result = await httpService.GetAsync(ServiceNames.BasicService, "/api/Partner/get", insuracneDic);
            Model.Live_Partner partner = new Live_Partner();
            if (!string.IsNullOrEmpty(result.Data.ToString()))
            {
                partner = JsonConvert.DeserializeObject<Model.Live_Partner>(result.Data.ToString());
            }
            if (partner == null)
            {
                throw new ErrorCodeException(-1, "服务商获取失败");
            }
            return Ok(new {items = partner });
        }


        [HttpPost]
        public async Task<IActionResult> Auditing([FromForm] int id, [FromForm] int method, [FromForm] int status)
        {
            var model = await budgetApplyRecordService.Get(id);
            //防止申请者已经作废
            if(model.ProcessStatus== (int)BudgetApplyRecordProcessStatus.作废 || model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.审核退回)
            {
                throw new ErrorCodeException(-1, "该预算已经作废或者审核退回，请刷新页面");
            }
            var manage = await budgetManageService.GetView(model.BudgetManageID.Value);
            var details = await budgetApplyRecordDetailService.GetByApply(id);
            FinancePageModel query = new FinancePageModel();
            query.Page = 0;
            var exp = Expressionable.Create<Model.Live_BudgetManage>();

            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            exp.And(t => t.ExpensesTitle == model.ExpensesTitle);
            exp.And(t => t.CostType == model.CostType);
            exp.And(t => t.Year == model.Year);
            var manages = await budgetManageService.ListModel(exp.ToExpression(), query);
            List<Model.Live_BudgetManage> list = new List<Model.Live_BudgetManage>();
            List<Model.Live_BudgetRechargeLog> logs = new List<Model.Live_BudgetRechargeLog>();
            foreach (var detail in details)
            {
                if(detail.PartnerGID==manage.PartnerGID) continue;
                Model.Live_BudgetRechargeLog otherLog = new Model.Live_BudgetRechargeLog();
                otherLog.CompanyID = model.CompanyID;
                otherLog.CompanyGID = model.CompanyGID;
                otherLog.UserName = model.UserName;
                otherLog.UserGID = model.UserGID;
                otherLog.CreateTime = DateTime.Now;
                otherLog.Money = detail.Money;
                otherLog.PartnerName = detail.PartnerName;
                otherLog.CostName = ((CostType)manage.CostType).ToString();
                otherLog.Title = model.ExpensesTitle;
                otherLog.GenerateName = ((GenerationType)manage.GenerateType).ToString();
                otherLog.OtherCostName = otherLog.CostName;
                otherLog.OtherGenerateName = otherLog.GenerateName;
                otherLog.OtherPartnerName = manage.PartnerName;
                otherLog.OtherTitle = model.ExpensesTitle;
                otherLog.Year = model.Year;
                Model.Live_BudgetManage bm = new Model.Live_BudgetManage();
                var data = list.Find(t => t.PartnerGID == detail.PartnerGID);
                if (data != null)
                {
                    bm = data;
                    list.Remove(data);
                }
                else
                {
                    var other = manages.Find(t => t.PartnerGID == detail.PartnerGID);
                    if (other != null)
                    {
                        bm = other;
                    }
                    else
                    {
                        continue;
                    }
                }
                otherLog.CID = bm.ID;
                //othermoney += detail.Money;
                
                if (method==(int)BudgetAutdingMethod.打款成功)
                {

                    otherLog.Type = (int)BudgetManageRecordActionEnum.总部申请已打款;
                    bm.HeadOfficeUsedAmount = bm.HeadOfficeUsedAmount + detail.Money;
                }
                else if (method == (int)BudgetAutdingMethod.打款后回退)
                {
                    otherLog.Type = (int)BudgetManageRecordActionEnum.总部回滚;
                    bm.HeadOfficeUsedAmount = bm.HeadOfficeUsedAmount - detail.Money;
                } 
                else if (method == (int)BudgetAutdingMethod.打款后作废)
                {
                    otherLog.Type = (int)BudgetManageRecordActionEnum.总部回滚;
                    bm.HeadOfficeUsedAmount = bm.HeadOfficeUsedAmount - detail.Money;
                }
                
               if(!bm.DirectlyPass)
                {
                    otherLog.LockAmount = bm.LockAmount;
                    otherLog.TotalAmount = bm.TotalAmount;
                    otherLog.AvailableAmount = bm.AvailableAmount;
                    otherLog.UsedAmount = bm.UsedAmount;
                    list.Add(bm);
                } 
                else
                {
                    otherLog.HeadOfficeUsedAmount = 0;
                }
                logs.Add(otherLog);
            }

            Model.Live_BudgetRechargeLog log = new Model.Live_BudgetRechargeLog();
            log.CompanyID = model.CompanyID;
            log.CompanyGID = model.CompanyGID;
            log.UserName = model.UserName;
            log.UserGID = model.UserGID;
            log.CreateTime = DateTime.Now;
            log.Money = model.ApplyAmount;
            log.PartnerName = manage.PartnerName;
            log.CostName = ((CostType)manage.CostType).ToString();
            log.Title = model.ExpensesTitle;
            log.GenerateName = ((GenerationType)manage.GenerateType).ToString();
            log.CID = manage.ID;
            log.TotalAmount = manage.TotalAmount;
            log.AvailableAmount = manage.AvailableAmount; 
            log.HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount;
            log.Year = manage.Year;
            var objs = new Dictionary<string, object>();
            objs.Add("ID", manage.ID);
            if (method == (int)BudgetAutdingMethod.打款成功)
            {

                log.Type = (int)BudgetManageRecordActionEnum.已打款;
                log.LockAmount = manage.LockAmount - model.ApplyAmount;
                log.UsedAmount = manage.UsedAmount + model.ApplyAmount;
                log.DecreaseAmount = model.ApplyAmount;
                objs.Add("LockAmount", manage.LockAmount - model.ApplyAmount);
                objs.Add("UsedAmount", manage.UsedAmount + model.ApplyAmount);
            }
            else if (method == (int)BudgetAutdingMethod.打款后回退)
            {
                log.Type = (int)BudgetManageRecordActionEnum.回滚打款前;
                log.LockAmount = manage.LockAmount + model.ApplyAmount;
                log.UsedAmount = manage.UsedAmount - model.ApplyAmount;
                log.DecreaseAmount = -model.ApplyAmount;
                objs.Add("LockAmount", manage.LockAmount + model.ApplyAmount);
                objs.Add("UsedAmount", manage.UsedAmount - model.ApplyAmount);
            }
            else if (method == (int)BudgetAutdingMethod.打款后作废)
            {
                log.Type = (int)BudgetManageRecordActionEnum.回滚作废;
                log.LockAmount = manage.LockAmount;
                log.AvailableAmount = manage.AvailableAmount + model.ApplyAmount;
                log.UsedAmount = manage.UsedAmount - model.ApplyAmount;
                log.DecreaseAmount = -model.ApplyAmount;
                objs.Add("AvailableAmount", manage.AvailableAmount + model.ApplyAmount);
                objs.Add("UsedAmount", manage.UsedAmount - model.ApplyAmount);
            }

            if(manage.DirectlyPass)
            {
                log.TotalAmount = 0;
                log.AvailableAmount = 0;
                log.LockAmount = 0;
                log.HeadOfficeUsedAmount = 0;
                log.AvailableAmount = 0;
                log.UsedAmount = 0;
                log.AddAmount = 0;
                log.DecreaseAmount = 0;
            }


            logs.Add(log);

            await budgetRechargeLogService.Add(logs);
            await budgetManageService.Update(list);

            if(!manage.DirectlyPass)
            {
                await budgetManageService.DictionarySet(objs);
            }
            

            var objs1 = new Dictionary<string, object>();
            objs1.Add("ID", model.ID);
            objs1.Add("ProcessStatus", status);
            if (method == (int)BudgetAutdingMethod.打款成功)
            {
                objs1.Add("PayTime", DateTime.Now);
            }
           
            await budgetApplyRecordService.DictionarySet(objs1);


            return Ok();
        }

        

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await budgetApplyRecordService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var details = await budgetApplyRecordDetailService.GetByApply(id);
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            var exp = Expressionable.Create<Model.Live_FileUpload>();
            //if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            //{
            //    exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            //}
            //else
            //{
            //    exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            //}
            exp.And(t => t.FID == id);
            exp.And(t => t.TypeKey == 10);
            var files = await fileUploadService.List(exp.ToExpression(), query);
            return Ok(new {model, details,files });
        }
        [HttpGet]
        public async Task<Model.Live_BudgetApplyRecord> GetSimple(int id)
        {
            var model = await budgetApplyRecordService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BudgetApplyRecord model)
        {
            await budgetApplyRecordService.Update(model);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> SalaryCustomerAutoComplete([FromForm] Guid? gid, [FromForm] Guid? userGID, [FromForm] string userName, [FromForm] int companyID, [FromForm] Guid companyGID)
        {
            
            if (!gid.HasValue)
            {
                throw new ErrorCodeException(-1, "id没有传值");
            }
            var salarycustomer = await salaryCustomerService.GetView((Guid)gid);
            var item = await salaryCustomerService.GetTransNav(t=>t.ID==(Guid)gid);
            if(item == null)
            {
                throw new ErrorCodeException(-1, "数据库没有匹配的工资数据");
            }
            var record = await budgetApplyRecordService.Get(t => t.SalaryGID == gid && t.ProcessStatus != (int)BudgetApplyRecordProcessStatus.审核退回 && t.ProcessStatus != (int)BudgetApplyRecordProcessStatus.作废);
            if(record !=null)
            {
                throw new ErrorCodeException(-1, "已经创建过预算申请，id："+ record.ID);
            }
            int year = DateTime.Now.AddMonths(-3).Year;
            var manage = await budgetManageService.GetV(it => it.ExpensesTitle == "内部员工应发工资" && it.PartnerGID== salarycustomer.PartnerGID && it.Year == year);
            decimal applyMony = item.Records.Sum(t => t.UnitSalaryAmount - t.UnitDeductionAmount + t.SubsidyAmount);
            decimal availableAmount = manage.AvailableAmount - applyMony;
            if ((manage.AvailableAmount <= 0 || availableAmount < 0) && !manage.DirectlyPass)
            {
                //防止申请金额时，已被别人借款导致金额不够
                throw new ErrorCodeException(-1, "剩余金额不够,申请失败");
            }
            Model.Live_BudgetApplyRecord model = new Model.Live_BudgetApplyRecord();
            model.CompanyID = companyID;
            model.CompanyGID = companyGID;
            model.Year = year;
            model.ExpensesTitle = manage.ExpensesTitle;
            model.CostType = manage.CostType;
            model.PartnerID = salarycustomer.PartnerID;
            model.PartnerName = salarycustomer.PartnerName;
            model.PartnerGID = salarycustomer.PartnerGID;
            model.UserGID = userGID;
            model.SalaryGID = gid;
            model.UserName = userName;
            model.TotalAmount = manage.TotalAmount;
            model.ApplyAmount = applyMony;
            model.AvailableAmount = availableAmount;
            model.PayerID = salarycustomer.SigningUnitID;
            model.PayType = "银行卡";
            model.Payer = salarycustomer.SigningUnitCompanyName;
            model.ProcessStatus = (int)BudgetApplyRecordProcessStatus.待审核;
            model.BudgetManageID = manage.ID;


            Model.Live_BudgetRechargeLog log = new Model.Live_BudgetRechargeLog();
            log.CompanyID = manage.CompanyID;
            log.CompanyGID = manage.CompanyGID;
            log.CID = manage.ID;
            log.CreateTime = DateTime.Now;
            log.UserName = model.UserName;
            log.UserGID = model.UserGID;
            log.Money = applyMony;
            log.PartnerName = manage.PartnerName;
            log.CostName = ((CostType)manage.CostType).ToString();
            log.Title = model.ExpensesTitle;
            log.GenerateName = ((GenerationType)manage.GenerateType).ToString();
            log.Content = "客户工资自动创建";
            log.Type = (int)BudgetManageRecordActionEnum.客服申请扣款;

            if (!manage.DirectlyPass)
            {
                log.TotalAmount = model.TotalAmount;
                log.AvailableAmount = availableAmount;
                log.LockAmount = manage.LockAmount + applyMony;
                log.UsedAmount = manage.UsedAmount;
                log.HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount;
            }

            log.Year = manage.Year;
            var logID = await budgetRechargeLogService.Add(log);






            model.RechargeLogID = logID;
            model.CreateTime = DateTime.Now;
            var recordID = await budgetApplyRecordService.Add(model);
            var objs = new Dictionary<string, object>();
            decimal lockmoney = manage.LockAmount;
            if (!manage.DirectlyPass)
            {
                lockmoney = manage.LockAmount + applyMony;//申请的金额添加到锁定金额中，等待财务审核
                objs.Add("ID", manage.ID);
                objs.Add("LockAmount", lockmoney);
                objs.Add("AvailableAmount", availableAmount);
                var count = await budgetManageService.DictionarySet(objs);
            }

            Live_BudgetApplyRecordDetail detail = new Live_BudgetApplyRecordDetail();
            detail.BudgetApplyRecordID = recordID;
            detail.CompanyID = manage.CompanyID;
            detail.Money = applyMony;
            detail.PartnerGID = model.PartnerGID;
            detail.PartnerName = model.PartnerName;
            detail.CreateTime = DateTime.Now;


            await budgetApplyRecordDetailService.Add(detail);
            return Ok(recordID);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromForm] int id, [FromForm] int status, [FromForm] string mssage)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("ID", id);
            dic.Add("ProcessStatus", status);
            if (!string.IsNullOrEmpty(mssage))
            {
                dic.Add("Mssage", mssage);
            }
            var model = await budgetApplyRecordService.Get(id);
            if (status == (int)BudgetApplyRecordProcessStatus.作废 || status == (int)BudgetApplyRecordProcessStatus.审核退回)
            {
                
                if (model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.待打款 || model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.已打款)
                {
                    throw new ErrorCodeException(-1, "财务已审核通过或者已打款,请联系财务");
                }
                else if (model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.作废 || model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.审核退回)
                {
                    throw new ErrorCodeException(-1, "该预算已作废或者审核退回,请刷新页面");
                }
                var manage = await budgetManageService.GetView(model.BudgetManageID.Value);
                var applymoney = model.ApplyAmount;
                if (model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.待审核 || model.ProcessStatus == (int)BudgetApplyRecordProcessStatus.待打款)
                {
                    Model.Live_BudgetRechargeLog otherLog = new Model.Live_BudgetRechargeLog();
                    otherLog.CompanyID = model.CompanyID;
                    otherLog.CompanyGID = model.CompanyGID;
                    otherLog.CID = manage.ID;
                    otherLog.UserName = model.UserName;
                    otherLog.UserGID = model.UserGID;
                    otherLog.CreateTime = DateTime.Now;
                    otherLog.Money = model.ApplyAmount;
                    otherLog.PartnerName = model.PartnerName;
                    otherLog.CostName = ((CostType)manage.CostType).ToString();
                    otherLog.Title = model.ExpensesTitle;
                    otherLog.GenerateName = ((GenerationType)manage.GenerateType).ToString();
                    otherLog.Type = (int)BudgetManageRecordActionEnum.作废;
                    otherLog.TotalAmount = manage.TotalAmount;
                    otherLog.AvailableAmount = manage.AvailableAmount + applymoney;
                    otherLog.LockAmount = manage.LockAmount - applymoney;
                    otherLog.UsedAmount = manage.UsedAmount;
                    otherLog.Year = manage.Year;
                    otherLog.HeadOfficeUsedAmount = manage.HeadOfficeUsedAmount;
                    await budgetRechargeLogService.Add(otherLog);
                    if (!manage.DirectlyPass)
                    {
                        var objs = new Dictionary<string, object>();
                        objs.Add("ID", manage.ID);
                        objs.Add("LockAmount", manage.LockAmount - applymoney);
                        objs.Add("AvailableAmount", manage.AvailableAmount + applymoney);
                        await budgetManageService.DictionarySet(objs);
                    }
                } 
               
            }
            else if (status == (int)BudgetApplyRecordProcessStatus.待打款)
            {
                if(model.ProcessStatus != (int)BudgetApplyRecordProcessStatus.待审核)
                {
                    string msg = string.Format("该预算当前状态是{0},请刷新页面", (BudgetApplyRecordProcessStatus)status);
                    throw new ErrorCodeException(-1, msg);
                }
            }
            await budgetApplyRecordService.DictionarySet(dic);
            return Ok();
        } 
        [HttpPost]
        public async Task<IActionResult> ImportBudgetDetailExcel([FromForm]int id)
        {
            
            
            List<Live_BudgetApplyRecordDetail> list = new List<Live_BudgetApplyRecordDetail>();
            try
            {
                var manage = await budgetManageService.GetView(id);
                
                FinancePageModel query = new FinancePageModel();
                query.Page = 0;
                var exp = Expressionable.Create<Model.Live_V_BudgetManage>();

                if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
                {
                    exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
                }
                else
                {
                    exp.And(it => it.CompanyID == CurrentUser.CompanyID);
                }
                exp.And(t => t.ExpensesTitle == manage.ExpensesTitle);
                exp.And(t => t.CostType == manage.CostType);
                var manages = await budgetManageService.List(exp.ToExpression(), query);
                
                var files = Request.Form.Files;
                string fn = string.Empty;
                var filePath = "/";
                foreach (var file in files)
                {
                    fn = await Common.UploadFile.UpFile(file, filePath, true);
                    if (fn.Contains("error"))
                    {
                        throw new ErrorCodeException(-1, "文件上传错误");
                    }
                    if (!string.IsNullOrEmpty(fn))
                    {
                        
                        var ds = Common.ExcelHeler.ReadExcelToDataSet(fn, "", 1);//从第几行开始获取数据，第一行为0
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                
                                Live_BudgetApplyRecordDetail detail = new Live_BudgetApplyRecordDetail();
                                if(row[0] != null)
                                {
                                    var item = manages.Where(t => t.PartnerName == row[0].ToString()).FirstOrDefault();
                                    if (item != null)
                                    {
                                        detail.PartnerGID = item.PartnerGID;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                                detail.PartnerName = row[0] != null ? row[0].ToString() : "";
                                detail.Money = row[1] != null ? Convert.ToDecimal(row[1].ToString()) : 0;
                                detail.Remark = row[2] != null ? row[2].ToString() : "";
                                list.Add(detail);
                            }
                        }

                    }
                }

            }
            catch(Exception e)
            {
                throw new ErrorCodeException(-1, e.ToString());
            }



           
            return Ok(new {items = list,id });
        }

        [HttpGet]
        public IActionResult CreateDefaultExcel()
        {
            var result = budgetApplyRecordService.CreateDefaultExcel();
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "预算总部拆分模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        //[HttpGet]
        //public async Task<IActionResult> TiaoZheng()
        //{
        //    //根据申请同步锁定，可用金额，和已申请金额
        //    FinancePageModel query = new FinancePageModel();
        //    query.Page = 0;
        //    query.Order = "partner";
        //    var exp = Expressionable.Create<Model.Live_BudgetManage>();
        //    exp.And(t => t.CompanyID == 1 && !t.DirectlyPass);
        //    var manages = await budgetManageService.ListModel(exp.ToExpression(), query);

        //    var exp_record = Expressionable.Create<Model.Live_BudgetApplyRecord>();
        //    exp_record.And(t => t.CompanyID == 1);
        //    var records = await budgetApplyRecordService.List(exp_record.ToExpression(), query);

        //    List<Live_BudgetManage> manage_list = new List<Live_BudgetManage>();


        //    foreach (var x in manages)
        //    {
        //        if (x.TotalAmount > 0)
        //        {
        //            x.LockAmount = records.Where(t => t.PartnerGID == x.PartnerGID && t.CostType == x.CostType && t.ExpensesTitle == x.ExpensesTitle && t.Year == x.Year && (t.ProcessStatus == 2 || t.ProcessStatus == 3)).Sum(t => t.ApplyAmount);
        //            x.UsedAmount = records.Where(t => t.PartnerGID == x.PartnerGID && t.CostType == x.CostType && t.ExpensesTitle == x.ExpensesTitle && t.Year == x.Year && (t.ProcessStatus == 4)).Sum(t => t.ApplyAmount);
        //            manage_list.Add(x);
        //            x.AvailableAmount = x.TotalAmount - x.LockAmount - x.UsedAmount;
        //        }

        //    }
        //    await budgetManageService.Update(manage_list);





        //    return Ok(new { items = manages, list = manage_list });
        //}

        //[HttpGet]
        //public async Task<IActionResult> TiaoZheng()
        //{
        //    //同步历史记录跟最新的所有金额
        //    FinancePageModel query = new FinancePageModel();
        //    query.Page = 0;
        //    query.Order = "partner";
        //    var exp = Expressionable.Create<Model.Live_V_BudgetManage>();
        //    exp.And(t => t.CompanyID == 1);
        //    var manages = await budgetManageService.List(exp.ToExpression(), query);

        //    var exp_log = Expressionable.Create<Model.Live_BudgetRechargeLog>();
        //    exp_log.And(t => t.CompanyID == 1);
        //    var logs = await budgetRechargeLogService.List(exp_log.ToExpression(), query);

        //    List<Live_BudgetRechargeLog> manage_list = new List<Live_BudgetRechargeLog>();
        //    List<Live_BudgetRechargeLog> kong = new List<Live_BudgetRechargeLog>();

        //    foreach (var x in logs)
        //    {
        //        //var mm = manages.Find(t => t.ID == x.CID);
        //        if(x.Type!=7 ||x.Type!=11 || x.Type != 11 || x.Type==5)
        //        {
        //            int costtype = x.CostName == "直接成本" ? 1 : 2;
        //            var mm = manages.Find(t => t.PartnerName == x.PartnerName && t.CostType == costtype && t.ExpensesTitle == x.Title);
        //            if(mm==null)
        //            {
        //                kong.Add(x);
        //                continue;
        //            }

        //            x.TotalAmount = mm.TotalAmount;
        //            x.LockAmount = mm.LockAmount;
        //            x.AvailableAmount = mm.AvailableAmount;
        //            x.UsedAmount = mm.UsedAmount;
        //            manage_list.Add(x);
        //        }


        //    }
        //    await budgetRechargeLogService.Update(manage_list);





        //    return Ok(new { list = manage_list,kong });
        //}


        [HttpGet]
        public async Task<IActionResult> TiaoZheng()
        {
            //根据
            FinancePageModel query = new FinancePageModel();
            query.Page = 0;
            query.Order = "partner";
            var exp = Expressionable.Create<Model.Live_BudgetManage>();
            exp.And(t => t.CompanyID == 1 && !t.DirectlyPass);
            var manages = await budgetManageService.ListModel(exp.ToExpression(), query);

            var exp_record = Expressionable.Create<Model.Live_BudgetApplyRecord>();
            exp_record.And(t => t.CompanyID == 1);
            var records = await budgetApplyRecordService.List(exp_record.ToExpression(), query);

            List<Live_BudgetManage> manage_list = new List<Live_BudgetManage>();

            foreach (var x in manages)
            {
                if (x.TotalAmount > 0)
                {
                    x.LockAmount = records.Where(t => t.PartnerGID == x.PartnerGID && t.CostType == x.CostType && t.ExpensesTitle == x.ExpensesTitle && t.Year == x.Year && (t.ProcessStatus == 2 || t.ProcessStatus == 3)).Sum(t => t.ApplyAmount);
                    x.UsedAmount = records.Where(t => t.PartnerGID == x.PartnerGID && t.CostType == x.CostType && t.ExpensesTitle == x.ExpensesTitle && t.Year == x.Year && (t.ProcessStatus == 4)).Sum(t => t.ApplyAmount);
                    manage_list.Add(x);
                    x.AvailableAmount = x.TotalAmount - x.LockAmount - x.UsedAmount;
                }

            }
            await budgetManageService.Update(manage_list);

            return Ok(new { items = manages, list = manage_list });
        }

        [HttpGet]
        public async Task<IActionResult> FinanceExportExcel([FromQuery] FinancePageModel query, string payer)
        {
            var exp = Expressionable.Create<Model.Live_BudgetApplyRecord>();
            query.Page = 0;
            exp.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrEmpty(query.Name), t => t.ExpensesTitle.Contains(query.Name));
            exp.AndIF(!string.IsNullOrEmpty(query.UserName), t => t.ReceivePaymentCompanyName.Contains(query.UserName));
            exp.AndIF(!string.IsNullOrEmpty(query.PartnerName), t => t.PartnerName.Contains(query.PartnerName));
            exp.AndIF(!string.IsNullOrEmpty(query.Description), t => t.Description.Contains(query.Description));
            exp.AndIF(query.Money.HasValue, t => t.ApplyAmount == query.Money);
            exp.AndIF(query.StartTime.HasValue, t => t.CreateTime >= query.StartTime);
            exp.AndIF(query.EndTime.HasValue, t => t.CreateTime <= query.EndTime);
            exp.AndIF(!string.IsNullOrEmpty(payer), t => t.Payer.Contains(payer));
            exp.AndIF(query.ProcessStatus.HasValue, t => t.ProcessStatus == query.ProcessStatus);
            exp.AndIF(query.Year.HasValue, t => t.Year == query.Year);
            var list = await budgetApplyRecordService.List(exp.ToExpression(), query);

            var exp1 = Expressionable.Create<Model.Live_V_BudgetApplyRecordDetail>();
            exp1.And(t => t.CompanyID == CurrentUser.CompanyID);
            exp1.And(t => t.ProcessStatus.HasValue);
            exp1.AndIF(!string.IsNullOrEmpty(query.Name), t => t.ExpensesTitle.Contains(query.Name));
            exp1.AndIF(!string.IsNullOrEmpty(query.UserName), t => t.ReceivePaymentCompanyName.Contains(query.UserName));
            exp1.AndIF(!string.IsNullOrEmpty(query.PartnerName), t => t.ApplyRecordPartnerName.Contains(query.PartnerName));
            exp1.AndIF(!string.IsNullOrEmpty(query.Description), t => t.Description.Contains(query.Description));
            exp1.AndIF(query.Money.HasValue, t => t.ApplyAmount == query.Money);
            exp1.AndIF(query.StartTime.HasValue, t => t.ApplyRecordCreateTime >= query.StartTime);
            exp1.AndIF(query.EndTime.HasValue, t => t.ApplyRecordCreateTime <= query.EndTime);
            exp1.AndIF(query.ProcessStatus.HasValue, t => t.ProcessStatus == query.ProcessStatus);
            exp1.AndIF(!string.IsNullOrEmpty(payer), t => t.Payer.Contains(payer));
            exp1.AndIF(query.Year.HasValue, t => t.Year == query.Year);

            var details = await budgetApplyRecordDetailService.ListV(exp1.ToExpression(), query);
            var result = budgetApplyRecordService.FinanceExportExcel(list,details);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "预算审核导出明细.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpGet]
        public async Task<IActionResult> SaveFile([FromQuery] int id)
        {
            var model = await fileUploadService.Get(id);

            byte[] buffer = new byte[1024 * 5];
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(model.Path);
                response.EnsureSuccessStatusCode(); // 确保请求成功

                // 将响应内容作为字节流读取
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                // 创建一个文件流，用于进一步处理或保存文件
                using (MemoryStream fileStream = new MemoryStream(fileBytes))
                {
                    buffer = fileStream.ToArray();
                }
            }
            string base64String = Convert.ToBase64String(buffer);
            return Ok(new { base64String, fileName = model.FileName, mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await budgetApplyRecordService.Delete(id);
            return Ok();
        }
    }
}
