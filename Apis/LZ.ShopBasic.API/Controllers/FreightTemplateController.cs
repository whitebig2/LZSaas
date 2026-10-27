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
    public class FreightTemplateController(IFreightTemplateService freightTemplateService) : ShopBasicControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_FreightTemplate>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                if (query.CompanyID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CompanyID);
                }
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await freightTemplateService.List(exp);
            return Ok(new { items = models});
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_FreightTemplate model)
        {
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.CreateTime = DateTime.Now;
            await freightTemplateService.Add(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_FreightTemplate model)
        {
            await freightTemplateService.Update(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_FreightTemplate> Get(int id)
        {
            var model = await freightTemplateService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await freightTemplateService.Delete(id);
            return Ok();
        }
    }
}
