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
    /// 个人实名认证（二要素认证）
    /// </summary>
    public class RealName2EventData:EventBase
    {
      
        public RealName2EventData(string name, string idCardNo)
        {
            Name = name;
            IdCardNo = idCardNo;
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证证件号码
        /// </summary>
        public string IdCardNo { get; set; }
        public RealNameEnumType RealNameType => RealNameEnumType.公安二要素;
    }
}
