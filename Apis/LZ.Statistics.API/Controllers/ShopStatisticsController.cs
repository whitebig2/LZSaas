using LZ.Model.PagedModel;
using LZ.Statistics.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Statistics.API.Controllers
{
    [AllowAnonymous]
    public class ShopStatisticsController(IShopStatisticsService shopStatisticsService) : StatisticsControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Dashboard([FromQuery] ProductPagedModel query)
        {
            var data = await shopStatisticsService.Dashboard(
                OrderExp(query).ToExpression(),
                ProductExp(query).ToExpression(),
                VisitExp(query).ToExpression(),
                UserExp(query).ToExpression(),
                IntegralExp(query).ToExpression(),
                TaskExp(query).ToExpression(),
                RankTop(query));
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> Sale([FromQuery] ProductPagedModel query)
        {
            return Ok(await shopStatisticsService.Sale(OrderExp(query).ToExpression()));
        }

        [HttpGet]
        public async Task<IActionResult> Portrait([FromQuery] ProductPagedModel query)
        {
            return Ok(await shopStatisticsService.UserPortrait(UserExp(query).ToExpression()));
        }

        [HttpGet]
        public async Task<IActionResult> Goods([FromQuery] ProductPagedModel query)
        {
            return Ok(await shopStatisticsService.Goods(ProductExp(query).ToExpression(), OrderExp(query).ToExpression(), RankTop(query)));
        }

        [HttpGet]
        public async Task<IActionResult> Visit([FromQuery] ProductPagedModel query)
        {
            return Ok(await shopStatisticsService.Visit(VisitExp(query).ToExpression(), RankTop(query)));
        }

        [HttpGet]
        public async Task<IActionResult> Growth([FromQuery] ProductPagedModel query)
        {
            return Ok(await shopStatisticsService.Growth(UserExp(query).ToExpression()));
        }

        [HttpGet]
        public async Task<IActionResult> Promoter([FromQuery] ProductPagedModel query)
        {
            return Ok(await shopStatisticsService.Promoter(IntegralExp(query).ToExpression(), TaskExp(query).ToExpression(), RankTop(query)));
        }

        private static Expressionable<Model.Shop_Order> OrderExp(ProductPagedModel query)
        {
            return Expressionable.Create<Model.Shop_Order>()
                .AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
        }

        private static Expressionable<Model.Shop_Product> ProductExp(ProductPagedModel query)
        {
            return Expressionable.Create<Model.Shop_Product>()
                .AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
        }

        private static Expressionable<Model.Shop_Visit> VisitExp(ProductPagedModel query)
        {
            return Expressionable.Create<Model.Shop_Visit>()
                .AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
        }

        private static Expressionable<Model.Live_User> UserExp(ProductPagedModel query)
        {
            return Expressionable.Create<Model.Live_User>()
                .AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
        }

        private static Expressionable<Model.Shop_V_IntegralLog> IntegralExp(ProductPagedModel query)
        {
            return Expressionable.Create<Model.Shop_V_IntegralLog>()
                .AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
        }

        private static Expressionable<Model.Shop_V_PromotionTask> TaskExp(ProductPagedModel query)
        {
            return Expressionable.Create<Model.Shop_V_PromotionTask>()
                .AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
        }

        private static int RankTop(ProductPagedModel query)
        {
            return query.Top > 0 ? query.Top : 10;
        }
    }
}
