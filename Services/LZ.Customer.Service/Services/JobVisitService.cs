using LZ.Core;
using LZ.Core.Interface;
using LZ.Customer.Service.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Customer.Service.Services
{
    /// <summary>
    /// 岗位访问记录服务（Live_JobsVisit），逻辑对齐商城 LZ.ShopBasic.Service.Services.VisitService：
    /// Add/Leave 服务端维护 StartTime/EndTime 并计算停留秒数；搜索词经 ExtractJobKeywords
    /// 命中岗位标题/类目名生成 Keyword（供人岗匹配画像消费）。
    /// </summary>
    public class JobVisitService : ServicesBase<Model.Live_JobsVisit>, IJobVisitService, IScopedDependency
    {
        public async Task<List<Live_JobsVisit>> List(Expression<Func<Live_JobsVisit, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_JobsVisit>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_JobsVisit>().Where(exp).OrderByDescending(it => it.StartTime).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Live_JobsVisit model)
        {
            FillSeconds(model);
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<bool> Leave(int id)
        {
            var model = await Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "访问记录不存在");
            }
            if (model.EndTime.HasValue)
            {
                return true;
            }
            model.EndTime = DateTime.Now;
            FillSeconds(model);
            return await base.Context.Updateable<Live_JobsVisit>()
                .SetColumns(it => it.EndTime == model.EndTime)
                .SetColumns(it => it.Seconds == model.Seconds)
                .Where(it => it.ID == id)
                .ExecuteCommandAsync() > 0;
        }

        public async Task<Live_JobsVisit> Get(int id)
        {
            return await base.Context.Queryable<Live_JobsVisit>().FirstAsync(it => it.ID == id);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_JobsVisit>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task ExtractJobKeywords(Live_JobsVisit model)
        {
            if (string.IsNullOrWhiteSpace(model.Search) || !model.CompanyID.HasValue)
            {
                return;
            }
            var search = model.Search.Trim();
            model.Search = search;
            var jobs = await base.Context.Queryable<Live_V_Jobs>()
                .Where(it => it.CompanyID == model.CompanyID && !string.IsNullOrEmpty(it.Title))
                .Where(it => it.Title.Contains(search)
                    || search.Contains(it.Title)
                    || (!string.IsNullOrEmpty(it.Tabs) && it.Tabs.Contains(search))
                    || (!string.IsNullOrEmpty(it.TabNames) && it.TabNames.Contains(search)))
                .OrderByDescending(it => it.UpTime)
                .Select(it => new { it.ID, it.Title })
                .Take(10)
                .ToListAsync();
            var names = jobs.Select(it => it.Title).Where(it => !string.IsNullOrEmpty(it) && it.Length >= 2).Distinct().ToList();
            if (names.Count == 0)
            {
                model.Keyword = search;
                return;
            }
            model.Keyword = string.Join(",", names);
            if (!model.JobID.HasValue && jobs.Count == 1)
            {
                model.JobID = jobs[0].ID;
            }
        }

        public async Task<List<VisitSearchStat>> SearchStats(Expression<Func<Live_JobsVisit, bool>> exp)
        {
            var list = await base.Context.Queryable<Live_JobsVisit>()
                .Where(exp)
                .GroupBy(it => new { it.Search, it.Keyword })
                .Select(it => new VisitSearchStat
                {
                    Search = it.Search,
                    Keyword = it.Keyword,
                    Count = SqlFunc.AggregateCount(it.ID)
                })
                .ToListAsync();
            return list.OrderByDescending(it => it.Count).ToList();
        }

        private static void FillSeconds(Live_JobsVisit model)
        {
            if (!model.StartTime.HasValue || !model.EndTime.HasValue)
            {
                return;
            }
            var seconds = (int)(model.EndTime.Value - model.StartTime.Value).TotalSeconds;
            model.Seconds = seconds < 0 ? 0 : seconds;
        }
    }
}
