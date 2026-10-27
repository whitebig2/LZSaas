namespace LZ.Model.Statistics
{
    /// <summary>
    /// 商城数据中心驾驶舱
    /// </summary>
    public class ShopMallDashboard
    {
        public ShopSalesTimeDim SalesTimeDim { get; set; } = new();
        public List<ShopNameValueCount> ProvinceSales { get; set; } = new();
        public Dictionary<string, List<ShopCitySale>> CitySales { get; set; } = new();
        public ShopUserPortrait UserPortrait { get; set; } = new();
        public ShopGoodsOverview GoodsOverview { get; set; } = new();
        public ShopVisitTimeDim VisitTimeDim { get; set; } = new();
        public List<ShopNameValue> GoodsVisitTop10 { get; set; } = new();
        public ShopUserGrowth UserGrowth { get; set; } = new();
        public List<ShopNameValue> SearchTop10 { get; set; } = new();
        public List<ShopNameIntValue> PromoterTop10 { get; set; } = new();
        public List<ShopPromoterTaskCompletion> PromoterTaskCompletion { get; set; } = new();
    }

    public class ShopSalesTimeDim
    {
        public ShopAmountOrders Last7Days { get; set; } = new();
        public ShopAmountOrders CurrentMonth { get; set; } = new();
        public ShopAmountOrders Cumulative { get; set; } = new();
        public List<ShopSaleDaily> DailyTrend { get; set; } = new();
    }

    public class ShopSaleDaily
    {
        public string Date { get; set; }
        public decimal Amount { get; set; }
        public int Orders { get; set; }
    }

    public class ShopAmountOrders
    {
        public decimal Amount { get; set; }
        public int Orders { get; set; }
    }

    public class ShopNameValue
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
    public class ShopNameIntValue
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
    public class ShopNameValueCount
    {
        public string Name { get; set; }
        /// <summary>
        /// 销售额
        /// </summary>
        public decimal Value { get; set; }
        /// <summary>
        /// 订单数
        /// </summary>
        public int Count { get; set; }
    }

    public class ShopCitySale
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public int Count { get; set; }
        public decimal Percent { get; set; }
    }

    public class ShopUserPortrait
    {
        public List<ShopNameValue> Gender { get; set; } = new();
        public List<ShopNameValue> Age { get; set; } = new();
    }

    public class ShopGoodsOverview
    {
        public int TotalGoods { get; set; }
        public List<ShopNameValue> SalesTop10 { get; set; } = new();
    }

    public class ShopVisitTimeDim
    {
        public ShopPvUv Last7Days { get; set; } = new();
        public ShopPvUv CurrentMonth { get; set; } = new();
        public ShopPvUv Cumulative { get; set; } = new();
        public List<ShopVisitDaily> DailyTrend { get; set; } = new();
    }

    public class ShopPvUv
    {
        public int Pv { get; set; }
        public int Uv { get; set; }
    }

    public class ShopVisitDaily
    {
        public string Date { get; set; }
        public int Pv { get; set; }
        public int Uv { get; set; }
    }

    public class ShopUserGrowth
    {
        public int TotalNewUsers { get; set; }
        public List<ShopDailyCount> DailyNew { get; set; } = new();
    }

    public class ShopDailyCount
    {
        public string Date { get; set; }
        public int Value { get; set; }
    }

    public class ShopPromoterTaskCompletion
    {
        public string Name { get; set; }
        public decimal Target { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
    }

    public class ShopSalePanel
    {
        public ShopSalesTimeDim SalesTimeDim { get; set; } = new();
        public List<ShopNameValueCount> ProvinceSales { get; set; } = new();
        public Dictionary<string, List<ShopCitySale>> CitySales { get; set; } = new();
    }

    public class ShopVisitPanel
    {
        public ShopVisitTimeDim VisitTimeDim { get; set; } = new();
        public List<ShopNameValue> GoodsVisitTop10 { get; set; } = new();
        public List<ShopNameValue> SearchTop10 { get; set; } = new();
    }

    public class ShopPromoterPanel
    {
        public List<ShopNameIntValue> PromoterTop10 { get; set; } = new();
        public List<ShopPromoterTaskCompletion> PromoterTaskCompletion { get; set; } = new();
    }
}
