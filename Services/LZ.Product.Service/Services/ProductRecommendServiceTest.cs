using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using LZ.Product.Service.Interface;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Product.Service.Services
{
    public class ProductRecommendServiceTest : ServicesBase<Shop_Product>, IProductRecommendService, IScopedDependency
    {
        private static readonly int[] ExcludeOrderStatus =
        [
            (int)OrderStatus.WaitPay,
            (int)OrderStatus.Cancel,
            (int)OrderStatus.Refund
        ];

        public async Task<List<Shop_Product>> List(Expression<Func<Shop_Product, bool>> exp, ProductPagedModel query, int userId, int companyId)
        {
            var expable = Expressionable.Create<Shop_Product>();
            if (exp != null)
            {
                expable.And(exp);
            }

            if (!query.Status.HasValue)
            {
                expable.And(it => it.Status == (int)ProductStatus.List);
            }

            var cateIds = await ResolveCateIds(query, companyId);
            if (cateIds.Count > 0)
            {
                ApplyCateFilter(expable, cateIds);
            }

            if (query.ProductID.HasValue)
            {
                var excludeId = query.ProductID.Value;
                expable.And(it => it.ProductID != excludeId);
            }

            var candidates = await Context.Queryable<Shop_Product>()
                .Where(expable.ToExpression())
                .Select(it => new CandidateProduct
                {
                    ProductID = it.ProductID,
                    CateID = it.CateID,
                    CateIDS = it.CateIDS,
                    BrandID = it.BrandID,
                    Tabs = it.Tabs,
                    SaleCount = it.SaleCount,
                    ProductName = it.ProductName
                })
                .ToListAsync();

            if (candidates.Count == 0)
            {
                query.Count = 0;
                return [];
            }

            var scores = await ScoreCandidates(candidates, userId, companyId, cateIds);
            var ranked = candidates
                .Select(it => new { it.ProductID, Score = scores.GetValueOrDefault(it.ProductID) })
                .OrderByDescending(it => it.Score)
                .ThenByDescending(it => it.ProductID)
                .ToList();

            query.Count = ranked.Count;
            List<int> pageIds;
            if (query.Page == 0)
            {
                pageIds = ranked.Select(it => it.ProductID).ToList();
            }
            else
            {
                var page = query.Page < 1 ? 1 : query.Page;
                var pageSize = query.PageSize < 1 ? 15 : query.PageSize;
                pageIds = ranked.Skip((page - 1) * pageSize).Take(pageSize).Select(it => it.ProductID).ToList();
            }

            if (pageIds.Count == 0)
            {
                return [];
            }

            var products = await Context.Queryable<Shop_Product>()
                .Where(it => pageIds.Contains(it.ProductID))
                .ToListAsync();
            return pageIds
                .Select(id => products.Find(p => p.ProductID == id))
                .Where(it => it != null)
                .Select(it => it!)
                .ToList();
        }

        private async Task<List<int>> ResolveCateIds(ProductPagedModel query, int companyId)
        {
            if (query.CID.HasValue)
            {
                var cid = query.CID.Value;
                var childIds = await Context.Queryable<Shop_Categories>()
                    .Where(it => it.ParentID == cid && it.CompanyID == companyId)
                    .Select(it => it.CateID)
                    .ToListAsync();
                var ids = new List<int> { cid };
                ids.AddRange(childIds);
                return ids.Distinct().ToList();
            }

            if (!query.ProductID.HasValue)
            {
                return [];
            }

            var product = await Context.Queryable<Shop_Product>()
                .Where(it => it.ProductID == query.ProductID.Value)
                .Select(it => new { it.CateID, it.CateIDS })
                .FirstAsync();
            if (product == null)
            {
                return [];
            }

            return ParseCateIds(product.CateID, product.CateIDS);
        }

        private static void ApplyCateFilter(Expressionable<Shop_Product> expable, List<int> cateIds)
        {
            var cateExp = Expressionable.Create<Shop_Product>();
            cateExp.Or(it => it.CateID.HasValue && cateIds.Contains(it.CateID.Value));
            foreach (var cateId in cateIds)
            {
                var pattern = $",{cateId},";
                cateExp.Or(it => it.CateIDS.Contains(pattern));
            }
            expable.And(cateExp.ToExpression());
        }

        private async Task<Dictionary<int, double>> ScoreCandidates(List<CandidateProduct> candidates, int userId, int companyId, List<int> filterCateIds)
        {
            var productIds = candidates.Select(it => it.ProductID).ToList();
            var now = DateTime.Now;
            var visitFrom = now.AddDays(-90);
            var popularFrom = now.AddDays(-30);
            var orderFrom = now.AddDays(-365);

            var userVisits = new List<Shop_Visit>();
            var userOrderItems = new List<(int ProductID, int Count)>();
            var userCarts = new List<(int ProductID, int Count)>();
            var userComments = new List<(int ProductID, int Score)>();

            if (userId > 0)
            {
                userVisits = await Context.Queryable<Shop_Visit>()
                    .Where(it => it.UserID == userId && it.CompanyID == companyId && it.StartTime >= visitFrom)
                    .OrderByDescending(it => it.StartTime)
                    .Take(200)
                    .ToListAsync();

                var orderIds = await Context.Queryable<Shop_Order>()
                    .Where(it => it.UserID == userId && it.CompanyID == companyId && it.OrderDate >= orderFrom)
                    .Where(it => it.OrderStatus.HasValue && !ExcludeOrderStatus.Contains(it.OrderStatus.Value))
                    .Select(it => it.OrderID)
                    .ToListAsync();
                if (orderIds.Count > 0)
                {
                    var items = await Context.Queryable<Shop_OrderItems>()
                        .Where(it => orderIds.Contains(it.OrderID) && it.ProductID.HasValue)
                        .Select(it => new { it.ProductID, it.ProductCount })
                        .ToListAsync();
                    userOrderItems = items
                        .Where(it => it.ProductID.HasValue)
                        .Select(it => (it.ProductID!.Value, it.ProductCount ?? 1))
                        .ToList();
                }

                var carts = await Context.Queryable<Shop_ShoppingCart>()
                    .Where(it => it.UserID == userId && it.ProductID.HasValue)
                    .Select(it => new { it.ProductID, it.ProductCount })
                    .ToListAsync();
                userCarts = carts
                    .Where(it => it.ProductID.HasValue)
                    .Select(it => (it.ProductID!.Value, it.ProductCount ?? 1))
                    .ToList();

                var comments = await Context.Queryable<Shop_Comment>()
                    .Where(it => it.UserID == userId && it.ProductID.HasValue && it.Score.HasValue)
                    .Select(it => new { it.ProductID, it.Score })
                    .ToListAsync();
                userComments = comments
                    .Where(it => it.ProductID.HasValue && it.Score.HasValue)
                    .Select(it => (it.ProductID!.Value, it.Score!.Value))
                    .ToList();
            }

            var globalVisits = await Context.Queryable<Shop_Visit>()
                .Where(it => it.CompanyID == companyId
                    && it.FromType == (int)VisitFromType.Product
                    && it.ProductID.HasValue
                    && productIds.Contains(it.ProductID.Value)
                    && it.StartTime >= popularFrom)
                .GroupBy(it => it.ProductID)
                .Select(it => new { ProductID = it.ProductID, Count = SqlFunc.AggregateCount(it.ID) })
                .ToListAsync();

            var globalCarts = await Context.Queryable<Shop_ShoppingCart>()
                .Where(it => it.ProductID.HasValue && productIds.Contains(it.ProductID.Value))
                .GroupBy(it => it.ProductID)
                .Select(it => new { ProductID = it.ProductID, Count = SqlFunc.AggregateCount(it.CartID) })
                .ToListAsync();

            var globalComments = await Context.Queryable<Shop_Comment>()
                .Where(it => it.ProductID.HasValue && it.Score.HasValue && productIds.Contains(it.ProductID.Value))
                .GroupBy(it => it.ProductID)
                .Select(it => new
                {
                    ProductID = it.ProductID,
                    Total = SqlFunc.AggregateSum(it.Score),
                    Count = SqlFunc.AggregateCount(it.ID)
                })
                .ToListAsync();

            var visitByProduct = userVisits
                .Where(it => it.FromType == (int)VisitFromType.Product && it.ProductID.HasValue && (it.Seconds == null || it.Seconds >= 3))
                .GroupBy(it => it.ProductID!.Value)
                .ToDictionary(g => g.Key, g => g.Sum(it => VisitWeight(it, now)));

            var orderByProduct = userOrderItems
                .GroupBy(it => it.ProductID)
                .ToDictionary(g => g.Key, g => g.Sum(it => it.Count));
            var cartByProduct = userCarts
                .GroupBy(it => it.ProductID)
                .ToDictionary(g => g.Key, g => g.Sum(it => it.Count));
            var commentByProduct = userComments
                .GroupBy(it => it.ProductID)
                .ToDictionary(g => g.Key, g => g.Average(it => it.Score));

            var globalVisitMap = globalVisits
                .Where(it => it.ProductID.HasValue)
                .ToDictionary(it => it.ProductID!.Value, it => it.Count);
            var globalCartMap = globalCarts
                .Where(it => it.ProductID.HasValue)
                .ToDictionary(it => it.ProductID!.Value, it => it.Count);
            var globalCommentMap = globalComments
                .Where(it => it.ProductID.HasValue && it.Count > 0)
                .ToDictionary(it => it.ProductID!.Value, it => (double)(it.Total ?? 0) / it.Count);

            var behaviorProductIds = visitByProduct.Keys
                .Concat(orderByProduct.Keys)
                .Concat(cartByProduct.Keys)
                .Concat(commentByProduct.Keys)
                .Distinct()
                .ToList();
            var preference = await BuildPreference(behaviorProductIds, orderByProduct, cartByProduct, visitByProduct, commentByProduct);

            var searchTexts = userVisits
                .Where(it => it.FromType == (int)VisitFromType.Search)
                .SelectMany(it => (it.Keyword ?? it.Search ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Where(it => it.Length >= 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var scores = new Dictionary<int, double>(candidates.Count);
            foreach (var product in candidates)
            {
                var score = 0d;
                if (orderByProduct.TryGetValue(product.ProductID, out var orderCount))
                {
                    score += Math.Min(orderCount, 5) * 40;
                }
                if (cartByProduct.TryGetValue(product.ProductID, out var cartCount))
                {
                    score += Math.Min(cartCount, 5) * 25;
                }
                if (commentByProduct.TryGetValue(product.ProductID, out var userScore))
                {
                    score += (userScore - 3) * 15;
                }
                if (visitByProduct.TryGetValue(product.ProductID, out var visitScore))
                {
                    score += visitScore * 12;
                }

                var productCates = ParseCateIds(product.CateID, product.CateIDS);
                var cateOverlap = productCates.Count(preference.CateIds.Contains);
                if (cateOverlap > 0)
                {
                    score += cateOverlap * 8;
                }
                if (product.BrandID.HasValue && preference.BrandIds.Contains(product.BrandID.Value))
                {
                    score += 5;
                }
                if (!string.IsNullOrEmpty(product.Tabs) && preference.Tabs.Any(tab => product.Tabs.Contains(tab)))
                {
                    score += 3;
                }
                if (filterCateIds.Count > 0 && productCates.Any(filterCateIds.Contains))
                {
                    score += 2;
                }
                if (searchTexts.Count > 0 && !string.IsNullOrEmpty(product.ProductName)
                    && searchTexts.Any(text => product.ProductName.Contains(text, StringComparison.OrdinalIgnoreCase)))
                {
                    score += 6;
                }

                score += Math.Log(1 + (product.SaleCount ?? 0)) * 6;
                if (globalVisitMap.TryGetValue(product.ProductID, out var gVisit))
                {
                    score += Math.Log(1 + gVisit) * 5;
                }
                if (globalCartMap.TryGetValue(product.ProductID, out var gCart))
                {
                    score += Math.Log(1 + gCart) * 3;
                }
                if (globalCommentMap.TryGetValue(product.ProductID, out var gComment))
                {
                    score += (gComment - 3) * 4;
                }

                scores[product.ProductID] = score;
            }

            return scores;
        }

        private async Task<UserPreference> BuildPreference(
            List<int> productIds,
            Dictionary<int, int> orders,
            Dictionary<int, int> carts,
            Dictionary<int, double> visits,
            Dictionary<int, double> comments)
        {
            var preference = new UserPreference();
            if (productIds.Count == 0)
            {
                return preference;
            }

            var products = await Context.Queryable<Shop_Product>()
                .Where(it => productIds.Contains(it.ProductID))
                .Select(it => new { it.ProductID, it.CateID, it.CateIDS, it.BrandID, it.Tabs })
                .ToListAsync();

            foreach (var product in products)
            {
                var weight = 1d;
                if (orders.ContainsKey(product.ProductID))
                {
                    weight += 4;
                }
                if (carts.ContainsKey(product.ProductID))
                {
                    weight += 2;
                }
                if (visits.ContainsKey(product.ProductID))
                {
                    weight += 1;
                }
                if (comments.TryGetValue(product.ProductID, out var score) && score >= 4)
                {
                    weight += 2;
                }
                else if (comments.TryGetValue(product.ProductID, out score) && score <= 2)
                {
                    weight -= 2;
                }

                if (weight <= 0)
                {
                    continue;
                }

                foreach (var cateId in ParseCateIds(product.CateID, product.CateIDS))
                {
                    preference.CateIds.Add(cateId);
                }
                if (product.BrandID.HasValue)
                {
                    preference.BrandIds.Add(product.BrandID.Value);
                }
                if (!string.IsNullOrEmpty(product.Tabs))
                {
                    foreach (var tab in product.Tabs.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    {
                        preference.Tabs.Add(tab);
                    }
                }
            }

            return preference;
        }

        private static double VisitWeight(Shop_Visit visit, DateTime now)
        {
            var start = visit.StartTime ?? visit.CreateTime ?? now;
            var days = (now - start).TotalDays;
            var recency = days <= 7 ? 1d : days <= 30 ? 0.6d : 0.3d;
            var seconds = visit.Seconds ?? 10;
            var duration = Math.Min(seconds / 30d, 3d);
            return recency * Math.Max(duration, 0.5);
        }

        private static List<int> ParseCateIds(int? cateId, string? cateIds)
        {
            var ids = new List<int>();
            if (cateId.HasValue && cateId.Value > 0)
            {
                ids.Add(cateId.Value);
            }
            if (!string.IsNullOrEmpty(cateIds))
            {
                foreach (var part in cateIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (int.TryParse(part, out var id) && id > 0)
                    {
                        ids.Add(id);
                    }
                }
            }
            return ids.Distinct().ToList();
        }

        private sealed class CandidateProduct
        {
            public int ProductID { get; set; }
            public int? CateID { get; set; }
            public string? CateIDS { get; set; }
            public int? BrandID { get; set; }
            public string? Tabs { get; set; }
            public int? SaleCount { get; set; }
            public string? ProductName { get; set; }
        }

        private sealed class UserPreference
        {
            public HashSet<int> CateIds { get; } = [];
            public HashSet<int> BrandIds { get; } = [];
            public HashSet<string> Tabs { get; } = new(StringComparer.OrdinalIgnoreCase);
        }
    }
}
