using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum OrderReturnStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        WaitAuto= 1,
        /// <summary>
        /// 待退货
        /// </summary>
        WaitReturn = 2,
        /// <summary>
        /// 已退回，待收货
        /// </summary>
        WaitReceipt = 3,
        /// <summary>
        /// 待买家修改
        /// </summary>
        WaitChange = 4,
        /// <summary>
        /// 换货关闭
        /// </summary>
        ExchangeClose = 5,
        /// <summary>
        /// 换货成功
        /// </summary>
        ExchangeSuccess = 6,
        /// <summary>
        /// 待发出换货商品
        /// </summary>
        WaitExchange = 7,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancel = 8,
        /// <summary>
        /// 审核不通过
        /// </summary>
        NoPass = 9,
        /// <summary>
        /// 退款完成
        /// </summary>
        RefundFinish = 10,
        /// <summary>
        /// 等待退款
        /// </summary>
        WaitFailure = 11,
        /// <summary>
        /// 退款失败
        /// </summary>
        RefundFailure = 12
    }
}
