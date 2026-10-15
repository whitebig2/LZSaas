using System;

namespace LZ.Model.Dtos.FinanceDo
{
    /// <summary>
    /// 财资云推送结果通知支付订单
    /// </summary>
    public class PaymentOrderPushNoticeDto
    {
        /// <summary>
        /// 来源单号，对应 PaymentOrder.ID
        /// </summary>
        public string RelBillsCode { get; set; }
        public string BillsPayOrderNo { get; set; }
        public bool IsSuccess { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public DateTime? PushTime { get; set; }
    }
}
