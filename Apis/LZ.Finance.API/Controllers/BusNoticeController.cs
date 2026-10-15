using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model.Dtos.FinanceDo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class BusNoticeController : ApiControllerBase
    {
        private readonly IPaymentOrderService _paymentOrderService;

        public BusNoticeController(IPaymentOrderService paymentOrderService)
        {
            _paymentOrderService = paymentOrderService;
        }

        /// <summary>
        /// 财资云推送结果通知支付订单
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PaymentOrderPushNotice(PaymentOrderPushNoticeDto dto)
        {
            await _paymentOrderService.HandlePushNotice(dto);
            return Ok();
        }
    }
}
