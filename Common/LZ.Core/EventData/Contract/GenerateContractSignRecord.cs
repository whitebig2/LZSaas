using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData.Contract
{

    public record ContractSignerParty(SignerPartyEnumType SignerParty, string Name, string Idno, string SignKeyword,int? PageSignHeight,bool? SignDate);
    /// <summary>
    /// 生成网签合同签署记录
    /// </summary>
    public class GenerateContractSignRecordEventData:EventBase
    {

      
        public Guid? ContractRecordID{ get; set; }

        public GenerateContractSignRecordEventData(Guid contractRecordID)
        {
            ContractRecordID = contractRecordID;
        }

       



        /// <summary>
        /// 合同文件OssUrl
        /// </summary>
        public string  ContractOssUrl { get; set; }

        ///// <summary>
        ///// 客户单位
        ///// </summary>
        //public Guid GustomerGID { get; set; }
        public int RelateID { get; set; }
        public Guid? RelateGID { get;  set; }
        /// <summary>
        /// 签署当事人集合
        /// </summary>
        public List<ContractSignerParty> ContractSignerPartys { get;  set; }
        /// <summary>
        /// 合同类型
        /// </summary>
        public ContractEnumType ContractType { get; set; }
        /// <summary>
        /// 合同模板替换的关键词,Key只能使用英文
        /// </summary>
        public Dictionary<string,string> KeywordReplaces { get;  set; }


        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }


    }
}
