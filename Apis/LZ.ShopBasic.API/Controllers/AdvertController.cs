using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.ShopBasic.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.ShopBasic.API
{
    public class AdvertController(IAdvertService advertService, ICompanyConfigService companyConfigService) : ShopBasicControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] ShopBasicPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Advert>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                if (query.CompanyID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CompanyID);
                }
            }
            else
            {
                var companygid = HttpContext.Request.Headers["company-guid"].ToString();
                if (!string.IsNullOrEmpty(companygid))
                {
                    var companyinfo = await companyConfigService.Get(new Guid(companygid));
                    expable.And(it => it.CompanyID == companyinfo.Company.CompanyID);
                }
                else
                {
                    expable.And(it => it.CompanyID == user.CompanyID);
                }
            }
            if (!string.IsNullOrEmpty(query.Location))
            {
                expable.And(it => it.Location.Contains(query.Location));
            }
            var exp = expable.ToExpression();
            var orderBySort = user.PlatformType == Model.Enums.PlatformType.XFZMini;
            var models = await advertService.List(exp, orderBySort);
            return Ok(new { items = models });
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_Advert model)
        {
            if (model == null || string.IsNullOrEmpty(model.Location))
            {
                throw new ErrorCodeException(-1, "请选择广告位");
            }
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            model.CreateTime = DateTime.Now;
            await advertService.Add(model);
            return Ok();
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_Advert model)
        {
            if (model == null || model.ID <= 0)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            await advertService.Update(model);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Shop_Advert> Get(int id)
        {
            var model = await advertService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Shop_Advert> GetInfo(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ErrorCodeException(-1, "请选择广告位");
            }
            var companygid = HttpContext.Request.Headers["company-guid"].ToString();
            return await advertService.Get(it=>it.Location==location && it.CompanyGID==new Guid(companygid));
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await advertService.Delete(id);
            return Ok();
        }
    }
}
