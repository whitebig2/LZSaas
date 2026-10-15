using LZ.Core.EventData.Contract;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.FxqEventData
{
    public class UserEntryContractRecordEventData : EventBase
    {
        public UserEntryContractRecordEventData(int userEntryID, ContractEnumType contractType, Guid batchNo)
        {
            UserEntryID = userEntryID;
            ContractType = contractType;
            BatchNo = batchNo;
        }

        public int UserEntryID { get; set; }
        /// <summary>
        /// 合同类型
        /// </summary>
        public ContractEnumType ContractType { get; set; }
        public Guid BatchNo { get; set; }


    }
}
