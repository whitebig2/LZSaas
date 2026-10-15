using AutoMapper;
using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Model.Enums;
using LZ.Model.ExcelTemplateModel;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.Options;
using LZ.Model.PagedModel;
using LZ.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SqlSugar.IOC;
using SqlSugar;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.EventHandler;
using LZ.Core.EventData.Insurance;
using NPOI.SS.Formula.Functions;
using NPOI.POIFS.FileSystem;

namespace LZ.Users.API.Controllers
{

    [AllowAnonymous]
    public class BusinessInsuranceOrderController : UsersControllerBase
    {
        public record InsuranceOrderAddRequest(string InsuranceId, string InsuranceCompany, InsuranceOrderEnumType OrderType, string SigningUnitID, string SigningUnitNames,List<string> PlanNames);




        private readonly IEventPublisher _publisher;
        private readonly IInsuranceApiService _insuranceApiService;
        private readonly IBusinessInsuranceOrderService _orderService;
        private readonly IBusinessInsuranceConfigService _configService;
        private readonly IEntryService _entryService;
        private readonly IBusinessInsuranceRecordService _recordService;
        private readonly IBusinessInsuranceOrderEmpoyeeService _orderEmpoyeeService;

        public BusinessInsuranceOrderController(IBusinessInsuranceOrderService orderService, IInsuranceApiService insuranceApiService, IEventPublisher publisher, IEntryService entryService, IBusinessInsuranceConfigService configService, IBusinessInsuranceRecordService recordService, IBusinessInsuranceOrderEmpoyeeService orderEmpoyeeService)
        {
            _orderService = orderService;
            _insuranceApiService = insuranceApiService;
            _publisher = publisher;
            _entryService = entryService;
            _configService = configService;
            _recordService = recordService;
            _orderEmpoyeeService = orderEmpoyeeService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string insuranceCompany, string insuranceId, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_BusinessInsuranceOrder>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceId), f => f.InsuranceId.Contains(insuranceId));
            var models = await _orderService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<Live_BusinessInsuranceOrder> Get(Guid id)
        {
            var model = await _orderService.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }


        [HttpPost]
        public async Task<IActionResult> Add(InsuranceOrderAddRequest request)
        {
            if (request.InsuranceCompany == "大地财险"|| request.InsuranceCompany == "众安保险")
            {
                var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                var nextMonth = currentMonth.AddMonths(1);
                Guid singningUnitID = Guid.Empty;
                if (!string.IsNullOrWhiteSpace(request.SigningUnitID))
                {
                    singningUnitID = Guid.Parse(request.SigningUnitID);
                }
                Live_BusinessInsuranceOrderExtension model = new Live_BusinessInsuranceOrderExtension()
                {
                    CreateTime = DateTime.Now,
                    StartDate = nextMonth,
                    EndDate = nextMonth.AddMonths(1).AddDays(-1),
                    InsuranceCompany = request.InsuranceCompany,
                    SigningUnitID = singningUnitID,
                    InsuranceId = request.InsuranceId.Trim(),
                    InsuranceNo = string.Empty,
                    OrderType = request.OrderType

                };
                if ( string.IsNullOrWhiteSpace(model.SigningUnitNames))
                {
                    model.IsDefaultPush = true;
                }
                else
                {
                    model.IsDefaultPush = false;
                }
                var plans = new List<Live_BusinessInsuranceOrderPlan>();
                if (request.PlanNames.Any())
                {
                    plans = request.PlanNames.Select(f => new Live_BusinessInsuranceOrderPlan()
                    {
                        GnteeId = 0,
                        GnteeName = f,
                    }).ToList();
                }
                model.Records = plans;
                await _orderService.AddNav(model);
                return Ok();
            }
            else
            {


                var resp = await _insuranceApiService.GetOrderDetail(request.InsuranceId);
                if (resp == null)
                {
                    throw new ErrorCodeException(-1, "接口调用异常");
                }

                if (resp.Code != 200)
                {
                    throw new ErrorCodeException(-1, $"接口调用异常,{resp.Msg}");
                }


                Guid singningUnitID = Guid.Empty;

                if (!string.IsNullOrWhiteSpace(request.SigningUnitID))
                {
                    singningUnitID = Guid.Parse(request.SigningUnitID);
                }

                Live_BusinessInsuranceOrderExtension model = new Live_BusinessInsuranceOrderExtension()
                {
                    CreateTime = DateTime.Now,
                    EndDate = Convert.ToDateTime(resp.Data.enddate),
                    StartDate = Convert.ToDateTime(resp.Data.startdate),
                    InsuranceCompany = request.InsuranceCompany,
                    SigningUnitID = singningUnitID,
                    InsuranceId = request.InsuranceId.Trim(),
                    InsuranceNo = string.Empty,
                    OrderType = request.OrderType

                };
                if (model.SigningUnitID == null || model.SigningUnitID == Guid.Empty)
                {
                    model.IsDefaultPush = true;
                }
                else
                {
                    model.IsDefaultPush = false;
                }
                var plans = resp.Data.gnteeList.Select(f => new Live_BusinessInsuranceOrderPlan()
                {
                    GnteeId = int.Parse(f.GnteeId),
                    GnteeName = f.GnteeName,
                }).ToList();
                model.Records = plans;

                model.SigningUnitNames = request.SigningUnitNames;
                await _orderService.AddNav(model);
                return Ok();
            }
        }




        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_BusinessInsuranceOrderExtension model)
        {


            if (string.IsNullOrWhiteSpace(model.SigningUnitNames))
            {
                model.IsDefaultPush = true;
            }
            else
            {
                model.IsDefaultPush = false;
            }
            await _orderService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _orderService.DeleteNav(f => f.ID == id);
            return Ok();
        }

        /// <summary>
        /// 导入保全名单
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        public async Task<IActionResult> UploadInsuranceNameList(IFormFile file)
        {
            var indexStart = file.FileName.IndexOf('N');
            var indexEnd = file.FileName.IndexOf('.');
            string insuranceId = file.FileName.Substring(indexStart, indexEnd - indexStart);
            var insuranceOrder = await _orderService.GetNav(f => f.InsuranceId == insuranceId);
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<InsuranceOrderNameRecordExcelTemplate>(ms, string.Empty);

                if (excelData != null && excelData.Count > 0)
                {
                    var nameRecordList = excelData.Select(f => new Live_BusinessInsuranceOrderEmpoyee()
                    {
                        Age = f.Age,
                        CertificateType = "身份证",
                        CreateTime = DateTime.Now,
                        FromType = "Excel导入",
                        IdCardNo = f.IdCardNo,
                        EndDate = f.InsuracneEndDate,
                        StartDate = f.InsuracneStartDate,
                        InsuranceAmount = f.InsuranceAmount,
                        JobCompanyName = f.JobCompanyName,
                        JobLevelType = f.JobLevelType.ToString(),
                        JobType = f.JobType,
                        Name = f.Name,
                        PlanName = f.PlanName,
                        Remark = string.Empty,
                        Sex = string.Empty,
                        SigningUnitName = f.SigningUnitName,
                        Status = InsuranceNameRecordStatusEnumType.已推送,
                        ActionType = InsuranceActionEnumType.待匹配,
                        MethodType = f.MethodType,
                    }).ToList();
                    insuranceOrder.EmployeeRecords = nameRecordList;
                    await _orderService.UpdateNav(insuranceOrder);
                }
                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<SalaryRecordExcelTemplate>(stream, "sheet1");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "在保人员清单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        [HttpGet]
        public async Task<IActionResult> Test()
        {

            BusinessInsuranceAddOrDelEventData eventData = new BusinessInsuranceAddOrDelEventData()
            {
                IsAdd = false,
                UserEntryID = 6
            };
            _publisher.Publish(eventData);

            return Ok();
        }








        // [HttpPost]
        // public async Task<IActionResult> GenerateBusinessRecords([FromForm] Guid id)
        // {



        //     var model = await _orderService.GetNav(f => f.ID == id);
        //     if (model.OrderType == (int)InsuranceOrderEnumType.日保单)
        //     {
        //         throw new ErrorCodeException(-1, "日保单无法使用此功能");
        //     }

        //     model.Status = BusinessInsuranceStatusEnumType.完成;

        //     var planNameRecords = model.NameRecords.ToList();


        //     // 获取当前已存在的参保记录

        //     var existList = await businessInsuranceRecordService.List(f => f.OrderNo == model.OrderNo);

        //     List<Live_BusinessInsuranceRecord> businessRecordList = new List<Live_BusinessInsuranceRecord>();
        //     Live_BusinessInsuranceRecord businessRecord = null;
        //     var orderPlans = await _orderService.GetOrderPlanViewList(f => f.InsuranceOrderID == id);
        //     var exp = Expressionable.Create<Live_V_InsuranceCustomerConfig>();
        //     exp.AndIF(model.IsCommon, f => f.IsCommon == true);
        //     exp.AndIF(!model.IsCommon, f => f.IsCommon == false);
        //     exp.And(f => f.InsuranceCompany == model.InsuranceCompany);
        //     exp.And(f => f.IsEnabled);
        //     var configList = await configService.ListView(exp.ToExpression());
        //     foreach (var config in configList)
        //     {
        //         var entryRes = await otherService.GetUserEntryListByInsuranceStartDate(config.CustomerGID.Value, model.StartDate.Value);

        //         // 在职员工
        //         var inJobList = entryRes.Data.Where(f => f.Status == 2);
        //         var leaveJobList = entryRes.Data.Where(f => f.Status == 4);

        //         foreach (var userEntryItem in inJobList)
        //         {
        //             var ageAndSex = IdCardHelper.GetAgeAndGender(userEntryItem.CardID);
        //             businessRecord = new Live_BusinessInsuranceRecord()
        //             {
        //                 AddIndecaseId = string.Empty,
        //                 AddMatchRecordID = Guid.Empty,
        //                 AddTime = model.StartDate.Value,
        //                 Age = ageAndSex.Age,
        //                 CreateTime = DateTime.Now,
        //                 PlanName = string.Empty,
        //                 CustomerGID = config.CustomerGID.Value,
        //                 CustomerName = config.CustomerName,
        //                 DelIndecaseId = string.Empty,
        //                 DelMatchRecordID = Guid.Empty,
        //                 DelTime = DateTime.MinValue,
        //                 EndDate = model.EndDate.Value,
        //                 EntryDate = userEntryItem.EntryTime.Value.Date,
        //                 IdCardNo = userEntryItem.CardID,
        //                 InsuranceCompany = config.InsuranceCompany,
        //                 InsuranceNo = model.InsuranceNo,
        //                 InsuranceSigningUnitID = config.InsuranceSigningUnitID.Value,
        //                 InsuranceSigningUnitName = config.SigningUnitName,
        //                 IsCommon = config.IsCommon,
        //                 IsExpired = false,
        //                 IsMain = config.IsMain,
        //                 JobLevelType = string.Empty,
        //                 JobType = string.Empty,
        //                 LeaveDate = DateTime.MinValue,
        //                 Name = userEntryItem.Name,
        //                 OrderNo = model.OrderNo,
        //                 OrderType = InsuranceOrderEnumType.月保单,
        //                 Remark = "白名单导入",
        //                 Sex = ageAndSex.Gender,
        //                 StartDate = model.StartDate.Value,
        //                 Status = InsuranceSendStatusEnumType.待减员,
        //             };
        //             if (userEntryItem.WorkClass == "3类")
        //             {
        //                 businessRecord.JobLevelType = "3";
        //                 businessRecord.PlanName = config.LevelThreePlan;
        //             }
        //             else if (userEntryItem.WorkClass == "4类")
        //             {
        //                 businessRecord.JobLevelType = "4";
        //                 businessRecord.PlanName = config.LevelFourPlan;
        //             }
        //             else if (userEntryItem.WorkClass == "5类")
        //             {
        //                 businessRecord.JobLevelType = "5";
        //                 businessRecord.PlanName = config.LevelFivePlan;
        //             }
        //             else
        //             {
        //                 continue;
        //             }
        //             var plan = orderPlans.FirstOrDefault(f => f.GnteeName == businessRecord.PlanName);
        //             if (plan != null)
        //             {
        //                 businessRecord.PlanNameID = plan.GnteeId;
        //                 businessRecord.OrderType = InsuranceOrderEnumType.月保单;
        //             }

        //             var planNameRecord = planNameRecords.FirstOrDefault(f =>
        //                   f.Name == businessRecord.Name
        //                   && f.IdCardNo == businessRecord.IdCardNo
        //                   && f.JobCompanyName == businessRecord.CustomerName
        //                   && f.SigningUnitName == businessRecord.InsuranceSigningUnitName
        //                   && f.PlanName == businessRecord.PlanName
        //                   && f.JobLevelType.ToString() == businessRecord.JobLevelType
        //                   && f.JobType == businessRecord.JobType
        //                   );
        //             if (planNameRecord != null && planNameRecord.ActionType == InsuranceActionEnumType.待匹配)
        //             {
        //                 planNameRecord.ActionType = InsuranceActionEnumType.生成已增员记录;
        //                 businessRecord.Status = InsuranceSendStatusEnumType.已增员;


        //                 if (!existList.Any(f =>
        //   f.CustomerName == businessRecord.CustomerName
        //  && f.InsuranceSigningUnitName == businessRecord.InsuranceSigningUnitName
        //  && f.IdCardNo == businessRecord.IdCardNo
        //  && f.JobLevelType == businessRecord.JobLevelType
        //  && f.JobType == businessRecord.JobType
        //  && f.PlanName == businessRecord.PlanName
        //  && f.Status == InsuranceSendStatusEnumType.已增员

        //   ))
        //                 {
        //                     businessRecordList.Add(businessRecord);
        //                 }
        //             }
        //             else
        //             {

        //                 if (!existList.Any(f =>
        //    f.CustomerName == businessRecord.CustomerName
        //   && f.InsuranceSigningUnitName == businessRecord.InsuranceSigningUnitName
        //   && f.IdCardNo == businessRecord.IdCardNo
        //   && f.JobLevelType == businessRecord.JobLevelType
        //   && f.JobType == businessRecord.JobType
        //   && f.PlanName == businessRecord.PlanName
        //   && f.Status == InsuranceSendStatusEnumType.待增员

        //    ))
        //                 {
        //                     businessRecordList.Add(businessRecord);
        //                 }


        //             }




        //         }
        //         foreach (var userEntryItem in leaveJobList)
        //         {
        //             var ageAndSex = IdCardHelper.GetAgeAndGender(userEntryItem.CardID);
        //             businessRecord = new Live_BusinessInsuranceRecord()
        //             {
        //                 AddIndecaseId = string.Empty,
        //                 AddMatchRecordID = Guid.Empty,
        //                 AddTime = model.StartDate.Value,
        //                 Age = ageAndSex.Age,
        //                 CreateTime = DateTime.Now,
        //                 PlanName = string.Empty,
        //                 CustomerGID = config.CustomerGID.Value,
        //                 CustomerName = config.CustomerName,
        //                 DelIndecaseId = string.Empty,
        //                 DelMatchRecordID = Guid.Empty,
        //                 DelTime = DateTime.MinValue,
        //                 EndDate = model.EndDate.Value,
        //                 EntryDate = userEntryItem.EntryTime.Value.Date,
        //                 IdCardNo = userEntryItem.CardID,
        //                 InsuranceCompany = config.InsuranceCompany,
        //                 InsuranceNo = model.InsuranceNo,
        //                 InsuranceSigningUnitID = config.InsuranceSigningUnitID.Value,
        //                 InsuranceSigningUnitName = config.SigningUnitName,
        //                 IsCommon = config.IsCommon,
        //                 IsExpired = false,
        //                 IsMain = config.IsMain,
        //                 JobLevelType = string.Empty,
        //                 JobType = string.Empty,
        //                 LeaveDate = DateTime.MinValue,
        //                 Name = userEntryItem.Name,
        //                 OrderNo = model.OrderNo,
        //                 OrderType = InsuranceOrderEnumType.月保单,
        //                 Remark = "白名单导入",
        //                 Sex = ageAndSex.Gender,
        //                 StartDate = model.StartDate.Value,
        //                 Status = InsuranceSendStatusEnumType.待增员,
        //             };
        //             if (userEntryItem.WorkClass == "3类")
        //             {
        //                 businessRecord.JobLevelType = "3";
        //                 businessRecord.PlanName = config.LevelThreePlan;
        //             }
        //             else if (userEntryItem.WorkClass == "4类")
        //             {
        //                 businessRecord.JobLevelType = "4";
        //                 businessRecord.PlanName = config.LevelFourPlan;
        //             }
        //             else if (userEntryItem.WorkClass == "5类")
        //             {
        //                 businessRecord.JobLevelType = "5";
        //                 businessRecord.PlanName = config.LevelFivePlan;
        //             }
        //             else
        //             {
        //                 continue;
        //             }
        //             var plan = orderPlans.FirstOrDefault(f => f.GnteeName == businessRecord.PlanName);
        //             if (plan != null)
        //             {
        //                 businessRecord.PlanNameID = plan.GnteeId;
        //                 businessRecord.OrderType = InsuranceOrderEnumType.月保单;
        //             }
        //             var planNameRecord = planNameRecords.FirstOrDefault(f =>
        //                   f.Name == businessRecord.Name
        //                   && f.IdCardNo == businessRecord.IdCardNo
        //                   && f.JobCompanyName == businessRecord.CustomerName
        //                   && f.SigningUnitName == businessRecord.InsuranceSigningUnitName
        //                   && f.PlanName == businessRecord.PlanName
        //                   && f.JobLevelType.ToString() == businessRecord.JobLevelType
        //                   && f.JobType == businessRecord.JobType
        //                   );
        //             if (planNameRecord != null)
        //             {
        //                 if (planNameRecord.ActionType == InsuranceActionEnumType.待匹配)
        //                 {
        //                     planNameRecord.ActionType = InsuranceActionEnumType.生成待减员记录;
        //                     businessRecord.Status = InsuranceSendStatusEnumType.待减员;

        //                     if (!existList.Any(f =>
        // f.CustomerName == businessRecord.CustomerName
        //&& f.InsuranceSigningUnitName == businessRecord.InsuranceSigningUnitName
        //&& f.IdCardNo == businessRecord.IdCardNo
        //&& f.JobLevelType == businessRecord.JobLevelType
        //&& f.JobType == businessRecord.JobType
        //&& f.PlanName == businessRecord.PlanName
        //&& f.Status == InsuranceSendStatusEnumType.待减员

        // ))
        //                     {
        //                         businessRecordList.Add(businessRecord);
        //                     }


        //                 }
        //                 else
        //                 {
        //                     continue;
        //                 }
        //             }

        //         }
        //     }

        //     DbScoped.SugarScope.BeginTran();
        //     await service.UpdateNav(model);
        //     await businessInsuranceRecordService.Add(businessRecordList);
        //     DbScoped.SugarScope.CommitTran();
        //     return Ok();
        // }



        //[HttpPut]
        //public async Task<IActionResult> MatchUserEntry(Guid id)
        //{
        //    var model = await service.GetNav(f => f.ID == id);
        //    if (model.OrderType== InsuranceOrderEnumType.日保单)
        //    {
        //        throw new ErrorCodeException(-1, "日保单无法使用此功能");
        //    }
        //    var orderPlans = await service.GetOrderPlanViewList(f => f.InsuranceOrderID == id);
        //    foreach (var planItem in orderPlans)
        //    {
        //        var lastMonthStartDate = planItem.StartDate.Value.AddMonths(-1);
        //        var lastMonthEndDate = planItem.StartDate.Value.AddDays(-1);
        //        var exp = Expressionable.Create<Live_V_BusinessInsuranceRecord>();
        //        exp.AndIF(planItem.IsCommon, f => f.IsCommon == true);
        //        exp.AndIF(!planItem.IsCommon, f => f.IsCommon == false && f.InsuranceSigningUnitName == planItem.SigningUnitName);
        //        exp.And(f => f.PlanName == planItem.GnteeName);// 方案相同
        //        exp.And(f => f.InsuranceCompany == planItem.InsuranceCompany);
        //        exp.And(f => f.StartDate >= lastMonthStartDate && f.EndDate <= lastMonthEndDate);
        //        var businessRecordList = await businessInsuranceRecordService.ListView(exp.ToExpression());
        //        if (model.NameRecords == null || model.NameRecords.Count <= 0)
        //        {
        //            throw new ErrorCodeException(-1, "保全名单获取失败");
        //        }
        //        var planNameRecords = model.NameRecords.Where(f => f.PlanName == planItem.GnteeName).ToList();
        //        Live_V_BusinessInsuranceRecord userEntryInsuracneRecord = null;
        //        foreach (var record in planNameRecords)
        //        {





        //            var userRecords = businessRecordList.Where(f =>
        //            f.IdCardNo == record.IdCardNo// 身份证号
        //              && f.JobType == record.JobType// 工种
        //              && f.JobLevelType == record.JobLevelType.ToString()// 类别
        //              && f.CustomerName == record.JobCompanyName// 客户单位
        //              && f.InsuranceSigningUnitName == record.SigningUnitName// 签章单位
        //              ).OrderByDescending(f=>f.StartDate).ToList();
        //            if (userRecords.Any(f =>f.UserEntryStatus.Value== 2))//在职
        //            {
        //                userEntryInsuracneRecord = userRecords.First(f => f.UserEntryStatus.Value == 2);
        //                record.UserEntryID = userEntryInsuracneRecord.UserEntryID;
        //                record.Remark = "生成一条已增员记录";
        //                record.ActionType = InsuranceActionEnumType.生成已增员记录;
        //            }
        //            else if (userRecords.Any(f =>f.UserEntryStatus== 4))// 已离职
        //            {
        //                userEntryInsuracneRecord = userRecords.First(f => f.UserEntryStatus == 4);
        //                record.UserEntryID = userEntryInsuracneRecord.UserEntryID;
        //                record.ActionType = InsuranceActionEnumType.生成待减员记录;
        //                record.Remark = "生成一条待减员记录";
        //            }
        //            else
        //            {
        //                // 未匹配到数据
        //                record.Remark = "推送异常记录，手动处理";
        //            }
        //        }
        //        // 符合该方案则需要生成增员记录
        //        var addList = businessRecordList.Where(f => f.Status == (int)InsuranceSendStatusEnumType.已增员).ToList();


        //        var addNameRecords = new List<Live_InsuranceOderNameRecord>();


        //        foreach (var record in addList)
        //        {
        //            if (!planNameRecords.Any(f =>
        //            f.IdCardNo == record.IdCardNo
        //            && f.JobType == record.JobType
        //            && f.JobLevelType == record.JobLevelType
        //            && f.JobCompanyName == record.InsuranceCompany
        //            && f.SigningUnitName == record.InsuranceSigningUnitName
        //            ))
        //            {
        //                //则需要增员一条记录
        //                addNameRecords.Add(new Live_InsuranceOderNameRecord()
        //                {
        //                    MethodType = InsuranceMethodEnumType.保全匹配增员,
        //                    Age = record.Age,
        //                    CertificateType = "身份证",
        //                    CreateTime = record.CreateTime,
        //                    FromType = "保全名单匹配增员",
        //                    IdCardNo = record.IdCardNo,
        //                    InsuracneEndDate = null,
        //                    InsuracneStartDate = null,
        //                    IsAddEmp = true,
        //                    InsuranceAmount = 0,
        //                    JobCompanyName = record.CustomerName,
        //                    JobLevelType = record.JobLevelType,
        //                    JobType = record.JobType,
        //                    Name = record.Name,
        //                    PlanName = record.PlanName,
        //                    Sex = record.Sex,
        //                    SigningUnitName = record.InsuranceSigningUnitName,
        //                    Status = InsuranceNameRecordStatusEnumType.待推送,
        //                    UserEntryID = record.UserEntryID,
        //                    ActionType = InsuranceActionEnumType.生成待增员记录,
        //                });
        //            }


        //        }

        //    }



        //        return Ok();
        //    }
    }
}
