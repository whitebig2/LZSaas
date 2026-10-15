using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using LZ.Product.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;

namespace LZ.Product.API
{
   
    public class CouponController(ICouponService couponService, ICompanyConfigService companyConfigService,IAuthService authService) : ProductControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Coupon>();
          
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.CouponName.Contains(query.Name));
            }
            expable.AndIF(query.ShopID.HasValue, it => it.ShopID == query.ShopID);
            expable.AndIF(query.Status.HasValue, it => it.Status == query.Status);
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
                if(user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    if (user.UserType == (int)CompanyUserType.Shop)
                    {
                        expable.And(it => it.ShopID == user.ShopID);
                    }
                }
            }
            var exp = expable.ToExpression();
            var models = await couponService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Products([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_V_CouponProduct>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.CouponName.Contains(query.Name));
            }
            expable.AndIF(query.CouponID.HasValue, it => it.CouponID == query.CouponID);
            if (!string.IsNullOrEmpty(query.IDS))
            {
                var idList = query.IDS.Split(',');
                expable.And(it => idList.Contains(it.ProductID.ToString()));
            }
            var exp = expable.ToExpression();
            var models = await couponService.ProductsView(exp);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.SubmitCoupon model)
        {
            if (model == null || model.Coupon == null)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            var user = CurrentUser;
            model.Coupon.CompanyID=user.CompanyID;
            model.Coupon.CompanyGID = user.CompanyGID;
            model.Coupon.CreateTime = DateTime.Now;
            model.Coupon.ShopID = user.ShopID;
            model.Coupon.CouponGID = Guid.NewGuid();
            model.Coupon.UseCouponCount = 0;
            if (!model.Coupon.CouponCount.HasValue) model.Coupon.CouponCount = 0;
            if (!model.Coupon.UserLimitCount.HasValue) model.Coupon.UserLimitCount = 0;
            if (!model.Coupon.LimitNumber.HasValue) model.Coupon.LimitNumber = 0;
            model.Coupon.CouponID = await couponService.Add(model.Coupon);
            foreach (var item in model.Products)
            {
                item.CouponGID = model.Coupon.CouponGID;
                item.CouponID = model.Coupon.CouponID;
            }
            await couponService.AddProducts(model.Products);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SubmitCoupon model)
        {
            if (model == null || model.Coupon == null)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            await couponService.UpdateAsync(model.Coupon);
            await couponService.UpdateProducts(model.Products, model.Coupon.CouponID, model.Coupon.CouponGID.Value);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Shop_Coupon> Get(int id)
        {
            var model = await couponService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Shop_Coupon> GetInfo(Guid id)
        {
            var model = await couponService.Get(it=>it.CompanyGID==id);
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
            await couponService.Delete(id);
            return Ok();
        }
    }
}
