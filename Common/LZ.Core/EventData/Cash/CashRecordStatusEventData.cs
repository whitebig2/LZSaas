using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Cash
{
    public class CashRecordStatusEventData : EventBase
    {
        public Guid SalaryCashSendRecordID { get; set; }

        public CashSendStatusEnumType Status { get; set; }
        public string Msg { get; set; }
        public string FilePath { get; set; }
        public string SignerOssUrl { get; set; }
        public string SignKeyword { get; set; }

    }
}
