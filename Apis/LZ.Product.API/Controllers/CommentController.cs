using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Product.API
{
    public class CommentController(ICommentService commentService, ICompanyConfigService companyConfigService, IAuthService authService,IHttpService httpService) : ProductControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Comment>();
            if (!string.IsNullOrEmpty(query.Name) || !string.IsNullOrEmpty(query.Keyword))
            {
                var keyword = !string.IsNullOrEmpty(query.Keyword) ? query.Keyword : query.Name;
                expable.And(it => it.Comment.Contains(keyword) || it.FullName.Contains(keyword));
            }
            expable.AndIF(query.ProductID.HasValue, it => it.ProductID == query.ProductID);
            expable.AndIF(query.Score.HasValue, it => it.Score == query.Score);
            expable.AndIF(!string.IsNullOrEmpty(query.OrderID), it => it.OrderID == query.OrderID);
            expable.AndIF(query.HasPicture, it => !string.IsNullOrEmpty(it.Pictures));
            if (!string.IsNullOrEmpty(query.ProductIds))
            {
                var ids = query.ProductIds.Split(',');
                expable.And(it => ids.Contains(it.ProductID.ToString()));
            }
            if (user.PlatformType == PlatformType.MiniAdmin)
            {
                var companyId = user.CompanyID;
                var companygid = HttpContext.Request.Headers["company-guid"].ToString();
                if (!string.IsNullOrEmpty(companygid))
                {
                    var companyinfo = await companyConfigService.Get(new Guid(companygid));
                    companyId = companyinfo.Company.CompanyID;
                }
                expable.And(it => SqlFunc.Subqueryable<Shop_Product>().Where(s => s.ProductID == it.ProductID && s.CompanyID == companyId).Any());
                if (user.UserType == (int)CompanyUserType.Shop)
                {
                    expable.And(it => SqlFunc.Subqueryable<Shop_Product>().Where(s => s.ProductID == it.ProductID && s.ShopID == user.ShopID).Any());
                }
            }
            else if (user.PlatformType == PlatformType.XFZMini && query.From == "my")
            {
                expable.And(it => it.UserID == user.UserID);
            }
            else if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.Api)
            {
                if (!query.ProductID.HasValue && string.IsNullOrEmpty(query.ProductIds))
                {
                    return Ok(new { items = new List<Shop_Comment>(), count = 0 });
                }
            }
            var models = await commentService.List(expable.ToExpression(), query);
            if (query.From != "my" && user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.MiniAdmin)
            {
                foreach (var model in models)
                {
                    if (model.IsAno)
                    {
                        model.FullName = "匿名用户";
                        model.UserAvatar = "";
                    }
                }
            }
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<CommentSummary> Summary(int productId)
        {
            if (productId <= 0)
            {
                throw new ErrorCodeException(-1, "商品信息错误");
            }
            return await commentService.Summary(productId);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Shop_Comment model)
        {
            var user = CurrentUser;
            await PrepareComment(model, user);
            var count = await commentService.Add(model);
            if (count > 0 && !string.IsNullOrEmpty(model.OrderID))
            {
                try
                {
                    await httpService.GetAsync(ServiceNames.OrderService, $"/api/order/updateassess?orderId={model.OrderID}");
                }
                catch (Exception ex)
                {
                    Common.StaticLog.Debug(ex);
                }
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Adds([FromBody] List<Model.Shop_Comment> models)
        {
            if (models == null || models.Count == 0)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            var user = CurrentUser;
            foreach (var model in models)
            {
                await PrepareComment(model, user);
            }
            await commentService.Add(models);
            return Ok();
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Shop_Comment model)
        {
            if (model == null || model.ID <= 0)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            var user = CurrentUser;
            if (user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.MiniAdmin)
            {
                throw new ErrorCodeException(-1, "评价提交后不能修改");
            }
            var current = await commentService.Get(model.ID);
            if (current == null)
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            if (model.Score.HasValue && (model.Score < 1 || model.Score > 5))
            {
                throw new ErrorCodeException(-1, "请选择评分");
            }
            await commentService.UpdateAsync(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Like([FromForm] int id)
        {
            if (id <= 0)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            if (!await commentService.Like(id))
            {
                throw new ErrorCodeException(-1, "点赞失败");
            }
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Shop_Comment> Get(int id)
        {
            var model = await commentService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var user = CurrentUser;
            if (model.IsAno && user.PlatformType != PlatformType.Admin && user.PlatformType != PlatformType.MiniAdmin && model.UserID != user.UserID)
            {
                model.FullName = "匿名用户";
                model.UserAvatar = "";
            }
            return model;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var user = CurrentUser;
            if (user.PlatformType == PlatformType.Admin || user.PlatformType == PlatformType.MiniAdmin)
            {
                await commentService.Delete(id);
            }
            else
            {
                await commentService.Delete(id, user.UserID);
            }
            return Ok();
        }

        private async Task PrepareComment(Shop_Comment model, ICurrentUser user)
        {
            if (model == null || !model.ProductID.HasValue || model.ProductID <= 0)
            {
                throw new ErrorCodeException(-1, "商品信息错误");
            }
            if (!model.Score.HasValue || model.Score < 1 || model.Score > 5)
            {
                throw new ErrorCodeException(-1, "请选择评分");
            }
            model.CreateTime = DateTime.Now;
            if (!model.LikeCount.HasValue)
            {
                model.LikeCount = 0;
            }
            if (user.PlatformType == PlatformType.Admin || user.PlatformType == PlatformType.MiniAdmin)
            {
                model.IsAno = true;
                return;
            }
            model.UserID = user.UserID;
            model.FullName = user.FullName;
            if (string.IsNullOrEmpty(model.UserAvatar) && user.PlatformType == PlatformType.XFZMini)
            {
                var umodel = await authService.UserInfo<Live_User>();
                model.UserAvatar = umodel?.Picture;
            }
        }
    }
}
