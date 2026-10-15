using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.Wallet
{
    public class OpenAcctHttpContext
    {
        /// <summary>
        /// 商户号（必填）
        /// </summary>
        public string Api { get; set; }
        /// <summary>
        /// 商户号（必填）
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 时间戳（必填）
        /// </summary>
        public string openacctStatus { get; set; }
        /// <summary>
        /// 接入系统编号（必填）
        /// </summary>
        public string mobileNo { get; set; }
        /// <summary>
        /// 接入系统名称（必填）
        /// </summary>
        public string openAcctErrorCode { get; set; }
        /// <summary>
        /// 报文请求号（必填）
        /// </summary>
        public string openAcctErrorMsg { get; set; }

        public string mediumId { get; set; }

        public string mediumIdProStatus { get; set; }

        public string mediumIdProId { get; set; }
    }

    public class OpenAcctModel
    {
        /// <summary>
        /// 商户号（必填）
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 商户号（必填）
        /// </summary>
        public string OutUserId { get; set; }
        /// <summary>
        /// 时间戳（必填）
        /// </summary>
        public string OpenacctStatus { get; set; }
        /// <summary>
        /// 接入系统编号（必填）
        /// </summary>
        public string MobileNo { get; set; }
        /// <summary>
        /// 接入系统名称（必填）
        /// </summary>
        public string OpenAcctErrorCode { get; set; }
        /// <summary>
        /// 报文请求号（必填）
        /// </summary>
        public string OpenAcctErrorMsg { get; set; }

        public string MediumId { get; set; }

        public string MediumIdProStatus { get; set; }

        public string MediumIdProId { get; set; }
    }

}
