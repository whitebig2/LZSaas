using LZ.Core.Attributes;

using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using LZ.Core.EventData.Insurance;
using LZ.Core.RabbitMQ.EventBus;
using SqlSugar;
using System.Security.Policy;
using LZ.Core.EventData;
using LZ.Model.Exceptions;
using LZ.Model.Enums;
using LZ.Users.Service.Interface;
using LZ.Common;
using LZ.Model.IExcelTemplate;
using LZ.Model;
using LZ.Model.Dtos.SpecialDeductionDtos;
using System.Xml.Schema;
using LZ.Model.ExcelTemplateModel;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar.IOC;
using LZ.Core.Redis;

namespace LZ.Users.API.Controllers
{
    [AllowAnonymous]
    public class BusinessInsuraceRecordController : UsersControllerBase
    {


        private readonly IRedisService   _redisService;
        private readonly IBusinessInsurancService   _businessInsurancService;
        private readonly IBusinessInsuranceOrderService   _orderService;
        private readonly IEntryService  _entryService;
        private readonly IBusinessInsuranceConfigService _configService;
        private readonly IBusinessInsuranceRecordService _businessInsuranceRecordService;

        public BusinessInsuraceRecordController(IBusinessInsuranceRecordService businessInsuranceRecordService, IBusinessInsuranceConfigService configService, IEntryService entryService, IBusinessInsuranceOrderService orderService, IBusinessInsurancService businessInsurancService, IRedisService redisService)
        {
            _businessInsuranceRecordService = businessInsuranceRecordService;
            _configService = configService;
            _entryService = entryService;
            _orderService = orderService;
            _businessInsurancService = businessInsurancService;
            _redisService = redisService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string name,string idCardNo, BusinessInsuranceStatusEnumType?status, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, f => f.Status==status.Value);
            var models = await _businessInsuranceRecordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string customerName, string name, string idCardNo, BusinessInsuranceStatusEnumType? status, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            var dataList = await _businessInsuranceRecordService.List(exp.ToExpression());
            var exportDataList = dataList.Select(f => new BusInsuranceRecordExcelTemplate()
            {
                Name = f.Name,
                IdCardNo = f.IdCardNo,
                Age = f.Age,
                InsuranceId = f.InsuranceId,
                PlanName = f.PlanName,
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
                InsuranceSigningUnitName = f.InsuranceSigningUnitName,
                CustomerName = f.CustomerName,
                StartDate = f.StartDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = f.EndDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateTime = f.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                StatusName = f.Status.ToString(),
                InsuranceCompany=f.InsuranceCompany
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "商保记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
       
        }


        [HttpPost]
        public async Task<IActionResult> ImportRecord(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<BusInsuranceRecordImportExcelTemplate>(ms, "");

                if (!excelData.Any()&&excelData.Any(f=>f.StatusName=="已增员"||f.StatusName=="待减员匹配"))
                {
                    throw new ErrorCodeException(-1, "只能导入【已增员、待减员匹配】状态数据！");
                }
                List<Live_BusinessInsuranceRecord> records = new List<Live_BusinessInsuranceRecord>();
                Live_BusinessInsuranceRecord record = null;
                foreach (var item in excelData)
                {
                    var config = await _configService.GetView(f => f.CustomerName == item.CustomerName && f.InsuranceCompany == item.InsuranceCompany && f.IsEnabled);


                    if (config==null)
                    {
                        throw new ErrorCodeException(-1, $"{item.CustomerName}--{item.InsuranceCompany}配置信息缺失或未启用");
                    }
                    record = new Live_BusinessInsuranceRecord()
                    {
                        Name = item.Name,
                        IdCardNo = item.IdCardNo,
                        CreateTime = DateTime.Now,
                        CustomerGID = config.CustomerGID,
                        CustomerName = item.CustomerName,
                        ID = Guid.NewGuid(),
                        InsuranceCompany = item.InsuranceCompany,
                        InsuranceId = item.InsuranceId,
                        IsMain = config.IsMain,
                        InsuranceSigningUnitName = config.InsuranceSigningUnitName,
                        InsuranceSigningUnitID = config.InsuranceSigningUnitID,
                        OrderNo=string.Empty,
                        InsuranceNo=string.Empty,
                         IsExpired=false,
                         WorkClass=item.WorkClass,
                         WorkName=item.WorkName,
                         StartDate=DateTime.MinValue,
                         EndDate=DateTime.MinValue
                    };

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(item.EntryDate))
                        {
                            record.EntryDate = Convert.ToDateTime(item.EntryDate);
                        }
                        if (!string.IsNullOrWhiteSpace(item.LeaveDate))
                        {
                            record.LeaveDate = Convert.ToDateTime(item.LeaveDate);
                        }

                    }
                    catch (Exception ex)
                    {

                        throw new ErrorCodeException(-1, "入职时间或者离职时间转换失败，请检查数据！");
                    }

                 

                    var exp = Expressionable.Create<Live_UserEntry>();
                    exp.And(f => f.Name == item.Name && f.CardID == item.IdCardNo && f.WorkClass == item.WorkClass && f.WorkName == item.WorkName &&f.EntryTime.HasValue&& f.EntryTime.Value.Date == Convert.ToDateTime(item.EntryDate));
                    exp.AndIF(record.LeaveDate.HasValue, f => f.LeaveTime ==record.LeaveDate);
                    var entry = await _entryService.Get(exp.ToExpression());

                    if (entry==null)
                    {
                        throw new ErrorCodeException(-1, $"{item.CustomerName}--{item.InsuranceCompany}--{item.Name} 员工信息匹配失败！");
                    }

                    if (entry.WorkClass == "3类")
                    {
                        record.PlanName = config.Plan3;
                    }
                    else if (entry.WorkClass == "4类") { record.PlanName = config.Plan4; }
                    else if (entry.WorkClass == "5类") { record.PlanName = config.Plan5; }



                    record.Status = Enum.Parse<BusinessInsuranceStatusEnumType>(item.StatusName);
                    var ageAndSex = IdCardHelper.GetAgeAndGender(record.IdCardNo);
                    record.Age = ageAndSex.Age;
                    record.Sex = ageAndSex.Gender;
                    record.UserEntryID = entry.ID;
                    record.UserID = entry.UserID ?? 0;
                    record.EntryDate = entry.EntryTime;
                    record.LeaveDate = entry.LeaveTime;


                    if (!string.IsNullOrWhiteSpace(record.InsuranceId))
                    {
                        var plan = await _orderService.GetOrderPlanView(f => f.InsuranceId == record.InsuranceId && f.GnteeName == record.PlanName);

                        if (plan==null)
                        {
                            throw new ErrorCodeException(-1, $"{item.CustomerName}--{item.InsuranceCompany}--{item.Name}--{record.PlanName} 订单信息匹配失败，请检查配置或订单编号 ！");
                        }

                        record.PlanNameId = plan.GnteeId;
                        record.IsDefaultPush = plan.IsDefaultPush;
                        record.IsDayOrder = plan.OrderType == 0;
                        record.StartDate = plan.StartDate;
                        record.EndDate = plan.EndDate;
                    }

                    records.Add(record);
                }
                if (records.Any())
                {
                    await _businessInsuranceRecordService.Add(records);
                }

            }

            return Ok();
        }

        [HttpPost]
        public IActionResult DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<BusInsuranceRecordImportExcelTemplate>(stream, "商保记录导入模板");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保记录导入模板.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpPost]
        public IActionResult DownloadTemplate1()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<BusInsuranceRecordImportExcelTemplate>(stream, "商保记录导入模板");
            byte[] fileBytes = stream.ToArray();


            string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileBytes, XlsxContentType, "商保记录导入模板.xlsx");
        }


        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(it => it.CustomerName == query.CustomerName);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name == query.Name);
            }
            if (query.Main.HasValue)
            {
                expable.And(it => it.IsMain == query.Main);
            }
            expable.AndIF(query.Time.HasValue, t => t.StartDate < query.Time && t.EndDate > query.Time);
            var exp = expable.ToExpression();
            var models = await _businessInsuranceRecordService.List(exp);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_BusinessInsuranceRecord>();
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(it => it.CustomerName == query.CustomerName);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name == query.Name);
            }
            if (query.Main.HasValue)
            {
                expable.And(it => it.IsMain == query.Main);
            }
            expable.AndIF(query.Time.HasValue, t => t.StartDate < query.Time && t.EndDate > query.Time);
            var exp = expable.ToExpression();
            var model = await _businessInsuranceRecordService.Get(exp);
            return Ok(model);
        }

        //[HttpPut]
        //public async Task<IActionResult> Update([FromBody] Model.Live_Enterprise model)
        //{
        //    if (!await businessInsuranceRecordService.Exist(model.ID))
        //    {
        //        throw new ErrorCodeException(-1, "你要修改的数据不存在");
        //    }
        //    await businessInsuranceRecordService.Update(model);
        //    return Ok();
        //}


        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _businessInsuranceRecordService.Delete(id);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm] Guid id, [FromForm] BusinessInsuranceStatusEnumType status)
        {
            var model = await _businessInsuranceRecordService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }

            if (status== BusinessInsuranceStatusEnumType.待增员匹配)
            {
                model.BusinessInsurancePushID = null;
                model.AddMatchRecordID = null;
            }


            if (status == BusinessInsuranceStatusEnumType.待减员匹配)
            {
                model.BusinessInsurancePushID = null;
                model.DelMatchRecordID = null;
            }

            model.Status = status;

            
            await _businessInsuranceRecordService.Update(model);
            return Ok();
        }



        [HttpPost]
        public async Task<IActionResult> ImportBaoQuanRecord(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<BusInsuranceRecordImportExcelTemplate>(ms, "");
                var configList = await _configService.ListView(f => f.IsEnabled);

              var insuranceIdList=  excelData.Select(f=>f.InsuranceId.Trim()).Distinct().ToList();


                var orderPlans = await _orderService.ListOrderPlanView(f => insuranceIdList.Contains(f.InsuranceId));

                List<Live_BusinessInsuranceRecord> records = new List<Live_BusinessInsuranceRecord>();
                Live_BusinessInsuranceRecord record = null;
                Live_V_BusinessInsuranceConfig config;
                Live_UserEntry userEntry;
                string insuranceCompany = string.Empty;

                //excelData = excelData.Where(f => f.Name == "张永菊").ToList();
                try
                {
                    DbScoped.SugarScope.BeginTran();
                    foreach (var item in excelData)
                    {



                        config = configList.FirstOrDefault(f => f.CustomerName == item.CustomerName && f.InsuranceCompany == item.InsuranceCompany);

                        if (config == null)
                        {
                            throw new ErrorCodeException(-1, $"配置异常：{item.CustomerName}--{item.Name}");
                        }

                        var entryList = await _entryService.List(f => f.Name == item.Name && f.CardID == item.IdCardNo && f.CustomerGID == config.CustomerGID);


                        if (entryList == null || entryList.Count <= 0)
                        {
                            throw new ErrorCodeException(-1, $"员工信息匹配异常：{item.CustomerName}--{item.Name}--{item.IdCardNo} 信息不存在！");
                        }

                        userEntry = entryList.FirstOrDefault(f => f.Status == 2);
                        //if (item.ApplyType == "增员")
                        //{
                        //    userEntry = entryList.FirstOrDefault(f => f.Status == 2);
                        //}
                        //else {
                        //    userEntry = entryList.FirstOrDefault(f => f.Status == 4);
                        //}

                        if (userEntry == null)
                        {
                            userEntry = entryList.FirstOrDefault(f => f.Status == 4);
                        }

                        if (userEntry == null)
                        {
                            throw new ErrorCodeException(-1, $"员工信息匹配异常：{item.CustomerName}--{item.Name}--{item.IdCardNo}在职或离职记录信息不存在！");
                        }

                        string planName=string.Empty;
                        if (item.WorkClass == "3类")
                        {
                            planName = config.Plan3;
                        }
                        else if (item.WorkClass == "4类")
                        {
                            planName = config.Plan4;
                        }
                        else if (item.WorkClass == "5类")
                        {
                            planName = config.Plan5;
                        }

                        if (string.IsNullOrWhiteSpace(planName))
                        {
                            throw new ErrorCodeException(-1, $"员工信息匹配异常：{item.CustomerName}--{item.Name}--{item.IdCardNo} 参保方案获取失败！");

                        }
                        item.InsuranceId = item.InsuranceId.Trim();
                        var orderPlan = orderPlans.FirstOrDefault(f => f.InsuranceId == item.InsuranceId && f.GnteeName == planName);
                        if (orderPlan == null)
                        { 
                            throw new ErrorCodeException(-1, $"参保方案不存在：{item.CustomerName}--{item.Name}--订单编号：{item.InsuranceId}_方案：{planName}！");
                        }

                        try
                        {
                            DbScoped.SugarScope.BeginTran();

                            var ageSex = IdCardHelper.GetAgeAndGender(item.IdCardNo);

                            record = new Live_BusinessInsuranceRecord()
                            {
                                Name = item.Name,
                                IdCardNo = item.IdCardNo,
                                CreateTime = DateTime.Now,
                                CustomerGID = config.CustomerGID,
                                CustomerName = item.CustomerName,
                                ID = Guid.NewGuid(),
                                InsuranceCompany = item.InsuranceCompany,
                                InsuranceId = item.InsuranceId,
                                IsMain = config.IsMain,
                                InsuranceSigningUnitName = config.InsuranceSigningUnitName,
                                InsuranceSigningUnitID = Guid.Empty,
                                OrderNo = "AABBCC",
                                InsuranceNo = string.Empty,
                                IsExpired = false,
                                WorkClass = item.WorkClass + "类",
                                WorkName = item.WorkName,
                                StartDate = Convert.ToDateTime(orderPlan.StartDate).Date,
                                EndDate = Convert.ToDateTime(orderPlan.EndDate).Date,
                                EntryDate = userEntry.EntryTime.Value.Date,
                                LeaveDate = userEntry.LeaveTime,
                                Age = ageSex.Age,
                                AddPushTime = DateTime.Today,
                                AddSysTime = DateTime.Today,
                                UserID = userEntry.UserID ?? 0,
                                UserEntryID = userEntry.ID,
                                IsDefaultPush = config.IsDefaultPush,
                                IsDayOrder = orderPlan.OrderType == (int)InsuranceOrderEnumType.日保单,
                                PlanName = orderPlan.GnteeName,
                                PlanNameId = orderPlan.GnteeId,
                                Sex = ageSex.Gender,
                                Status = BusinessInsuranceStatusEnumType.已增员
                            };

                            await _businessInsuranceRecordService.Add(record);

                            userEntry.PlanName1 = planName;
                            userEntry.WorkClass = item.WorkClass;
                            userEntry.WorkName = item.WorkName;

                            //if (item.ApplyType == "增员")
                            //{
                            //    userEntry.PlanStatus1 = UserEntryPlanStatusEnumType.已增员;
                            //}
                            //else {
                            //    userEntry.PlanStatus1 = UserEntryPlanStatusEnumType.已减员;
                            //}
                            userEntry.PlanStatus1 = UserEntryPlanStatusEnumType.已增员;
                            await _entryService.UpdateBusInsurance(userEntry);
                            DbScoped.SugarScope.CommitTran();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"异常员工：{item.CustomerName}--{item.Name}");
                        }
                    }
                    DbScoped.SugarScope.CommitTran();
                }
                catch (Exception ex)
                {
                    throw new ErrorCodeException(-1, ex.Message);
                }

            }

            return Ok();
        }

        /// <summary>
        /// 导出保全名单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ExportEmpInsuranceNames([FromForm] string insuranceCompany)
        {
            //EmpInsuranceNamesExcelTemplate

            var today= DateTime.Today;
            //约定是23号，不包含23号
            DateTime dt = new DateTime(today.Year, today.Month, 24);

           var configList=await _configService.ListView(f =>f.CustomerName== "宁波普锐流体科技有限公司"&& f.InsuranceCompany==insuranceCompany&& f.IsEnabled);

           //var configGroups= configList.GroupBy(f => f.CustomerGID).ToList();

            List<EmpInsuranceNamesExcelTemplate> recordList = new List<EmpInsuranceNamesExcelTemplate>();
            EmpInsuranceNamesExcelTemplate record = null;
            foreach (var config in configList)
            {

                var entryList = await _entryService.List(f => (f.Status == 2 || f.Status == 4)|| f.CustomerGID == config.CustomerGID && f.EntryTime.HasValue && f.EntryTime.Value < dt && (f.LeaveTime == null || f.LeaveTime.Value >= dt));
                // 主险
                if (config.IsMain)
                {
                    foreach (var entry in entryList)
                    {
                        if (entry.PlanStatus1.HasValue && entry.PlanStatus1 == UserEntryPlanStatusEnumType.未参保)
                        {
                            continue;
                        }

                        record = new EmpInsuranceNamesExcelTemplate()
                        {
                            InsuranceCompany = config.InsuranceCompany,
                            CustomerName = config.CustomerName,
                            IdCardNo = entry.CardID,
                            IdCardType = "身份证",
                            InsuranceSigningUnitName = config.InsuranceSigningUnitName,
                            IsContain = true,
                            Name = entry.Name,
                            Remark = string.Empty,
                            WorkCity = config.City,
                            WorkProvince = config.Province,
                            WorkClass = entry.WorkClass?.Replace("类", ""),
                            WorkName = entry.WorkName,
                            IsOnJob = entry.Status == 2
                        };

                        if (entry.PlanStatus1 == null)
                        {
                            record.IsContain = false;
                            record.Remark = "员工信息【主险状态】信息缺失";
                        }
                        if (string.IsNullOrWhiteSpace(record.WorkClass))
                        {
                            record.IsContain = false;
                            if (!string.IsNullOrWhiteSpace(record.Remark))
                            {
                                record.Remark += "，";
                            }
                            record.Remark = "【职业类别】信息缺失";
                        }
                        else {

                            switch (record.WorkClass)
                            {
                                case "3":
                                    record.PlanName = config.Plan3;
                                    break;
                                case "4":
                                    record.PlanName = config.Plan4;
                                    break;
                                case "5":
                                    record.PlanName = config.Plan5;
                                    break;
                                default:
                                    break;
                            }

                        }

                        if (string.IsNullOrWhiteSpace(record.WorkName))
                        {
                            record.IsContain = false;
                            if (!string.IsNullOrWhiteSpace(record.Remark))
                            {
                                record.Remark += "，";
                            }
                            record.Remark += "【雇佣工种】信息缺失";
                        }
                        recordList.Add(record);
                    }
                }
                else
                {

                    foreach (var entry in entryList)
                    {
                        if (entry.PlanStatus2.HasValue && entry.PlanStatus2 == UserEntryPlanStatusEnumType.未参保)
                        {
                            continue;
                        }

                        record = new EmpInsuranceNamesExcelTemplate()
                        {
                            InsuranceCompany = config.InsuranceCompany,
                            CustomerName = config.CustomerName,
                            IdCardNo = entry.CardID,
                            IdCardType = "身份证",
                            InsuranceSigningUnitName = config.InsuranceSigningUnitName,
                            IsContain = false,
                            Name = entry.Name,
                            Remark = string.Empty,
                            WorkCity = config.City,
                            WorkProvince = config.Province,
                            WorkClass = entry.WorkClass?.Replace("类", ""),
                            WorkName = entry.WorkName,
                        };

                        if (entry.PlanStatus2 == null)
                        {
                            record.IsContain = false;
                            record.Remark = "员工信息【次险状态】信息缺失";
                        }
                        if (string.IsNullOrWhiteSpace(record.WorkClass))
                        {
                            record.IsContain = false;
                            if (!string.IsNullOrWhiteSpace(record.Remark))
                            {
                                record.Remark += "，";
                            }
                            record.Remark = "【职业类别】信息缺失";
                        }
                        else
                        {

                            switch (record.WorkClass)
                            {
                                case "3":
                                    record.PlanName = config.Plan3;
                                    break;
                                case "4":
                                    record.PlanName = config.Plan4;
                                    break;
                                case "5":
                                    record.PlanName = config.Plan5;
                                    break;
                                default:
                                    break;
                            }

                        }

                        if (string.IsNullOrWhiteSpace(record.WorkName))
                        {
                            record.IsContain = false;
                            if (!string.IsNullOrWhiteSpace(record.Remark))
                            {
                                record.Remark += "，";
                            }
                            record.Remark += "【雇佣工种】信息缺失";
                        }
                        recordList.Add(record);
                    }
                }
            }
            var fileBytes = NpoiExcelHelper.WriteExcel(recordList, "保全名单");
            string base64String = Convert.ToBase64String(fileBytes);
            //return Ok(new { base64String, fileName = "工资记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "11.xlsx");


        }

        #region 商保推送流程
        [HttpPost]
        public async Task BusinessPushJob()
        {

            var cacheKey = $"BusinessPushJob";
            var isb = await _redisService.SetAsync(cacheKey, "11", TimeSpan.FromMinutes(5), CSRedis.RedisExistence.Nx);
            if (!isb)
            {
                throw new ErrorCodeException(-1, "此定时任务5分钟内只能触发一次");
            }

            await _businessInsurancService.PushProcess();

           await Task.Delay(1000);

            await _businessInsurancService.CheckProcess();
        }
        [HttpPost]
        public async Task BusinessCheckJob()
        {
            await _businessInsurancService.CheckProcess();
        }
        [HttpPost]
        public async Task BusinessNoticeJob()
        {
            await _businessInsurancService.EmailNotice();
        }
        #endregion
    }
}
