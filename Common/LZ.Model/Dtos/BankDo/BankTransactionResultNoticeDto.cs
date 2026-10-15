using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public class BankTransactionResultNoticeDto
    {
        public string  TransactionID { get; set; }
        public string  BusinessID { get; set; }
        public TransactionStatusEnumType TransStatus { get; set; }
        public string  TransMsg { get; set; }
        public DateTime?  TransTime { get; set; }

        public bool IsTransSuc { get; set; }

        public List<BankTransactionResultNoticeItemDto> Records { get; set; }
    }
    public class BankTransactionResultNoticeItemDto
    {
        public string RecordTransactionID { get; set; }
        public string RecordBusinessID { get; set; }
        public TransactionStatusEnumType RecordTransStatus { get; set; }
        public bool IsRecordTransSuc { get; set; }
        public decimal RecordTransAmount { get; set; }
        public string RecordTransMsg { get; set; }
        public DateTime? RecordTransTime { get; set; }
        public string Name { get; set; }
        public string CardID { get; set; }
    }
}
