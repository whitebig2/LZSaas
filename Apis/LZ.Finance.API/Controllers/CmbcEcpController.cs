using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model.Enums;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using SqlSugar.IOC;
using Microsoft.AspNetCore.Authorization;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class CmbcEcpController : ApiControllerBase
    {

        private readonly IFinanceOtherService  _otherService;

        public CmbcEcpController(IFinanceOtherService otherService)
        {
            _otherService = otherService;
        }

        [HttpGet]
        public async Task<IActionResult> SignalTransactionPageList(DateTime? dateFrom, DateTime? dateTo, string subAccountBookNo, [FromQuery] PagedModelBase query)
        {

            if (dateFrom == null)
            {
                dateFrom = DateTime.Now.Date;

                dateTo = DateTime.Now.Date;
            }

            if (!string.IsNullOrWhiteSpace(subAccountBookNo))
            {
                var resp = await _otherService.EcpSignalTransactionPageList(dateFrom.Value, dateTo.Value, subAccountBookNo, query.Page, query.PageSize);
                if (resp != null && resp.Result != null)
                {
                    return Ok(new { items = resp.Result.Data, count = resp.Result.TotalCount });
                }
                else
                {
                    return Ok(new { items = new object[] { }, count = 0 });
                }
            }
            return Ok(new { items = new object[] { }, count = 0 });


        }

        [HttpGet]
        public async Task<IActionResult> GetLastSignalTransactionRecord(string subAccountBookNo)
        {
            DateTime dateFrom = DateTime.Today.AddDays(-30);
            DateTime dateTo = DateTime.Today.AddDays(1);
            if (!string.IsNullOrWhiteSpace(subAccountBookNo))
            {
                var resp = await _otherService.EcpSignalTransactionPageList(dateFrom, dateTo, subAccountBookNo, 1, 1);
                if (resp != null && resp.Result != null)
                {
                    return Ok(resp.Result.Data.FirstOrDefault());
                }
                else
                {
                    return Ok(null);
                }
            }
            return Ok(null);
        }


    }
}
