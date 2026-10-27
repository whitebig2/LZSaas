using LZ.Core.Attributes;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.ShopBasic.API;
using LZ.ShopBasic.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SqlSugar;

namespace LZ.ShopBasic.API
{
    public class BrandController(IBrandService brandService) : ShopBasicControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Brand>();
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
            var models = await brandService.List(exp);
            return Ok(new { items = models});
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_Brand model)
        {
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.CreateTime = DateTime.Now;
            await brandService.Add(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_Brand model)
        {
            await brandService.Update(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_Brand> Get(int id)
        {
            var model = await brandService.Get(id);
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
            await brandService.Delete(id);
            return Ok();
        }
    }
}
