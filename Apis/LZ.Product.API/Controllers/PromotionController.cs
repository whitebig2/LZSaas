using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Product.API
{
   
    public class PromotionController(IPromotionService promotionService, ICompanyConfigService companyConfigService) : ProductControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Promotion>();
          
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.PromotionName.Contains(query.Name));
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
            var exp = expable.ToExpression();
            var models = await promotionService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Items([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_V_PromotionProduct>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.PromotionName.Contains(query.Name));
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
            expable.AndIF(query.PromotionID.HasValue, it => it.PromotionID == query.PromotionID);
            var exp = expable.ToExpression();
            var models = await promotionService.Items(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.SubmitPromotion data)
        {
            var user = CurrentUser;
            if(data==null || data.Promotion==null || data.Items == null)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            data.Promotion.CompanyID = user.CompanyID;
            data.Promotion.CompanyGID = user.CompanyGID;
            data.Promotion.PromotionGID = Guid.NewGuid();
            data.Promotion.ShopID = user.ShopID;
            data.Promotion.CreateTime = DateTime.Now;
            data.Promotion.PromotionID = await promotionService.Add(data.Promotion);
            foreach (var item in data.Items)
            {
                item.PromotionGID = data.Promotion.PromotionGID;
                item.PromotionID = data.Promotion.PromotionID;
            }
            await promotionService.AddItems(data.Items);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SubmitPromotion data)
        {
            if (data == null || data.Promotion == null || data.Items == null)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            await promotionService.UpdateAsync(data.Promotion);
            await promotionService.UpdateItems(data.Items, data.Promotion.PromotionID, data.Promotion.PromotionGID.Value);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Shop_Promotion> Get(int id)
        {
            var model = await promotionService.Get(id);
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
            await promotionService.Delete(id);
            return Ok();
        }
    }
}
