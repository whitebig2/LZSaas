using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.FxqEventData
{
    /// <summary>
    /// 电子签名事件
    /// </summary>
    public class ElectronicSignatureEventData : EventBase
    {
        public string Name { get; set; }
        public string IdCardNo { get; set; }
        public bool IsCompany { get; set; }

        public ElectronicSignatureEventData(string name, string idCardNo, bool isCompany)
        {
            Name = name;
            IdCardNo = idCardNo;
            IsCompany = isCompany;
        }
    }
}
