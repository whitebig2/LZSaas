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
    public class PicturesController(IPicturesService picturesService) : ShopBasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_V_Pictures>();
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
            expable.AndIF(query.CID.HasValue, it => it.AssortID == query.CID);
            expable.AndIF(!string.IsNullOrEmpty(query.Name), it =>it.Name.Contains(query.Name));
            var exp = expable.ToExpression();
            var models = await picturesService.List(exp);
            return Ok(new { items = models});
        }
        
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_Pictures model)
        {
            var user= CurrentUser;
            model.UserID = user.UserID;
            model.ShopID = user.ShopID;
            model.CreateTime = DateTime.Now;
            await picturesService.Add(model);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Adds([FromBody] List<Model.Shop_Pictures> models)
        {
            var user = CurrentUser;
            foreach (var model in models)
            {
                model.ShopID = user.ShopID;
                model.CreateTime = DateTime.Now;
            }
            await picturesService.Add(models);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAssort([FromForm] string ids, [FromForm] int assortId)
        {
            if (string.IsNullOrEmpty(ids) || assortId==0)
            {
                throw new ErrorCodeException(-1, "提交信息不正确");
            }
            var pictureIds=ids.Split(',');
            await picturesService.Update(pictureIds,assortId);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateName([FromForm] int id, [FromForm] string name)
        {
            if (string.IsNullOrEmpty(name) || id==0)
            {
                throw new ErrorCodeException(-1, "提交信息不正确");
            }
            await picturesService.Update(id, name);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_Pictures model)
        {
            await picturesService.Update(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Shop_Pictures> Get(int id)
        {
            var model = await picturesService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await picturesService.Delete(id);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Deletes([FromForm] string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                throw new ErrorCodeException(-1, "提交信息不正确");
            }
            var pictureIds = ids.Split(',');
            await picturesService.Delete(pictureIds);
            return Ok();
        }
    }
}
