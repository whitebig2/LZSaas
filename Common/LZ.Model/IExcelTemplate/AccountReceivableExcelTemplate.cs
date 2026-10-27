using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class AccountReceivableExcelTemplate
    {
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        [DisplayName("应收账款金额")]
        public decimal TotalAmount { get; set; }

        [DisplayName("备注")]
        public string Description { get; set; }
    }
}
