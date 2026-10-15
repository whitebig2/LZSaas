using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SalaryConfigController : ApiControllerBase
    {

        private readonly ISalaryConfigService _salaryConfigService;
        public SalaryConfigController(ISalaryConfigService salaryConfigService)
        {
            _salaryConfigService = salaryConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> List(string customerName, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_SalaryConfig>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);

            exp.AndIF(!string.IsNullOrWhiteSpace(customerName), f => f.CustomerName.Contains(customerName));
            var models = await _salaryConfigService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_SalaryConfig model)
        {
            model.Id = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await _salaryConfigService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_SalaryConfig> Get(Guid id)
        {
            var model = await _salaryConfigService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_SalaryConfig model)
        {
            await _salaryConfigService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm]Guid id)
        {
            await _salaryConfigService.Delete(id);
            return Ok();
        }
    }
}
