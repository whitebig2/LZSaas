using LZ.Core.Attributes;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Product.API
{
    public class ProductTypeController(IProductTypesService productTypesService) : ProductControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_ProductTypes>();
          
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.TypeName.Contains(query.Name));
            }
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
            var models = await productTypesService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_ProductTypes model)
        {
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            if(await productTypesService.Exist(it=>it.TypeName==model.TypeName && it.CompanyID== user.CompanyID))
            {
                throw new ErrorCodeException(-1, "该类型名称已存在!");
            }
            await productTypesService.Add(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_ProductTypes model)
        {
            var user = CurrentUser;
            if (await productTypesService.Exist(it => it.TypeName == model.TypeName && it.CompanyID == user.CompanyID && it.TypeID!=model.TypeID))
            {
                throw new ErrorCodeException(-1, "该类型名称已存在!");
            }
            await productTypesService.UpdateAsync(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_ProductTypes> Get(int id)
        {
            var model = await productTypesService.Get(id);
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
            await productTypesService.Delete(id);
            return Ok();
        }
    }
}
