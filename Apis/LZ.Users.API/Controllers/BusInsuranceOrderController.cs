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
using LZ.ApiService.Interface;
using System.Linq;

namespace LZ.Users.API.Controllers
{
    [AllowAnonymous]
    public class BusInsuranceOrderController : UsersControllerBase
    {
        private readonly IBusInsuracneOrderService _orderService;
        private readonly IInsuranceApiService _insuranceApiService;

        public BusInsuranceOrderController(IBusInsuracneOrderService orderService, IInsuranceApiService insuranceApiService)
        {
            _orderService = orderService;
            _insuranceApiService = insuranceApiService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string signUnitName, string orderNo, string policyNum, string planName, string insuranceCompany, DateTime? orderMonth, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BusInsuracneOrder>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(orderNo), f => f.OrderNo.Contains(orderNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(!string.IsNullOrWhiteSpace(policyNum), f => f.PolicyNum.Contains(policyNum));
            exp.AndIF(orderMonth.HasValue, f => f.OrderMonth==orderMonth.Value);
            var models = await _orderService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string signUnitName, string orderNo, string policyNum, string planName, string insuranceCompany, DateTime? orderMonth)
        {

            var exp = Expressionable.Create<Model.Live_V_BusInsuracneOrderPlan>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(orderNo), f => f.OrderNo.Contains(orderNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(!string.IsNullOrWhiteSpace(policyNum), f => f.PolicyNum.Contains(policyNum));
            exp.AndIF(orderMonth.HasValue, f => f.OrderMonth == orderMonth.Value);
            var models = await _orderService.ListOrderPlan(exp.ToExpression());

            var exportDataList = models.Select(f => new BusInsuranceOrderExcelTemplate()
            {
                InsuranceCompany = f.InsuranceCompany,
                IsEnable = f.IsEnable ? "是" : "否",
                OrderNo = orderNo,
                PolicyNum = policyNum,
                OrderMonth = f.OrderMonth.Value.ToString("yyyy-MM-dd"),
                PlanName = f.PlanName,
                SignUnitNames = f.SignUnitNames,
                StartDate = f.StartDate?.ToString("yyyy-MM-dd"),
                EndDate = f.EndDate?.ToString("yyyy-MM-dd"),
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "商保订单");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保订单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }





        /// <summary>
        /// 下载模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<BusInsuranceOrderExcelTemplate>(stream, "商保订单");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保订单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }




        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file)
        {
            List<BusInsuranceOrderExcelTemp> configs = new List<BusInsuranceOrderExcelTemp>();



        
       

          
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    file.CopyTo(ms);
            //    configs = NpoiExcelHelper.ReadExcelFile<BusInsuranceOrderExcelTemplate>(ms, "");
            //}
            //if (configs == null || configs.Count <= 0)
            //{
            //    throw new ErrorCodeException(-1, "未能读取到数据，请检查模板文件数据是否存在！");
            //}
            //var models = configs.Select(f => new Live_BusInsuracneOrder()
            //{
            //    CreateTime = DateTime.Now,
            //    PlanName = f.PlanName.Trim(),
            //    InsuranceCompany = f.InsuranceCompany.Trim(),
            //    IsEnable = f.IsEnable.Trim() == "是",
            //     StartDate=f.sta
            //}).ToList();

            //await _workPlanService.Add(models);
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
            var model = await _orderService.GetNav(f => f.ID == id);
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Live_BusInsuracneOrderExtension model)
        {
            model.CreateTime = DateTime.Now;
            if (model.OrderMonth == null)
            {
                throw new ErrorCodeException(-1, "订单月份缺失");
            }
            else
            {
                model.StartDate = model.OrderMonth;
                model.EndDate = model.OrderMonth.Value.AddMonths(1).AddDays(-1);
            }

            var existModel = await _orderService.Get(f => f.OrderNo == model.OrderNo);
            if (existModel != null)
            {
                throw new ErrorCodeException(-1, $"订单号码：{model.OrderNo}，重复订单号！");
            }

            if (model.InsuranceCompany == "阳光保险公司")
            {
                var resp = await _insuranceApiService.GetOrderDetail(model.OrderNo);
                if (resp == null)
                {
                    throw new ErrorCodeException(-1, "接口调用异常");
                }
                if (resp.Code != 200)
                {
                    throw new ErrorCodeException(-1, $"接口调用异常,{resp.Msg}");
                }
                if (resp.Data?.gnteeList == null || resp.Data.gnteeList.Count == 0)
                {
                    throw new ErrorCodeException(-1, "接口未返回参保方案");
                }

                model.Records = resp.Data.gnteeList.Select(f => new Live_BusInsuracneOrderPlan()
                {
                    PlanName = f.GnteeName,
                    PlanNameID = Convert.ToInt32(f.GnteeId),
                    PlanPriceAmount = 0,
                }).ToList();

                SyncOrderPlanSummary(model);
            }
            else
            {
                ValidateOrderPlans(model.Records);
                SyncOrderPlanSummary(model);
            }

            if (!model.InsuranceCompany.StartsWith("大地365"))
            {

                if (model.Records.Count>1)
                {
                    throw new ErrorCodeException(-1, $"此保险公司不允许多参保方案！");
                }
            }

            await _orderService.AddNav(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_BusInsuracneOrderExtension model)
        {

            if (model.InsuranceCompany == "阳光保险公司")
            {
                var resp = await _insuranceApiService.GetOrderDetail(model.OrderNo);
                if (resp == null)
                {
                    throw new ErrorCodeException(-1, "接口调用异常");
                }
                if (resp.Code != 200)
                {
                    throw new ErrorCodeException(-1, $"接口调用异常,{resp.Msg}");
                }
                if (resp.Data?.gnteeList == null || resp.Data.gnteeList.Count == 0)
                {
                    throw new ErrorCodeException(-1, "接口未返回参保方案");
                }

                model.Records = resp.Data.gnteeList.Select(f => new Live_BusInsuracneOrderPlan()
                {
                    PlanName = f.GnteeName,
                    PlanNameID = Convert.ToInt32(f.GnteeId),
                    PlanPriceAmount = 0,
                }).ToList();
            }


            ValidateOrderPlans(model.Records);
            SyncOrderPlanSummary(model);

            if (!model.InsuranceCompany.StartsWith("大地365"))
            {

                if (model.Records.Count > 1)
                {
                    throw new ErrorCodeException(-1, $"此保险公司不允许多参保方案！");
                }
            }
            await _orderService.UpdateNav(model);
            return Ok();
        }

        private static void ValidateOrderPlans(List<Live_BusInsuracneOrderPlan> records)
        {
            if (records == null || records.Count == 0)
            {
                throw new ErrorCodeException(-1, "参保方案不能为空");
            }
            if (records.Any(f => string.IsNullOrWhiteSpace(f.PlanName)))
            {
                throw new ErrorCodeException(-1, "参保方案名称不能为空");
            }
            //if (records.Any(f => f.PlanNameID <= 0))
            //{
            //    throw new ErrorCodeException(-1, "参保方案ID不能为空");
            //}
        }

        private static void SyncOrderPlanSummary(Live_BusInsuracneOrderExtension model)
        {
            model.PlanName = string.Join('、', model.Records.Select(f => f.PlanName));
            model.PlanNameID = model.Records.First().PlanNameID;
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _orderService.DeleteNav(f => f.ID == id);
            return Ok();
        }
    }
}
