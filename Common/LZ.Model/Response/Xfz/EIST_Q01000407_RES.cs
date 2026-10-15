using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class EIST_Q01000407_RES
    {
        /// <summary>
        /// 交易状态
        /// </summary>
        public string transStatus { get; set; }
        /// <summary>
        /// 交易状态描述
        /// </summary>
        public string transStatusDesc { get; set; }
        /// <summary>
        /// 总笔数
        /// </summary>
        public string totNum { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public string totAmt { get; set; }
        /// <summary>
        /// 成功总笔数
        /// </summary>
        public string sucNum { get; set; }
        /// <summary>
        /// 成功总金额
        /// </summary>
        public string sucAmt { get; set; }
        /// <summary>
        /// 失败总笔数
        /// </summary>
        public string failNum { get; set; }

        /// <summary>
        /// 失败总金额
        /// </summary>
        public string failAmt { get; set; }
        /// <summary>
        /// 转账申请时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 返回码
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// 付款子账簿
        /// </summary>
        public string draweeVacctNo { get; set; }
        /// <summary>
        /// 付款子账簿名称
        /// </summary>
        public string draweeVacctName { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string purpose { get; set; }
        /// <summary>
        /// 付款方客户附言
        /// </summary>
        public string postscript { get; set; }
        /// <summary>
        /// 付款方银行附言
        /// </summary>
        public string summary { get; set; }
        /// <summary>
        /// 代发说明
        /// </summary>
        public string issuingInstructions { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string retMsg { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public string currentPage { get; set; }

        /// <summary>
        /// 查询行数
        /// </summary>
        public string pageSize { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public string totalNum { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public string totalPage { get; set; }
        /// <summary>
        /// 结果列表
        /// </summary>
        public List<EIST_Q01000407_RES_ITEM> resultList { get; set; }
    }
    public class EIST_Q01000407_RES_ITEM
    {
        /// <summary>
        /// 明细序号
        /// </summary>
        public string serNo { get; set; }
        /// <summary>
        /// 其他交易流水号
        /// </summary>
        public string othSerialNo { get; set; }
        /// <summary>
        /// 付款方凭证类型
        /// </summary>
        public string draweeVoucherType { get; set; }
        /// <summary>
        /// 付款方凭证编码
        /// </summary>
        public string draweeVoucherNo { get; set; }
        /// <summary>
        /// 付款方账号
        /// </summary>
        public string draweeAcctNo { get; set; }
        /// <summary>
        /// 付款方名称
        /// </summary>
        public string draweeAcctName { get; set; }
        /// <summary>
        /// 收款方账号
        /// </summary>
        public string payeeAcctNo { get; set; }
        /// <summary>
        /// 收款方户名
        /// </summary>
        public string payeeAcctName { get; set; }
        /// <summary>
        /// 收款行行号
        /// </summary>
        public string payeeBankNo { get; set; }
        /// <summary>
        /// 收款行行名
        /// </summary>
        public string payeeBankName { get; set; }
        /// <summary>
        /// 收款方账户类型
        /// </summary>
        public string payeeAcctType { get; set; }
        /// <summary>
        /// 收款方账户类型描述
        /// </summary>
        public string payeeAcctTypeDesc { get; set; }
        /// <summary>
        /// 汇路
        /// </summary>
        public string paymentPath { get; set; }
        /// <summary>
        /// 汇路描述
        /// </summary>
        public string paymentPathDesc { get; set; }
        /// <summary>
        /// 钞汇标志
        /// </summary>
        public string cashOrRemitFlag { get; set; }
        /// <summary>
        /// 钞汇标志描述
        /// </summary>
        public string cashOrRemitFlagDesc { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string transAmt { get; set; }
        /// <summary>
        /// 收款方摘要
        /// </summary>
        public string draweePostscript { get; set; }
        /// <summary>
        /// 收款方备注
        /// </summary>
        public string draweeSummary { get; set; }
        /// <summary>
        /// 付款方摘要
        /// </summary>
        public string payeePostscript { get; set; }
        /// <summary>
        /// 付款方备注
        /// </summary>
        public string payeeSummary { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public string retType { get; set; }
        /// <summary>
        /// 返回类型描述
        /// </summary>
        public string retTypeDesc { get; set; }
        /// <summary>
        /// 返回类型码值
        /// </summary>
        public string retCode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string retMsg { get; set; }
        /// <summary>
        /// 退汇标识
        /// </summary>
        public string refundFlag { get; set; }
        /// <summary>
        /// 退汇标识描述
        /// </summary>
        public string refundFlagDesc { get; set; }
        /// <summary>
        /// 退汇原因
        /// </summary>
        public string refundReason { get; set; }
        /// <summary>
        /// 退汇时间
        /// </summary>
        public string refundTime { get; set; }
    }
}
