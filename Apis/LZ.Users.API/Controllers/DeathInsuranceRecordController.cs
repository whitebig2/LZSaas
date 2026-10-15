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
    public class DeathInsuranceRecordController : UsersControllerBase
    {


        private readonly IDeathInsuranceRecordService _service;

        public DeathInsuranceRecordController(IDeathInsuranceRecordService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string name,string cardID, string workName, string signUnitName, string planName, string insuranceCompany, BusInsuranceRecordStatusEnumType? status,[FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_DeathInsuranceRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(status != null, f => f.Status == status.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string customerName, string name, string cardID, string workName, string signUnitName, string planName, string insuranceCompany, BusInsuranceRecordStatusEnumType? status)
        {
            var exp = Expressionable.Create<Model.Live_DeathInsuranceRecord>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(status != null, f => f.Status == status.Value);


            var models = await _service.List(exp.ToExpression());

            var exportDataList = models.Select(f => new DeathInsuranceExcelTemplate()
            {
                Name = f.Name,
                CardID = f.CardID,
                StartDate = f.StartDate.Value.ToString("yyyy-MM-dd"),
                EndDate = f.EndDate.Value.ToString("yyyy-MM-dd"),
                WorkName = f.WorkName,
                SignUnitName = f.SignUnitName,
                Status = f.Status.ToString()

            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteDeathInsurance(exportDataList);
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "参保记录.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get( int id)
        {
            var model = await _service.Get(f=>f.ID==id);
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Live_DeathInsuranceRecord model)
        {
            model.CreateTime=DateTime.Now;
            await _service.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_DeathInsuranceRecord model)
        {
            await _service.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _service.Delete(f=>f.ID==id);
            return Ok();
        }


        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file)
        {
            List<DeathInsuranceExcelTemplate> configs = new List<DeathInsuranceExcelTemplate>();
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                configs = NpoiExcelHelper.ReadExcelFile<DeathInsuranceExcelTemplate>(ms, "");
            }
            var insuranceStartDate = new DateTime(2026, 05, 01);
            var dataList = configs.Select(f => new Live_DeathInsuranceRecord()
            {
                CreateTime = DateTime.Now,
                ChangeType = "批增",
                ApplyDate = DateTime.Today,
                CardID = f.CardID,
                CustomerName = string.Empty,
                EndDate = Convert.ToDateTime(f.EndDate).Date,
                ID = 0,
                InsuranceEndDate = Convert.ToDateTime(f.EndDate).Date,
                InsuranceStartDate = Convert.ToDateTime(f.StartDate).Date,
                InsuranceName = "医疗健康险",
                IsHasSocialInsurance = false,
                Name = f.Name,
                Passport = string.Empty,
                PlanName = string.Empty,
                Remark = string.Empty,
                SignUnitAddress = string.Empty,
                SignUnitName = f.SignUnitName,
                StartDate = Convert.ToDateTime(f.StartDate).Date,
                Status = BusInsuranceRecordStatusEnumType.已增员,
                WorkName = f.WorkName,

            }).ToList();
            await _service.Add(dataList);
            return Ok();
        }




    }
}
