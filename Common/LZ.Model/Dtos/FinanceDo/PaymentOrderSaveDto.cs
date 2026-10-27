using LZ.Model.Enums;
using System;
using System.Collections.Generic;

namespace LZ.Model.Dtos.FinanceDo
{
    /// <summary>
    /// 支付订单更新（仅允许修改以下字段）
    /// </summary>
    public class PaymentOrderSaveDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid ID { get; set; }

        public CzyPayTypeEnumType PayType { get; set; }
        public CzyPayChannelEnumType PayChannel { get; set; }
        public bool WhetherUrgent { get; set; }
        public string PayUse { get; set; }
        public string Purpose { get; set; }
        public string Remark { get; set; }
        public DateTime? PaymentDate { get; set; }
        public Guid? PaymentAccountID { get; set; }
        public byte[] Timestamp { get; set; }

    }

}
