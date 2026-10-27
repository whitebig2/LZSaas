using LZ.Core.RabbitMQ.EventBus;
using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.EventData
{
    public class TestEventData:EventBase
    {

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string Idno { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCardNo { get; set; }
        /// <summary>
        /// 实名认证方式
        /// </summary>
        public RealNameEnumType RealNameType { get; set; }
        /// <summary>
        /// 实名认证方式
        /// </summary>
        public bool IsCompany { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNo { get; set; }
        /// <summary>
        /// 企业法人
        /// </summary>
        public string LegalPerson { get; set; }
        /// <summary>
        /// 企业法人身份证号
        /// </summary>
        public string CertNo { get; set; }
    }
}
