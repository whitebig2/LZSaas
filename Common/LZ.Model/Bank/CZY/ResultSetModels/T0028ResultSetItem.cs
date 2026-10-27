using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{
    public class T0028ResultSetItem : ICzyResultSetItem
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件内容（Base64编码）
        /// </summary>
        public string FileContent { get; set; }

        /// <summary>
        /// 财资云系统内部流水号
        /// </summary>
        public string ReceiptInnerNo { get; set; }

        /// <summary>
        /// 本次查询批次号
        /// </summary>
        public string ReceiptBatchNo { get; set; }

        /// <summary>
        /// 外部系统应付单编号
        /// </summary>
        public string BillsPayOrderNo { get; set; }

        /// <summary>
        /// 财资云系统付款单编号
        /// </summary>
        public string RelBillsCode { get; set; }
    }
}
