using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Contract
{
    public class ContractSigningEventData:EventBase
    {
        public Guid ContractRecordID { get; set; }
        public Guid BatchNo { get; set; }
        /// <summary>
        /// 签署人姓名
        /// </summary>
        public string  SignerName { get; set; }
        /// <summary>
        /// 签署人验证码
        /// </summary>
        public string SignerSmsCode { get; set; }

        public ContractSigningEventData(Guid batchNo, string signerSmsCode)
        {
            BatchNo = batchNo;
            SignerSmsCode = signerSmsCode;
        }
    }
}
