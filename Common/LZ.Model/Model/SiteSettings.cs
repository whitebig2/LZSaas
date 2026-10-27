using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class SiteSettings
    {
        public string AliAccessID { get; set; } = "";
		public string AliSmsAccessID { get; set; } = "";
		public string AliSmsAccessSecret { get; set; } = "";
		public string AliAccessSecret { get; set; } = "";
        public string AliSmsSignName { get; set; } = "";
        public string AliSmsVercode { get; set; } = "";
        public string AliSmsVerifyTempCode { get; set; } = "";
        /// <summary>
        /// Oss的目录
        /// </summary>
        public string AliBucketName { get; set; } = "";
        /// <summary>
        /// Oss域名
        /// </summary>
        public string AliBucketDomain { get; set; } = "";
        /// <summary>
        /// 阿里云市场的AppCode
        /// </summary>
        public string AliAppCode { get; set; } = "";
        public string IMServicesUrl { get; set; } = "";
        public string StaticUrl { get; set; } = "";
        public string ApiServicesUrl { get; set; } = "";
        /// <summary>
        /// 小程序发布路径
        /// </summary>
        public string MiniReleasePath { get; set; } = "";
        /// <summary>
        /// 小程序上传说明
        /// </summary>
        public string MiniReleaseApiUrl { get; set; } = "";
        public string MiniReleaseDesc { get; set; } = "";
        /// <summary>
        /// 手机网页地址
        /// </summary>
        public string MobileUrl { get; set; } = "";
        /// <summary>
        /// 总访问量
        /// </summary>
        public int TotalVisits { get; set; } = 0;
        /// <summary>
        /// 日访问量
        /// </summary>
        public int DayTotalVisits { get; set; } = 0;
        /// <summary>
        /// 默认人力公司
        /// </summary>
        public int DefaultCompanyID { get; set; } = 0;
        /// <summary>
        /// 默认人力公司
        /// </summary>
        public string DefaultCompanyGID { get; set; }
        /// <summary>
        /// 微支付证书存放路径
        /// </summary>
        public string WeiXinPayCertPath { get; set; }
        /// <summary>
        /// 是否生产环境
        /// </summary>
        public bool IsProduction { get; set; }
        /// <summary>
        /// 接口token
        /// </summary>
        public string ApiToken { get; set; }
    }
}
