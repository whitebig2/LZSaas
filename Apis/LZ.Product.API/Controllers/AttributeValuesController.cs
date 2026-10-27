using Microsoft.AspNetCore.Mvc;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using LZ.Core.Attributes;

namespace LZ.Product.API
{
    public class AttributeValuesController(IAttributeValuesService attributeValuesService) : ProductControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_AttributeValues>();
          
            if (query.ParentID.HasValue)
            {
                expable.And(it => it.AttributeID == query.ParentID);
            }
            var exp = expable.ToExpression();
            var models = await attributeValuesService.List(exp);
            return Ok(new { items = models });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_AttributeValues model)
        {
            await attributeValuesService.Add(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_AttributeValues model)
        {
            await attributeValuesService.UpdateAsync(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_AttributeValues> Get(int id)
        {
            var model = await attributeValuesService.Get(id);
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
            await attributeValuesService.Delete(id);
            return Ok();
        }
    }
}
