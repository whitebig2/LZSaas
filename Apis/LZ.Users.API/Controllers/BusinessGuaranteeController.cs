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

namespace LZ.Users.API.Controllers
{
    [AllowAnonymous]
    public class BusinessGuaranteeController : UsersControllerBase
    {


        private readonly IBusinessInsuranceOrderService   _orderService;
        private readonly IEntryService  _entryService;
        private readonly IBusinessInsuranceConfigService _configService;
        private readonly IBusinessGuaranteeService _guaranteeService;

        public BusinessGuaranteeController(IBusinessInsuranceOrderService orderService, IEntryService entryService, IBusinessInsuranceConfigService configService, IBusinessGuaranteeService guaranteeService)
        {
            _orderService = orderService;
            _entryService = entryService;
            _configService = configService;
            _guaranteeService = guaranteeService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName,string name,string idCardNo, BusinessInsuranceStatusEnumType?status, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BusinessGuarantee>();
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            var models = await _guaranteeService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
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
        public async Task<IActionResult> Get( int id)
        {
            var model = await _guaranteeService.Get(f=>f.Id==id);
            return Ok(model);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _guaranteeService.Delete(f=>f.Id==id);
            return Ok();
        }
    }
}
