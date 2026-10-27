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
    /// 个人实名认证（全网三要素）
    /// </summary>
    public class RealName3EventData:EventBase
    {
      
        public RealName3EventData(string name, string idCardNo,string phone)
        {
            Name = name;
            IdCardNo = idCardNo;
            Phone = phone;
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证证件号码
        /// </summary>
        public string IdCardNo { get; set; }
        public string Phone { get; set; }
        public RealNameEnumType RealNameType => RealNameEnumType.全网三要素;
    }
}
