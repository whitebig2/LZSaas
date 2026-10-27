using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.ShopBasic.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.ShopBasic.API
{
    public class VisitController(IVisitService visitService, ICompanyConfigService companyConfigService) : ShopBasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Visit>();
            if (user.PlatformType == PlatformType.Admin || user.PlatformType == PlatformType.Api)
            {
                if (query.CompanyID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CompanyID);
                }
            }
            else if (user.PlatformType == PlatformType.MiniAdmin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            else if (user.PlatformType == PlatformType.XFZMini)
            {
                expable.And(it => it.UserID == user.UserID);
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            expable.AndIF(query.ProductID.HasValue, it => it.ProductID == query.ProductID);
            expable.AndIF(query.Type.HasValue, it => it.FromType == query.Type);
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                expable.And(it => it.IPAddress.Contains(query.Keyword) || it.City.Contains(query.Keyword) || it.Search.Contains(query.Keyword) || it.Keyword.Contains(query.Keyword));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.FromPage.Contains(query.Name));
            }
            if (query.Year.HasValue && query.Month.HasValue)
            {
                var start = new DateTime(query.Year.Value, query.Month.Value, 1);
                var end = start.AddMonths(1);
                expable.And(it => it.StartTime >= start && it.StartTime < end);
            }
            var models = await visitService.List(expable.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] int productId)
        {
            var user = CurrentUser;
            if (user.UserID <= 0)
            {
                throw new ErrorCodeException(-1, "请先登录");
            }
            var model = await FillNewVisit(productId, (int)VisitFromType.Product, user, false);
            var id = await visitService.Add(model);
            return Ok(new { id });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddAnonymous([FromForm] int productId)
        {
            var model = await FillNewVisit(productId, (int)VisitFromType.Product, CurrentUser, true);
            var id = await visitService.Add(model);
            return Ok(new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromForm] string search)
        {
            var user = CurrentUser;
            if (user.UserID <= 0)
            {
                throw new ErrorCodeException(-1, "请先登录");
            }
            var model = await FillSearchVisit(search, user, false);
            var id = await visitService.Add(model);
            return Ok(new { id, keyword = model.Keyword });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SearchAnonymous([FromForm] string search)
        {
            var model = await FillSearchVisit(search, CurrentUser, true);
            var id = await visitService.Add(model);
            return Ok(new { id, keyword = model.Keyword });
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> SearchStats([FromQuery] ProductPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_Visit>();
            expable.And(it => it.FromType == (int)VisitFromType.Search);
            if (user.PlatformType == PlatformType.Admin || user.PlatformType == PlatformType.Api)
            {
                if (query.CompanyID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CompanyID);
                }
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            if (!string.IsNullOrEmpty(query.Keyword) || !string.IsNullOrEmpty(query.Name))
            {
                var keyword = !string.IsNullOrEmpty(query.Keyword) ? query.Keyword : query.Name;
                expable.And(it => it.Search.Contains(keyword) || it.Keyword.Contains(keyword));
            }
            if (query.Year.HasValue && query.Month.HasValue)
            {
                var start = new DateTime(query.Year.Value, query.Month.Value, 1);
                var end = start.AddMonths(1);
                expable.And(it => it.StartTime >= start && it.StartTime < end);
            }
            var models = await visitService.SearchStats(expable.ToExpression());
            return Ok(new { items = models });
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> Leave([FromForm] int id)
        {
            if (id <= 0)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            await visitService.Leave(id);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Shop_Visit> Get(int id)
        {
            var model = await visitService.Get(id);
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
            await visitService.Delete(id);
            return Ok();
        }

        private async Task<Shop_Visit> FillSearchVisit(string search, ICurrentUser user, bool anonymous)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                throw new ErrorCodeException(-1, "请输入搜索内容");
            }
            var model = await FillNewVisit(0, (int)VisitFromType.Search, user, anonymous, true);
            model.Search = search.Trim();
            await visitService.ExtractProductKeywords(model);
            return model;
        }

        private async Task<Shop_Visit> FillNewVisit(int productId, int fromType, ICurrentUser user, bool anonymous, bool allowEmptyProduct = false)
        {
            if (!allowEmptyProduct && productId <= 0)
            {
                throw new ErrorCodeException(-1, "商品信息错误");
            }
            var now = DateTime.Now;
            var model = new Shop_Visit
            {
                ProductID = productId > 0 ? productId : null,
                FromType = fromType,
                CreateTime = now,
                StartTime = now,
                UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                IPAddress = GetVisitIp()
            };
            if (anonymous)
            {
                model.UserID = null;
                var companygid = HttpContext.Request.Headers["company-guid"].ToString();
                if (string.IsNullOrEmpty(companygid) || !Guid.TryParse(companygid, out var gid))
                {
                    throw new ErrorCodeException(-1, "公司信息错误");
                }
                var companyinfo = await companyConfigService.Get(gid);
                if (companyinfo?.Company == null)
                {
                    throw new ErrorCodeException(-1, "公司信息不存在");
                }
                model.CompanyID = companyinfo.Company.CompanyID;
                model.CompanyGID = companyinfo.Company.CompanyGID;
                return model;
            }
            model.UserID = user.UserID;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            var headerGid = HttpContext.Request.Headers["company-guid"].ToString();
            if (!string.IsNullOrEmpty(headerGid) && Guid.TryParse(headerGid, out var companyGid))
            {
                var companyinfo = await companyConfigService.Get(companyGid);
                if (companyinfo?.Company != null)
                {
                    model.CompanyID = companyinfo.Company.CompanyID;
                    model.CompanyGID = companyinfo.Company.CompanyGID;
                }
            }
            return model;
        }

        private string GetVisitIp()
        {
            var ip = HttpContext.Request.GetRemoteIpAddress();
            if (!string.IsNullOrEmpty(ip))
            {
                return ip.Split(',')[0].Trim();
            }
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        }
    }
}
