using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.Banks
{
    public class ZhaoShangYinHangExcelTemplate
    {
        /// <summary>
        /// 账号
        /// </summary>
        [DisplayName("账号")]
        public string BankNo { get; set; }

        /// <summary>
        /// 户名
        /// </summary>
        [DisplayName("户名")]
        public string BankUserName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [DisplayName("金额")]

        public decimal TransactionAmount { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        [DisplayName("开户行")]

        public string BankName { get; set; }
        /// <summary>
        /// 开户地址
        /// </summary>
        [DisplayName("开户地")]

        public string BankAddreess { get; set; }
        /// <summary>
        /// 汇款备注
        /// </summary>
        [DisplayName("汇款备注")]
        public string Remark { get; set; }
    }
}
