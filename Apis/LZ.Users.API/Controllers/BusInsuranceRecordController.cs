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
using LZ.Model.Dtos;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Senparc.Weixin.MP.AdvancedAPIs.Card;

namespace LZ.Users.API.Controllers
{
    [AllowAnonymous]
    public class BusInsuranceRecordController : UsersControllerBase
    {
        private readonly IEntryService _entryService;
        private readonly IBusInsuranceRecordService _recordService;
        private readonly IBusInsuracneOrderService  _orderService;
        private readonly IBusInsuracneService _busInsuracneService;
        private readonly IBusInsuranceWorkPlanService _workPlanService;

        public BusInsuranceRecordController(IBusInsuranceRecordService recordService, IBusInsuracneOrderService orderService, IBusInsuracneService busInsuracneService, IEntryService entryService, IBusInsuranceWorkPlanService workPlanService)
        {
            _recordService = recordService;
            _orderService = orderService;
            _busInsuracneService = busInsuracneService;
            _entryService = entryService;
            _workPlanService = workPlanService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string name,string cardID, string workName, string orderNo, string planName, string insuranceCompany, BusInsuranceRecordStatusEnumType? status,[FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BusInsuranceRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(!string.IsNullOrWhiteSpace(orderNo), f => f.OrderNo.Contains(orderNo));
            exp.AndIF(status != null, f => f.Status == status.Value);
            var models = await _recordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string customerName, string name, string cardID, string workName, string orderNo, string planName, string insuranceCompany, BusInsuranceRecordStatusEnumType? status)
        {

            var exp = Expressionable.Create<Model.Live_BusInsuranceRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(!string.IsNullOrWhiteSpace(orderNo), f => f.OrderNo.Contains(orderNo));
            exp.AndIF(status!=null, f => f.Status== status.Value);

            var models = await _recordService.List(exp.ToExpression());

            var exportDataList = models.Select(f => new BusInsuranceRecordTemplate()
            {
                CustomerName = f.CustomerName,
                InsuranceCompany = f.InsuranceCompany,
                PlanName = f.PlanName,
                WorkName = f.WorkName,
                Age = f.Age,
                CardID = f.CardID,
                CreateTime = f.CreateTime?.ToString("yyyy-MM-dd"),
                EndDate = f.EndDate?.ToString("yyyy-MM-dd"),
                InsuranceMonth = f.InsuranceMonth?.ToString("yyyy-MM-dd"),
                MatchDate = f.InsuranceMonth?.ToString("yyyy-MM-dd"),
                Name = f.Name,
                OrderNo = f.OrderNo,
                PolicyNum = f.PolicyNum,
                Sex = f.Sex,
                SignUnitName = f.SignUnitName,
                StartDate = f.InsuranceMonth?.ToString("yyyy-MM-dd"),
                Status = f.Status.ToString(),
                WorkClass = f.WorkClass,
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "参保记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "参保记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }





        /// <summary>
        /// 下载模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<BusInsuranceRecordTemplate>(stream, "参保记录");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "参保记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }


        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file)
        {
            List<BusInsuranceRecordTemplate> configs = new List<BusInsuranceRecordTemplate>();

            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                configs = NpoiExcelHelper.ReadExcelFile<BusInsuranceRecordTemplate>(ms, "");
            }
            if (configs == null || configs.Count <= 0)
            {
                throw new ErrorCodeException(-1, "未能读取到数据，请检查模板文件数据是否存在！");
            }

            var insuranceMonth= Convert.ToDateTime(configs.First().InsuranceMonth);
            var insuranceMonthEnd= insuranceMonth.AddMonths(1).AddDays(-1);
            var orderNos = configs.Select(f => f.OrderNo).Distinct().ToList();

            var orders=await _orderService.ListOrderPlan(f =>f.OrderMonth== insuranceMonth&& orderNos.Contains(f.OrderNo));

            Live_V_BusInsuracneOrderPlan order = null;
            var configGroups = configs.GroupBy(f =>new { f.InsuranceCompany, f.OrderNo }).ToList();
            List<Live_BusInsuranceRecord> records = new List<Live_BusInsuranceRecord>();

            // 工种匹配方案
            var workPlans = await _workPlanService.List(f => f.InsuranceCompany == f.InsuranceCompany && f.IsEnable);
            foreach (var groupItem in configGroups)
            {
                if (groupItem.Key.InsuranceCompany== "安责险"|| groupItem.Key.InsuranceCompany == "团体意外险")
                {
                    continue;
                }
                var addRecords = groupItem.Select(f => new Live_BusInsuranceRecord()
                {
                    CreateTime = DateTime.Now,
                    PlanName = f.PlanName.Trim(),
                    CustomerName = f.CustomerName.Trim(),
                    InsuranceCompany = f.InsuranceCompany.Trim(),
                    WorkName = f.WorkName.Trim(),
                    IsDelMatch = false,
                    AddPushID = string.Empty,
                    Age = f.Age,
                    CardID = f.CardID.Trim(),
                    DelPushID = string.Empty,
                    EndDate = insuranceMonthEnd,
                    ID = 0,
                    MatchDate = DateTime.MinValue,
                    Name = f.Name,
                    Sex = f.Sex,
                    SignUnitName = f.SignUnitName.Trim(),
                    StartDate = insuranceMonth,
                    OrderNo= f.OrderNo,
                    
                    Status = BusInsuranceRecordStatusEnumType.已增员,
                    WorkClass = f.WorkClass.Trim(),
                }).ToList();
           
                //方案方案匹配订单好
                Live_BusInsuranceWorkPlan workPlan = null;
                foreach (var item in addRecords)
                {
                    workPlan = workPlans.FirstOrDefault(f =>f.CustomerName==item.CustomerName&&  f.WorkName == item.WorkName);

                    if (workPlan==null)
                    {
                        workPlan = workPlans.FirstOrDefault(f => f.CustomerName == item.CustomerName && f.InsuranceWorkName == item.WorkName);
                    }
                    if (workPlan==null)
                    {
                        throw new ErrorCodeException(-1, $"{item.CustomerName}--{item.WorkName} 工种配置缺失");
                    }

                    order = orders.FirstOrDefault(f => f.InsuranceCompany == groupItem.Key.InsuranceCompany && f.OrderNo == groupItem.Key.OrderNo.Trim()&&f.PlanName==item.PlanName);
                    if (order == null)
                    {
                        throw new ErrorCodeException(-1, $"订单号：{groupItem.Key.OrderNo},保险公司：{groupItem.Key.InsuranceCompany},方案：{workPlan.PlanName}，签章单位：{item.SignUnitName},未录入到系统中");
                    }
                    item.WorkName = workPlan.WorkName;
                    item.InsuranceWorkName = workPlan.InsuranceWorkName;
                  
                    item.PlanName=workPlan.PlanName;
                    item.PlanNameID= order.PlanNameID;
                    item.InsuranceMonth= order.OrderMonth;
                    item.PolicyNum= order.PolicyNum;
                }
                records.AddRange(addRecords);
            }


            //var jobs = await _busInsuracneService.ListEntryJob();
            foreach (var item in records)
            {
               var ageAndGender= IdCardHelper.GetAgeAndGender(item.CardID);
                item.Age=ageAndGender.Age;
                item.Sex=ageAndGender.Gender;

                //var job = jobs.FirstOrDefault(f => f.Name == item.WorkName);
                //if (job == null)
                //{
                //    Console.WriteLine($"工种缺失：{item.WorkName}");
                //}
                //else
                //{
                //    item.WorkClass = job.Type;
                //}



            }
            await _recordService.Add(records);
            return Ok();
        }


        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadTestTemplate(IFormFile file)
        {
            List<BusInsuranceRecordTemplate> configs = new List<BusInsuranceRecordTemplate>();

            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                configs = NpoiExcelHelper.ReadExcelFile<BusInsuranceRecordTemplate>(ms, "");
            }
            if (configs == null || configs.Count <= 0)
            {
                throw new ErrorCodeException(-1, "未能读取到数据，请检查模板文件数据是否存在！");
            }

            var insuranceMonth = Convert.ToDateTime(configs.First().InsuranceMonth);
            var insuranceMonthEnd = insuranceMonth.AddMonths(1).AddDays(-1);

            var orders = await _orderService.List(f => f.OrderMonth == insuranceMonth &&f.IsEnable);
            List<Live_BusInsuranceRecord> records = new List<Live_BusInsuranceRecord>();

            var jobs = await _busInsuracneService.ListEntryJob();


            foreach (var item in configs)
            {
                var order = orders.FirstOrDefault(f =>f.OrderNo==item.OrderNo&& f.InsuranceCompany == item.InsuranceCompany && f.PlanName == item.PlanName);
                var job = jobs.FirstOrDefault(f => f.Name == item.WorkName);
                Console.WriteLine(item.CardID);

               var ageAndGender= IdCardHelper.GetAgeAndGender(item.CardID);
                var addRecord = new Live_BusInsuranceRecord()
                {
                    CreateTime = DateTime.Now,
                    PlanName = item.PlanName.Trim(),
                    CustomerName = item.CustomerName.Trim(),
                    InsuranceCompany = item.InsuranceCompany.Trim(),
                    WorkName = item.WorkName.Trim(),
                    IsDelMatch = false,
                    InsuranceMonth = order.OrderMonth,
                    AddPushID = string.Empty,
                    Age = ageAndGender.Age,
                    CardID = item.CardID.Trim(),
                    DelPushID = string.Empty,
                    EndDate = insuranceMonthEnd,
                    ID = 0,
                    MatchDate = DateTime.MinValue,
                    Name = item.Name,
                    OrderNo = order.OrderNo,
                    PlanNameID = order.PlanNameID,
                    PolicyNum = order.PolicyNum,
                    Sex = ageAndGender.Gender,
                    SignUnitName = item.SignUnitName??"",
                    StartDate = insuranceMonth,
                    Status = BusInsuranceRecordStatusEnumType.已增员,
                    WorkClass = job?.Type??"",
                };
                records.Add(addRecord);
            }

            await _recordService.Add(records);
            return Ok();
        }


        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get( int id)
        {
            var model = await _recordService.Get(f=>f.ID==id);
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Live_BusInsuranceRecord model)
        {
            model.CreateTime=DateTime.Now;
            await _recordService.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_BusInsuranceRecord model)
        {
            await _recordService.Update(model);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromForm]int id, [FromForm] BusInsuranceRecordStatusEnumType status )
        {
           var model= await _recordService.Get(f=>f.ID==id);

            var month = model.StartDate.Value.AddDays(1 - model.StartDate.Value.Day);
            var monthEnd = month.AddMonths(1).AddDays(-1);

            if (status == BusInsuranceRecordStatusEnumType.已增员)
            {
                model.EndDate = monthEnd;
            }
            else if (status == BusInsuranceRecordStatusEnumType.已减员)
            {
                model.EndDate = DateTime.Today;
            }
            else
            {
                throw new ErrorCodeException(-1, $"无法修改为：{status.ToString()}！");
            }
            model.Status=status;
          await  _recordService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _recordService.Delete(f=>f.ID==id);
            return Ok();
        }



        #region 商保处理

        [HttpPost]
        public async Task<IActionResult> MatchWorkPlan()
        {
            try
            {
                await _busInsuracneService.WorkPlanMatch();
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, $"异常信息:{ex.Message}]");
            }
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> MatchByOrderNo([FromForm]string insuranceCompany)
        {
            try
            {
                // 立即匹配一次将待匹配数据，将待匹配数据 匹配参保方案
                await _busInsuracneService.WorkPlanMatch();
                await Task.Delay(1000);
                var today = DateTime.Today;
                var nextDay = DateTime.Today.AddDays(1);
                var insuranceMonth = nextDay.AddDays(1 - nextDay.Day).AddMonths(1);
                var orders = await _orderService.ListOrderPlan(f => f.InsuranceCompany == insuranceCompany && f.IsEnable && f.OrderMonth == insuranceMonth && f.IsEnable);

               var orderGroup= orders.GroupBy(f => new { f.OrderNo, f.InsuranceCompany }).ToList();
                foreach (var item in orderGroup)
                {
                    // 只匹配 不生成增减记录
                    await _busInsuracneService.MatchByOrderNo(item.Key.InsuranceCompany, item.Key.OrderNo, false);
                }
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, $"异常信息:{ex.Message}]");
            }
            return Ok();
        }



        #endregion
    }
}
