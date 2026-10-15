using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.Banks
{
    public class YouZhengYinHangExcelTemplate
    {
        /// <summary>
        /// 签约转好（非必输）
        /// </summary>
        [DisplayName("签约专号\r\n（非必输）")]        
        public string SignNo { get; set; }
        /// <summary>
        /// 证件类型（非必输）
        /// </summary>
        [DisplayName("证件类型\r\n（非必输）")]
        public string IdType { get; set; }
        /// <summary>
        /// 证件号码（非必输）
        /// </summary>
        [DisplayName("证件号码\r\n（非必输）")]
        public string IdCardNo { get; set; }
        /// <summary>
        /// 是否为邮储银行账户（必输）
        /// </summary>
        [DisplayName("是否为邮储银行账户\r\n（必输）")]
        public string IsYouZhengBank { get; set; }

        /// <summary>
        /// 收款方账号（必输）
        /// </summary>
        [DisplayName("收款方账号\r\n（必输）")]
        public string BankNo { get; set; }

        /// <summary>
        /// 收款方户名（必输）
        /// </summary>
        [DisplayName("收款方户名\r\n（必输）")]
        public string BankUserName { get; set; }

        /// <summary>
        /// 金额（必输）
        /// </summary>
        [DisplayName("金额\r\n（必输）")]
        public decimal TransactionAmount { get; set; }


        /// <summary>
        /// 附言/备注（非必输）
        /// </summary>
        [DisplayName("附言/备注\r\n（非必输）")]
        public string Remark { get; set; }

        /// <summary>
        /// 行号（非邮储银行账户必输）
        /// </summary>
        [DisplayName("行号\r\n（非邮储银行账户必输）")]
        public string BankNameNo { get; set; }
    }
}
