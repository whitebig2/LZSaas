using LZ.Model.Bank.CZY.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.NoticeModels
{
    public class T0018Notice
    {
        /// <summary>
        /// 本系统实际单据号
        /// </summary>
        public string RelBillsCode { get; set; }

        /// <summary>
        /// 业务状态：1-支付中，2-支付成功，3-支付失败，4-已退款，5-部分支付成功，6-部分退款，10-未知
        /// </summary>
        public string PayStatus { get; set; }

        /// <summary>
        /// 业务状态名称
        /// </summary>
        public string PayStatusName { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string FailReasons { get; set; }

        /// <summary>
        /// 实际付款日期(部分付款有成功时为最大支付日期)
        /// </summary>
        public string RelPaymentDate { get; set; }

        /// <summary>
        /// 支付成功金额
        /// </summary>
        public string PayAmount { get; set; }

        /// <summary>
        /// 合计退款金额
        /// </summary>
        public string RefundAmount { get; set; }

        /// <summary>
        /// 最近退款日期
        /// </summary>
        public string RefundDate { get; set; }

        /// <summary>
        /// 明细数据列表
        /// </summary>
        public ICollection<T0018BatchListItem> BatchList { get; set; }
    }

  
}
