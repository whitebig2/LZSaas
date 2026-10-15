using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class BatchTransactionRequest
    {

        /// 客户单位名称
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 签章单位名称
        /// </summary>
        public string SigningUnitName { get; set; }
        public string Postscript { get; set; }
        public Guid BusinessID { get; set; }
        public List<BatchTransactionRecord> Records { get; set; }
    }
    public class BatchTransactionRecord
    {
        public string Name { get; set; }
        public string IdCardNo { get; set; }
        public string Bank { get; set; }
        public string BankNo { get; set; }
        public string BankUserName { get; set; }
        public decimal TransAmt { get; set; }
        public Guid BusinessID { get; set; }
    }
}
