using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Product.API
{
   
    public class CouponItemsController(ICouponItemsService couponItemsService, ICompanyConfigService companyConfigService,ICouponService couponService) : ProductControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_V_CouponItems>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.CouponName.Contains(query.Name));
            }
            if (query.ShopID.HasValue)
            {
                expable.And(it => it.ShopID == query.ShopID);
            }
            if (!string.IsNullOrEmpty(query.IDS))
            {
                var idList= query.IDS.Split(',');
                expable.And(it => idList.Contains(it.ID.ToString()));
            }
            expable.AndIF(query.Status.HasValue, it => it.Status == query.Status);
            expable.AndIF(query.CID.HasValue, it => it.CouponID == query.CID);
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
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    if (user.UserType == (int)CompanyUserType.Shop)
                    {
                        expable.And(it => it.ShopID == user.ShopID);
                    }
                }
                else if (user.PlatformType == Model.Enums.PlatformType.XFZMini)
                {
                    expable.And(it => it.UserID == user.UserID);
                    var now = DateTime.Now;
                    if (query.Expire)
                    {
                        expable.And(it => it.EndTime < now);
                    }
                    else
                    {
                        expable.And(it => it.EndTime >= now);
                    }
                }
            }
            var exp = expable.ToExpression();
            var models = await couponItemsService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        //[PlatformLimit(Model.Enums.PlatformType.Admin)]
        //[PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        //[HttpPost]
        //public async Task<IActionResult> Add([FromBody] Model.Shop_CouponItems model)
        //{
        //    var user = CurrentUser;
        //    model.CreateTime = DateTime.Now;
        //    model.GID=Guid.NewGuid();
        //    model.Status = (int)CouponItemStatus.WaitExchange;
        //    model.CouponNumber = String.Format("{0:D4}", model.CouponID) + Common.Tool.GetRandomPureNumber(8);
        //    await couponItemsService.Add(model);
        //    return Ok();
        //}
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> Batch([FromForm] int couponId, [FromForm] int count)
        {
            var user = CurrentUser;
            var models=new List<Model.Shop_CouponItems>();
            var cmodel = await couponService.Get(couponId);
            if (cmodel == null)
            {
                throw new ErrorCodeException(-1, "该优惠券不存在");
            }
            for (int i = 0; i < count; i++)
            {
                var model = new Model.Shop_CouponItems();
                model.CreateTime = DateTime.Now;
                model.GID = Guid.NewGuid();
                model.CouponID= couponId;
                model.CouponGID = cmodel.CouponGID;
                model.CouponNumber = String.Format("{0:D5}", couponId)+Common.Tool.GetRandomPureNumber(8);
                model.Status = (int)CouponItemStatus.WaitExchange;
                models.Add(model);
            }
            await couponItemsService.Add(models);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Receive([FromForm] Guid gid)
        {
            var user = CurrentUser;
            var now = DateTime.Now;
            var model = await couponService.Get(it => it.CouponGID==gid);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该优惠券不存在");
            }
            if (model.Status != (int)CouponStatus.Launch)
            {
                throw new ErrorCodeException(-1, "该优惠券已下架！");
            }
            if (model.ReceiveStartTime > now)
            {
                throw new ErrorCodeException(-1, "该优惠券还未到领取时间！");
            }
            if (model.ReceiveEndTime < now)
            {
                throw new ErrorCodeException(-1, "该优惠券已过领取时间！");
            }
            if (model.EndTime < now)
            {
                throw new ErrorCodeException(-1, "该优惠券已过期！");
            }
            if (model.UseCouponCount>0 && model.CouponCount<=model.UseCouponCount)
            {
                throw new ErrorCodeException(-1, "该优惠券已下架！");
            }
            if (model.UserLimitCount > 0 && model.UserLimitCount <= await couponItemsService.Count(it => it.CouponID == model.CouponID && it.UserID == user.UserID))
            {
                throw new ErrorCodeException(-1, "你已达获取优惠券的上限了！");
            }
            var m = new Model.Shop_CouponItems();
            m.UserID = user.UserID;
            m.Status = (int)CouponItemStatus.WaitUse;
            m.FullName = user.FullName;
            m.CouponID = model.CouponID;
            m.CouponGID = model.CouponGID;
            m.CreateTime = now;
            m.ExchangeTime = now;
            m.GID=Guid.NewGuid();
            m.CouponNumber = String.Format("{0:D5}", model.CouponID) + Common.Tool.GetRandomPureNumber(8);
            await couponItemsService.Add(m);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Receives([FromForm] string gids)
        {
            if (string.IsNullOrEmpty(gids))
            {
                throw new ErrorCodeException(-1, "参数错误！");
            }
            var user = CurrentUser;
            var now = DateTime.Now;
            var gidList = gids.Split(',');
            var guidList = new List<Guid>();
            foreach (var id in gidList)
            {
                guidList.Add(new Guid(id));
            }
            var models = await couponService.List(it => guidList.Contains(it.CouponGID.Value), new PagedModelBase() { Page = 0 });
            var items = new List<Model.Shop_CouponItems>();
            foreach (var model in models)
            {
                if (model.Status != (int)CouponStatus.Launch)
                {
                    throw new ErrorCodeException(-1, $"{model.CouponName}优惠券已下架！");
                }
                if (model.ReceiveStartTime > now)
                {
                    throw new ErrorCodeException(-1, $"{model.CouponName}优惠券还未到领取时间！");
                }
                if (model.ReceiveEndTime < now)
                {
                    throw new ErrorCodeException(-1, $"{model.CouponName}优惠券已过领取时间！");
                }
                if (model.EndTime < now)
                {
                    throw new ErrorCodeException(-1, $"{model.CouponName}优惠券已过期！");
                }
                if (model.UseCouponCount > 0 && model.CouponCount <= model.UseCouponCount)
                {
                    throw new ErrorCodeException(-1, $"{model.CouponName}优惠券已领完！");
                }
                if (model.UserLimitCount > 0 && model.UserLimitCount <= await couponItemsService.Count(it => it.CouponID == model.CouponID && it.UserID == user.UserID))
                {
                    throw new ErrorCodeException(-1, $"你已达获取{model.CouponName}优惠券的上限了！");
                }
                var m = new Model.Shop_CouponItems();
                m.UserID = user.UserID;
                m.Status = (int)CouponItemStatus.WaitUse;
                m.FullName = user.FullName;
                m.CouponID = model.CouponID;
                m.CouponGID = model.CouponGID;
                m.CreateTime = now;
                m.ExchangeTime = now;
                m.GID = Guid.NewGuid();
                m.CouponNumber = String.Format("{0:D5}", model.CouponID) + Common.Tool.GetRandomPureNumber(8);
                items.Add(m);
            }
            if (items.Count > 0)
            {
                await couponItemsService.Add(items);
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Binding([FromForm] string number)
        {
            var user = CurrentUser;
            var model = await couponItemsService.GetView(it => it.CouponNumber == number && it.CompanyID == user.CompanyID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该优惠券不存在");
            }
            if (model.EndTime < DateTime.Now)
            {
                throw new ErrorCodeException(-1, "该优惠券已过期！");
            }
            if (model.CouponStatus!=(int)CouponStatus.Launch)
            {
                throw new ErrorCodeException(-1, "该优惠券已下架！");
            }
            if (model.Status != (int)CouponItemStatus.WaitExchange)
            {
                throw new ErrorCodeException(-1, "该优惠券已被使用！");
            }
            if (model.UserLimitCount > 0 && model.UserLimitCount<=await couponItemsService.Count(it=>it.CouponID==model.CouponID && it.UserID==user.UserID))
            {
                throw new ErrorCodeException(-1, "你已达获取优惠券的上限了！");
            }
            var m = new Model.Shop_CouponItems();
            m.UserID = user.UserID;
            m.Status= (int)CouponItemStatus.WaitUse;
            m.ExchangeTime= DateTime.Now;
            m.FullName = user.FullName;
            m.ID = model.ID;
            await couponItemsService.UpdateAsync(m);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Write([FromForm] Guid gid)
        {
            var user = CurrentUser;
            if (user.UserType != (int)UserTypeEnum.WriteUser)
            {
                throw new ErrorCodeException(-1, "您无权核销！");
            }
            var model = await couponItemsService.GetView(it => it.GID == gid && it.CompanyID == user.CompanyID && it.ShopID==user.ShopID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该优惠券不存在或你无权核销");
            }
            if (model.EndTime < DateTime.Now)
            {
                throw new ErrorCodeException(-1, "该优惠券已过期！");
            }
            if (model.CouponStatus != (int)CouponStatus.Launch)
            {
                throw new ErrorCodeException(-1, "该优惠券已下架！");
            }
            if (model.Status != (int)CouponItemStatus.WaitUse)
            {
                throw new ErrorCodeException(-1, "该优惠券已被使用！");
            }
            var m = new Model.Shop_CouponItems();
            m.Status = (int)CouponItemStatus.HaveUse;
            m.WriteTime = DateTime.Now;
            m.WriteUserID = user.UserID;
            m.WriteFullName = user.FullName;
            m.ID=model.ID;
            await couponItemsService.UpdateAsync(m);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_CouponItems model)
        {
            await couponItemsService.UpdateAsync(model);
            return Ok();
        }
        //[PlatformLimit(Model.Enums.PlatformType.Admin)]
        //[PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        //[HttpPut]
        //public async Task<IActionResult> Update([FromBody] Model.Shop_CouponItems model)
        //{
        //    await couponItemsService.UpdateAsync(model);
        //    return Ok();
        //}
        [HttpGet]
        public async Task<Model.Shop_V_CouponItems> GetInfo(Guid gid)
        {
            var user = CurrentUser;
            if (user.PlatformType==PlatformType.XFZMini && user.UserType != (int)UserTypeEnum.WriteUser)
            {
                throw new ErrorCodeException(-1, "您无权核销！");
            }
            var model = await couponItemsService.GetView(it => it.GID == gid && it.CompanyID == user.CompanyID && it.ShopID == user.ShopID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该优惠券不存在或你无权核销");
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Shop_CouponItems> Get(Guid gid)
        {
            var model = await couponItemsService.Get(it=>it.GID==gid);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<int> GetUserCount(Guid id,int userId)
        {
            return await couponItemsService.Count(it => it.CouponGID == id && it.UserID==userId);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await couponItemsService.Delete(id);
            return Ok();
        }
    }
}
