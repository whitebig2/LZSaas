using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.ShopBasic.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.ShopBasic.Service.Services
{
    public class VisitService : ServicesBase<Model.Shop_Visit>, IVisitService, IScopedDependency
    {
        public async Task<List<Shop_Visit>> List(Expression<Func<Shop_Visit, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Shop_Visit>().Where(exp).OrderByDescending(it => it.ID).ToListAsync();
            }
            var list = await base.Context.Queryable<Shop_Visit>().Where(exp).OrderByDescending(it => it.StartTime).OrderByDescending(it => it.ID).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Shop_Visit model)
        {
            FillSeconds(model);
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }

        public async Task<bool> Exist(Expression<Func<Shop_Visit, bool>> exp)
        {
            return await base.Context.Queryable<Shop_Visit>().AnyAsync(exp);
        }

        public async Task<bool> Update(Shop_Visit model)
        {
            FillSeconds(model);
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
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
            return await base.Context.Updateable<Shop_Visit>()
                .SetColumns(it => it.EndTime == model.EndTime)
                .SetColumns(it => it.Seconds == model.Seconds)
                .Where(it => it.ID == id)
                .ExecuteCommandAsync() > 0;
        }

        public async Task<Shop_Visit> Get(int id)
        {
            return await base.Context.Queryable<Shop_Visit>().FirstAsync(it => it.ID == id);
        }

        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Shop_Visit>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task ExtractProductKeywords(Shop_Visit model)
        {
            if (string.IsNullOrWhiteSpace(model.Search) || !model.CompanyID.HasValue)
            {
                return;
            }
            var search = model.Search.Trim();
            model.Search = search;
            var products = await base.Context.Queryable<Shop_Product>()
                .Where(it => it.CompanyID == model.CompanyID && it.Status == (int)ProductStatus.List && !string.IsNullOrEmpty(it.ProductName))
                .Where(it => it.ProductName.Contains(search)
                    || search.Contains(it.ProductName)
                    || (!string.IsNullOrEmpty(it.Tabs) && it.Tabs.Contains(search))
                    || (!string.IsNullOrEmpty(it.ProductTabs) && it.ProductTabs.Contains(search)))
                .OrderByDescending(it => it.SaleCount)
                .Select(it => new { it.ProductID, it.ProductName })
                .Take(10)
                .ToListAsync();
            var names = products.Select(it => it.ProductName).Where(it => !string.IsNullOrEmpty(it) && it.Length >= 2).Distinct().ToList();
            if (names.Count == 0)
            {
                model.Keyword = search;
                return;
            }
            model.Keyword = string.Join(",", names);
            if (!model.ProductID.HasValue && products.Count == 1)
            {
                model.ProductID = products[0].ProductID;
            }
        }

        public async Task<List<VisitSearchStat>> SearchStats(Expression<Func<Shop_Visit, bool>> exp)
        {
            var list = await base.Context.Queryable<Shop_Visit>()
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

        private static void FillSeconds(Shop_Visit model)
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
