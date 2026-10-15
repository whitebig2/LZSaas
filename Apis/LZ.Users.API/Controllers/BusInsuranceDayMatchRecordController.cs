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
    public class BusInsuranceDayMatchRecordController : UsersControllerBase
    {
        private readonly IBusInsuranceDayMatchRecordService _dayMatchRecordService;

        public BusInsuranceDayMatchRecordController(IBusInsuranceDayMatchRecordService dayMatchRecordService)
        {
            _dayMatchRecordService = dayMatchRecordService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, string name, string cardID, string workName, string planName, string insuranceCompany, bool? isAddRecord, BusInsuranceDayMatchRecordStatusEnumType? status, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BusInsuranceDayMatchRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(status!=null, f => f.Status==status.Value);
            exp.AndIF(isAddRecord != null, f => f.IsAddRecord == isAddRecord.Value);
            var models = await _dayMatchRecordService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string customerName, string name, string cardID, string workName, string planName, string insuranceCompany, bool? isAddRecord, BusInsuranceDayMatchRecordStatusEnumType? status)
        {
            var exp = Expressionable.Create<Model.Live_BusInsuranceDayMatchRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            exp.AndIF(status != null, f => f.Status == status.Value);
            exp.AndIF(isAddRecord != null, f => f.IsAddRecord == isAddRecord.Value);
            var models = await _dayMatchRecordService.List(exp.ToExpression());
            var exportDataList = models.Select(f => new BusInsuranceDayMatchRecordExcelTemplate()
            {
                CustomerName = f.CustomerName,
                InsuranceCompany = f.InsuranceCompany,
                PlanName = f.PlanName,
                WorkName = f.WorkName,
                CardID = f.CardID,
                Name = f.Name,
                Age = f.Age,
                CreateTime = f.CreateTime?.ToString("yyyy-MM-dd"),
                EndDate = f.EndDate?.ToString("yyyy-MM-dd"),
                IsAddRecord = f.IsAddRecord ? "增员" : "减员",
                OrderNo = f.OrderNo,
                PushResult = f.PushResult,
                RecordDate = f.RecordDate?.ToString("yyyy-MM-dd"),
                Remark = f.Remark,
                Sex = f.Sex,
                SignUnitName = f.SignUnitName,
                StartDate = f.StartDate?.ToString("yyyy-MM-dd"),
                Status = f.Status,
                WorkClass = f.WorkClass,
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "商保日增减记录");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保日增减记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }





        /// <summary>
        /// 下载模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DownloadTemplate()
        {
            using var stream = new MemoryStream();
            NpoiExcelHelper.WriteExcelTemplateFile<BusInsuranceWorkPlanExcelTemplate>(stream, "商保工种方案");
            byte[] fileBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保工种方案.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }


 

        ///// <summary>
        ///// 上传
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> UploadTemplate(IFormFile file)
        //{
        //    List<BusInsuranceWorkPlanExcelTemplate> configs = new List<BusInsuranceWorkPlanExcelTemplate>();

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        file.CopyTo(ms);
        //        configs = NpoiExcelHelper.ReadExcelFile<BusInsuranceWorkPlanExcelTemplate>(ms, "");
        //    }
        //    if (configs == null || configs.Count <= 0)
        //    {
        //        throw new ErrorCodeException(-1, "未能读取到数据，请检查模板文件数据是否存在！");
        //    }
        //    var models = configs.Select(f => new Live_BusInsuranceWorkPlan()
        //    {
        //        CreateTime = DateTime.Now,
        //        PlanName = f.PlanName.Trim(),
        //        CustomerName = f.CustomerName.Trim(),
        //        InsuranceCompany = f.InsuranceCompany.Trim(),
        //        IsEnable = f.IsEnable.Trim() == "是",
        //        WorkName = f.WorkName.Trim(),
        //    }).ToList();

        //    await _workPlanService.Add(models);
        //    return Ok();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get( int id)
        {
            var model = await _dayMatchRecordService.Get(f=>f.ID==id);
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Live_BusInsuranceDayMatchRecord model)
        {
            model.CreateTime=DateTime.Now;
            await _dayMatchRecordService.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_BusInsuranceDayMatchRecord model)
        {
            await _dayMatchRecordService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _dayMatchRecordService.Delete(f=>f.ID==id);
            return Ok();
        }
    }
}
