using LZ.Core.Attributes;
using LZ.Model;
using LZ.Statistics.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Statistics.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class SaleCustomerController : StatisticsControllerBase
    {
        private readonly ISaleCustomerService _saleCustomer;

        public SaleCustomerController(ISaleCustomerService saleCustomer)
        {
            _saleCustomer = saleCustomer;
        }
        [HttpPost]
        public async Task<IActionResult> SaleEntryStatistics([FromForm] string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Ok(new { msg = "未传值", code = 1 });
            }
            var list = await _saleCustomer.SaleEntryStatistics(ids);
            return Ok(new { msg = "OK", code = 0, data = list });
        }
        [HttpPost]
        public async Task<IActionResult> SaleUpdatePersonnel([FromForm] int userID, [FromForm] int cid, [FromForm] int type)
        {

            string msg = "";
            msg = await _saleCustomer.SaleUpdatePersonnel(userID, cid,type);
            return Ok(new { data = msg, code = 0 });

        }

        /// <summary>
        /// 销售系统
        /// 一个月保存前三销售员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> HandleCustomerTopSaleMan()
        {
            var date = Convert.ToDateTime("2025-08-01");
            //var date = DateTime.Now.AddDays(-30);
            var list = await _saleCustomer.PersonnelTopReserve(date);
            List<Live_PersonnelItemsLog> logs = new List<Live_PersonnelItemsLog>();
            foreach (var item in list)
            {
                Live_PersonnelItemsLog log = new Live_PersonnelItemsLog();
            }
            return Ok(new {items=list});
        }
    }
}
