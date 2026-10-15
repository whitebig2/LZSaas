using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    /// <summary>
    /// 实名认证
    /// 个人实名认证（银行卡四要素）
    /// </summary>
    public class RealNameBank4EventData : EventBase
    {
      
      

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证证件号码
        /// </summary>
        public string IdCardNo { get; set; }
        public string BankNo { get; set; }
        public string Phone { get; set; }
        public RealNameEnumType RealNameType => RealNameEnumType.银行卡四要素;

        public RealNameBank4EventData(string name, string idCardNo, string bankNo, string phone)
        {
            Name = name;
            IdCardNo = idCardNo;
            BankNo = bankNo;
            Phone = phone;
        }
    }
}
