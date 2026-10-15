using LZ.Model.Bank.CZY.BodyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.NoticeModels
{
    public class TransactionNoticeRequest
    {
        public required CzyCommonBody<TransactionNotice> Body { get; set; }
        public required CzyCommonHead Head { get; set; }
    }
}
