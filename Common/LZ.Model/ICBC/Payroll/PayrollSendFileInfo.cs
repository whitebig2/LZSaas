using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Payroll
{
    public class PayrollSendFileInfo
    {
        /// <summary>
        /// 交易总笔数（必填）
        /// </summary>
        public string total_qt { get; set; }
        /// <summary>
        /// 交易总金额（必填）
        /// </summary>
        public string total_at { get; set; }
        /// <summary>
        /// 批次号（必填）
        /// </summary>
        public string batch_series_no { get; set; }
    }

    public class PayrollSendFileInfoItem
    {
        /// <summary>
        /// 序号
        /// 明细序号，唯一标识（可用来对应返回的明细记录查询）
        /// </summary>
        public string batch_tran_sq { get; set; }
        /// <summary>
        /// 超网行号（必填）
        /// </summary>
        public string super_acc_no { get; set; }
        /// <summary>
        /// 超网行名（必填）
        /// </summary>
        public string super_acc_name { get; set; }
        /// <summary>
        /// 帐号/卡号
        /// 输入帐号为：18 位长度的 折，19 位长的卡，信用卡 16 位，公司的正常帐号外 23 位虚拟帐号。（只允许 输入此四种长度的帐号/卡 号）
        /// </summary>
        public string acc_id { get; set; }
        /// <summary>
        /// 交易金额
        /// 单位为分
        /// </summary>
        public string tran_at { get; set; }
        /// <summary>
        /// 账户户名
        /// 描述账户所有者的姓名
        /// </summary>
        public string acc_name { get; set; }
        /// <summary>
        /// 摘要
        /// 最长 4 位，缺省为空
        /// </summary>
        public string summary { get; set; }
        /// <summary>
        /// 其他信息
        /// 备注域，最大长度为 1024, 禁止有分隔符‘｜ ’。
        /// </summary>
        public string oth_msg4_tx { get; set; }
    }
}
