using LZ.ApiService.Services;
using LZ.Basic.Service.Interface;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{

    [AllowAnonymous]
    public class CityPartnerController : BasicControllerBase
    {
        private readonly ICityPartnerService _cityPartnerService;

        public CityPartnerController(ICityPartnerService cityPartnerService)
        {
            _cityPartnerService = cityPartnerService;
        }
        [HttpGet]
        public async Task<IActionResult> PageList(string title, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_CityPartner>();
            exp.AndIF(!string.IsNullOrWhiteSpace(title), it => it.Title.Contains(title));
            var models = await _cityPartnerService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string title,bool? enabled)
        {
            var exp = Expressionable.Create<Model.Live_CityPartner>();
            exp.AndIF(!string.IsNullOrWhiteSpace(title), it => it.Title.Contains(title));
            exp.AndIF(enabled.HasValue, it => it.IsEnabled==enabled.Value);
            var models = await _cityPartnerService.List(exp.ToExpression());
            return Ok(new { items = models});
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_CityPartner model)
        {
            model.ID=Guid.NewGuid();
            model.CreateTime=DateTime.Now;
            await _cityPartnerService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _cityPartnerService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_CityPartner model)
        {

            await _cityPartnerService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _cityPartnerService.Delete(f => f.ID == id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
           
            return Ok("succ");
        }
    }
}
