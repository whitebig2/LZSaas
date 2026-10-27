using LZ.Core;
using LZ.Core.CommonHandle;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Statistics;
using LZ.Statistics.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Services
{
    public class ShopStatisticsService : ServicesBase<Shop_Order>, IShopStatisticsService, IScopedDependency
    {
        public static readonly int[] ExcludeSaleStatus =
        {
            (int)OrderStatus.WaitPay,
            (int)OrderStatus.Cancel,
            (int)OrderStatus.Refund
        };

        public static readonly string[] AgeGroupNames =
        {
            "18岁以下", "18-25", "26-30", "31-35", "36-40", "41-50", "50岁以上"
        };

        public static readonly string[] MapProvinces =
        {
            "广东", "浙江", "江苏", "上海", "北京", "山东", "福建", "四川", "湖北", "河南",
            "湖南", "河北", "安徽", "辽宁", "陕西", "天津", "江西", "重庆", "广西", "云南",
            "山西", "贵州", "黑龙江", "吉林", "香港", "甘肃", "内蒙古", "新疆", "海南", "宁夏",
            "青海", "澳门", "西藏", "台湾"
        };

        public ISqlSugarClient UserDb => DbScoped.SugarScope.GetConnection(DatabaseNames.Main);

        public async Task<ShopMallDashboard> Dashboard(Expression<Func<Shop_Order, bool>> orderExp, Expression<Func<Shop_Product, bool>> productExp, Expression<Func<Shop_Visit, bool>> visitExp, Expression<Func<Live_User, bool>> userExp, Expression<Func<Shop_V_IntegralLog, bool>> integralExp, Expression<Func<Shop_V_PromotionTask, bool>> taskExp, int top)
        {
            var saleRows = await LoadSaleRows(orderExp);
            var visit = await Visit(visitExp, top);
            var promoter = await Promoter(integralExp, taskExp, top);
            return new ShopMallDashboard
            {
                SalesTimeDim = BuildSalesTimeDim(saleRows),
                ProvinceSales = BuildProvinceSales(saleRows),
                CitySales = BuildCitySales(saleRows),
                UserPortrait = await UserPortrait(userExp),
                GoodsOverview = await BuildGoods(productExp, saleRows, top),
                VisitTimeDim = visit.VisitTimeDim,
                GoodsVisitTop10 = visit.GoodsVisitTop10,
                UserGrowth = await Growth(userExp),
                SearchTop10 = visit.SearchTop10,
                PromoterTop10 = promoter.PromoterTop10,
                PromoterTaskCompletion = promoter.PromoterTaskCompletion
            };
        }

        public async Task<ShopSalePanel> Sale(Expression<Func<Shop_Order, bool>> exp)
        {
            var saleRows = await LoadSaleRows(exp);
            return new ShopSalePanel
            {
                SalesTimeDim = BuildSalesTimeDim(saleRows),
                ProvinceSales = BuildProvinceSales(saleRows),
                CitySales = BuildCitySales(saleRows)
            };
        }

        public async Task<ShopUserPortrait> UserPortrait(Expression<Func<Live_User, bool>> exp)
        {
            var users = await UserDb.Queryable<Live_User>()
                .Where(exp)
                .Select(it => new { it.Sex, it.Age })
                .ToListAsync();
            return BuildPortrait(users.Select(it => (it.Sex, it.Age)));
        }

        public async Task<ShopUserPortrait> Portrait(Expression<Func<Shop_V_UserOrder, bool>> exp)
        {
            var rows = await Context.Queryable<Shop_V_UserOrder>()
                .Where(exp)
                .Where(it => it.OrderStatus.HasValue && !ExcludeSaleStatus.Contains(it.OrderStatus.Value))
                .Select(it => new { it.UserID, it.Sex, it.Age })
                .ToListAsync();
            var users = rows
                .Where(it => it.UserID.HasValue)
                .GroupBy(it => it.UserID)
                .Select(g => g.First())
                .ToList();
            return BuildPortrait(users.Select(it => (it.Sex, it.Age)));
        }

        public async Task<ShopGoodsOverview> Goods(Expression<Func<Shop_Product, bool>> productExp, Expression<Func<Shop_Order, bool>> orderExp, int top)
        {
            var saleRows = await LoadSaleRows(orderExp);
            return await BuildGoods(productExp, saleRows, top);
        }

        public async Task<ShopVisitPanel> Visit(Expression<Func<Shop_Visit, bool>> exp, int top)
        {
            var now = DateTime.Now;
            var day7 = now.Date.AddDays(-6);
            var monthStart = now.Date.AddDays(1 - now.Day);
            var dayVisits = await Context.Queryable<Shop_Visit>()
                .Where(exp)
                .Where(it => it.StartTime >= day7)
                .Select(it => new { it.StartTime, it.UserID, it.IPAddress, it.ID })
                .ToListAsync();
            var daily = new List<ShopVisitDaily>();
            for (var date = day7; date <= now.Date; date = date.AddDays(1))
            {
                var rows = dayVisits.Where(it => it.StartTime.HasValue && it.StartTime.Value.Date == date).ToList();
                daily.Add(new ShopVisitDaily
                {
                    Date = date.ToString("MM-dd"),
                    Pv = rows.Count,
                    Uv = DistinctUv(rows.Select(it => (it.UserID, it.IPAddress)))
                });
            }
            return new ShopVisitPanel
            {
                VisitTimeDim = new ShopVisitTimeDim
                {
                    Last7Days = new ShopPvUv { Pv = dayVisits.Count, Uv = DistinctUv(dayVisits.Select(it => (it.UserID, it.IPAddress))) },
                    CurrentMonth = await VisitPvUv(exp, it => it.StartTime >= monthStart),
                    Cumulative = await VisitPvUv(exp, null),
                    DailyTrend = daily
                },
                GoodsVisitTop10 = await GoodsVisitTop(exp, top),
                SearchTop10 = await SearchTop(exp, top)
            };
        }

        public async Task<ShopUserGrowth> Growth(Expression<Func<Live_User, bool>> exp)
        {
            var now = DateTime.Now;
            var day7 = now.Date.AddDays(-6);
            var users = await UserDb.Queryable<Live_User>()
                .Where(exp)
                .Where(it => it.CreateTime >= day7)
                .Select(it => it.CreateTime)
                .ToListAsync();
            var daily = new List<ShopDailyCount>();
            for (var date = day7; date <= now.Date; date = date.AddDays(1))
            {
                daily.Add(new ShopDailyCount
                {
                    Date = date.ToString("MM-dd"),
                    Value = users.Count(it => it.HasValue && it.Value.Date == date)
                });
            }
            return new ShopUserGrowth
            {
                TotalNewUsers = users.Count,
                DailyNew = daily
            };
        }

        public async Task<ShopPromoterPanel> Promoter(Expression<Func<Shop_V_IntegralLog, bool>> integralExp, Expression<Func<Shop_V_PromotionTask, bool>> taskExp, int top)
        {
            var promoterTop10 = await Context.Queryable<Shop_V_IntegralLog>()
                .Where(integralExp)
                .Where(it => it.IsShopPromotion)
                .GroupBy(it => it.FromUserID)
                .OrderByDescending(it => SqlFunc.AggregateSum(SqlFunc.IsNull(it.Integral, 0)))
                .Select(it => new ShopNameIntValue
                {
                    Name = SqlFunc.AggregateMax(it.FromFullName),
                    Value = SqlFunc.AggregateSum(SqlFunc.IsNull(it.Integral.Value, 0))
                })
                .Take(top)
                .ToListAsync();
            foreach (var item in promoterTop10)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    item.Name = "未知";
                }
            }
            var tasks = await Context.Queryable<Shop_V_PromotionTask>()
                .Where(taskExp)
                .Where(it => it.IsShopPromotion)
                .Select(it => new { it.UserID, it.FullName, it.TargetMoney, it.HaveMoney })
                .ToListAsync();
            var promoterTaskCompletion = tasks.GroupBy(it => it.UserID).Select(g =>
            {
                var target = g.Sum(it => it.TargetMoney);
                var amount = g.Sum(it => it.HaveMoney);
                return new ShopPromoterTaskCompletion
                {
                    Name = string.IsNullOrEmpty(g.First().FullName) ? "未知" : g.First().FullName,
                    Target = target,
                    Amount = amount,
                    Rate = target > 0 ? Math.Round(amount / target, 2) : 0
                };
            }).OrderByDescending(it => it.Amount).Take(top).ToList();
            return new ShopPromoterPanel
            {
                PromoterTop10 = promoterTop10,
                PromoterTaskCompletion = promoterTaskCompletion
            };
        }

        private async Task<List<SaleRow>> LoadSaleRows(Expression<Func<Shop_Order, bool>> exp)
        {
            var items = await SaleQueryable(exp)
                .Select(it => new SaleRow
                {
                    OrderID = it.OrderID,
                    OrderDate = it.OrderDate,
                    OrderTotal = it.OrderTotal,
                    Province = it.Province,
                    City = it.City
                })
                .ToListAsync();
            foreach (var item in items)
            {
                item.Province = NormalizeProvince(item.Province);
                item.City = string.IsNullOrWhiteSpace(item.City) ? "未知" : item.City.Trim();
            }
            return items;
        }

        private async Task<ShopGoodsOverview> BuildGoods(Expression<Func<Shop_Product, bool>> productExp, List<SaleRow> saleRows, int top)
        {
            var orderIds = saleRows.Select(it => it.OrderID).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
            return new ShopGoodsOverview
            {
                TotalGoods = await Context.Queryable<Shop_Product>().Where(productExp).Where(it => it.Status == (int)ProductStatus.List).CountAsync(),
                SalesTop10 = await LoadSalesTopFromItems(orderIds, top)
            };
        }

        private async Task<List<ShopNameValue>> LoadSalesTopFromItems(List<string> orderIds, int top)
        {
            if (orderIds.Count == 0)
            {
                return new List<ShopNameValue>();
            }
            var items = await Context.Queryable<Shop_OrderItems>()
                .Where(it => orderIds.Contains(it.OrderID))
                .Where(it => !it.Status.HasValue || !ExcludeSaleStatus.Contains(it.Status.Value))
                .Select(it => new
                {
                    it.ProductID,
                    it.ProductName,
                    ProductCount = SqlFunc.IsNull(it.ProductCount, 0)
                })
                .ToListAsync();
            return items.GroupBy(it => new { it.ProductID, it.ProductName })
                .Select(g => new ShopNameValue
                {
                    Name = string.IsNullOrEmpty(g.Key.ProductName) ? "未知商品" : g.Key.ProductName,
                    Value = g.Sum(x => x.ProductCount.GetValueOrDefault())
                })
                .OrderByDescending(it => it.Value)
                .Take(top)
                .ToList();
        }

        private static ShopSalesTimeDim BuildSalesTimeDim(List<SaleRow> items)
        {
            var now = DateTime.Now;
            var day7 = now.Date.AddDays(-6);
            var monthStart = now.Date.AddDays(1 - now.Day);
            var daily = new List<ShopSaleDaily>();
            for (var date = day7; date <= now.Date; date = date.AddDays(1))
            {
                var rows = items.Where(it => it.OrderDate.HasValue && it.OrderDate.Value.Date == date).ToList();
                daily.Add(new ShopSaleDaily
                {
                    Date = date.ToString("MM-dd"),
                    Amount = rows.Sum(it => it.OrderTotal),
                    Orders = rows.Count
                });
            }
            return new ShopSalesTimeDim
            {
                Last7Days = ToAmountOrders(items, it => it.OrderDate >= day7),
                CurrentMonth = ToAmountOrders(items, it => it.OrderDate >= monthStart),
                Cumulative = ToAmountOrders(items, null),
                DailyTrend = daily
            };
        }

        private static ShopAmountOrders ToAmountOrders(List<SaleRow> items, Func<SaleRow, bool> timeExp)
        {
            var rows = timeExp == null ? items : items.Where(timeExp).ToList();
            return new ShopAmountOrders
            {
                Orders = rows.Count,
                Amount = rows.Sum(it => it.OrderTotal)
            };
        }

        private static List<ShopNameValueCount> BuildProvinceSales(List<SaleRow> items)
        {
            var map = items.Where(it => !string.IsNullOrEmpty(it.Province)).GroupBy(it => it.Province).ToDictionary(g => g.Key, g => new ShopNameValueCount
            {
                Name = g.Key,
                Value = g.Sum(it => it.OrderTotal),
                Count = g.Count()
            });
            return MapProvinces.Select(name => map.TryGetValue(name, out var item) ? item : new ShopNameValueCount { Name = name }).ToList();
        }

        private static Dictionary<string, List<ShopCitySale>> BuildCitySales(List<SaleRow> items)
        {
            var result = new Dictionary<string, List<ShopCitySale>>();
            foreach (var provinceGroup in items.Where(it => !string.IsNullOrEmpty(it.Province)).GroupBy(it => it.Province))
            {
                var provinceAmount = provinceGroup.Sum(it => it.OrderTotal);
                var cities = provinceGroup.GroupBy(it => it.City).Select(g =>
                {
                    var amount = g.Sum(it => it.OrderTotal);
                    return new ShopCitySale
                    {
                        Name = g.Key,
                        Value = amount,
                        Count = g.Count(),
                        Percent = provinceAmount > 0 ? Math.Round(amount * 100m / provinceAmount, 1) : 0
                    };
                }).OrderByDescending(it => it.Value).ToList();
                result[provinceGroup.Key] = cities;
            }
            return result;
        }

        private async Task<ShopPvUv> VisitPvUv(Expression<Func<Shop_Visit, bool>> exp, Expression<Func<Shop_Visit, bool>> timeExp)
        {
            var pvQb = Context.Queryable<Shop_Visit>().Where(exp);
            var userQb = Context.Queryable<Shop_Visit>().Where(exp);
            var anonQb = Context.Queryable<Shop_Visit>().Where(exp);
            if (timeExp != null)
            {
                pvQb = pvQb.Where(timeExp);
                userQb = userQb.Where(timeExp);
                anonQb = anonQb.Where(timeExp);
            }
            var pv = await pvQb.CountAsync();
            var uvUser = await userQb.Where(it => it.UserID.HasValue).Select(it => it.UserID).Distinct().CountAsync();
            var uvAnon = await anonQb.Where(it => it.UserID == null).Select(it => it.IPAddress).Distinct().CountAsync();
            return new ShopPvUv
            {
                Pv = pv,
                Uv = uvUser + uvAnon
            };
        }

        private async Task<List<ShopNameValue>> GoodsVisitTop(Expression<Func<Shop_Visit, bool>> exp, int top)
        {
            var ranks = await Context.Queryable<Shop_Visit>()
                .Where(exp)
                .Where(it => it.FromType == (int)VisitFromType.Product && it.ProductID.HasValue)
                .GroupBy(it => it.ProductID)
                .Select(it => new { ID = it.ProductID, Count = SqlFunc.AggregateCount(it.ID) })
                .ToListAsync();
            ranks = ranks.OrderByDescending(it => it.Count).Take(top).ToList();
            var ids = ranks.Where(it => it.ID.HasValue).Select(it => it.ID.Value).ToList();
            var products = ids.Count == 0
                ? new List<Shop_Product>()
                : await Context.Queryable<Shop_Product>().Where(it => ids.Contains(it.ProductID)).Select(it => new Shop_Product
                {
                    ProductID = it.ProductID,
                    ProductName = it.ProductName
                }).ToListAsync();
            return ranks.Select(it =>
            {
                var product = products.Find(p => p.ProductID == it.ID);
                return new ShopNameValue
                {
                    Name = string.IsNullOrEmpty(product?.ProductName) ? "未知商品" : product.ProductName,
                    Value = it.Count
                };
            }).ToList();
        }

        private async Task<List<ShopNameValue>> SearchTop(Expression<Func<Shop_Visit, bool>> exp, int top)
        {
            var list = await Context.Queryable<Shop_Visit>()
                .Where(exp)
                .Where(it => it.FromType == (int)VisitFromType.Search && !string.IsNullOrEmpty(it.Search))
                .GroupBy(it => it.Search)
                .Select(it => new ShopNameValue
                {
                    Name = it.Search,
                    Value = SqlFunc.AggregateCount(it.ID)
                })
                .ToListAsync();
            return list.OrderByDescending(it => it.Value).Take(top).ToList();
        }

        private ISugarQueryable<Shop_Order> SaleQueryable(Expression<Func<Shop_Order, bool>> exp)
        {
            return Context.Queryable<Shop_Order>()
                .Where(exp)
                .Where(it => it.OrderStatus.HasValue && !ExcludeSaleStatus.Contains(it.OrderStatus.Value));
        }

        private static int DistinctUv(IEnumerable<(int? UserID, string IPAddress)> rows)
        {
            return rows.Select(it => it.UserID.HasValue ? "u" + it.UserID : "ip" + (it.IPAddress ?? "")).Distinct().Count();
        }

        private static ShopUserPortrait BuildPortrait(IEnumerable<(string Sex, int? Age)> users)
        {
            var list = users.ToList();
            var total = list.Count;
            var genders = new[] { "男", "女", "未知" }.Select(name =>
            {
                var count = name == "未知"
                    ? list.Count(it => string.IsNullOrWhiteSpace(it.Sex) || (it.Sex != "男" && it.Sex != "女"))
                    : list.Count(it => it.Sex == name);
                return new ShopNameValue
                {
                    Name = name,
                    Value = total == 0 ? 0 : Math.Round(count * 100m / total, 1)
                };
            }).ToList();
            var ageMap = list.GroupBy(it => AgeGroup(it.Age)).ToDictionary(g => g.Key, g => g.Count());
            var ages = AgeGroupNames.Select(name => new ShopNameValue
            {
                Name = name,
                Value = ageMap.TryGetValue(name, out var count) ? count : 0
            }).ToList();
            return new ShopUserPortrait { Gender = genders, Age = ages };
        }

        private static string AgeGroup(int? age)
        {
            if (!age.HasValue || age.Value <= 0)
            {
                return "未知";
            }
            if (age.Value < 18)
            {
                return "18岁以下";
            }
            if (age.Value <= 25)
            {
                return "18-25";
            }
            if (age.Value <= 30)
            {
                return "26-30";
            }
            if (age.Value <= 35)
            {
                return "31-35";
            }
            if (age.Value <= 40)
            {
                return "36-40";
            }
            if (age.Value <= 50)
            {
                return "41-50";
            }
            return "50岁以上";
        }

        private static string NormalizeProvince(string province)
        {
            if (string.IsNullOrWhiteSpace(province))
            {
                return "";
            }
            var name = province.Trim()
                .Replace("维吾尔自治区", "")
                .Replace("壮族自治区", "")
                .Replace("回族自治区", "")
                .Replace("特别行政区", "")
                .Replace("自治区", "")
                .Replace("省", "");
            if (name is "北京市" or "北京")
            {
                return "北京";
            }
            if (name is "天津市" or "天津")
            {
                return "天津";
            }
            if (name is "上海市" or "上海")
            {
                return "上海";
            }
            if (name is "重庆市" or "重庆")
            {
                return "重庆";
            }
            if (name.EndsWith("市") && name.Length <= 3)
            {
                name = name[..^1];
            }
            return name;
        }

        private class SaleRow
        {
            public string OrderID { get; set; }
            public DateTime? OrderDate { get; set; }
            public decimal OrderTotal { get; set; }
            public string Province { get; set; }
            public string City { get; set; }
        }
    }
}
