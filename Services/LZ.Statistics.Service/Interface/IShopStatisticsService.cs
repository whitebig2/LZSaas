using LZ.Model;
using LZ.Model.Statistics;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Interface
{
    public interface IShopStatisticsService
    {
        Task<ShopMallDashboard> Dashboard(Expression<Func<Shop_Order, bool>> orderExp, Expression<Func<Shop_Product, bool>> productExp, Expression<Func<Shop_Visit, bool>> visitExp, Expression<Func<Live_User, bool>> userExp, Expression<Func<Shop_V_IntegralLog, bool>> integralExp, Expression<Func<Shop_V_PromotionTask, bool>> taskExp, int top);
        Task<ShopSalePanel> Sale(Expression<Func<Shop_Order, bool>> exp);
        Task<ShopUserPortrait> UserPortrait(Expression<Func<Live_User, bool>> exp);
        Task<ShopUserPortrait> Portrait(Expression<Func<Shop_V_UserOrder, bool>> exp);
        Task<ShopGoodsOverview> Goods(Expression<Func<Shop_Product, bool>> productExp, Expression<Func<Shop_Order, bool>> orderExp, int top);
        Task<ShopVisitPanel> Visit(Expression<Func<Shop_Visit, bool>> exp, int top);
        Task<ShopUserGrowth> Growth(Expression<Func<Live_User, bool>> exp);
        Task<ShopPromoterPanel> Promoter(Expression<Func<Shop_V_IntegralLog, bool>> integralExp, Expression<Func<Shop_V_PromotionTask, bool>> taskExp, int top);
    }
}
