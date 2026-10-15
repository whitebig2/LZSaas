using LZ.Core.Attributes;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.ShopBasic.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.ShopBasic.API
{
    public class ReturnAddressController(IReturnAddressService returnAddressService) : ShopBasicControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_ReturnAddress>();
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
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin && user.UserType == (int)CompanyUserType.Shop)
                {
                    expable.And(it => it.ShopID == user.ShopID);
                }
            }
            expable.AndIF(query.ShopID.HasValue, it => it.ShopID == query.ShopID);
            var exp = expable.ToExpression();
            var models = await returnAddressService.List(exp);
            return Ok(new { items = models });
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_ReturnAddress model)
        {
            if (model == null || string.IsNullOrEmpty(model.Contact) || string.IsNullOrEmpty(model.Phone)
                || string.IsNullOrEmpty(model.Province) || string.IsNullOrEmpty(model.City)
                || string.IsNullOrEmpty(model.Address))
            {
                throw new ErrorCodeException(-1, "请完善退货地址信息");
            }
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.ShopID = user.ShopID;
            model.CreateTime = DateTime.Now;
            await returnAddressService.Add(model);
            return Ok();
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_ReturnAddress model)
        {
            if (model == null || model.ID <= 0)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            await returnAddressService.Update(model);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Shop_ReturnAddress> Get(int id)
        {
            var model = await returnAddressService.Get(id);
            if (model == null)
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
            await returnAddressService.Delete(id);
            return Ok();
        }
    }
}
