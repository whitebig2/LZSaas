using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Customer.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Customer.API
{
    /// <summary>
    /// 岗位访问记录（Live_JobsVisit）
    /// 岗位详情浏览埋点（登录 Add / 匿名 AddAnonymous）、离开回执 Leave（服务端算停留秒数）、
    /// 搜索埋点（Search/SearchAnonymous，提取岗位关键词供人岗匹配画像消费）、后台查询/删除。
    /// </summary>
    public class VisitController(IJobVisitService jobVisitService, ICompanyConfigService companyConfigService) : CustomerControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] JobVisitPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_JobsVisit>();
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
            else if (user.PlatformType == PlatformType.LZMini)
            {
                expable.And(it => it.UserID == user.UserID);
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            expable.AndIF(query.JobID.HasValue, it => it.JobID == query.JobID);
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
            var models = await jobVisitService.List(expable.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        /// <summary>
        /// 岗位浏览埋点：岗位详情页打开时调用，停留时长由 Leave 回执在服务端计算。
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] int jobId)
        {
            var user = CurrentUser;
            if (user == null || user.UserID <= 0)
            {
                throw new ErrorCodeException(-1, "请先登录");
            }
            var model = await FillNewVisit(jobId, (int)VisitFromType.Job, user, false);
            var id = await jobVisitService.Add(model);
            return Ok(new { id });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddAnonymous([FromForm] int jobId)
        {
            var model = await FillNewVisit(jobId, (int)VisitFromType.Job, CurrentUser, true);
            var id = await jobVisitService.Add(model);
            return Ok(new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromForm] string search)
        {
            var user = CurrentUser;
            if (user == null || user.UserID <= 0)
            {
                throw new ErrorCodeException(-1, "请先登录");
            }
            var model = await FillSearchVisit(search, user, false);
            var id = await jobVisitService.Add(model);
            return Ok(new { id, keyword = model.Keyword });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SearchAnonymous([FromForm] string search)
        {
            var model = await FillSearchVisit(search, CurrentUser, true);
            var id = await jobVisitService.Add(model);
            return Ok(new { id, keyword = model.Keyword });
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> SearchStats([FromQuery] JobVisitPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_JobsVisit>();
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
            var models = await jobVisitService.SearchStats(expable.ToExpression());
            return Ok(new { items = models });
        }

        /// <summary>
        /// 离开岗位详情页回执：记录 EndTime 并按 StartTime 计算停留秒数（Seconds）。
        /// </summary>
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> Leave([FromForm] int id)
        {
            if (id <= 0)
            {
                throw new ErrorCodeException(-1, "数据错误");
            }
            await jobVisitService.Leave(id);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Live_JobsVisit> Get(int id)
        {
            var model = await jobVisitService.Get(id);
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
            await jobVisitService.Delete(id);
            return Ok();
        }

        private async Task<Live_JobsVisit> FillSearchVisit(string search, ICurrentUser user, bool anonymous)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                throw new ErrorCodeException(-1, "请输入搜索内容");
            }
            var model = await FillNewVisit(0, (int)VisitFromType.Search, user, anonymous, true);
            model.Search = search.Trim();
            await jobVisitService.ExtractJobKeywords(model);
            return model;
        }

        private async Task<Live_JobsVisit> FillNewVisit(int jobId, int fromType, ICurrentUser user, bool anonymous, bool allowEmptyJob = false)
        {
            if (!allowEmptyJob && jobId <= 0)
            {
                throw new ErrorCodeException(-1, "岗位信息错误");
            }
            var now = DateTime.Now;
            var model = new Live_JobsVisit
            {
                JobID = jobId > 0 ? jobId : null,
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
