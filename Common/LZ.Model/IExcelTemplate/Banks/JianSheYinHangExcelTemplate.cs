using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class JianSheYinHangExcelTemplate
    {
        /// <summary>
        /// 序号（必填）
        /// </summary>
        [DisplayName("序号（必填）")]
        public int OrderNo { get; set; }
        [DisplayName("账号（必填）")]
        public string BankNo { get; set; }
        /// <summary>
        /// 户名（必填）
        /// </summary>
        [DisplayName("户名（必填）")]
        public string BankUserName { get; set; }
        /// <summary>
        /// 金额（必填）
        /// </summary>
        [DisplayName("金额（必填）")]
        public decimal TransactionAmount { get; set; }
        /// <summary>
        /// 跨行标识（选填 建行填0 他行填1）
        /// </summary>
        [DisplayName("跨行标识（选填 建行填0 他行填1）")]
        public string OtherBankTag { get; set; }
        /// <summary>
        /// 行名（跨行业务与联行行号不能同时为空）
        /// </summary>
        [DisplayName("行名（跨行业务与联行行号不能同时为空）")]
        public string BankName { get; set; }
        /// <summary>
        /// 联行行号（跨行业务与行名不能同时为空）
        /// </summary>
        [DisplayName("联行行号（跨行业务与行名不能同时为空）")]
        public string BankNameNo { get; set; }
        /// <summary>
        /// 摘要（选填 显示在收款账户流水明细中）
        /// </summary>
        [DisplayName("摘要（选填 显示在收款账户流水明细中）")]
        public string Summary { get; set; }
        /// <summary>
        /// 摘要（选填 显示在收款账户流水明细中）
        /// </summary>
        [DisplayName("摘要（选填 显示在收款账户流水明细中）")]
        public string Remark { get; set; }
    }
}
