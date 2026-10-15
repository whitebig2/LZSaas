using LZ.Core.Attributes;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using Senparc.Weixin.MP.AdvancedAPIs.MerChant;
using SqlSugar;

namespace LZ.Product.API
{
    public class ShoppingCartController(IShoppingCartService shoppingCartService, IPromotionService promotionService,ICouponItemsService couponItemsService,ICouponService couponService) : ProductControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_V_ShoppingCart>();
            if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
            {
                expable.And(it => it.UserID == user.UserID);
            }

            if (!string.IsNullOrEmpty(query.IDS))
            {
                var idarr = query.IDS.Split(',');
                expable.And(it => idarr.Contains(it.CartID.ToString()));
            }
            var exp = expable.ToExpression();
            var models = await shoppingCartService.List(exp, query);
            var skuids = models.Select(x => x.SkuID);
            var now = DateTime.Now;
            var promotions = await promotionService.Items(it => it.CompanyID == user.CompanyID && skuids.Contains(it.SkuID) && it.StartTime <= now && it.EndTime >= now && it.Enable);
            var productIds = models.Select(it => it.ProductID);
            var coupons = await couponService.ProductsView(it => (productIds.Contains(it.ProductID) || it.CouponUseType == (int)CouponUseType.ProductCurrency) && it.CouponStatus == (int)CouponStatus.Launch && it.StartTime <= now && it.EndTime >= now && it.IsOpen);
            foreach (var model in models)
            {
                var promotion = promotions.Find(it => it.SkuID == model.SkuID);
                if (promotion != null)
                {
                    model.PromotionPrice = promotion.PromotionType == (int)PromotionType.Discount && promotion.PromotionDiscount.HasValue ? model.SalePrice * promotion.PromotionDiscount.Value : promotion.PromotionMoney;
                    model.DataTabs = promotion.PromotionName;
                }
                var productCoupon = coupons.Find(it => it.ProductID == model.ProductID);//商品优惠券
                if (productCoupon != null)
                {
                    model.CouponName = productCoupon.CouponName;
                    model.CouponGID = productCoupon.CouponGID;
                }
                else
                {
                    var currencyCoupon = coupons.Find(it => it.CouponUseType == (int)CouponUseType.ProductCurrency);//通用商品优惠券
                    if (currencyCoupon != null)
                    {
                        model.CouponName = currencyCoupon.CouponName;
                        model.CouponGID = currencyCoupon.CouponGID;
                    }
                }
            }
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> Items(string ids)
        {
            var idarr = ids.Split(',');
            var models = await shoppingCartService.List(it=> idarr.Contains(it.CartID.ToString()));
            return Ok(models);
        }
        /// <summary>
        /// 订单页面专用
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userId"></param>
        /// <param name="couponIds"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpGet]
        public async Task<IActionResult> GetOrderAmount(string ids, int userId = 0, string couponIds = "")
        {
            var user = CurrentUser;
            if (user.PlatformType != Model.Enums.PlatformType.Api)
            {
                userId = user.UserID;
            }
            if (string.IsNullOrEmpty(ids))
            {
                throw new ErrorCodeException(-1, "信息错误");
            }
            var idarr = ids.Split(',');
            var items = await shoppingCartService.List(it => idarr.Contains(it.CartID.ToString()), new PagedModelBase() { Page = 0 });
            var shoppingCart =await GetShoppingCart(items, userId, couponIds);
            if (string.IsNullOrEmpty(couponIds))
            {
                shoppingCart.UseCoupons = shoppingCart.GetUsableCoupons();//可使用的优惠券
                return Ok(shoppingCart);
            }
            else
            {
                return Ok(shoppingCart.CartAmountResult);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAmount()
        {
            var user= CurrentUser;
            var items = await shoppingCartService.List(it =>it.UserID==user.UserID && it.IsCheck, new PagedModelBase() { Page = 0 });
            var shoppingCart =await GetShoppingCart(items, user.UserID);
            return Ok(shoppingCart.CartAmountResult);
        }
        /// <summary>
        /// 购物车提交订单专用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderCartAmount(string ids, int userId = 0)
        {
            var user = CurrentUser;
            if (user.PlatformType != Model.Enums.PlatformType.Api)
            {
                userId = user.UserID;
            }
            if (string.IsNullOrEmpty(ids))
            {
                throw new ErrorCodeException(-1, "信息错误");
            }
            var idarr = ids.Split(',');
            var shoppingCart = new Model.ShoppingCart();
            shoppingCart.Items = await shoppingCartService.List(it => idarr.Contains(it.CartID.ToString()) && it.UserID==userId, new PagedModelBase() { Page = 0 });
            var skuIds = shoppingCart.Items.Select(it => it.SkuID);
            var now = DateTime.Now;
            var promotions = await promotionService.Items(it => skuIds.Contains(it.SkuID) && it.StartTime <= now && it.EndTime >= now && it.Enable);
            shoppingCart.Promotions = promotions;
            return Ok(shoppingCart);
        }
        private async Task<ShoppingCart> GetShoppingCart(List<Shop_V_ShoppingCart> items,int userId, string couponIds = "")
        {
            var shoppingCart = new Model.ShoppingCart();
            shoppingCart.Items = items;
           var skuIds = shoppingCart.Items.Select(it => it.SkuID);
            var productIds = shoppingCart.Items.Select(it => it.ProductID);
            var now = DateTime.Now;
            var promotions = await promotionService.Items(it => skuIds.Contains(it.SkuID) && it.StartTime <= now && it.EndTime >= now && it.Enable);
            shoppingCart.Promotions = promotions;
            if (string.IsNullOrEmpty(couponIds))
            {
                var coupons = await couponItemsService.List(it => it.UserID == userId && it.StartTime <= now && it.EndTime >= now && it.Status == (int)CouponItemStatus.WaitUse && it.CouponStatus == (int)CouponStatus.Launch && (SqlFunc.Subqueryable<Model.Shop_CouponProduct>().Where(s => s.CouponID == it.CouponID && productIds.Contains(s.ProductID)).Any() || it.CouponUseType == (int)CouponUseType.ProductCurrency), new PagedModelBase() { Page = 0 });
                var userCouponIds = coupons.Select(it => it.CouponID);
                var couponProducts = await couponService.ProductsView(it => userCouponIds.Contains(it.CouponID) && productIds.Contains(it.ProductID));
                shoppingCart.Coupons = coupons;
                shoppingCart.CouponProducts = couponProducts;
            }
            else
            {
                var couponIdsList = couponIds.Split(',');
                var coupons = await couponItemsService.List(it => it.UserID == userId && it.StartTime <= now && it.EndTime >= now && it.Status == (int)CouponItemStatus.WaitUse && it.CouponStatus == (int)CouponStatus.Launch && (SqlFunc.Subqueryable<Model.Shop_CouponProduct>().Where(s => s.CouponID == it.CouponID && productIds.Contains(s.ProductID)).Any() || it.CouponUseType == (int)CouponUseType.ProductCurrency) && couponIdsList.Contains(it.ID.ToString()), new PagedModelBase() { Page = 0 });
                var userCouponIds = coupons.Select(it => it.CouponID);
                var couponProducts = await couponService.ProductsView(it => userCouponIds.Contains(it.CouponID) && productIds.Contains(it.ProductID));
                shoppingCart.Coupons = coupons;
                shoppingCart.CouponProducts = couponProducts;
            }
            shoppingCart.CartAmountResult = shoppingCart.CalculateAmount();
            return shoppingCart;
        }
        [HttpGet]
        public async Task<int> UserCount()
        {
            return await shoppingCartService.CartCount(it => it.UserID==CurrentUser.UserID);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_ShoppingCart model)
        {
            var user = CurrentUser;
            model.UserID = user.UserID;
            model.CreateTime = DateTime.Now;
            if (!model.ProductCount.HasValue) model.ProductCount = 1;
            if (string.IsNullOrEmpty(model.SkuID))
            {
                model.SkuID = model.ProductID + "_0";
            }
            if (await shoppingCartService.Exist(it => it.ProductID == model.ProductID && it.SkuID == model.SkuID && it.UserID == user.UserID))
            {
                await shoppingCartService.UpdateIncrement(model.ProductID.Value, model.SkuID, user.UserID);
            }
            else
            {
                await shoppingCartService.Add(model);
            }
            var count = await shoppingCartService.CartCount(it => it.UserID == CurrentUser.UserID);
            return Ok(count);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] int id, [FromForm] int count)
        {
            var has = await shoppingCartService.UpdateCount(id, count);
            if (has)
            {
                var user= CurrentUser;
                var items = await shoppingCartService.List(it => it.UserID==user.UserID && it.IsCheck, new PagedModelBase() { Page = 0 });
                if (items.Count > 0)
                {
                    var shoppingCart = await GetShoppingCart(items, user.UserID);
                    return Ok(shoppingCart.CartAmountResult);
                }
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCheck([FromForm] int id, [FromForm] bool isCheck)
        {
            var has = await shoppingCartService.UpdateCheck(id, isCheck);
            if (has)
            {
                var user = CurrentUser;
                var items = await shoppingCartService.List(it => it.UserID == user.UserID && it.IsCheck, new PagedModelBase() { Page = 0 });
                if (items.Count>0)
                {
                    var shoppingCart = await GetShoppingCart(items, user.UserID);
                    return Ok(shoppingCart.CartAmountResult);
                }
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAllCheck([FromForm] bool isCheck)
        {
            var user = CurrentUser;
            var has = await shoppingCartService.UpdateAllCheck(user.UserID, isCheck);
            if (has && isCheck)
            {
                var items = await shoppingCartService.List(it => it.UserID == user.UserID && it.IsCheck, new PagedModelBase() { Page = 0 });
                if (items.Count > 0)
                {
                    var shoppingCart = await GetShoppingCart(items, user.UserID);
                    return Ok(shoppingCart.CartAmountResult);
                }
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var user = CurrentUser;
            var has = await shoppingCartService.Delete(id, user.UserID);
            if (has)
            {
                var items = await shoppingCartService.List(it => it.UserID == user.UserID && it.IsCheck, new PagedModelBase() { Page = 0 });
                var shoppingCart = await GetShoppingCart(items, user.UserID);
                return Ok(shoppingCart.CartAmountResult);
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> BatchDelete([FromForm] string ids)
        {
            var user = CurrentUser;
            var has = await shoppingCartService.Delete(ids, user.UserID);
            if (has)
            {
                var items = await shoppingCartService.List(it => it.UserID == user.UserID && it.IsCheck, new PagedModelBase() { Page = 0 });
                var shoppingCart = await GetShoppingCart(items, user.UserID);
                return Ok(shoppingCart.CartAmountResult);
            }
            return Ok();
        }
    }
}
