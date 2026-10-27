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

namespace LZ.Users.API.Controllers
{
    [AllowAnonymous]
    public class BusInsuranceWorkPlanController : UsersControllerBase
    {
        private readonly IBusInsuranceWorkPlanService _workPlanService;

        public BusInsuranceWorkPlanController(IBusInsuranceWorkPlanService workPlanService)
        {
            _workPlanService = workPlanService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, string workName, string planName, string insuranceCompany, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BusInsuranceWorkPlan>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            var models = await _workPlanService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }


        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string customerName, string workName, string planName, string insuranceCompany)
        {

            var exp = Expressionable.Create<Model.Live_BusInsuranceWorkPlan>();
            exp.AndIF(!string.IsNullOrWhiteSpace(insuranceCompany), f => f.InsuranceCompany.Contains(insuranceCompany));
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(workName), f => f.WorkName.Contains(workName));
            exp.AndIF(!string.IsNullOrWhiteSpace(planName), f => f.PlanName.Contains(planName));
            var models = await _workPlanService.List(exp.ToExpression());

            var exportDataList = models.Select(f => new BusInsuranceWorkPlanExcelTemplate()
            {
                CustomerName = f.CustomerName,
                InsuranceCompany = f.InsuranceCompany,
                IsEnable = f.IsEnable ? "是" : "否",
                PlanName = f.PlanName,
                WorkName = f.WorkName,
                InsuranceType = f.InsuranceType.ToString()

            }).OrderBy(f => f.CustomerName).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "商保工种方案");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "商保工种方案.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
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


 

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadTemplate(IFormFile file)
        {
            List<BusInsuranceWorkPlanExcelTemplate> configs = new List<BusInsuranceWorkPlanExcelTemplate>();

            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                configs = NpoiExcelHelper.ReadExcelFile<BusInsuranceWorkPlanExcelTemplate>(ms, "");
            }
            if (configs == null || configs.Count <= 0)
            {
                throw new ErrorCodeException(-1, "未能读取到数据，请检查模板文件数据是否存在！");
            }
            var models = configs.Where(f=>!string.IsNullOrWhiteSpace(f.CustomerName)).Select(f => new Live_BusInsuranceWorkPlan()
            {
                CreateTime = DateTime.Now,
                PlanName = f.PlanName ?? string.Empty,
                CustomerName = f.CustomerName.Trim(),
                InsuranceCompany = f.InsuranceCompany ?? string.Empty,
                IsEnable = f.IsEnable.Trim() == "是",
                IsUseBusInsurance = true,
                InsuranceType = Enum.Parse<InsuranceTypeEnumType>(f.InsuranceType),
                InsuranceWorkClass = string.Empty,
                InsuranceWorkName = f.InsuranceWorkName,
                WorkName = f.WorkName ?? string.Empty.Trim(),
            }).ToList();

            await _workPlanService.Add(models);
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
            var model = await _workPlanService.Get(f=>f.ID==id);
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(Live_BusInsuranceWorkPlan model)
        {


          var exist=await  _workPlanService.Get(f => f.CustomerName == model.CustomerName && f.WorkName == model.WorkName);
            if (exist!=null)
            {
                throw new ErrorCodeException(-1, "此工种已配置，无需重复添加");
            }

            model.CreateTime=DateTime.Now;
            await _workPlanService.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Live_BusInsuranceWorkPlan model)
        {
            var exist = await _workPlanService.Get(f => f.CustomerName == model.CustomerName && f.WorkName == model.WorkName && f.ID != model.ID);
            if (exist != null)
            {
                throw new ErrorCodeException(-1, "此工种已配置，无法修改为此工种！");
            }
            await _workPlanService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _workPlanService.Delete(f=>f.ID==id);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _workPlanService.Delete(f => f.ID>0);
            return Ok();
        }
    }
}
