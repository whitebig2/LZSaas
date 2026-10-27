using LZ.Clients.API;
using LZ.Core.Attributes;
using LZ.Core.Services;
using LZ.Finance.Service.Interface;

using LZ.Finance.Service.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SqlSugar;
using LZ.Core.Interface;
using System;
using NetTaste;
using NPOI.HPSF;
using LZ.Model.Enums.Budget;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BudgetManageController(IBudgetConfigService budgetConfigService,IBudgetManageService budgetManageService, IHttpService httpService,IBudgetRechargeLogService budgetRechargeLogService) : FinanceControllerBase
    {
        public record BudgetManageQuery(string PartnerGID,string ExpensesTitle, int? CostType,int? CompanyID,int? year);
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FinancePageModel query)
        {
            var exp = Expressionable.Create<Model.Live_V_BudgetManage>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            }

            if (!string.IsNullOrEmpty(query.UserName))
            {
                exp.And(t => t.PartnerName.Contains(query.UserName));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                exp.And(t => t.ExpensesTitle.Contains(query.Name));
            }
            if (query.GID.HasValue)
            {
                exp.And(t => t.PartnerGID == query.GID);
            }
            if (query.Type.HasValue)
            {
                exp.And(t => t.CostType==query.Type);
            }
            exp.AndIF(query.Year.HasValue, t => t.Year == query.Year);
            exp.AndIF(query.Enabled.HasValue, t => t.Hidden == query.Enabled);
            var models = await budgetManageService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_BudgetManage model)
        {
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = CurrentUser.CompanyGID;
                cid = CurrentUser.CompanyID;
            }

            model.CompanyID =cid;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            await budgetManageService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_BudgetManage> GetFunc([FromQuery] BudgetManageQuery query)
        {
            var exp = Expressionable.Create<Model.Live_BudgetManage>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                exp.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                exp.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            exp.AndIF(query.year.HasValue, f => f.Year == query.year);
            exp.AndIF(!string.IsNullOrEmpty(query.PartnerGID), f => f.PartnerGID == Guid.Parse(query.PartnerGID));
            exp.AndIF(!string.IsNullOrEmpty(query.ExpensesTitle), f => f.ExpensesTitle == query.ExpensesTitle);
            exp.AndIF(query.CostType.HasValue, f => f.CostType == query.CostType);
            var model = await budgetManageService.Get(exp.ToExpression());
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Live_BudgetManage> Get(int id)
        {
            var model = await budgetManageService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        
        [HttpGet]
        public async Task<Model.Live_V_BudgetManage> GetView(int id)
        {
            var model = await budgetManageService.GetView(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPost]
        public async Task<IActionResult> UpdateHidden([FromForm] int id, [FromForm] bool isHidden)
        {
            await budgetManageService.UpdateHidden(id, isHidden);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyTransferre([FromForm] int id, [FromForm] int pid, [FromForm] decimal money, [FromForm] string gid, [FromForm] string name)
        {
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                cgid = CurrentUser.CompanyGID;
            }
            var model = await budgetManageService.GetView(id);
            var pmodel = await budgetManageService.GetView(pid);
            if(model.AvailableAmount<money)
            {
                throw new ErrorCodeException(-1, "转账金额大于可用金额,操作失败");
            }
            model.TotalAmount = model.TotalAmount - money;
            model.AvailableAmount = model.AvailableAmount - money;
            pmodel.TotalAmount = pmodel.TotalAmount + money;
            pmodel.AvailableAmount = pmodel.AvailableAmount + money;
            await budgetManageService.UpdateMoney(model);
            await budgetManageService.UpdateMoney(pmodel);

            List<Live_BudgetRechargeLog> logs = new List<Live_BudgetRechargeLog>();

            //付款人划账记录
            Live_BudgetRechargeLog payLog = new Live_BudgetRechargeLog();
            payLog.CompanyGID = cgid;
            payLog.CompanyID = CurrentUser.CompanyID;
            payLog.CID = id;
            payLog.UserGID = Guid.Parse(gid);
            payLog.UserName = name;
            payLog.Year = model.Year;
            payLog.CreateTime = DateTime.Now;
            payLog.Money = money;
            payLog.PartnerName = model.PartnerName;
            payLog.Title = model.ExpensesTitle;
            payLog.Type = (int)BudgetManageRecordActionEnum.划拨划出;
            payLog.CostName = ((CostType)model.CostType).ToString();
            payLog.GenerateName = ((GenerationType)model.GenerateType).ToString();
            payLog.OtherGenerateName = ((GenerationType)pmodel.GenerateType).ToString();
            payLog.OtherCostName = ((CostType)pmodel.CostType).ToString();
            payLog.OtherPartnerName = pmodel.PartnerName;
            payLog.OtherTitle = pmodel.ExpensesTitle;
            payLog.CreateTime = DateTime.Now;
            payLog.TotalAmount = model.TotalAmount;
            payLog.AvailableAmount = model.AvailableAmount;
            payLog.LockAmount = model.LockAmount;
            payLog.UsedAmount = model.UsedAmount;
            payLog.HeadOfficeUsedAmount = model.HeadOfficeUsedAmount;
            payLog.DecreaseAmount = money;
            logs.Add(payLog);

            //收款人入账记录
            Live_BudgetRechargeLog log = new Live_BudgetRechargeLog();
            log.CompanyGID = cgid;
            log.CompanyID = CurrentUser.CompanyID;
            log.CID = pid;
            log.UserGID = Guid.Parse(gid);
            log.UserName = name;
            log.Year = model.Year;
            log.CreateTime = DateTime.Now;
            log.Money = money;
            log.PartnerName = pmodel.PartnerName;
            log.Title = pmodel.ExpensesTitle;
            log.Type = (int)BudgetManageRecordActionEnum.划拨划入;
            log.CostName = ((CostType)pmodel.CostType).ToString();
            log.GenerateName = ((GenerationType)pmodel.GenerateType).ToString();
            log.OtherGenerateName = ((GenerationType)model.GenerateType).ToString();
            log.OtherCostName = ((CostType)model.CostType).ToString();
            log.OtherPartnerName = model.PartnerName;
            log.OtherTitle = model.ExpensesTitle;
            log.CreateTime = DateTime.Now;
            log.TotalAmount = pmodel.TotalAmount;
            log.AvailableAmount = pmodel.AvailableAmount;
            log.LockAmount = pmodel.LockAmount;
            log.UsedAmount = pmodel.UsedAmount;
            log.HeadOfficeUsedAmount = pmodel.HeadOfficeUsedAmount;
            log.AddAmount = money;
            logs.Add(log);

            await budgetRechargeLogService.Add(logs);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteNotBudget()
        {
            PagedModelBase query = new PagedModelBase();
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
            List<int> ll = new List<int>();
            exp.AndIF(query.Year.HasValue, t => t.Year == query.Year);
            exp.AndIF(query.Enabled.HasValue, t => t.Hidden == query.Enabled);
            var models = await budgetManageService.List(exp.ToExpression(), query);
            foreach(var x in models)
            {
                if(!x.IsBudget)
                {
                    await budgetManageService.Delete(x.ID);
                }
            }



            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> BatchCreate()
        {
            try
            {
                int year = DateTime.Now.AddMonths(-3).Year;
                PagedModelBase query = new PagedModelBase();
                query.Page = 0;
                var manageExp = Expressionable.Create<Model.Live_V_BudgetManage>();
                if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
                {
                    manageExp.And(t => t.CompanyID == CurrentUser.CompanyID);
                }
                manageExp.And(t => t.Year == year);
                var manages = await budgetManageService.List(manageExp.ToExpression(),query);


                var exp = Expressionable.Create<Model.Live_V_BudgetConfig>();
                var paras = new Dictionary<string, string>();
                paras.Add("page", "0");
                if (CurrentUser.PlatformType != Model.Enums.PlatformType.Admin)
                {
                    paras.Add("CompanyID", CurrentUser.CompanyID.ToString());
                    exp.And(t => t.CompanyID == CurrentUser.CompanyID);
                }
                exp.And(t => t.Enable == true);
                paras.Add("IsBudget", "true");
                //paras.Add("AppID", "");
                var obj = await httpService.GetAsync(ServiceNames.BasicService, "/api/partner/list", paras);
                List<Model.Live_Partner> partners = new List<Live_Partner>();
                if (!string.IsNullOrEmpty(obj.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    partners = JsonConvert.DeserializeObject<List<Model.Live_Partner>>(ii["items"].ToString());
                }
                
                var configs = await budgetConfigService.List(exp.ToExpression(), query);
                List<Live_BudgetManage> list = new List<Live_BudgetManage>();
                var relatelist = configs.Where(t => t.IsRelate);
                var notrelatelist = configs.Where(t => !t.IsRelate);
                foreach (var x in relatelist)
                {
                    var ids = x.PartnerIDS.Split(',');
                    foreach(var id in ids)
                    {
                        try
                        {
                            var gid = Guid.Parse(id);
                            var m = manages.Where(t => t.PartnerGID == gid && t.ExpensesTitle == x.Title).ToList();
                            if (m.Count > 0) continue;
                            var partner = partners.Find(t => t.PartnerGID == gid);
                            Live_BudgetManage manage = new Live_BudgetManage();
                            manage.CompanyID = partner.CompanyID;
                            manage.CompanyGID = partner.CompanyGID;
                            manage.Year = year;
                            manage.PartnerID = x.PartnerID;
                            manage.PartnerGID = gid;
                            manage.CostType = x.CostType;
                            manage.GenerateType = x.GenerateType;
                            manage.ExpensesTitle = x.Title;
                            manage.Percentage = x.Percentage;
                            manage.CreateTime = DateTime.Now;
                            manage.DirectlyPass = x.DirectlyPass;
                            manage.Hidden = false;
                            manage.PublicUse = x.PublicUse;
                            list.Add(manage);
                        }
                        catch(Exception e)
                        {
                            throw new ErrorCodeException(-1, "生成关联服务商的费用名称" + x.Title+",服务商："+id+"出问题");
                        }
                    }
                    
                }

                foreach (var x in notrelatelist)
                {
                    foreach (var p in partners)
                    {
                        try
                        {
                            var m = manages.Where(t => t.PartnerGID == p.PartnerGID && t.ExpensesTitle == x.Title).ToList();
                            if (m.Count > 0) continue;
                            var isAddModel = list.Where(t => t.PartnerGID == p.PartnerGID && t.ExpensesTitle == x.Title).ToList();
                            if (isAddModel.Count > 0) continue;
                            Live_BudgetManage manage = new Live_BudgetManage();
                            manage.CompanyID = p.CompanyID;
                            manage.CompanyGID = p.CompanyGID;
                            manage.Year = year;
                            manage.PartnerID = p.PartnerID;
                            manage.PartnerGID = p.PartnerGID;
                            manage.CostType = x.CostType;
                            manage.GenerateType = x.GenerateType;
                            manage.ExpensesTitle = x.Title;
                            manage.Percentage = x.Percentage;
                            manage.CreateTime = DateTime.Now;
                            manage.DirectlyPass = x.DirectlyPass;
                            manage.Hidden = false;
                            manage.PublicUse = x.PublicUse;
                            list.Add(manage);
                        }
                        catch(Exception e)
                        {
                            throw new ErrorCodeException(-1, "生成不关联服务商的费用名称" + x.Title + ",服务商：" + p.PartnerName + "出问题");
                        }
                        

                    }
                }









                    //foreach (var x in relatelist)
                    //{

                    //    if (x.IsRelate)
                    //    {

                    //        var m = manages.Where(t => t.PartnerGID == x.PartnerGID && t.ExpensesTitle==x.Title).ToList();
                    //        if (m.Count > 0) continue;
                    //        var partner = partners.Find(t => t.PartnerGID == x.PartnerGID);
                    //        Live_BudgetManage manage = new Live_BudgetManage();
                    //        manage.CompanyID = partner.CompanyID;
                    //        manage.CompanyGID = partner.CompanyGID;
                    //        manage.Year = year;
                    //        manage.PartnerID = x.PartnerID;
                    //        manage.PartnerGID = x.PartnerGID;
                    //        manage.CostType = x.CostType;
                    //        manage.GenerateType = x.GenerateType;
                    //        manage.ExpensesTitle = x.Title;
                    //        manage.Percentage = x.Percentage;
                    //        manage.CreateTime = DateTime.Now;
                    //        manage.DirectlyPass = x.DirectlyPass;
                    //        manage.Hidden = false;
                    //        manage.PublicUse = x.PublicUse;
                    //        list.Add(manage);
                    //    }
                    //    else
                    //    {
                    //        //continue;
                    //        foreach (var p in partners)
                    //        {
                    //            var m = manages.Where(t => t.PartnerGID == p.PartnerGID && t.ExpensesTitle == x.Title).ToList();
                    //            if (m.Count > 0) continue;
                    //            Live_BudgetManage manage = new Live_BudgetManage();
                    //            manage.CompanyID = p.CompanyID;
                    //            manage.CompanyGID = p.CompanyGID;
                    //            manage.Year = year;
                    //            manage.PartnerID = p.PartnerID;
                    //            manage.PartnerGID = p.PartnerGID;
                    //            manage.CostType = x.CostType;
                    //            manage.GenerateType = x.GenerateType;
                    //            manage.ExpensesTitle = x.Title;
                    //            manage.Percentage = x.Percentage;
                    //            manage.CreateTime = DateTime.Now;
                    //            manage.DirectlyPass = x.DirectlyPass;
                    //            manage.Hidden = false;
                    //            manage.PublicUse = x.PublicUse;
                    //            list.Add(manage);
                    //        }
                    //    }

                    //}
                    await budgetManageService.Add(list);
                return Ok(new { itemss = partners });
            }
            catch(Exception e)
            {
                throw new ErrorCodeException(-1, e.ToString());
            }
            

        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BudgetManage model)
        {
            await budgetManageService.Update(model);
            return Ok();
        }

		[HttpGet]
		public async Task<IActionResult> ExportExcel()
		{
            int year = DateTime.Now.AddMonths(-3).Year;
			var expable = Expressionable.Create<Model.Live_V_BudgetManage>();
            PagedModelBase query = new PagedModelBase();
            query.Page = 0;
            query.Order = "partner";
			expable.And(t => t.Year == year);
            expable.And(t => t.CompanyID == CurrentUser.CompanyID);

            var models = await budgetManageService.List(expable.ToExpression(),query);

            var expable_log = Expressionable.Create<Model.Live_BudgetRechargeLog>();
            expable_log.And(t => t.CompanyID == CurrentUser.CompanyID);
            expable_log.And(t => t.Year == year);
            var logs = await budgetRechargeLogService.List(expable_log.ToExpression(), query);


            var result = budgetManageService.ExportExcel(models, logs);
			string base64String = Convert.ToBase64String(result);
			return Ok(new { base64String, fileName = "总预算管理.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
		}

		[HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await budgetManageService.Delete(id);
            return Ok();
        }
    }
}
