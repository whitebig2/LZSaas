using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{
    public class T0023ResultSetItem : ICzyResultSetItem
    {
        /// <summary>
        /// 认领单单据编号
        /// </summary>
        public string ClaimBusinessNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 认领人
        /// </summary>
        public string CreateByName { get; set; }

        /// <summary>
        /// 认领状态描述
        /// </summary>
        public string ClaimClaimStatusDesc { get; set; }

        /// <summary>
        /// 认领金额
        /// </summary>
        public string ClaimTotalAmount { get; set; }

        /// <summary>
        /// 收款单单据总额
        /// </summary>
        public string ReceiptTotalAmount { get; set; }

        /// <summary>
        /// 收款单待认领金额
        /// </summary>
        public string ReceiptWaitClaimAmount { get; set; }

        /// <summary>
        /// 收款单已认领金额
        /// </summary>
        public string ReceiptClaimedAmount { get; set; }

        /// <summary>
        /// 收款单单据状态描述
        /// </summary>
        public string ReceiptBusinessStateDesc { get; set; }

        /// <summary>
        /// 收款单财资云编号
        /// </summary>
        public string ReceiptBusinessNo { get; set; }

        /// <summary>
        /// 应收单单据编号
        /// </summary>
        public string ReceivableBusinessNo { get; set; }

        /// <summary>
        /// 应收单应收金额
        /// </summary>
        public string ReceivableTotalAmount { get; set; }

        /// <summary>
        /// 应收单待核销金额
        /// </summary>
        public string ReceivableWaitClaimAmount { get; set; }

        /// <summary>
        /// 应收单已核销金额
        /// </summary>
        public string ReceivedAmount { get; set; }

        /// <summary>
        /// 应收单单据状态描述
        /// </summary>
        public string ClaimStatusDesc { get; set; }

        /// <summary>
        /// 应收单外部系统编号
        /// </summary>
        public string ExtBusinessNo { get; set; }

        /// <summary>
        /// 收款单外部系统编号
        /// </summary>
        public string ReceiptExtBusinessNo { get; set; }


    }
}
