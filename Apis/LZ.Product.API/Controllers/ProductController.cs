using Aliyun.OSS;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs.MerChant;
using SqlSugar;
using System.Linq.Expressions;
using System.Text;

namespace LZ.Product.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    public class ProductController(IProductService productService,ISKUsService sKUsService,ISKUItemsService sKUItemsService,ICompanyConfigService companyConfigService,IAttributeService attributeService,IAttributeValuesService attributeValuesService, IPromotionService promotionService, ICouponItemsService couponItemsService, ICouponService couponService, IProductRecommendService productRecommendService, IProductCfRecommendService productCfRecommendService) : ProductControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Product>();
            await SetSearch(expable, query, user);
            var exp = expable.ToExpression();
            Expression<Func<Shop_Product, object>> order =null;
            if (query.From == "comm")
            {
                order = it => new { SaleCount = SqlFunc.Desc(it.SaleCount), ProductID = SqlFunc.Desc(it.ProductID) };
            }
            var models = await productService.List(exp, query, order);
            await SetListInfo(models);
            return Ok(new { items = models, count = query.Count });
        }
        private async Task SetSearch(Expressionable<Shop_Product> expable, ProductPagedModel query, ICurrentUser user)
        {
           
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.ProductName.Contains(query.Name));
            }
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
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
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    if (user.UserType == (int)CompanyUserType.Shop)
                    {
                        expable.And(it => it.ShopID == user.ShopID);
                    }
                }
            }
            if (!string.IsNullOrEmpty(query.Tab))
            {
                var tabs = query.Tab.Split(',');
                expable.And(it => tabs.Any(s => it.Tabs.Contains(s)));
                // expable.And(it => it.Tabs.Contains(query.Tab));
            }
            if (!string.IsNullOrEmpty(query.ProductIds))
            {
                var ids = query.ProductIds.Split(',');
                expable.And(it => ids.Contains(it.ProductID.ToString()));
            }
            expable.AndIF(query.Status.HasValue, it => it.Status == query.Status);
            expable.AndIF(query.ProductType.HasValue, it => it.ProductType == query.ProductType);
            expable.AndIF(query.ShopID.HasValue, it => it.ShopID == query.ShopID);
            expable.AndIF(query.CID.HasValue, it => it.CateID == query.CID || SqlFunc.Subqueryable<Model.Shop_Categories>().Where(s => s.CateID == it.CateID && s.ParentID == query.CID).Any() || $",{it.CateIDS},".Contains($",{query.CID},"));
        }
        private async Task SetListInfo(List<Model.Shop_Product> models)
        {
            var productIds = models.Select(it => it.ProductID);
            var now = DateTime.Now;
            var promotions = await promotionService.Items(it => productIds.Contains(it.ProductID.Value) && it.StartTime <= now && it.EndTime >= now && it.Enable);
            foreach (var promotion in promotions)
            {
                if (promotion.PromotionType == (int)PromotionType.Discount)
                {
                    promotion.PromotionMoney = promotion.SalePrice * promotion.PromotionDiscount.Value;
                }
            }
            var coupons = await couponService.ProductsView(it => productIds.Contains(it.ProductID.Value) && it.CouponStatus == (int)CouponStatus.Launch && it.StartTime <= now && it.EndTime >= now);
            foreach (var model in models)
            {
                //是否存在促销商品
                var promotionList = promotions.Where(it => it.ProductID == model.ProductID);
                if (promotionList != null && promotionList.Count() > 0)
                {
                    var mini = promotionList.OrderBy(it => it.PromotionMoney).ToList()[0];
                    if (mini.PromotionMoney > 0)
                    {
                        model.PromotionPrice = mini.PromotionMoney;
                        model.PromotionSkuID = mini.SkuID;
                        model.DataTabs = mini.PromotionName;
                    }
                }
                //是否存在优惠券的商品
                if (coupons.Exists(it => it.ProductID == model.ProductID))
                {
                    model.DataTabs = (!string.IsNullOrEmpty(model.DataTabs) ? "," : "") + "券";
                }
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Recommend1([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;

            var expable = Expressionable.Create<Model.Shop_Product>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.ProductName.Contains(query.Name));
            }

            var companyId = user.CompanyID;
            var companygid = HttpContext.Request.Headers["company-guid"].ToString();
            if (!string.IsNullOrEmpty(companygid))
            {
                var companyinfo = await companyConfigService.Get(new Guid(companygid));
                companyId = companyinfo.Company.CompanyID;
            }
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            if (!string.IsNullOrEmpty(query.Tab))
            {
                var tabs = query.Tab.Split(',');
                expable.And(it => tabs.Any(s => it.Tabs.Contains(s)));
            }
            if (!string.IsNullOrEmpty(query.ProductIds))
            {
                var ids = query.ProductIds.Split(',');
                expable.And(it => ids.Contains(it.ProductID.ToString()));
            }
            expable.AndIF(query.Status.HasValue, it => it.Status == query.Status);
            expable.AndIF(query.ProductType.HasValue, it => it.ProductType == query.ProductType);
            expable.AndIF(query.ShopID.HasValue, it => it.ShopID == query.ShopID);

            var models = await productRecommendService.List(expable.ToExpression(), query, user.UserID, companyId);
            var productIds = models.Select(it => it.ProductID);
            var now = DateTime.Now;
            var promotions = await promotionService.Items(it => productIds.Contains(it.ProductID.Value) && it.StartTime <= now && it.EndTime >= now && it.Enable);
            foreach (var promotion in promotions)
            {
                if (promotion.PromotionType == (int)PromotionType.Discount)
                {
                    promotion.PromotionMoney = promotion.SalePrice * promotion.PromotionDiscount.Value;
                }
            }
            var coupons = await couponService.ProductsView(it => productIds.Contains(it.ProductID.Value) && it.CouponStatus == (int)CouponStatus.Launch && it.StartTime <= now && it.EndTime >= now);
            foreach (var model in models)
            {
                var promotionList = promotions.Where(it => it.ProductID == model.ProductID);
                if (promotionList != null && promotionList.Count() > 0)
                {
                    var mini = promotionList.OrderBy(it => it.PromotionMoney).ToList()[0];
                    if (mini.PromotionMoney > 0)
                    {
                        model.PromotionPrice = mini.PromotionMoney;
                        model.PromotionSkuID = mini.SkuID;
                        model.DataTabs = "促";
                    }
                }
                if (coupons.Exists(it => it.ProductID == model.ProductID))
                {
                    model.DataTabs = (!string.IsNullOrEmpty(model.DataTabs) ? "," : "") + "卷";
                }
            }
            return Ok(new { items = models, count = query.Count });
        }

        /// <summary>
        /// 推荐接口 V2：基于用户加购 + 访问记录的 Item-CF 协同过滤。
        /// 以用户近 30 天的加购(权重5.0)/浏览(权重1.0,7天半衰期)商品为兴趣种子，
        /// 通过相同行为用户群体的物品共现相似度召回，混合分类内容相似与热度排序；
        /// 无行为用户降级为销量热度榜。传 ProductID 时按该商品所在分类做相关推荐。
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Recommend([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Product>();
            await SetSearch(expable, query, user);
            var companyId = user.CompanyID;
            var companygid = HttpContext.Request.Headers["company-guid"].ToString();
            if (!string.IsNullOrEmpty(companygid))
            {
                var companyinfo = await companyConfigService.Get(new Guid(companygid));
                companyId = companyinfo.Company.CompanyID;
            }
            var models = await productCfRecommendService.List(expable.ToExpression(), query, user.UserID, companyId);
            await SetListInfo(models);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo(int id)
        {
            var model = await productService.Get(id);
            var now = DateTime.Now;
            if (model.HasSKU)
            {
                var skus = await sKUsService.List(it => it.ProductID == id);
                var skuIds = skus.Select(it => it.SkuID);
                var promotions = await promotionService.Items(it => skuIds.Contains(it.SkuID) && it.StartTime <= now && it.EndTime >= now && it.Enable);//获得促销价
                foreach (var sku in skus)
                {
                    var pmodel = promotions.Find(it => it.SkuID == sku.SkuID);
                    if (pmodel != null)
                    {
                        sku.PromotionPrice = pmodel.PromotionType == (int)PromotionType.Discount && pmodel.PromotionDiscount.HasValue ? sku.SalePrice * pmodel.PromotionDiscount.Value : promotions[0].PromotionMoney;
                    }
                }
                var attrs = await attributeService.List(it => it.ProductID == id && !it.IsDelete);
                var values = await attributeValuesService.List(it => SqlFunc.Subqueryable<Model.Shop_Attribute>().Where(s => s.ProductID == model.ProductID && s.AttributeID == it.AttributeID).Any() && !it.IsDelete);
                var productAttrs = new List<Model.ProductAttributeModel>();
                foreach (var attr in attrs)
                {
                    var productAttr = new Model.ProductAttributeModel();
                    productAttr.AttributeID = attr.AttributeID;
                    productAttr.AttributeName = attr.AttributeName;
                    productAttr.TypeID = attr.TypeID;
                    productAttr.Type = attr.Type;
                    productAttr.IsImage = attr.IsImage;
                    productAttr.ProductID = attr.ProductID;
                    productAttr.Sort = attr.Sort;
                    productAttr.values = values.Where(it => it.AttributeID == attr.AttributeID).ToList();
                    productAttrs.Add(productAttr);
                }
                return Ok(new { product = model, skus, attrs = productAttrs });
            }
            else
            {
                var promotions = await promotionService.Items(it => it.ProductID == id && it.StartTime <= now && it.EndTime >= now && it.Enable);
                if (promotions != null && promotions.Count > 0)
                {
                    var pmodel = promotions[0];
                    model.PromotionPrice = pmodel.PromotionType == (int)PromotionType.Discount && pmodel.PromotionDiscount.HasValue ? model.ProductPrice * pmodel.PromotionDiscount.Value : promotions[0].PromotionMoney;
                }
                return Ok(new { product = model });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Gets(int id)
        {
            var model = await productService.Get(id);
            var now = DateTime.Now;
            var userCoupons = new List<Model.Shop_V_CouponItems>();
            var user = CurrentUser;
            if (user.UserID > 0)
            {
                userCoupons = await couponItemsService.List(it => it.UserID == user.UserID && it.StartTime <= now && it.EndTime >= now && it.Status == (int)CouponItemStatus.WaitUse && it.CouponStatus == (int)CouponStatus.Launch && (SqlFunc.Subqueryable<Model.Shop_CouponProduct>().Where(s => s.CouponID == it.CouponID && s.ProductID == id).Any() || it.CouponUseType == (int)CouponUseType.ProductCurrency), new PagedModelBase() { Page = 0 });
            }
            var coupons = await couponService.ProductsView(it => (it.ProductID == id || it.CouponUseType==(int)CouponUseType.ProductCurrency) && it.CouponStatus == (int)CouponStatus.Launch && it.StartTime <= now && it.EndTime >= now && it.IsOpen);
            if (model.HasSKU)
            {
                var skus = await sKUsService.List(it => it.ProductID == id);
                var skuIds = skus.Select(it => it.SkuID);
                var promotions = await promotionService.Items(it => skuIds.Contains(it.SkuID) && it.StartTime <= now && it.EndTime >= now && it.Enable);//获得促销价
                foreach(var sku in skus)
                {
                    var pmodel = promotions.Find(it => it.SkuID == sku.SkuID);
                    if (pmodel != null)
                    {
                        sku.PromotionPrice = pmodel.PromotionType == (int)PromotionType.Discount && pmodel.PromotionDiscount.HasValue ? sku.SalePrice * pmodel.PromotionDiscount.Value : pmodel.PromotionMoney;
                        sku.DataTabs = pmodel.PromotionName;
                    }
                }
                var attrs = await attributeService.List(it=>it.ProductID==id && !it.IsDelete);
                var values = await attributeValuesService.List(it => SqlFunc.Subqueryable<Model.Shop_Attribute>().Where(s => s.ProductID == model.ProductID && s.AttributeID == it.AttributeID).Any() && !it.IsDelete);
                var productAttrs = new List<Model.ProductAttributeModel>();
                foreach (var attr in attrs)
                {
                    var productAttr=new Model.ProductAttributeModel();
                    productAttr.AttributeID = attr.AttributeID;
                    productAttr.AttributeName = attr.AttributeName;
                    productAttr.TypeID = attr.TypeID;
                    productAttr.Type = attr.Type;
                    productAttr.IsImage = attr.IsImage;
                    productAttr.ProductID = attr.ProductID;
                    productAttr.Sort = attr.Sort;
                    productAttr.values = values.Where(it => it.AttributeID == attr.AttributeID).ToList();
                    productAttrs.Add(productAttr);
                }
                return Ok(new { product = model, skus, attrs=productAttrs, coupons, userCoupons });
            }
            else
            {
                var promotions = await promotionService.Items(it => it.ProductID == id && it.StartTime <= now && it.EndTime >= now && it.Enable);
                if(promotions!=null && promotions.Count > 0)
                {
                    var pmodel = promotions[0];
                    model.PromotionPrice = pmodel.PromotionType == (int)PromotionType.Discount && pmodel.PromotionDiscount.HasValue ? model.ProductPrice * pmodel.PromotionDiscount.Value : pmodel.PromotionMoney;
                    model.DataTabs = pmodel.PromotionName;
                }
                return Ok(new { product = model, coupons, userCoupons });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSku(string skuId,int userId=0, int count = 1,string couponIds="")
        {
            if (string.IsNullOrEmpty(skuId))
            {
                throw new ErrorCodeException(-1, "该suk不正确!");
            }
            if (skuId.IndexOf("_") == -1)//判断是否单规格产品
            {
                skuId = $"{skuId}_0";
            }
            var sku = await sKUsService.Get(skuId);
            var items = await sKUItemsService.List(it => it.SkuID == skuId);
            if (sku == null)
            {
                throw new ErrorCodeException(-1, "该suk不存在!");
            }
            var user = CurrentUser;
            if (user.PlatformType != Model.Enums.PlatformType.Api)
            {
                userId = user.UserID;
            }
            var shoppingCart = new Model.ShoppingCart();
            var cart = new Model.Shop_V_ShoppingCart();
            cart.ProductID = sku.ProductID;
            cart.SkuID = skuId;
            cart.CartID = 0;
            cart.SKU = sku.SKU;
            var skuContent = new StringBuilder();
            foreach (var item in items)
            {
                skuContent.Append(item.AttributeName + ":" + item.ValueName + ";");
            }
            cart.SalePrice = sku.SalePrice;
            cart.CostPrice= sku.CostPrice;
            cart.SkuContent = skuContent.ToString() ;
            cart.Weight = sku.Weight;
            cart.Stock = sku.Stock;
            var product = await productService.GetView(sku.ProductID.Value);
            if (product == null)
            {
                throw new ErrorCodeException(-1, "该产品不存在!");
            }
            cart.ProductName = product.ProductName;
            cart.ShopID = product.ShopID;
            cart.HasSKU = product.HasSKU;
            cart.IsFree = product.IsFree;
            cart.FTID = product.FTID;
            cart.ProductType= product.ProductType;
            cart.IsIntegral = product.IsIntegral;
            cart.ProductCount = count;
            shoppingCart.Items = new List<Shop_V_ShoppingCart>();
            shoppingCart.Items.Add(cart);
            var now = DateTime.Now;
            var promotions = await promotionService.Items(it => it.SkuID==cart.SkuID && it.StartTime <= now && it.EndTime >= now && it.Enable);
            shoppingCart.Promotions = promotions;
            if(promotions!=null && promotions.Count > 0)
            {
                var pmodel = promotions[0];
                cart.PromotionPrice = pmodel.PromotionType == (int)PromotionType.Discount && pmodel.PromotionDiscount.HasValue ? sku.SalePrice * pmodel.PromotionDiscount.Value : pmodel.PromotionMoney;
            }
            if (string.IsNullOrEmpty(couponIds))
            {
                var coupons = await couponItemsService.List(it => it.UserID == userId && it.StartTime <= now && it.EndTime >= now && it.Status == (int)CouponItemStatus.WaitUse && it.CouponStatus == (int)CouponStatus.Launch && (SqlFunc.Subqueryable<Model.Shop_CouponProduct>().Where(s => s.CouponID == it.CouponID && s.ProductID == cart.ProductID).Any() || it.CouponUseType == (int)CouponUseType.ProductCurrency), new PagedModelBase() { Page = 0 });
                var userCouponIds = coupons.Select(it => it.CouponID);
                var couponProducts = await couponService.ProductsView(it => userCouponIds.Contains(it.CouponID) && it.ProductID == cart.ProductID);
                shoppingCart.Coupons = coupons;
                shoppingCart.CouponProducts = couponProducts;
                shoppingCart.CartAmountResult = shoppingCart.CalculateAmount();
                shoppingCart.UseCoupons = shoppingCart.GetUsableCoupons();//可使用的优惠券
                return Ok(shoppingCart);
            }
            else
            {
                var couponIdsList = couponIds.Split(',');
                var coupons = await couponItemsService.List(it => it.UserID == userId && it.StartTime <= now && it.EndTime >= now && it.Status == (int)CouponItemStatus.WaitUse && it.CouponStatus == (int)CouponStatus.Launch && (SqlFunc.Subqueryable<Model.Shop_CouponProduct>().Where(s => s.CouponID == it.CouponID && s.ProductID == cart.ProductID).Any() || it.CouponUseType == (int)CouponUseType.ProductCurrency) && couponIdsList.Contains(it.ID.ToString()), new PagedModelBase() { Page = 0 });
                if (couponIdsList.Count() == 0)
                {
                    throw new ErrorCodeException(-1, "您选择的优惠券不可用!");
                }
                var userCouponIds = coupons.Select(it => it.CouponID);
                var couponProducts = await couponService.ProductsView(it => userCouponIds.Contains(it.CouponID) && it.ProductID == cart.ProductID);
                shoppingCart.Coupons = coupons;
                shoppingCart.CouponProducts = couponProducts;
                shoppingCart.CartAmountResult = shoppingCart.CalculateAmount();
                return Ok(shoppingCart.CartAmountResult);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderSku(string skuId, int userId = 0, int count = 1)
        {
            if (string.IsNullOrEmpty(skuId))
            {
                throw new ErrorCodeException(-1, "该suk不正确!");
            }
            if (skuId.IndexOf("_") == -1)//判断是否单规格产品
            {
                skuId = $"{skuId}_0";
            }
            var sku = await sKUsService.Get(skuId);
            var items = await sKUItemsService.List(it => it.SkuID == skuId);
            if (sku == null)
            {
                throw new ErrorCodeException(-1, "该suk不存在!");
            }
            var user = CurrentUser;
            if (user.PlatformType != Model.Enums.PlatformType.Api)
            {
                userId = user.UserID;
            }
            var shoppingCart = new Model.ShoppingCart();
            var cart = new Model.Shop_V_ShoppingCart();
            cart.ProductID = sku.ProductID;
            cart.SkuID = skuId;
            cart.CartID = 0;
            cart.SKU = sku.SKU;
            var skuContent = new StringBuilder();
            foreach (var item in items)
            {
                skuContent.Append(item.AttributeName + ":" + item.ValueName + ";");
            }
            cart.SalePrice = sku.SalePrice;
            cart.CostPrice = sku.CostPrice;
            cart.SkuContent = skuContent.ToString();
            cart.Weight = sku.Weight;
            cart.Stock = sku.Stock;
            var product = await productService.GetView(sku.ProductID.Value);
            if (product == null)
            {
                throw new ErrorCodeException(-1, "该产品不存在!");
            }
            cart.ProductName = product.ProductName;
            cart.ShopID = product.ShopID;
            cart.HasSKU = product.HasSKU;
            cart.IsFree = product.IsFree;
            cart.FTID = product.FTID;
            cart.ProductType = product.ProductType;
            cart.IsIntegral = product.IsIntegral;
            cart.ProductCount = count;
            cart.Commission = product.Commission;
            cart.IsCommission= product.IsCommission;
            shoppingCart.Items = new List<Shop_V_ShoppingCart>();
            shoppingCart.Items.Add(cart);
            var now = DateTime.Now;
            var promotions = await promotionService.Items(it => it.SkuID == cart.SkuID && it.StartTime <= now && it.EndTime >= now && it.Enable);
            shoppingCart.Promotions = promotions;
            if (promotions != null && promotions.Count > 0)
            {
                var pmodel = promotions[0];
                cart.PromotionPrice = pmodel.PromotionType == (int)PromotionType.Discount && pmodel.PromotionDiscount.HasValue ? sku.SalePrice * pmodel.PromotionDiscount.Value : pmodel.PromotionMoney;
            }
            return Ok(shoppingCart);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.ProductModel data)
        {
            try
            {
                var user = CurrentUser;
                if (data == null || data.model == null)
                {
                    throw new ErrorCodeException(-1, "产品错误!");
                }
                if (data.model.HasSKU && (data.skus == null || data.skus.Count == 0 || data.attrs == null || data.attrs.Count == 0))
                {
                    throw new ErrorCodeException(-1, "规格错误!");
                }
                if (!string.IsNullOrEmpty(data.model.Barcode))
                {
                    if (await productService.Exist(it => it.Barcode == data.model.Barcode && it.CompanyID == user.CompanyID))
                    {
                        throw new ErrorCodeException(-1, "该商品条码已存在!");
                    }
                }
                if (string.IsNullOrEmpty(data.model.ProductCode) && (data.model.ProductType==(int)ProductType.GroupProduct || data.model.ProductType == (int)ProductType.GroupCoupon))
                {
                    data.model.ProductCode = Common.Tool.GetRandomPureNumber(8);
                }
                if (!string.IsNullOrEmpty(data.model.ProductCode))
                {
                    if (await productService.Exist(it => it.ProductCode == data.model.ProductCode && it.CompanyID == user.CompanyID))
                    {
                        throw new ErrorCodeException(-1, "该商品编号已存在!");
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, "请输入商品编号");
                }
                if (!data.model.Status.HasValue)
                {
                    data.model.Status = 1;
                }
                data.model.CompanyID = user.CompanyID;
                data.model.CompanyGID = user.CompanyGID;
                data.model.CreateTime = DateTime.Now;
                data.model.ShopID = user.ShopID;
                if (!data.model.SaleCount.HasValue)
                {
                    data.model.SaleCount = 0;
                }
                var attrs = new List<Model.Shop_Attribute>();
                var values = new List<Model.Shop_AttributeValues>();
                if (data.model.HasSKU)
                {
                    data.model.ProductPrice = data.skus.OrderBy(it => it.SalePrice).ToList()[0].SalePrice;
                    data.model.ProductCount = data.skus.Sum(it => it.Stock);
                    foreach (var sku in data.skus)
                    {
                        if (string.IsNullOrEmpty(sku.SKU))
                        {
                            throw new ErrorCodeException(-1, "有sku码未输入");
                        }
                    }
                    var codes = data.skus.Select(it => it.SKU);
                    if (await sKUsService.Exist(it => codes.Contains(it.SKU)))
                    {
                        throw new ErrorCodeException(-1, "有sku码已存在!");
                    }
                    var barcodeCodeList = data.skus.Where(it => !string.IsNullOrEmpty(it.Barcode)).Select(it => it.Barcode);
                    if (await sKUsService.Exist(it => barcodeCodeList.Contains(it.Barcode)))
                    {
                        throw new ErrorCodeException(-1, "有sku条码已存在!");
                    }
                    var i = 0;
                    foreach (var item in data.attrs)
                    {
                        i++;
                        var attr = new Model.Shop_Attribute();
                        attr.AttributeStrID = item.AttributeStrID;
                        attr.AttributeName = item.AttributeName;
                        attr.TypeID = item.TypeID;
                        attr.Type = item.Type;
                        attr.IsImage = item.IsImage;
                        attr.ProductID = item.ProductID;
                        attr.Sort = i;
                        foreach (var value in item.values)
                        {
                            value.AttributeStrID = item.AttributeStrID;
                        }
                        values.AddRange(item.values);
                        attrs.Add(attr);
                    }
                }
                else
                {
                    var sku = new Model.Shop_SKUs();
                    sku.SkuID = "0";
                    sku.Stock = data.model.ProductCount;
                    sku.CostPrice = data.model.CostPrice;
                    sku.SalePrice = data.model.ProductPrice;
                    sku.SKU = data.model.ProductCode;
                    sku.Barcode = data.model.Barcode;
                    data.skus = new List<Model.Shop_SKUs>();
                    data.skus.Add(sku);
                }
                data.model.ProductID = await productService.Add(data.model, data.skus, attrs, values);
                if (data.model.ProductID > 0)
                {
                    return Ok();
                }
                else
                {
                    throw new ErrorCodeException(-1, "新增商品错误!");
                }
            }
            catch(Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
            
           
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.ProductModel data)
        {
            var user = CurrentUser;
            if (data == null || data.model == null)
            {
                throw new ErrorCodeException(-1, "产品错误!");
            }
            if (data.model.HasSKU && data.skus.Count == 0)
            {
                throw new ErrorCodeException(-1, "规格错误!");
            }
            if (!string.IsNullOrEmpty(data.model.Barcode))
            {
                if (await productService.Exist(it => it.Barcode == data.model.Barcode && it.CompanyID == user.CompanyID && it.ProductID != data.model.ProductID))
                {
                    throw new ErrorCodeException(-1, "该商品条码已存在!");
                }
            }
            if (string.IsNullOrEmpty(data.model.ProductCode) && (data.model.ProductType == (int)ProductType.GroupProduct || data.model.ProductType == (int)ProductType.GroupCoupon))
            {
                data.model.ProductCode = Common.Tool.GetRandomPureNumber(8);
            }
            if (!string.IsNullOrEmpty(data.model.ProductCode))
            {
                if (await productService.Exist(it => it.ProductCode == data.model.ProductCode && it.ProductID != data.model.ProductID && it.CompanyID == user.CompanyID))
                {
                    throw new ErrorCodeException(-1, "该商品编号已存在!");
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "请输入商品编号");
            }
            var attrs = new List<Model.Shop_Attribute>();
            var values = new List<Model.Shop_AttributeValues>();
            if (data.model.HasSKU)
            {
                data.model.ProductPrice = data.skus.OrderBy(it => it.SalePrice).ToList()[0].SalePrice;
                data.model.ProductCount = data.skus.Sum(it => it.Stock);
                foreach (var sku in data.skus)
                {
                    if (string.IsNullOrEmpty(sku.SKU))
                    {
                        throw new ErrorCodeException(-1, "有sku码未输入");
                    }
                }
                var codes = data.skus.Select(it => it.SKU);
                if (await sKUsService.Exist(it => codes.Contains(it.SKU) && it.ProductID!=data.model.ProductID))
                {
                    throw new ErrorCodeException(-1, "有sku码已存在!");
                }
                var barcodeCodeList = data.skus.Where(it => !string.IsNullOrEmpty(it.Barcode)).Select(it => it.Barcode);
                if (await sKUsService.Exist(it => barcodeCodeList.Contains(it.Barcode) && it.ProductID != data.model.ProductID))
                {
                    throw new ErrorCodeException(-1, "有sku条码已存在!");
                }
                var i = 0;
                foreach (var item in data.attrs)
                {
                    i++;
                    var attr = new Model.Shop_Attribute();
                    attr.AttributeName = item.AttributeName;
                    attr.AttributeStrID = item.AttributeStrID;
                    attr.TypeID = item.TypeID;
                    attr.Type = item.Type;
                    attr.IsImage = item.IsImage;
                    attr.ProductID = data.model.ProductID;
                    attr.AttributeID= item.AttributeID;
                    attr.Sort = i;
                    foreach (var value in item.values)
                    {
                        value.AttributeStrID = item.AttributeStrID;
                        value.AttributeID= item.AttributeID;
                    }
                    values.AddRange(item.values);
                    attrs.Add(attr);
                }
            }
            else
            {
                var sku = new Model.Shop_SKUs();
                sku.SkuID = "0";
                sku.Stock = data.model.ProductCount;
                sku.CostPrice = data.model.CostPrice;
                sku.SalePrice = data.model.ProductPrice;
                sku.SKU = data.model.ProductCode;
                sku.Barcode = data.model.Barcode;
                data.skus = new List<Model.Shop_SKUs>();
                data.skus.Add(sku);
            }
            if(await productService.UpdateAsync(data.model,data.skus,attrs, values))
            {
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "更新失败");
            }
               
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Shop_Product> Get(int id)
        {
            var model = await productService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeInventory([FromForm] string data, [FromForm] string app_key)
        {
            try
            {
                if(string.IsNullOrEmpty(data) || string.IsNullOrEmpty(app_key))
                {
                    throw new ErrorCodeException(-1, "库存同步错误，信息不全！");
                }
                if(app_key!= "3068922137")
                {
                    throw new ErrorCodeException(-1, "key错误！");
                }
                var model = JsonConvert.DeserializeObject<Model.ApiModel.HupunInventoryDataModel>(data);
                if (model == null || string.IsNullOrEmpty(model.item_id) || string.IsNullOrEmpty(model.shop_nick))
                {
                    throw new ErrorCodeException(-1, "库存同步错误，信息不全！");
                }
                var product = await productService.Get(Convert.ToInt32(model.item_id));
                if (product == null)
                {
                    throw new ErrorCodeException(-1, "产品不存在！");
                }
                var sku_id = model.sku_id;
                if (product.HasSKU)
                {
                    await sKUsService.ChangeInventory(product.ProductID, sku_id, model.quantity);
                    await productService.ChangeInventory(product.ProductID);
                }
                else
                {
                    await productService.ChangeInventory(product.ProductID, model.quantity);
                    await sKUsService.ChangeInventory(product.ProductID, $"{product.ProductID}_0", model.quantity);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "同步错误！");
            }
            
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<bool> BatchStatus([FromForm] string productIds, [FromForm] ProductStatus status)
        {
            var idList= productIds.Split(',');
            var pids=new List<int>();
            foreach (var id in idList)
            {
                pids.Add(Convert.ToInt32(id));
            }
            return await productService.ChangeStatus(pids, status);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<bool> SetStatus([FromForm] int id, [FromForm] int status)
        {
            return await productService.SetStatus(id, status);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> SyncErp([FromForm] string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                throw new ErrorCodeException(-1, "信息错误！");
            }
            var idList = ids.Split(',');
            var productIds = new List<int>();
            foreach ( var id in idList)
            {
                productIds.Add(Convert.ToInt32(id));
            }
            var res = await productService.SendProducts(it=> productIds.Contains(it.ProductID));
            if(res!=null && res.code==0 && res.data.success)
            {
                return Ok();
            }
            else
            {
                if (res.data != null)
                {
                    throw new ErrorCodeException(-1, res.data.error_msg);
                }
                else
                {
                    throw new ErrorCodeException(-1, "同步错误");
                }
            }
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await productService.Delete(id);
            return Ok();
        }
    }
}
