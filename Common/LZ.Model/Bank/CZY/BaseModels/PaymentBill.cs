using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BaseModels
{
    /// <summary>
    /// 关联应付单据信息
    /// </summary>
    public class PaymentBill
    {
        /// <summary>
        /// 外部系统单据编号
        /// </summary>
        public string BillsPayOrderNo { get; set; }
    }
}
