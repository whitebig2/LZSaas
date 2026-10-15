using LZ.Core.Attributes;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.ShopBasic.API;
using LZ.ShopBasic.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.ShopBasic.API
{
    public class ExpressController(IExpressService expressService) : ShopBasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Express>();
            if (!string.IsNullOrEmpty(query.Types))
            {
                var typeList=query.Types.Split(',');
                expable.And(it=>typeList.Contains(it.ExpressType));
            }
            var exp = expable.ToExpression();
            var models = await expressService.List(exp);
            return Ok(new { items = models});
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_Express model)
        {
            model.CreateTime = DateTime.Now;
            await expressService.Add(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_Express model)
        {
            await expressService.Update(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_Express> Get(int id)
        {
            var model = await expressService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<IActionResult> InfoList(string expressNo)
        {
            if (string.IsNullOrEmpty(expressNo))
            {
                throw new ErrorCodeException(-1, "快递单号错误");
            }
            var models = await expressService.InfoList(expressNo);
            return Ok(new { items = models });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await expressService.Delete(id);
            return Ok();
        }
    }
}
