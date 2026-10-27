using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{
    /// <summary>
    /// 1.2.4. T0004-应付单业务状态查询接口
    /// </summary>
    public class T0004ResultSetItem : ICzyResultSetItem
    {
        /// <summary>
        /// 外部系统单据编号
        /// 长度 50
        /// </summary>
        public  string BillsPayOrderNo { get; set; }
        /// <summary>
        /// 财资云应付单号
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
        /// 实际付款日期
        /// </summary>
        public string RelPaymentDate { get; set; }
        /// <summary>
        /// 实际支付金额
        /// </summary>
        public string PayAmount { get; set; }
        /// <summary>
        /// 交易流水号集合（财资云根据交易流水匹配规则同步到的银行流水，可能多笔，当前只支持T0004录入的单笔付款单据）
        /// </summary>
        public IEnumerable<string> TransInfoNoList { get; set; }
        public IEnumerable<T0004BatchItem> BatchList { get; set; }
        
    }

    public class T0004BatchItem
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


}
