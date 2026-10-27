using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.BankDo
{
    public class BankRequestDto
    {
        public  Guid TransactionID { get; set; }
        /// <summary>
        /// 期望支付日期
        /// </summary>
        public DateTime PaymentDate { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public CzyPayChannelEnumType PayChannel { get; set; }
        /// <summary>
        /// 支付令牌
        /// </summary>
        public string TransactionPwd { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string Purpose { get; set; }
        public Guid ConfigID { get; set; }
        /// <summary>
        /// 是否加急
        /// </summary>
        public bool WhetherUrgent { get; set; }
    }
}
