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
    /// 个人实名认证（企业四要素）
    /// </summary>
    public class RealNameCompany4EventData : EventBase
    {
        public RealNameCompany4EventData(string name, string companyNo, string legalPerson, string legalPersonIdCardNo)
        {
            Name = name;
            CompanyNo = companyNo;
            LegalPerson = legalPerson;
            LegalPersonIdCardNo = legalPersonIdCardNo;
        }


        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证证件号码
        /// </summary>
        public string CompanyNo { get; set; }
        public string LegalPerson { get; set; }
        public string LegalPersonIdCardNo { get; set; }

        public RealNameEnumType RealNameType => RealNameEnumType.企业四要素;
    }
}
