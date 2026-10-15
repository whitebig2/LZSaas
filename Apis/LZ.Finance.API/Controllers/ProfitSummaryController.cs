using LZ.Clients.API;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Finance.API.Controllers
{
    public class ProfitSummaryController(IProfitSummaryService profitSummaryService,IProfitManageService profitManageService,ICityPartnerBillService cityPartnerBillService) : FinanceControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List(DateTime? billMonth, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_ProfitSummary>();
            exp.AndIF(billMonth.HasValue, f => f.BillMonth == billMonth);
            var models = await profitSummaryService.List(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add(Live_ProfitSummary model)
        {
            await profitSummaryService.Add(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddList(List<Live_ProfitSummary> models)
        {
            await profitSummaryService.Add(models);
            return Ok();
        }

        [HttpGet]
        public async Task<Live_ProfitSummary> Get(int id)
        {
            var model = await profitSummaryService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_ProfitSummary model)
        {
            await profitSummaryService.Update(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BitchCreate([FromForm] DateTime? billMonth)
        {
            if (!billMonth.HasValue)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var model = await profitSummaryService.Get(t => t.BillMonth == billMonth);
            if (model != null)
            {
                throw new ErrorCodeException(-1, "已生成");
            }
            var user = CurrentUser;
            var profitManage = await profitManageService.List(t => t.ProfitMonth == billMonth && t.SigningUnitName != "宁波新莱博金浩供应链管理有限公司" && t.SigningUnitName != "宁波职联供应链管理有限公司");
            var cityPartnerBill = await cityPartnerBillService.List(t => t.BillMonth == billMonth && t.CityPartnerTitle != "城市合伙人总部留存");
            Model.Live_ProfitSummary summary = new Live_ProfitSummary();
            summary.BillMonth = billMonth;
            summary.ProfitAmount = profitManage.Sum(t => t.ProfitAmount);
            decimal camount = 0M;
            foreach(var x in cityPartnerBill)
            {
                camount += x.InsuranceRebateAmount + x.LaborRebateAmount + x.OutsourcingRebateAmount + x.SalesmanAmount + x.LaborProfitAmount + x.RecruitmentProfitAmount + x.SaleBill;
            }
            summary.CityPartnerAmount = camount;
            summary.TotalAmount = summary.ProfitAmount + summary.CityPartnerAmount + 40000;
            await profitSummaryService.Add(summary);
            return Ok(new { profitManage,cityPartnerBill, summary });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await profitSummaryService.Delete(id);
            return Ok();
        }
    }
}
