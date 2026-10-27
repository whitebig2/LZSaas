using LZ.Common;
using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate.profit;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProfitConfigController : ApiControllerBase
    {

        private readonly IProfitConfigService _configService;

        public ProfitConfigController(IProfitConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string customerName, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_ProfitConfig>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            var models = await _configService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string customerName, DateTime? profitMonth)
        {
            var exp = Expressionable.Create<Model.Live_V_ProfitConfig>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            var models = await _configService.ListView(exp.ToExpression());
            var exportDataList = models.Select(model => new ProfitConfigExcelTemplate()
            {
                AgentCountTypeName = model.AgentCountType.ToString(),
                AgentPaidPercent = model.AgentPaidPercent,
                BusinessPercent = model.BusinessPercent,
                BusInsurance5Amount = model.BusInsurance5Amount,
                BusInsuranceAmount = model.BusInsuranceAmount,
                BusInsuranceCountTypeName = model.BusInsuranceCountType == 1 ? "按天计算" : "增减替换",
                CustomerName = model.CustomerName,
                CustomerThingPercent = model.CustomerThingPercent,
                DeductionAmount = model.DeductionAmount,
                JoinManagePercent = model.JoinManagePercent,
                PreBusInsuranceTotalAmount = model.PreBusInsuranceTotalAmount,
                RiskAmount = model.RiskAmount,
                SocialRepayRiskBaseAmount = model.SocialRepayRiskBaseAmount,
                SubsidyAmount = model.SubsidyAmount,
                IsEnabled = model.IsEnabled ? "启用" : "禁用",
                DeathRiskAmount= model.DeathRiskAmount,
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "毛利配置");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "毛利配置.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_ProfitConfig model)
        {
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            var existModel = await _configService.Get(f => f.CustomerGID == model.CustomerGID);
            if (existModel != null)
            {
                throw new ErrorCodeException(-1, "此客户单位毛利配置已存在，无法重复创建");
            }
            await _configService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _configService.Get(f=>f.ID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model); 
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_ProfitConfig model)
        {
            await _configService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]Guid id)
        {
            await _configService.Delete(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> InjuryList([FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_ProfitConfig>();
            var models = await _configService.ListView(exp.ToExpression());
            return Ok(new { items = models, count = query.Count });
        }
    }
}
