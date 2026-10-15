using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.NoticeModels
{



    /// <summary>
    /// 支付查询结果实体类
    /// </summary>
    public class T0022Notice
    {
        /// <summary>
        /// 外部系统单据流水号
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 外部系统单据编号
        /// </summary>
        public string BillsPayOrderNo { get; set; }

        /// <summary>
        /// 本系统实际单据号
        /// </summary>
        public string RelBillsCode { get; set; }

        /// <summary>
        /// 业务状态：1-支付中，2-支付成功，3-支付失败，4-已退款，5-部分支付成功，6-部分退款，10-未知
        /// </summary>
        public string PayStatus { get; set; }

        /// <summary>
        /// 业务状态说明
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
        /// 实际付款类型
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 付款账号，当实际付款类型为非票据类付款时，有值
        /// </summary>
        public string PayAccountNo { get; set; }

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
        public List<T0022PaymentDetailItem> BatchList { get; set; }

        /// <summary>
        /// 票据列表
        /// </summary>
        public List<T0022DraftInfoItem> DraftList { get; set; }
    }

    /// <summary>
    /// 支付明细项实体类
    /// </summary>
    public class T0022PaymentDetailItem
    {
        /// <summary>
        /// 外部系统单据编号
        /// </summary>
        public string BillsPayOrderNo { get; set; }

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
        /// 本次实际付款日期
        /// </summary>
        public string RelPaymentDate { get; set; }

        /// <summary>
        /// 本次支付成功金额
        /// </summary>
        public string PayAmount { get; set; }

        /// <summary>
        /// 本次退款金额
        /// </summary>
        public string RefundAmount { get; set; }

        /// <summary>
        /// 本次退款日期
        /// </summary>
        public string RefundDate { get; set; }
    }

    /// <summary>
    /// 票据信息项实体类
    /// </summary>
    public class T0022DraftInfoItem
    {
        /// <summary>
        /// 票据申请单据上的付款账号
        /// </summary>
        public string PayAccountNo { get; set; }

        /// <summary>
        /// 票据（包）号
        /// </summary>
        public string DraftCode { get; set; }

        /// <summary>
        /// 子票区间
        /// </summary>
        public string DraftRange { get; set; }

        /// <summary>
        /// 出票日
        /// </summary>
        public string DrawDate { get; set; }

        /// <summary>
        /// 到期日
        /// </summary>
        public string DueDate { get; set; }

        /// <summary>
        /// 承兑行行号
        /// </summary>
        public string AcceptorBankCode { get; set; }

        /// <summary>
        /// 承兑行名称
        /// </summary>
        public string AcceptorBankName { get; set; }

        /// <summary>
        /// 银行反馈信息
        /// </summary>
        public string ProcessDesc { get; set; }

        /// <summary>
        /// 消息类型：INVOICE_APPLY, RECEIVE_APPLY, RECEIVE_REPLY, ACCEPT_APPLY, ACCEPT_REPLY, DRAFT_REVOCATION, ENDORSE_APPLY, ENDORSE_REPLY, RECEIVE_REVOCATION, ACCEPT_REVOCATION
        /// </summary>
        public string MessageType { get; set; }
    }

}
