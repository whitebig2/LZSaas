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
    public class CitySalesmanController : BasicControllerBase
    {
        private readonly ICitySalesmanService _citySalesmanService;

        public CitySalesmanController(ICitySalesmanService citySalesmanService)
        {
            _citySalesmanService = citySalesmanService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string name, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_CitySalesman>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), it => it.Name.Contains(name));
            var models = await _citySalesmanService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(string name)
        {
            var exp = Expressionable.Create<Model.Live_CitySalesman>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), it => it.Name.Contains(name));
            var models = await _citySalesmanService.List(exp.ToExpression());
            return Ok(new { items = models});
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_CitySalesman model)
        {
            model.ID=Guid.NewGuid();
            model.CreateTime=DateTime.Now;
            await _citySalesmanService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _citySalesmanService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_CitySalesman model)
        {
            await _citySalesmanService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _citySalesmanService.Delete(f => f.ID == id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
           
            return Ok("succ");
        }
    }
}
