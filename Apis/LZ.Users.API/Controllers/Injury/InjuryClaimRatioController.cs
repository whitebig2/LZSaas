using LZ.Core.Interface;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SqlSugar;
using LZ.Core;
using LZ.Model.Enums;
using System.IO;
using NPOI.SS.Formula.Functions;
using System.Security.Cryptography;
using Senparc.Weixin.WxOpen.Entities;
using NPOI.SS.UserModel;
using LZ.Core.Attributes;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.ApiService.Services;
using LZ.Model.FxqModel;
using LZ.ApiService.Interface;
using System.Security.Policy;
using System;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using LZ.Core.CommonHandle;
using LZ.Model.Enums.Injury;
using NPOI.HSSF.Record;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace LZ.Users.API.Controllers.Injury
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryClaimRatioController(IInjuryClaimRatioService injuryClaimRatioService,IHttpService httpService, IInjuryInfoService injuryInfoService,IEntryService entryService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_InjuryClaimRatio>();
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(t => t.CustomerName.Contains(query.CustomerName));
            }
            if (!string.IsNullOrEmpty(query.PartnerName))
            {
                expable.And(t => t.PartnerName.Contains(query.PartnerName));
            }
            if (!string.IsNullOrEmpty(query.Type))
            {
                expable.And(t => t.Type==Convert.ToInt32(query.Type));
            }
            var exp = expable.ToExpression();

            var models = await injuryClaimRatioService.ListView(exp, query, "ID desc");
            return Ok(new { items = models, count = query.Count });
        }
        

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_InjuryClaimRatio model)
        {

            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            var expable = Expressionable.Create<Model.Live_InjuryClaimRatio>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var user = CurrentUser;
                cgid = user.CompanyGID;
                cid = user.CompanyID;
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            await injuryClaimRatioService.Add(model);



            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Calculate()
        {
            var date = DateTime.Now;
            var premonth = date.AddMonths(-1);
            //每个月底的年度数据不删除，其他时间点只保留最新数据
            if (premonth.Month!=12)
            {
                await injuryClaimRatioService.Delete(t => t.SignTime.HasValue && t.SignTime.Value.ToString("yyyy-MM")== date.ToString("yyyy-MM"));
            }

            var exp = Expressionable.Create<Live_V_UserEntryInsuracneStatus>();
            exp.And(t => t.IsUseBusInsurance && t.CusEnable && t.InsuranceType != InsuranceTypeEnumType.不参保 && t.InsuranceProcessStatus == InsuranceProcessEnumType.已推送);
            exp.And(t => t.Status == 2 || t.Status == 4);
            var insuracneList = await entryService.UserEntryInsuranceList(exp.ToExpression());

            //平台工伤理赔率
            List<Live_InjuryClaimRatio> list = new List<Live_InjuryClaimRatio>();
            Live_InjuryClaimRatio item1 = new Live_InjuryClaimRatio();
            item1.CompanyID = 1;
            item1.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
            item1.Type = (int)InjuryClaimRatioTypeEnum.平台工伤理赔率;
            item1.TypeName = ((InjuryClaimRatioTypeEnum)item1.Type).ToString();
            item1.CreateTime = date;
            item1.SignTime = date;
            item1.Year = date.Year;
            var paras = new Dictionary<string, string>();
            paras.Add("Name", "商业保险费");//7月份开始加
            paras.Add("Types", "4");
            paras.Add("Page", "0");
            paras.Add("Year", date.Year.ToString());
            paras.Add("StartTime", "2026-07-01");
            var obj = await httpService.GetAsync(ServiceNames.FinanceService, "/api/BudgetApplyRecord/list", paras);
            List<Model.Live_BudgetApplyRecord> items = new List<Live_BudgetApplyRecord>();
            try
            {
                if (!string.IsNullOrEmpty(obj.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    items = JsonConvert.DeserializeObject<List<Model.Live_BudgetApplyRecord>>(ii["items"].ToString());
                }
            }
            catch (Exception e)
            {
                return Ok(new { msg = "预算接口错误", obj = obj.Data, code = 1 });
            }
            var expable = Expressionable.Create<Model.Live_V_InjuryInfo>();
            var infos = await injuryInfoService.List(t => t.ProcessStatus != (int)InjuryProcessStatus.审核失败 && t.ProcessStatus != (int)InjuryProcessStatus.待添加 && t.ProcessStatus != (int)InjuryProcessStatus.工伤撤回, new Model.PagedModel.UserPagedModel() { Page = 0 });
            var yearInfos = infos.Where(t => t.InjuryTime.HasValue && t.InjuryTime.Value.Year == date.Year).ToList();
            if (items.Count==0)
            {
                item1.PremiumReceivedAmount = (897367.98M + 771648M);
            }
            else
            {
                item1.PremiumReceivedAmount = items.Sum(t => t.ApplyAmount);
            }
            item1.ResolvedClaimAmount = yearInfos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.工伤完成).Sum(t => t.InsuranceTotalAmount);
            #region 计算全部员工的全部累计未决理赔金额
            var peifu = infos.Where(t => !string.IsNullOrEmpty(t.InjuryLevel) && t.StaffOneTimePackAmount == 0M).Sum(t => t.EstimatedClaim);
            var lipei = infos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.理赔待打款).Sum(t => t.InsuranceTotalAmount);
            var repay = infos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.恢复期 || t.ProcessStatus == (int)InjuryProcessStatus.医疗期 || t.ProcessStatus == (int)InjuryProcessStatus.理赔待打款).Sum(t => t.ReturnBackMoneyTotalAmount);
            item1.TotalAmount = peifu + lipei + repay;  // 预估赔付金额+理赔待打款+员工医疗所有发票（医疗期，恢复期，工伤理赔期状态下）
            #endregion
            
            var yearPeifu = yearInfos.Where(t=> !string.IsNullOrEmpty(t.InjuryLevel) && t.StaffOneTimePackAmount==0M).Sum(t => t.EstimatedClaim);
            var yearLipei = yearInfos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.理赔待打款).Sum(t => t.InsuranceTotalAmount);
            var yearRepay = yearInfos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.恢复期 || t.ProcessStatus == (int)InjuryProcessStatus.医疗期 || t.ProcessStatus == (int)InjuryProcessStatus.理赔待打款).Sum(t => t.ReturnBackMoneyTotalAmount);
            item1.OutstandingClaimAmount = yearPeifu + yearLipei + yearRepay;  // 预估赔付金额+理赔待打款+员工医疗所有发票（医疗期，恢复期，工伤理赔期状态下）
            item1.InjuryNumber = infos.Where(t=>t.InjuryTime.HasValue && t.InjuryTime.Value.ToString("yyyy-MM")== date.ToString("yyyy-MM")).ToList().Count;
            item1.InsuranceNumber = insuracneList.Count; //当月平台已参保状态人数还在调整中;
            item1.InjuryRate = 0;
            if (item1.InsuranceNumber != 0M)
            {
                item1.InjuryRate = decimal.Round((decimal)item1.InjuryNumber / (decimal)item1.InsuranceNumber, 4);
            }
            var pinjuryamount = item1.ResolvedClaimAmount + item1.OutstandingClaimAmount;
            item1.ClaimRatio = 0M;
            if (pinjuryamount != 0M)
            {
                item1.ClaimRatio = decimal.Round(item1.PremiumReceivedAmount / pinjuryamount, 4);
            }
            list.Add(item1);


            //var dics = records.Select(p => p.CustomerID).Distinct();

            
            var paras_partner = new Dictionary<string, string>();
            paras_partner.Add("Page", "0");
            var obj_partner = await httpService.GetAsync(ServiceNames.BasicService, "/api/Partner/list", paras_partner);
            List<Model.Live_Partner> partners = new List<Live_Partner>();
            try
            {
                if (!string.IsNullOrEmpty(obj_partner.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj_partner.Data.ToString()) as JObject;
                    partners = JsonConvert.DeserializeObject<List<Model.Live_Partner>>(ii["items"].ToString());
                }
            }
            catch (Exception e)
            {
                return Ok(new { msg = "服务商接口错误", obj = obj.Data, code = 1 });
            }
            var paras_Profit = new Dictionary<string, string>();
            paras_Profit.Add("Year", date.Year.ToString());
            paras_Profit.Add("Page", "0");
            var obj_Profit = await httpService.GetAsync(ServiceNames.FinanceService, "/api/ProfitManageRecord/list", paras_Profit);
            List<Model.Live_ProfitManageRecord> items_Profit = new List<Live_ProfitManageRecord>();
            try
            {
                if (!string.IsNullOrEmpty(obj_Profit.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj_Profit.Data.ToString()) as JObject;
                    items_Profit = JsonConvert.DeserializeObject<List<Model.Live_ProfitManageRecord>>(ii["items"].ToString());
                }
            }
            catch (Exception e)
            {
                return Ok(new { msg = "毛利获取错误", obj = obj.Data, code = 1, config = paras_Profit });
            }
            var paras_Invoice = new Dictionary<string, string>();
            paras_Invoice.Add("year", date.Year.ToString());
            paras_Invoice.Add("Page", "0");
            var obj_Invoice = await httpService.GetAsync(ServiceNames.FinanceService, "/api/Invoice/InjuryCompanyBillList", paras_Profit);
            List<Model.Live_V_Invoice> items_Invoice = new List<Live_V_Invoice>();
            try
            {
                if (!string.IsNullOrEmpty(obj_Invoice.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj_Invoice.Data.ToString()) as JObject;
                    items_Invoice = JsonConvert.DeserializeObject<List<Model.Live_V_Invoice>>(ii["items"].ToString());
                }
            }
            catch (Exception e)
            {
                return Ok(new { msg = "发票获取错误", obj = obj.Data, code = 1 });
            }

            var paras_ProfitConfig = new Dictionary<string, string>();
            paras_ProfitConfig.Add("Page", "0");
            var obj_ProfitConfig = await httpService.GetAsync(ServiceNames.FinanceService, "/api/ProfitConfig/InjuryList", paras_ProfitConfig);
            List<Model.Live_V_ProfitConfig> items_ProfitConfig = new List<Live_V_ProfitConfig>();
            try
            {
                if (!string.IsNullOrEmpty(obj_ProfitConfig.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj_ProfitConfig.Data.ToString()) as JObject;
                    items_ProfitConfig = JsonConvert.DeserializeObject<List<Model.Live_V_ProfitConfig>>(ii["items"].ToString());
                }
            }
            catch (Exception e)
            {
                return Ok(new { msg = "客户单位毛利配置获取错误", obj = obj.Data, code = 1 });
            }
            //服务商/人力公司工伤理赔率
            foreach (var x in partners)
            {
                Live_InjuryClaimRatio item = new Live_InjuryClaimRatio();
                item.CompanyID = x.CompanyID;
                item.CompanyGID = x.CompanyGID;
                item.Type = (int)InjuryClaimRatioTypeEnum.服务商工伤理赔率;
                item.TypeName = ((InjuryClaimRatioTypeEnum)item.Type).ToString();
                item.PartnerID = x.PartnerID;
                item.PartnerGID = x.PartnerGID;
                item.PartnerName = x.PartnerName;
                item.CreateTime = date;
                item.SignTime = date;
                item.Year = date.Year;
                var injurys = yearInfos.Where(t => t.PartnerID == x.PartnerID).ToList();
                if (injurys.Count == 0)
                {
                    item.PremiumReceivedAmount = 0M;
                    item.ResolvedClaimAmount = 0M;
                    item.OutstandingClaimAmount = 0M;
                    item.InjuryNumber = 0;

                }
                else
                {
                    //服务商保费从服务商毛利中获取
                    if (x.CompanyID == 1 && items_Profit.Count>0)
                    {
                        item.PremiumReceivedAmount = items_Profit.Where(t => t.PartnerGID == x.PartnerGID).ToList().Sum(t => t.BusInsuranceSumAmount);

                    }
                    //人力公司保费从人力公司保费账单中获取
                    else if (x.CompanyID != 1 && items_Invoice.Count>0)
                    {
                        //第三方数量不多，改成switch固定名称获取，不用做关联
                        //item.PremiumReceivedAmount = items_Invoice.Where(t => t.PartnerGID == x.PartnerGID).ToList().Sum(t => t.TotalAmount);
                        switch (x.PartnerName)
                        {
                            case "畅新人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName== "宁波畅新企业管理服务有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "超亿人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波超亿人力资源有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "登发人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波登发人力资源有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "慧服人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "安徽慧服人力资源有限公司-商保").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "合流科技":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波合流科技有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "浙江宏迈":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "浙江宏迈数字科技有限公司-系统").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "集聘人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波集聘人力资源有限公司-商保").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "蓝鹰人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "台州蓝鹰人力资源外包服务有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "募贤人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波募贤人力资源有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "启博":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波启博商务信息咨询有限公司-商保").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "薪加倍":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "浙江薪加倍人力资源有限公司-商保").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "星煜供应链":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波星煜供应链管理有限公司-商保").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "蚁力企业":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "蚁力企业管理(上海)有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "职引力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波职南针人力资源有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "超帆人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波超帆人力资源有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            case "毅航人力":
                                item.PremiumReceivedAmount = items_Invoice.Where(t => t.RecipientName == "宁波卓毅人力资源有限公司").ToList().Sum(t => t.TotalAmount);
                                break;
                            default:
                                continue;
                        }
                    }
                    else
                    {
                        item.PremiumReceivedAmount = 0M;
                    }
                    item.ResolvedClaimAmount = yearInfos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.工伤完成 && t.PartnerGID == x.PartnerGID).Sum(t => t.InsuranceTotalAmount);
                    var peifu_partner = yearInfos.Where(t => !string.IsNullOrEmpty(t.InjuryLevel) && t.StaffOneTimePackAmount == 0M && t.PartnerGID == x.PartnerGID).Sum(t => t.EstimatedClaim);
                    var lipei_partner = yearInfos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.理赔待打款 && t.PartnerGID==x.PartnerGID).Sum(t => t.InsuranceTotalAmount);
                    var repay_partner = yearInfos.Where(t =>  (t.ProcessStatus == (int)InjuryProcessStatus.恢复期 || t.ProcessStatus == (int)InjuryProcessStatus.医疗期 || t.ProcessStatus == (int)InjuryProcessStatus.理赔待打款) && t.PartnerGID == x.PartnerGID).Sum(t => t.ReturnBackMoneyTotalAmount);
                    item.OutstandingClaimAmount = peifu_partner + lipei_partner + repay_partner;  // 预估赔付金额+理赔待打款+员工医疗所有发票（医疗期，恢复期，工伤理赔期状态下）
                    //item.InjuryNumber = injurys.Count;
                    item.InjuryNumber = injurys.Where(t => t.InjuryTime.HasValue && t.InjuryTime.Value.ToString("yyyy-MM") == date.ToString("yyyy-MM")).ToList().Count;

                }
                var insurances = insuracneList.Where(t => t.PartnerID == x.PartnerID).ToList();
                item.InsuranceNumber = insurances.Count;//当月平台已参保状态人数还在调整中;
                item.InjuryRate = 0M;
                if (item.InsuranceNumber != 0M)
                {
                    item.InjuryRate = decimal.Round((decimal)item.InjuryNumber / (decimal)item.InsuranceNumber, 4);
                }
                item.ClaimRatio = 0M;
                var injuryamount = item.ResolvedClaimAmount + item.OutstandingClaimAmount;
                if (injuryamount != 0M)
                {
                    item.ClaimRatio = decimal.Round(item.PremiumReceivedAmount / injuryamount, 4);
                }
                list.Add(item);
            }



            //客户单位理赔率
            foreach (var x in items_ProfitConfig)
            {
                if (!x.CompanyID.HasValue)
                {
                    continue;
                }
                Live_InjuryClaimRatio item = new Live_InjuryClaimRatio();
                item.CompanyID = x.CompanyID;
                item.CompanyGID = x.CompanyGID;
                item.Type = (int)InjuryClaimRatioTypeEnum.客户单位理赔率;
                item.TypeName = ((InjuryClaimRatioTypeEnum)item.Type).ToString();
                item.PartnerID = x.PartnerID;
                item.PartnerGID = x.PartnerGID;
                item.PartnerName = x.PartnerName;
                item.CustomerName = x.CustomerName;
                item.CustomerGID = x.CustomerGID;
                item.CreateTime = date;
                item.SignTime = date;
                item.Year = date.Year;
                var injurys = yearInfos.Where(t => t.CustomerGID == x.CustomerGID).ToList();
                if (injurys.Count == 0)
                {
                    
                    item.PremiumReceivedAmount = 0M;
                    item.ResolvedClaimAmount = 0M;
                    item.OutstandingClaimAmount = 0M;
                    item.InjuryNumber = 0;
                }
                else
                {
                    if (items_Profit.Count > 0)
                    {
                        item.PremiumReceivedAmount = items_Profit.Where(t => t.CustomerGID == x.CustomerGID).ToList().Sum(t => t.BusInsuranceSumAmount);
                    }
                    else
                    {
                        item.PremiumReceivedAmount = 0M;
                    }
                    item.ResolvedClaimAmount = yearInfos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.工伤完成 && t.CustomerGID == x.CustomerGID).Sum(t => t.InsuranceTotalAmount);
                    var peifu_customer = yearInfos.Where(t => !string.IsNullOrEmpty(t.InjuryLevel) && t.StaffOneTimePackAmount == 0M && t.CustomerGID == x.CustomerGID).Sum(t => t.EstimatedClaim);
                    var lipei_customer = yearInfos.Where(t => t.ProcessStatus == (int)InjuryProcessStatus.理赔待打款 && t.CustomerGID == x.CustomerGID).Sum(t => t.InsuranceTotalAmount);
                    var repay_customer = yearInfos.Where(t => (t.ProcessStatus == (int)InjuryProcessStatus.恢复期 || t.ProcessStatus == (int)InjuryProcessStatus.医疗期 || t.ProcessStatus == (int)InjuryProcessStatus.理赔待打款) && t.CustomerGID == x.CustomerGID).Sum(t => t.ReturnBackMoneyTotalAmount);
                    item.OutstandingClaimAmount = peifu_customer + lipei_customer + repay_customer;  // 预估赔付金额+理赔待打款+员工医疗所有发票（医疗期，恢复期，工伤理赔期状态下）
                    //item.InjuryNumber = injurys.Count;
                    item.InjuryNumber = injurys.Where(t => t.InjuryTime.HasValue && t.InjuryTime.Value.ToString("yyyy-MM") == date.ToString("yyyy-MM")).ToList().Count;

                }
                var insurances = insuracneList.Where(t => t.CustomerGID == x.CustomerGID).ToList();
                item.InsuranceNumber = insurances.Count;//当月平台已参保状态人数还在调整中;
                item.InjuryRate = 0M;
                if (item.InsuranceNumber != 0M)
                {
                    item.InjuryRate = decimal.Round((decimal)item.InjuryNumber / (decimal)item.InsuranceNumber, 4);
                }
                item.ClaimRatio = 0M;
                var injuryamount = item.ResolvedClaimAmount + item.OutstandingClaimAmount;
                if (injuryamount != 0M)
                {
                    item.ClaimRatio = decimal.Round(item.PremiumReceivedAmount / injuryamount, 4);
                }
                list.Add(item);
            }




            //平台工伤风险金赔付率
            Live_InjuryClaimRatio item_fengxian = new Live_InjuryClaimRatio();
            item_fengxian.CompanyID = 1;
            item_fengxian.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
            item_fengxian.Type = (int)InjuryClaimRatioTypeEnum.平台工伤风险金赔付率;
            item_fengxian.TypeName = ((InjuryClaimRatioTypeEnum)item_fengxian.Type).ToString();
            item_fengxian.CreateTime = date;
            item_fengxian.SignTime = date;
            item_fengxian.PremiumReceivedAmount = 0M;
            if (items_Profit.Count > 0)
            {
                item_fengxian.PremiumReceivedAmount = items_Profit.Sum(t => t.RiskCurrentAmount);
            }
            item_fengxian.ResolvedClaimAmount = yearInfos.Sum(t => t.RiskTotalAmount);
            item_fengxian.OutstandingClaimAmount = 0M;
            item_fengxian.ClaimRatio = 0M;
            if (item_fengxian.ResolvedClaimAmount != 0M)
            {
                item_fengxian.ClaimRatio = decimal.Round(item_fengxian.PremiumReceivedAmount / item_fengxian.ResolvedClaimAmount, 4);
            }
            list.Add(item_fengxian);



            //服务商风险金赔付率

            //var peifu = infos.Sum(t => t.EstimatedClaim);    //累计赔付金额
            foreach(var x in partners)
            {
                Live_InjuryClaimRatio item = new Live_InjuryClaimRatio();
                item.CompanyID = x.CompanyID;
                item.CompanyGID = x.CompanyGID;
                item.Type = (int)InjuryClaimRatioTypeEnum.服务商风险金赔付率;
                item.TypeName = ((InjuryClaimRatioTypeEnum)item.Type).ToString();
                item.PartnerID = x.PartnerID;
                item.PartnerGID = x.PartnerGID;
                item.PartnerName = x.PartnerName;
                item.CreateTime = date;
                item.SignTime = date;
                var injurys = yearInfos.Where(t => t.PartnerID == x.PartnerID).ToList();
                item.PremiumReceivedAmount = items_Profit.Where(t => t.PartnerGID == x.PartnerGID).ToList().Sum(t => t.RiskCurrentAmount);
                item.ResolvedClaimAmount = injurys.Sum(t => t.RiskTotalAmount);
                item.OutstandingClaimAmount = 0M;
                item.ClaimRatio = 0M;
                if (item.ResolvedClaimAmount != 0M)
                {
                    item.ClaimRatio = decimal.Round(item.PremiumReceivedAmount / item.ResolvedClaimAmount, 4);
                }
                list.Add(item);
            }
            await injuryClaimRatioService.Add(list);

            return Ok();
        }

        [HttpGet]
        public async Task<Model.Live_InjuryClaimRatio> Get(int id)
        {
            return await injuryClaimRatioService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryClaimRatio model)
        {
            if (!await injuryClaimRatioService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryClaimRatioService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryClaimRatioService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryClaimRatioService.Delete(id);
            return Ok();
        }
    }
}
