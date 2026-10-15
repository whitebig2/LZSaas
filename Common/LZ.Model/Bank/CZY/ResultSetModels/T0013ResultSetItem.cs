using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BusinessModels
{
    public class T0013ResultSetItem : ICzyResultSetItem
    {
        public string FileName { get; set; }
        public string ReceiptInnerNo { get; set; }
        public string ReceiptBatchNo { get; set; }
    }
}
