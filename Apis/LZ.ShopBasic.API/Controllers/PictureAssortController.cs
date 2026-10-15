using LZ.Core.Attributes;
using LZ.Model.Enums;
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
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class PictureAssortController(IPictureAssortService pictureAssortService) : ShopBasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_PictureAssort>();
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
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    if (user.UserType == (int)CompanyUserType.Shop)
                    {
                        expable.And(it => it.ShopID == user.ShopID);
                    }
                }
            }
            expable.AndIF(query.ParentID.HasValue, it => it.ParentID == query.ParentID);
            expable.AndIF(!string.IsNullOrEmpty(query.Name), it => it.AssortName.Contains(query.Name));
            var exp = expable.ToExpression();
            var models = await pictureAssortService.List(exp);
            return Ok(new { items = models});
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_PictureAssort model)
        {
            var user = CurrentUser;
            model.UserID = user.UserID;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.ShopID = user.ShopID;
            model.CreateTime = DateTime.Now;
            model.AssortGID=Guid.NewGuid();
            await pictureAssortService.Add(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_PictureAssort model)
        {
            await pictureAssortService.Update(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_PictureAssort> Get(int id)
        {
            var model = await pictureAssortService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await pictureAssortService.Delete(id);
            return Ok();
        }
    }
}
