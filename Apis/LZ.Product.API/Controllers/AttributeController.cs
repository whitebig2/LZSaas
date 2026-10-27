using LZ.Core.Attributes;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;

namespace LZ.Product.API
{
    public class AttributeController(IAttributeService attributeService,IAttributeValuesService attributeValuesService) : ProductControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Attribute>();
          
            if (query.ParentID.HasValue)
            {
                expable.And(it => it.TypeID== query.ParentID);
            }
            var exp = expable.ToExpression();
            var models = await attributeService.List(exp);
            return Ok(new { items = models});
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_Attribute model)
        {
            await attributeService.Add(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Adds([FromForm] int typeID, [FromForm] string name, [FromForm] string values)
        {
            if(typeID == 0 || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(values))
            {
                throw new ErrorCodeException(-1, "规格集信息错误");
            }
            var model = new Model.Shop_Attribute();
            model.AttributeName = name;
            model.TypeID = typeID;
            var vs = values.Split(',');
            var items=new List<Model.Shop_AttributeValues>();
            foreach ( var v in vs)
            {
                if (!string.IsNullOrEmpty(v))
                {
                    var item = new Model.Shop_AttributeValues();
                    item.Name = v;
                    items.Add(item);
                }
            }
            if (items.Count == 0)
            {
                 throw new ErrorCodeException(-1, "规格值错误");
            }
            await attributeService.Add(model, items);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> ListItems(int type)
        {

            try
            {
                var models = await attributeService.List(it => it.TypeID == type);
                var ids = models.Select(it => it.AttributeID).ToList();
                var items = await attributeValuesService.List(it => ids.Contains(it.AttributeID.Value));

                return Ok(new { models, items });

            }
            catch (Exception e)
            {
                throw new ErrorCodeException(-1, "获取规格错误");
            }

        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_Attribute model)
        {
            await attributeService.UpdateAsync(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_Attribute> Get(int id)
        {
            var model = await attributeService.Get(id);
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
            await attributeService.Delete(id);
            return Ok();
        }
    }
}
