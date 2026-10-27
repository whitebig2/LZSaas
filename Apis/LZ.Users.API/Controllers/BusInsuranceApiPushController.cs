using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.IExcelTemplate.busInsurance;
using LZ.Model.InsuranceApi.ThirtySix;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace LZ.Users.API.Controllers
{
    public class BusInsuranceApiPushController : UsersControllerBase
    {
        private readonly IBusInsuranceApiPushService _service;
        private readonly IBusInsuracneService _busInsuracneService;
        private readonly IBusInsuranceDayMatchRecordService _dayMatchRecordService;
        private readonly IRedisService _redisService;
        private readonly IThirtySixService _thirtySixService;
        private readonly IMailSenderService _mailSenderService;
        private readonly IBusInsuranceRecordService _recordService;

        public BusInsuranceApiPushController(
            IBusInsuranceApiPushService service,
            IBusInsuracneService busInsuracneService,
            IBusInsuranceDayMatchRecordService dayMatchRecordService,
            IRedisService redisService,
            IThirtySixService thirtySixService,
            IMailSenderService mailSenderService,
            IBusInsuranceRecordService recordService)
        {
            _service = service;
            _busInsuracneService = busInsuracneService;
            _dayMatchRecordService = dayMatchRecordService;
            _redisService = redisService;
            _thirtySixService = thirtySixService;
            _mailSenderService = mailSenderService;
            _recordService = recordService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PageList(string insuranceCompany, string pushType, BusInsuranceApiPushStatusEnumType? status, DateTime? recordDate, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_BusInsuranceApiPush>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(pushType), f => f.PushType.Contains(pushType));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(recordDate.HasValue, f => f.RecordDate == recordDate.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string insuranceCompany, string pushType, BusInsuranceApiPushStatusEnumType? status, DateTime? recordDate)
        {
            var exp = Expressionable.Create<Live_BusInsuranceApiPush>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(pushType), f => f.PushType.Contains(pushType));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(recordDate.HasValue, f => f.RecordDate == recordDate.Value);
            var models = await _service.List(exp.ToExpression());
            var exportDataList = models.Select(f => new BusInsuranceApiPushExcelTemplate
            {
                InsuranceCompany = f.InsuranceCompany,
                RecordDate = f.RecordDate?.ToString("yyyy-MM-dd"),
                PushType = f.PushType,
                Status = f.Status.ToString(),
                AddCount = f.AddCount,
                DelCount = f.DelCount,
                ConfirmUrl = f.ConfirmUrl,
                CreateTime = f.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "商保接口推送");
            var base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保接口推送.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpGet]
        public async Task<IActionResult> PageDetailList(Guid? busInsuranceApiPushID, string name, string cardID, string delName, string delCardID, string customerName, string orderNo, string planName, BusInsuranceApiPushDetailStatusEnumType? status, int? addBusinessID, int? delBusinessID, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_BusInsuranceApiPushDetail>();
            exp.AndIF(busInsuranceApiPushID.HasValue && busInsuranceApiPushID.Value != Guid.Empty, f => f.BusInsuranceApiPushID == busInsuranceApiPushID.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(delName), f => f.DelName.Contains(delName));
            exp.AndIF(!string.IsNullOrWhiteSpace(delCardID), f => f.DelCardID.Contains(delCardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(orderNo), f => f.OrderNo.Contains(orderNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(addBusinessID.HasValue && addBusinessID.Value > 0, f => f.AddBusinessID == addBusinessID.Value);
            exp.AndIF(delBusinessID.HasValue && delBusinessID.Value > 0, f => f.DelBusinessID == delBusinessID.Value);
            var models = await _service.PageDetailList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ExportDetailQueryList(Guid? busInsuranceApiPushID, string name, string cardID, string delName, string delCardID, string customerName, string orderNo, string planName, BusInsuranceApiPushDetailStatusEnumType? status, int? addBusinessID, int? delBusinessID)
        {
            var exp = Expressionable.Create<Live_BusInsuranceApiPushDetail>();
            exp.AndIF(busInsuranceApiPushID.HasValue && busInsuranceApiPushID.Value != Guid.Empty, f => f.BusInsuranceApiPushID == busInsuranceApiPushID.Value);
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(delName), f => f.DelName.Contains(delName));
            exp.AndIF(!string.IsNullOrWhiteSpace(delCardID), f => f.DelCardID.Contains(delCardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(orderNo), f => f.OrderNo.Contains(orderNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(addBusinessID.HasValue && addBusinessID.Value > 0, f => f.AddBusinessID == addBusinessID.Value);
            exp.AndIF(delBusinessID.HasValue && delBusinessID.Value > 0, f => f.DelBusinessID == delBusinessID.Value);
            var models = await _service.ListDetail(exp.ToExpression());
            var exportDataList = models.Select(f => new BusInsuranceApiPushDetailExcelTemplate
            {
                Name = f.Name,
                CardID = f.CardID,
                Age = f.Age,
                Sex = f.Sex,
                Phone = f.Phone,
                CustomerName = f.CustomerName,
                OrderNo = f.OrderNo,
                PlanName = f.PlanName,
                WorkName = f.WorkName,
                WorkClass = f.WorkClass,
                InsuranceWorkName = f.InsuranceWorkName,
                StartDate = f.StartDate,
                IsSocial = f.IsSocial ? "是" : "否",
                IsWorkHeight = f.IsWorkHeight ? "是" : "否",
                DelName = f.DelName,
                DelCardID = f.DelCardID,
                Status = f.Status.ToString(),
                FailMsg = f.FailMsg,
                Remark = f.Remark,
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "商保接口推送明细");
            var base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保接口推送明细.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpGet]
        public async Task<IActionResult> List(string insuranceCompany, string pushType, BusInsuranceApiPushStatusEnumType? status)
        {
            var exp = Expressionable.Create<Live_BusInsuranceApiPush>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(pushType), f => f.PushType.Contains(pushType));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            var models = await _service.List(exp.ToExpression());
            return Ok(new { items = models, count = models.Count });
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _service.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetNav(Guid id)
        {
            var model = await _service.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var model = await _service.GetDetail(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_BusInsuranceApiPushExtension model)
        {
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不能为空");
            }

            if (model.ID == Guid.Empty)
            {
                model.ID = Guid.NewGuid();
            }
            model.CreateTime ??= DateTime.Now;

            if (model.Details != null)
            {
                foreach (var detail in model.Details)
                {
                    if (detail.ID == Guid.Empty)
                    {
                        detail.ID = Guid.NewGuid();
                    }
                    detail.BusInsuranceApiPushID = model.ID;
                    detail.CreateTime ??= DateTime.Now;
                }
            }

            await _service.AddNav(model);
            return Ok();
        }

        /// <summary>
        /// 推送到大地365（按 PushType 自动区分增员/替换）
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Push([FromForm] Guid id)
        {
            var isb = await _redisService.SetAsync($"Push_{id.ToString()}", id.ToString(), 60, CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                throw new ErrorCodeException(-1, $"1分钟内 不可重复推送!");
            }
            var model = await _service.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "推送记录不存在");
            }
            if (model.Status != BusInsuranceApiPushStatusEnumType.待推送)
            {
                throw new ErrorCodeException(-1, "当前状态不允许推送");
            }
            if (model.Details == null || model.Details.Count == 0)
            {
                throw new ErrorCodeException(-1, "没有可推送的明细");
            }

            if (model.InsuranceCompany == "大地365")
            {
                var defaultDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
                if (model.PushType == "新增")
                {
                    var addRecords = model.Details.Select(f => new ThirtySixAddRecord
                    {
                        InsuredName = f.Name,
                        DocumentType = "身份证",
                        IdNumber = f.CardID,
                        Phone = f.Phone ?? string.Empty,
                        ActualInsuredCustomers = f.CustomerName,
                        PolicyNumber = f.OrderNo,
                        PlanInfoName = f.PlanName,
                        Profession = string.IsNullOrWhiteSpace(f.InsuranceWorkName) ? f.WorkName : f.InsuranceWorkName,
                        EffectiveDate = string.IsNullOrWhiteSpace(f.StartDate) ? defaultDate : f.StartDate,
                        HasSocialS = f.IsSocial ? "是" : "否",
                        RelatedHeightS = f.IsWorkHeight ? "是" : "否",
                        Remark = f.Remark ?? string.Empty,
                    }).ToList();

                    var res = await _thirtySixService.PushAdd(addRecords);
                    ApplyPushResult(model, res, isReplace: false);
                }
                else if (model.PushType == "替换")
                {
                    var replaceRecords = model.Details.Select(f => new ThirtySixReplaceRecord
                    {
                        InsuredName = f.Name,
                        DocumentType = "身份证",
                        IdNumber = f.CardID,
                        Phone = f.Phone ?? string.Empty,
                        ActualInsuredCustomers = f.CustomerName,
                        PolicyNumber = f.OrderNo,
                        PlanInfoName = f.PlanName,
                        Profession = f.InsuranceWorkName,
                        EffectiveDate = string.IsNullOrWhiteSpace(f.StartDate) ? defaultDate : f.StartDate,
                        HasSocialS = f.IsSocial ? "是" : "否",
                        RelatedHeightS = f.IsWorkHeight ? "是" : "否",
                        Remark = f.Remark ?? string.Empty,
                        AttritionInsuredName = f.DelName,
                        AttritionIdNumber = f.DelCardID,
                    }).ToList();

                    var res = await _thirtySixService.PushReplace(replaceRecords);
                    ApplyPushResult(model, res, isReplace: true);
                }
                else
                {
                    throw new ErrorCodeException(-1, $"不支持的推送类型：{model.PushType}");
                }
                await _service.UpdateNav(model);
            }
            else if (model.InsuranceCompany == "阳光保险公司-新")
            {
                Dictionary<string, byte[]> files= new Dictionary<string, byte[]>();

                if (model.PushType == "新增")
                {
                    var pushRecords = model.Details.Where(f => f.Status== BusInsuranceApiPushDetailStatusEnumType.已核对).Select(f => new NewYangGuangExcelTemplate()
                    {
                        CardID = f.CardID,
                        CustomerName = f.CustomerName,
                        Name = f.Name,
                        CardType = "01-身份证",
                        ChangeFlag = "I-新增",
                        ExistCardID = string.Empty,
                        ExistCardType = string.Empty,
                        ExistName = string.Empty,
                        HealthInfo = string.Empty,
                        InsuranceWorkName = f.InsuranceWorkName,
                        MonthSalaryAmount = string.Empty,
                        SalaryCurrencyName = string.Empty,
                        Sex = f.Sex,
                        Age = f.Age,
                    }).ToList();
                    var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "表单名称");
                    files.Add($"{model.OrderNo}增员.xlsx", fileBytes);
                }
                else if (model.PushType == "减员")
                {
                    var pushRecords = model.Details.Where(f => f.Status == BusInsuranceApiPushDetailStatusEnumType.已核对).Select(f => new NewYangGuangExcelTemplate()
                    {
                        CardID = string.Empty,
                        CustomerName = f.CustomerName,
                        Name = string.Empty,
                        ExistName = f.Name,
                        ExistCardType = "01-身份证",
                        ExistCardID = f.CardID,
                        CardType = string.Empty,
                        ChangeFlag = "D-删除",
                        HealthInfo = string.Empty,
                        InsuranceWorkName = f.InsuranceWorkName,
                        MonthSalaryAmount = string.Empty,
                        SalaryCurrencyName = string.Empty,
                        Sex = f.Sex,
                        Age = f.Age,
                    }).ToList();
                    var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "表单名称");
                    files.Add($"{model.OrderNo}减员.xlsx", fileBytes);
                }
                if (files.Any())
                {
                    try
                    {
                        var isSuc = await _mailSenderService.SendUseForInsuranceAsync("jinchenchen-pnb@sinosig.com ", $"新阳光_{model.PushType}_{model.OrderNo}_（{DateTime.Today.ToString("yyyy-MM-dd")}）", "增减推送", null, files);

                        if (isSuc)
                        {
                            model.Status = BusInsuranceApiPushStatusEnumType.已推送待确认;
                            await _service.UpdateNav(model);
                        }
                        else
                        {

                            throw new ErrorCodeException(-1, "邮件发送失败,请稍后再试!!");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ErrorCodeException(-1,ex.Message);
                    }
                }
            }


            return Ok();
        }

        /// <summary>
        /// 确认：已推送待确认 → 已确认，回写日匹配记录为已推送，并执行推送完成处理
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Confirm([FromForm] Guid id)
        {
            var today = DateTime.Today;
            var nextDay = DateTime.Today.AddDays(1);
            var insuranceMonth = today.AddDays(1 - today.Day);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);
      
            var model = await _service.GetNav(f => f.ID == id);
            if (model.Status!= BusInsuranceApiPushStatusEnumType.已推送待确认)
            {
                throw new ErrorCodeException(-1,"当前状态无发执行此操作");
            }


            if (model.InsuranceCompany == "大地365")
            {
                if (model?.Details != null && model.Details.Count > 0)
                {
                    var businessIds = model.Details
                        .SelectMany(f => new[] { f.AddBusinessID, f.DelBusinessID })
                        .Where(f => f > 0)
                        .Distinct()
                        .ToList();
                    if (businessIds.Count > 0)
                    {
                        var matchRecords = await _dayMatchRecordService.List(f => businessIds.Contains(f.ID));
                        if (matchRecords.Any())
                        {
                            foreach (var item in matchRecords)
                            {
                                item.Status = BusInsuranceDayMatchRecordStatusEnumType.已推送;
                            }
                            await _dayMatchRecordService.Update(matchRecords);
                            var addIds = model.Details.Select(f => f.AddBusinessID).ToList();
                            var delIds = model.Details.Select(f => f.DelBusinessID).ToList();
                            await _busInsuracneService.PushCompleted36k(addIds, delIds);

                            await _service.Confirm(id);
                        }
                    }
                }

            }
            else {
                if (model?.Details != null && model.Details.Count > 0)
                {
                        var dayMatchRecordIds = model.Details.Select(f => f.DayMatchRecordID??0);
                        var recordIds = model.Details.Select(f => f.InsuranceRecordID??0);
                        if (dayMatchRecordIds.Any())
                        {


                            if (model.PushType == "新增")
                            {
                                var matchRecordList = await _dayMatchRecordService.List(f => f.IsAddRecord && dayMatchRecordIds.Contains(f.ID));
                                // 将标记的记录标记为已推送 并生成保全记录
                                if (matchRecordList.Any())
                                {
                                    foreach (var matchRecord in matchRecordList)
                                    {
                                        matchRecord.Status = BusInsuranceDayMatchRecordStatusEnumType.已推送;
                                    }
                                    // 生成保全就
                                    var records = matchRecordList.Select(f => new Live_BusInsuranceRecord()
                                    {
                                        Status = Model.Enums.BusInsuranceRecordStatusEnumType.已增员,
                                        AddPushID = string.Empty,
                                        Age = f.Age,
                                        CardID = f.CardID,
                                        CreateTime = f.CreateTime,
                                        CustomerName = f.CustomerName,
                                        DelPushID = string.Empty,
                                        EndDate = insuranceMonthEnd,
                                        InsuranceCompany = f.InsuranceCompany,
                                        ID = 0,
                                        InsuranceMonth = insuranceMonth,
                                        IsDelMatch = false,
                                        MatchDate = today,
                                        Name = f.Name,
                                        OrderNo = f.OrderNo,
                                        PlanName = f.PlanName,
                                        PlanNameID = f.PlanNameID,
                                        PolicyNum = string.Empty,
                                        Sex = f.Sex,
                                        SignUnitName = f.SignUnitName,
                                        StartDate = f.StartDate,
                                        WorkClass = f.WorkClass,
                                        WorkName = f.WorkName,
                                        InsuranceWorkName = f.InsuranceWorkName,
                                    }).ToList();

                                    try
                                    {
                                        DbScoped.SugarScope.BeginTran();
                                        await _service.Confirm(id);
                                        await _dayMatchRecordService.Update(matchRecordList);
                                        await _recordService.Add(records);

                                    DbScoped.SugarScope.CommitTran();
                                    }
                                    catch (Exception ex)
                                    {
                                        DbScoped.SugarScope.RollbackTran();
                                        throw new ErrorCodeException(-1,ex.Message);
                                    }

                                }
                            }
                            else if (model.PushType == "减员")
                            {
                                var matchRecordList = await _dayMatchRecordService.List(f => f.IsAddRecord==false && dayMatchRecordIds.Contains(f.ID));
                                // 将标记的记录标记为已推送 并生成保全记录
                            
                                
                                    // 生成保全就
                                    var records = await _recordService.List(f => recordIds.Contains(f.ID));

                                    try
                                    {
                                    DbScoped.SugarScope.BeginTran();
                                    if (matchRecordList.Any())
                                    {
                                        foreach (var matchRecord in matchRecordList)
                                        {
                                            matchRecord.Status = BusInsuranceDayMatchRecordStatusEnumType.已推送;
                                        }

                                        await _dayMatchRecordService.Update(matchRecordList);
                                    }
                                    if (records.Any())
                                    {

                                        foreach (var item in records)
                                        {
                                            item.Status = BusInsuranceRecordStatusEnumType.已减员;
                                            item.EndDate = today;
                                        }
                                        await _recordService.Update(records);
                                    }
                                    DbScoped.SugarScope.CommitTran();
                                    }
                                    catch (Exception ex)
                                    {
                                        DbScoped.SugarScope.RollbackTran();
                                        throw new ErrorCodeException(-1, ex.Message);
                                    }

                            }

                        }
                }
            }

           
            return Ok();
        }

        /// <summary>
        /// 状态重置：已推送待处理 → 待推送
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ResetStatus([FromForm] Guid id)
        {
            var model = await _service.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "推送记录不存在");
            }
            if (model.Status != BusInsuranceApiPushStatusEnumType.已推送待处理)
            {
                throw new ErrorCodeException(-1, "仅已推送待处理状态可重置为待推送");
            }

            model.Status = BusInsuranceApiPushStatusEnumType.待推送;
            model.ConfirmUrl = string.Empty;
            if (model.Details != null)
            {
                foreach (var detail in model.Details)
                {
                    detail.Status = BusInsuranceApiPushDetailStatusEnumType.待核对;
                    detail.FailMsg = string.Empty;
                }
            }

            await _service.UpdateNav(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_BusInsuranceApiPushExtension model)
        {
            if (model == null || model.ID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "数据不能为空");
            }
            if (!await _service.Exist(f => f.ID == model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }

            if (model.Details != null)
            {
                foreach (var detail in model.Details)
                {
                    if (detail.ID == Guid.Empty)
                    {
                        detail.ID = Guid.NewGuid();
                    }
                    detail.BusInsuranceApiPushID = model.ID;
                    detail.CreateTime ??= DateTime.Now;
                }
            }

            await _service.UpdateNav(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDetail([FromBody] Live_BusInsuranceApiPushDetail model)
        {
            if (model == null || model.ID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "数据不能为空");
            }
            var exist = await _service.GetDetail(f => f.ID == model.ID);
            if (exist == null)
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await _service.UpdateDetail(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await _service.Exist(f => f.ID == id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await _service.DeleteNav(id);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDetail([FromForm] Guid id)
        {
            var model = await _service.GetDetail(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await _service.DeleteDetail(id);
            return Ok();
        }




        [HttpPost]
        public async Task<IActionResult> PushDaDi365ProcessJob()
        {
            var isb = await _redisService.SetAsync("PushDaDi365ProcessJob", "PushDaDi365ProcessJob", 60, CSRedis.RedisExistence.Nx);
            if (isb)
            {
                var pendingPushes = await _service.List(f => f.InsuranceCompany == "大地365" && f.Status != BusInsuranceApiPushStatusEnumType.已确认 && f.Status != BusInsuranceApiPushStatusEnumType.作废);
                if (pendingPushes.Any())
                {
                    foreach (var item in pendingPushes)
                    {
                        item.Status = BusInsuranceApiPushStatusEnumType.作废;
                    }
                    await _service.Update(pendingPushes);
                }

                await _busInsuracneService.PushDaDi365Process();

                return Ok(new { isSuccess = true, msg = "成功" });
            }
            else
            {
                return Ok(new { isSuccess = false, msg = "一分钟内只能推送一次" });
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PushNewYangGuangProcess()
        {
            var isb = await _redisService.SetAsync("PushNewYangGuangProcess", "PushNewYangGuangProcess", 1, CSRedis.RedisExistence.Nx);
            if (isb)
            {
                var pendingPushes = await _service.List(f =>f.InsuranceCompany== "阳光保险公司-新" && f.Status != BusInsuranceApiPushStatusEnumType.已确认 && f.Status != BusInsuranceApiPushStatusEnumType.作废);
                if (pendingPushes.Any())
                {
                    foreach (var item in pendingPushes)
                    {
                        item.Status = BusInsuranceApiPushStatusEnumType.作废;
                    }
                    await _service.Update(pendingPushes);
                }

                await _busInsuracneService.PushNewYangGuangProcess();

                return Ok(new { isSuccess = true, msg = "成功" });
            }
            else
            {
                return Ok(new { isSuccess = false, msg = "一分钟内只能推送一次" });
            }
        }



        public record ExportBySignUnitFileRecord(string FileName,string FileBase64);
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ExportBySignUnit([FromForm] Guid id)
        {
            var model = await _service.GetNav(f => f.ID == id);
            List<ExportBySignUnitFileRecord> files = new List<ExportBySignUnitFileRecord>();
            if (model.PushType == "新增")
            {
               var groups= model.Details.GroupBy(f => new { f.SignUnitName, f.OrderNo }).ToList();
                foreach (var item in groups)
                {

                   

                    var pushRecords = item.ToList().Where(f => f.Status == BusInsuranceApiPushDetailStatusEnumType.已核对).Select(f => new NewYangGuangExcelTemplate()
                    {
                        CardID = f.CardID,
                        CustomerName = f.CustomerName,
                        Name = f.Name,
                        CardType = "01-身份证",
                        ChangeFlag = "I-新增",
                        ExistCardID = string.Empty,
                        ExistCardType = string.Empty,
                        ExistName = string.Empty,
                        HealthInfo = string.Empty,
                        InsuranceWorkName = f.InsuranceWorkName,
                        MonthSalaryAmount = string.Empty,
                        SalaryCurrencyName = string.Empty,
                        Sex = f.Sex,
                        Age = f.Age,
                    }).ToList();

                
                    var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "表单名称");
                    files.Add(new ExportBySignUnitFileRecord($"{item.Key.SignUnitName.Trim()}-增员.xlsx",Convert.ToBase64String(fileBytes)) );
                }

            }
            else if (model.PushType == "减员")
            {
                var groups = model.Details.GroupBy(f => new { f.SignUnitName, f.OrderNo }).ToList();
                foreach (var item in groups)
                {
                    var pushRecords = item.ToList().Where(f => f.Status == BusInsuranceApiPushDetailStatusEnumType.已核对).Select(f => new NewYangGuangExcelTemplate()
                    {
                        CardID = string.Empty,
                        CustomerName = f.CustomerName,
                        Name = string.Empty,
                        ExistName = f.Name,
                        ExistCardType = "01-身份证",
                        ExistCardID = f.CardID,
                        CardType = string.Empty,
                        ChangeFlag = "D-删除",
                        HealthInfo = string.Empty,
                        InsuranceWorkName = string.Empty,
                        MonthSalaryAmount = string.Empty,
                        SalaryCurrencyName = string.Empty,
                        Sex = f.Sex,
                        Age = f.Age,
                    }).ToList();

                  
                    var fileBytes = NpoiExcelHelper.WriteExcel(pushRecords, "表单名称");
                    files.Add(new ExportBySignUnitFileRecord($"{item.Key.SignUnitName.Trim()}-减员.xlsx", Convert.ToBase64String(fileBytes)));
                }
            }
            return Ok(files);
        }



        private static void ApplyPushResult(Live_BusInsuranceApiPushExtension model, ThirtySixCusResponse res, bool isReplace)
        {
            if (res == null)
            {
                model.Status = BusInsuranceApiPushStatusEnumType.待推送;
                foreach (var detail in model.Details)
                {
                    detail.Status = BusInsuranceApiPushDetailStatusEnumType.核对失败;
                    detail.FailMsg = "推送无返回结果";
                }
                return;
            }

            if (res.IsSuccess || res.Code == 0 || res.Code == 200)
            {
                model.Status = BusInsuranceApiPushStatusEnumType.已推送待确认;
                model.ConfirmUrl = res.ConfirmUrl;
                foreach (var detail in model.Details)
                {
                    detail.Status = BusInsuranceApiPushDetailStatusEnumType.已核对;
                    detail.FailMsg = string.Empty;
                }
                return;
            }

            model.Status = BusInsuranceApiPushStatusEnumType.已推送待处理;
            model.ConfirmUrl = string.Empty;
            var results = res.Records;
            if (results == null || results.Count == 0)
            {
                var failMsg = !string.IsNullOrWhiteSpace(res.ErrorMsg) ? res.ErrorMsg : (!string.IsNullOrWhiteSpace(res.Msg) ? res.Msg : "推送失败");
                foreach (var detail in model.Details)
                {
                    detail.Status = BusInsuranceApiPushDetailStatusEnumType.核对失败;
                    detail.FailMsg = failMsg;
                }
                return;
            }

            foreach (var detail in model.Details)
            {
                ThirtySixResponseRecord item;
                if (isReplace)
                {
                    item = results.FirstOrDefault(f => f.IdNumber == detail.CardID) ?? results.FirstOrDefault(f => f.AttritionIdNumber == detail.DelCardID);
                }
                else
                {
                    item = results.FirstOrDefault(f => f.IdNumber == detail.CardID);
                }

                if (item == null)
                {
                    detail.Status = BusInsuranceApiPushDetailStatusEnumType.核对失败;
                    detail.FailMsg = "未匹配到推送结果";
                    continue;
                }
                if (item.ErrorMessage == "可以导入" || string.IsNullOrWhiteSpace(item.ErrorMessage))
                {
                    detail.Status = BusInsuranceApiPushDetailStatusEnumType.已核对;
                    detail.FailMsg = item.ErrorMessage;
                }
                else
                {
                    detail.Status = BusInsuranceApiPushDetailStatusEnumType.核对失败;
                    detail.FailMsg = item.ErrorMessage;
                }
            }
        }

    }
}
