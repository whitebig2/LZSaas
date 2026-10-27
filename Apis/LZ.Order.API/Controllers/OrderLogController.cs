using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Order.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Text;

namespace LZ.Order.API
{
    public class OrderLogController(IOrderLogService orderLogService) : OrderControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] OrderPagedModel query)
        {
            if (string.IsNullOrEmpty(query.OrderID))
            {
                throw new ErrorCodeException(-1, "订单号错误！");
            }
            var models = await orderLogService.List(it=>it.OrderID==query.OrderID);
            return Ok(models);
        }
       
    }
}
