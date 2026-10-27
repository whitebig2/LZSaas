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
    public class ProfitPartnerConfigController : ApiControllerBase
    {

        private readonly IProfitPartnerConfigService _configService;

        public ProfitPartnerConfigController(IProfitPartnerConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string partnerName, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_V_ProfitPartnerConfig>();
            exp.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            exp.AndIF(!string.IsNullOrWhiteSpace(partnerName), f => f.PartnerName.Contains(partnerName));
            var models = await _configService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add(Model.Live_ProfitPartnerConfig model)
        {
            model.ID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
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
        public async Task<IActionResult> Update([FromBody] Model.Live_ProfitPartnerConfig model)
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
    }
}
