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
using System.Linq;
using NPOI.XSSF.UserModel;
using SqlSugar.SplitTableExtensions;
using System.DirectoryServices.ActiveDirectory;
using Senparc.NeuChar.NeuralSystems;

namespace LZ.Users.API.Controllers
{
    [AllowAnonymous]
    public class BusinessInsuranceInitialRecordController : UsersControllerBase
    {


        private readonly IBusinessInsuranceInitialRecordService _initialRecordService;
        private readonly IEntryService _entryService;
        private readonly IBusinessInsuranceConfigService _configService;
        private readonly IBusinessInsuranceOrderService _orderService;

        public BusinessInsuranceInitialRecordController(IBusinessInsuranceInitialRecordService initialRecordService, IEntryService entryService, IBusinessInsuranceConfigService configService, IBusinessInsuranceOrderService orderService)
        {
            _initialRecordService = initialRecordService;
            _entryService = entryService;
            _configService = configService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, string name, string idCardNo, BusInsuranceInitialStatusEnumType? status, string insuranceCompany,DateTime? insuranceMonth ,[FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BusinessInsuranceInitialRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(insuranceMonth.HasValue, f => f.InsuranceMonth == insuranceMonth.Value);
            var models = await _initialRecordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string customerName, string name, string idCardNo, BusInsuranceInitialStatusEnumType? status, string insuranceCompany, DateTime? insuranceMonth)
        {
            var exp = Expressionable.Create<Model.Live_BusinessInsuranceInitialRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));

            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(status.HasValue, f => f.Status == status.Value);
            exp.AndIF(insuranceMonth.HasValue, f => f.InsuranceMonth == insuranceMonth.Value);

            var dataList = await _initialRecordService.List(exp.ToExpression());
            var exportDataList = dataList.Select(f => new BusinessInsuranceInitialRecordExportExcelTemplate()
            {
                Name = f.Name,
                IdCardNo = f.IdCardNo,
                InsuranceId = f.InsuranceId,
                PlanName = f.PlanName,
                WorkClass = f.WorkClass,
                WorkName = f.WorkName,
                InsuranceSigningUnitName = f.InsuranceSigningUnitName,
                CustomerName = f.CustomerName,
                EntryDate = f.EntryDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                DataTypeName = f.DataType.ToString(),
                Msg = f.Msg,
                Remark = f.Remark,
                StatusName = f.Status.ToString(),
                InsuranceCompany = f.InsuranceCompany,
                ActionTypeName = f.ActionType.ToString(),
                IsMain = f.IsMain,
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "商保记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

        }


        [HttpPost]
        public async Task<IActionResult> ImportRecord(IFormFile file)
        {
            var currentMonth = DateTime.Now.Date.AddDays(1 - DateTime.Today.Day);
            var InsuranceMonth = currentMonth.AddMonths(1);
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<BusinessInsuranceInitialRecordImportExcelTemplate>(ms, "");

              var orderPlans =await _orderService.ListOrderPlanView(f => f.StartDate >= InsuranceMonth);


                List<Live_BusinessInsuranceInitialRecord> records = new List<Live_BusinessInsuranceInitialRecord>();
                Live_BusinessInsuranceInitialRecord record = null;
                foreach (var item in excelData)
                {
                    if (item.PlanName!="90万+9万")
                    {
                        continue;
                    }
                  var orderPlan=  orderPlans.FirstOrDefault(f => f.InsuranceId == item.InsuranceId&&f.GnteeName==item.PlanName);




                    record = new Live_BusinessInsuranceInitialRecord()
                    {
                        Name = item.Name,
                        IdCardNo = item.IdCardNo,
                        CreateTime = DateTime.Now,
                        CustomerName = item.CustomerName,
                        ID = Guid.NewGuid(),
                        InsuranceCompany = item.InsuranceCompany,
                        InsuranceId = item.InsuranceId,
                        InsuranceSigningUnitName = item.InsuranceSigningUnitName,
                        WorkClass = item.WorkClass,
                        WorkName = item.WorkName,
                        DataType = BusInsuranceInitialDataTypeEnumType.初始导入,
                        EntryDate = item.EntryDate,
                        InsuranceMonth = InsuranceMonth,
                        Msg = string.Empty,
                        PlanName = orderPlan.GnteeName,
                        PlanNameId = orderPlan.GnteeId,
                        StartDate = orderPlan.StartDate,
                        EndDate = orderPlan.EndDate,
                        IsDayOrder = orderPlan.OrderType.Value == (int)InsuranceOrderEnumType.日保单,
                        IsDefaultPush = orderPlan.IsDefaultPush,
                        ResultMsg = string.Empty,
                        Remark = item.Remark,
                        Status = BusInsuranceInitialStatusEnumType.待匹配,
                        UserEntryID = 0,
                        IsMain = true,
                        ActionType = BusInsuranceInitialActionTypeEnumType.无,
                    };
                    records.Add(record);
                }
                if (records.Any())
                {

                    records = records.Where(f => !string.IsNullOrWhiteSpace(f.InsuranceId)).ToList();

                    foreach (var item in records)
                    {
                        var modelView = await _entryService.View(f => f.CustomerName == item.CustomerName && f.Name == item.Name && f.CardID == item.IdCardNo && f.EntryTime.Value.Date == item.EntryDate.Value);
                        item.UserEntryID = modelView.ID;
                    }


                   var failRecords= records.Where(f => f.UserEntryID <= 0);


                    if (failRecords.Any())
                    {
                        throw new ErrorCodeException(-1, $"信息不匹配：{string.Join(',', failRecords.Select(f=>f.Name))}");
                    }

                    await _initialRecordService.Add(records);
                }

            }

            return Ok();
        }

        [HttpPost]
        public IActionResult DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<BusinessInsuranceInitialRecordImportExcelTemplate>(stream, "保全初始名单");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "保全初始名单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpPost]
        public IActionResult DownloadTemplate1()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<BusinessInsuranceInitialRecordImportExcelTemplate>(stream, "商保记录导入模板");
            byte[] fileBytes = stream.ToArray();


            string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileBytes, XlsxContentType, "商保记录导入模板.xlsx");
        }




        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {

            var model = await _initialRecordService.Get(f => f.ID == id);
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
            await _initialRecordService.Delete(id);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> BatchDelete([FromForm] string insuranceCompany,[FromForm]DateTime? insuranceMonth)
        {

            if (string.IsNullOrWhiteSpace(insuranceCompany))
            {
                throw new ErrorCodeException(-1, "批量删除保险公司不能为空");
            }
            if (!insuranceMonth.HasValue)
            {
                throw new ErrorCodeException(-1, "保险月份不能为空");
            }
            var exp=  Expressionable.Create<Live_BusinessInsuranceInitialRecord>();
            exp.And(f => f.InsuranceCompany == insuranceCompany);
            exp.And(f => f.InsuranceMonth == insuranceMonth.Value);
            await _initialRecordService.Delete(exp.ToExpression());
            return Ok();
        }


        /// <summary>
        /// 导出系统保全名单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ExportSysUserEntryInitialRecord([FromForm] string insuranceCompany)
        {
            //EmpInsuranceNamesExcelTemplate

            var today = DateTime.Today;
            //约定是23号，不包含23号
            DateTime dt = new DateTime(today.Year, today.Month, 24);

            var configList = await _configService.ListView(f =>f.InsuranceCompany == insuranceCompany && f.IsEnabled);

            //var configGroups= configList.GroupBy(f => f.CustomerGID).ToList();

            List<EmpInsuranceNamesExcelTemplate> recordList = new List<EmpInsuranceNamesExcelTemplate>();
            EmpInsuranceNamesExcelTemplate record = null;
            foreach (var config in configList)
            {

                var entryList = await _entryService.List(f => (f.Status == 2 || f.Status == 4) && f.CustomerGID == config.CustomerGID && f.EntryTime.HasValue && f.EntryTime.Value < dt && (f.LeaveTime == null || f.LeaveTime.Value >= dt));
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
                            IsOnJob = entry.Status == 2,
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
          

            recordList= recordList.OrderBy(f=>f.CustomerName).ToList();
            int num = 0;

            foreach (var item in recordList)
            {
                num++;
                item.No = num.ToString();
                item.InsuranceId = string.Empty;
            }


            var fileBytes = NpoiExcelHelper.WriteExcel(recordList, "保全名单");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "保全名单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            //return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "11.xlsx");


        }


        ///// <summary>
        ///// 根据保险公司匹配员工记录
        ///// </summary>
        ///// <param name="insuranceCompany"></param>
        ///// <returns></returns>
        //[HttpPut]
        //public async Task<IActionResult> MatchUserEntry([FromForm] string insuranceCompany)
        //{
        //    var configList = await _configService.ListView(f => f.IsEnabled && f.InsuranceCompany == insuranceCompany);
        //    var recordList = await _initialRecordService.List(f => f.Status == BusInsuranceInitialStatusEnumType.待匹配 && f.InsuranceCompany == insuranceCompany);
        //    List<Live_V_UserEntry> userEntryList = null;
        //    Live_V_UserEntry userEntry = null;

        //    foreach (var item in recordList)
        //    {
        //        userEntryList = new List<Live_V_UserEntry>();
        //        var config = configList.FirstOrDefault(f => f.InsuranceCompany == item.InsuranceCompany && f.CustomerName == item.CustomerName);

        //        if (config == null)
        //        {
        //            item.Msg = $"客户单位商保配置缺失！";
        //            item.Status = BusInsuranceInitialStatusEnumType.已处理;
        //            item.ActionType = BusInsuranceInitialActionTypeEnumType.人工处理;
        //            continue;
        //        }
        //        item.IsMain = config.IsMain;
        //        if (config.InsuranceSigningUnitName != item.InsuranceSigningUnitName)
        //        {
        //            item.Msg = $"配置中的商保签章单位：{config.InsuranceSigningUnitName}与导入的商保劳动合同单位 不一致！";
        //            item.Status = BusInsuranceInitialStatusEnumType.已处理;
        //            item.ActionType = BusInsuranceInitialActionTypeEnumType.人工处理;
        //            continue;
        //        }




        //        userEntryList = await _entryService.ListView(f => ((config.IsMain && f.PlanName1 == item.PlanName) || (!config.IsMain && f.PlanName2 == item.PlanName)) && f.EntryTime.HasValue && f.EntryTime.Value == item.EntryDate && f.Name == item.Name && f.CardID == item.IdCardNo && f.CustomerName == item.CustomerName);
        //        userEntry = userEntryList.FirstOrDefault(f => f.Status == 2 && f.WorkClass == item.WorkClass && f.WorkName == item.WorkName && f.PlanName1 == item.PlanName);
        //        if (userEntry != null)
        //        {
        //            item.UserEntryID = userEntry.ID;
        //            item.Status = BusInsuranceInitialStatusEnumType.待处理;
        //            item.ActionType = BusInsuranceInitialActionTypeEnumType.已增员;
        //            continue;
        //        }
        //        userEntry = userEntryList.FirstOrDefault(f => f.Status == 2);
        //        if (userEntry != null)
        //        {
        //            item.UserEntryID = userEntry.ID;
        //            item.Status = BusInsuranceInitialStatusEnumType.待处理;
        //            item.ActionType = BusInsuranceInitialActionTypeEnumType.先减员后增;
        //            item.Msg = "员工工种与记录不匹配，【减员再增员】";
        //            continue;
        //        }
        //        userEntry = userEntryList.FirstOrDefault(f => f.Status == 4 && f.WorkClass == item.WorkClass && f.WorkName == item.WorkName && f.PlanName1 == item.PlanName);
        //        if (userEntry != null)
        //        {
        //            item.UserEntryID = userEntry.ID;
        //            item.Status = BusInsuranceInitialStatusEnumType.待处理;
        //            item.ActionType = BusInsuranceInitialActionTypeEnumType.待减员;
        //            item.Msg = "【减员】";
        //            continue;
        //        }
        //        userEntry = userEntryList.FirstOrDefault(f => f.Status == 4);
        //        if (userEntry != null)
        //        {
        //            item.UserEntryID = userEntry.ID;
        //            item.Status = BusInsuranceInitialStatusEnumType.待处理;
        //            item.ActionType = BusInsuranceInitialActionTypeEnumType.待减员;
        //            item.Msg = "员工工种与记录不匹配，【减员】";
        //            continue;
        //        }


        //        item.Status = BusInsuranceInitialStatusEnumType.已处理;
        //        item.Msg = "未匹配到员工信息，【减员】";
        //        item.ActionType = BusInsuranceInitialActionTypeEnumType.人工处理;

        //    }

        //    if (recordList.Any())
        //    {
        //        await _initialRecordService.Update(recordList);
        //    }

        //    return Ok();
        //}


        [HttpPut]
        public async Task<IActionResult> GenerateUserEntryInitialRecord([FromForm] string insuranceCompany)
        {
            List<string> insuranceCompanyList=new List<string>();
            if (insuranceCompany=="阳光保险公司")
            {
                insuranceCompanyList.Add(insuranceCompany);
                insuranceCompanyList.Add("大地财险");
            }
            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            var insuranceMonth = currentMonth.AddMonths(1);
            var currentMonthEndDay = insuranceMonth.AddDays(-1);
            var timeNode = new DateTime(currentMonth.Year, currentMonth.Month, 24);
            var orderPlans = await _orderService.ListOrderPlanView(f => f.InsuranceCompany == insuranceCompany && f.StartDate >= insuranceMonth);
            var configList = await _configService.ListView(f => f.IsEnabled && insuranceCompanyList.Contains(f.InsuranceCompany));
            await _initialRecordService.Delete(f => f.DataType == BusInsuranceInitialDataTypeEnumType.系统生成 && f.InsuranceMonth == insuranceMonth && f.InsuranceCompany == insuranceCompany);
            var recordList = await _initialRecordService.List(f => f.InsuranceMonth == insuranceMonth && f.InsuranceCompany == insuranceCompany);
            List<Live_V_UserEntry> userEntryList = null;
            //Live_V_UserEntry userEntry = null;
            List<Live_BusinessInsuranceInitialRecord> initialRecordList = new List<Live_BusinessInsuranceInitialRecord>();
            Live_BusinessInsuranceInitialRecord initialRecord = null;
            foreach (var configItem in configList)
            {
                // 在职且围在保全名单则增员
                var exp = Expressionable.Create<Live_V_UserEntry>();
                exp.And(f => f.CustomerGID == configItem.CustomerGID);
                exp.And(f => f.PlanStatus1 == null || f.PlanStatus1 != UserEntryPlanStatusEnumType.未参保);
                exp.And(f => f.PlanName1 == "90万+9万"|| f.PlanName1 == "80万+8万");
                exp.And(f => f.EntryTime != null && f.EntryTime.Value.Date < insuranceMonth && f.EntryTime.Value.Date >= timeNode && f.Status == 2);
                userEntryList = await _entryService.ListView(exp.ToExpression());
                var customerInitialRecord = recordList.Where(f => f.CustomerName == configItem.CustomerName).ToList();
                foreach (var entryItem in userEntryList)
                {
                    initialRecord = customerInitialRecord.FirstOrDefault(f => f.IdCardNo == entryItem.CardID);
                    if (initialRecord == null)
                    {
                        initialRecord = new Live_BusinessInsuranceInitialRecord()
                        {
                            ActionType = BusInsuranceInitialActionTypeEnumType.待增员,
                            CustomerName = configItem.CustomerName,
                            DataType = BusInsuranceInitialDataTypeEnumType.系统生成,
                            CreateTime = DateTime.Now,
                            EntryDate = entryItem.EntryTime,
                            ID = Guid.NewGuid(),
                            IdCardNo = entryItem.CardID,
                            InsuranceCompany ="阳光保险公司",
                            InsuranceId = string.Empty,
                            InsuranceMonth = insuranceMonth,
                            InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
                            Msg = "系统在职，未在保全名单中，系统生成待增员记录",
                            Name = entryItem.Name,
                            PlanName = "90万+9万",
                            Remark = string.Empty,
                            Status = BusInsuranceInitialStatusEnumType.待处理,
                            UserEntryID = entryItem.ID,
                            WorkClass = entryItem.WorkClass,
                            WorkName = entryItem.WorkName,
                            IsMain = configItem.IsMain,
                            IsDefaultPush = configItem.IsDefaultPush,
                            IsDayOrder = false,
                            EndDate = DateTime.MinValue,
                            StartDate = DateTime.MinValue,
                        };
                        initialRecordList.Add(initialRecord);
                    }
                    else
                    {
                        initialRecord.UserEntryID = entryItem.ID;
                        if (initialRecord.InsuranceSigningUnitName == configItem.InsuranceSigningUnitName && initialRecord.WorkClass == entryItem.WorkClass && initialRecord.WorkName == entryItem.WorkName && initialRecord.PlanName == initialRecord.PlanName)
                        {
                            //  已增员
                            initialRecord.Status = BusInsuranceInitialStatusEnumType.待处理;
                            initialRecord.ActionType = BusInsuranceInitialActionTypeEnumType.已增员;
                        }
                        else
                        {
                            // 先减员后增员
                            initialRecord.Status = BusInsuranceInitialStatusEnumType.待处理;
                            initialRecord.ActionType = BusInsuranceInitialActionTypeEnumType.待减员;
                            initialRecord.Msg = "参保信息不匹配，先减员后增其中的减员记录";
                            var addRecord = new Live_BusinessInsuranceInitialRecord()
                            {
                                ActionType = BusInsuranceInitialActionTypeEnumType.待增员,
                                CustomerName = configItem.CustomerName,
                                DataType = BusInsuranceInitialDataTypeEnumType.系统生成,
                                CreateTime = DateTime.Now,
                                EntryDate = entryItem.EntryTime,
                                ID = Guid.NewGuid(),
                                IdCardNo = entryItem.CardID,
                                InsuranceCompany ="阳光保险公司",
                                InsuranceId = string.Empty,
                                InsuranceMonth = insuranceMonth,
                                InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
                                Msg = "参保信息不匹配，先减员后增其中的增员记录",
                                Name = entryItem.Name,
                                PlanName = "90万+9万",
                                Remark = string.Empty,
                                Status = BusInsuranceInitialStatusEnumType.待处理,
                                UserEntryID = entryItem.ID,
                                WorkClass = entryItem.WorkClass,
                                WorkName = entryItem.WorkName,
                                IsMain = configItem.IsMain,
                                IsDefaultPush = configItem.IsDefaultPush,
                            };
                            initialRecordList.Add(addRecord);
                        }
                    }
                }




                foreach (var item in customerInitialRecord)
                {

                    initialRecord = item;

                    if (item.Status == BusInsuranceInitialStatusEnumType.待处理)
                    {
                        continue;
                    }
                    var entry = await _entryService.Get(f => f.ID == item.UserEntryID);

                    if (entry != null)
                    {
                        if (entry.Status == 2)
                        {

                            if (initialRecord.InsuranceSigningUnitName == configItem.InsuranceSigningUnitName && initialRecord.WorkClass == entry.WorkClass && initialRecord.WorkName == entry.WorkName && initialRecord.PlanName == entry.PlanName1)
                            {
                                //  已增员
                                initialRecord.Status = BusInsuranceInitialStatusEnumType.待处理;
                                initialRecord.ActionType = BusInsuranceInitialActionTypeEnumType.已增员;
                            }
                            else
                            {
                                // 先减员后增员
                                initialRecord.Status = BusInsuranceInitialStatusEnumType.待处理;
                                initialRecord.ActionType = BusInsuranceInitialActionTypeEnumType.待减员;
                                initialRecord.Msg = "参保信息不匹配，先减员后增其中的减员记录";
                                var addRecord = new Live_BusinessInsuranceInitialRecord()
                                {
                                    ActionType = BusInsuranceInitialActionTypeEnumType.待增员,
                                    CustomerName = configItem.CustomerName,
                                    DataType = BusInsuranceInitialDataTypeEnumType.系统生成,
                                    CreateTime = DateTime.Now,
                                    EntryDate = entry.EntryTime,
                                    ID = Guid.NewGuid(),
                                    IdCardNo = entry.CardID,
                                    InsuranceCompany = "阳光保险公司",
                                    InsuranceId = string.Empty,
                                    InsuranceMonth = insuranceMonth,
                                    InsuranceSigningUnitName = configItem.InsuranceSigningUnitName,
                                    Msg = "参保信息不匹配，先减员后增其中的增员记录",
                                    Name = entry.Name,
                                    PlanName = configItem.IsMain ? entry.PlanName1 : entry.PlanName2,
                                    Remark = string.Empty,
                                    Status = BusInsuranceInitialStatusEnumType.待处理,
                                    UserEntryID = entry.ID,
                                    WorkClass = entry.WorkClass,
                                    WorkName = entry.WorkName,
                                    IsMain = configItem.IsMain,
                                    IsDefaultPush = configItem.IsDefaultPush,
                                };
                                initialRecordList.Add(addRecord);
                            }
                        }
                        else
                        {
                            item.ActionType = BusInsuranceInitialActionTypeEnumType.待减员;
                            item.Status = BusInsuranceInitialStatusEnumType.待处理;
                        }
                    }
                    else
                    {

                        item.UserEntryID = 0;
                    }

                }

                foreach (var record in customerInitialRecord)
                {
                    record.IsMain = configItem.IsMain;
                    record.IsDefaultPush = configItem.IsDefaultPush;
                    if (record.UserEntryID <= 0)
                    {
                        record.Status = BusInsuranceInitialStatusEnumType.匹配失败;
                        record.ActionType = BusInsuranceInitialActionTypeEnumType.人工处理;
                        record.Msg = "未匹配到入离职记录，请人工核对";
                    }
                }
            }
            if (initialRecordList.Any())
            {
                var faillRecordList = initialRecordList.Where(f => string.IsNullOrWhiteSpace(f.PlanName) || string.IsNullOrWhiteSpace(f.WorkName) || string.IsNullOrWhiteSpace(f.WorkClass)).ToList();
                if (faillRecordList.Any())
                {
                    foreach (var item in faillRecordList)
                    {
                        item.Status = BusInsuranceInitialStatusEnumType.匹配失败;
                        item.ActionType = BusInsuranceInitialActionTypeEnumType.人工处理;
                        item.Msg = "参保方案、工种、工作类型 缺失";
                    }
                }
                await _initialRecordService.Add(initialRecordList);
            }
            if (recordList.Any())
            {
                await _initialRecordService.Update(recordList);
            }

            return Ok();
        }



    }
}
