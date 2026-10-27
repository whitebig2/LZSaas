using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Plkh
{
    public class PlkhExcelRecord
    {
        /// <summary>
        /// 传送日期（必填）
        /// 长度：
        /// YYYYMMDD
        /// </summary>
        [DisplayName("SENDATE")]
        public string SENDATE { get; set; }
        /// <summary>
        /// 开户地区号（必填）
        /// 长度：5
        /// </summary>
        [DisplayName("OPENZONO")]
        public string OPENZONO { get; set; }
        /// <summary>
        /// 开户网点号（必填）
        /// 长度：5
        /// </summary>
        [DisplayName("OPENBRNO")]
        public string OPENBRNO { get; set; }
        /// <summary>
        /// 个人中文姓名（必填）
        /// 长度：30
        /// </summary>
        [DisplayName("CHNSNAME")]
        public string CHNSNAME { get; set; }
        /// <summary>
        /// 出生日期（必填）
        /// 长度：10
        /// YYYY-MM-DD
        /// </summary>
        [DisplayName("BIRTHDATE")]
        public string BIRTHDATE { get; set; }
        /// <summary>
        /// 性别（必填）
        /// 长度：4
        /// </summary>
        [DisplayName("SEX")]
        public string SEX { get; set; }
        /// <summary>
        /// 个人证件类型（必填）
        /// 长度：4
        /// 0：身份证/1：护照/2：军官证/3：士兵证/4：港澳台通行证/5：临时身份证/6：户口本/7：其它/9：警官证/12：外国人永久居留证/21：边民通行证
        /// </summary>
        [DisplayName("LGLDOCTP")]
        public string LGLDOCTP { get; set; }
        /// <summary>
        /// 个人证件号码（必填）
        /// 长度：18
        /// 其中下面的证件号码中允许输入中文、大写英文、数字：002-军官证、003-士兵证、007-其他、009-警官证；其他类型的证件号码中不允许包含汉字，只允许大写英文、数字
        /// </summary>
        [DisplayName("LGLDOCNO")]
        public string LGLDOCNO { get; set; }
        /// <summary>
        /// 证件有效期截止日
        /// YYYY-MM-DD
        /// 长度：10
        /// </summary>
        [DisplayName("LGEPIRD")]
        public string LGEPIRD { get; set; }
        /// <summary>
        /// 个人地址
        /// 长度：50
        /// </summary>
        [DisplayName("ADDRESS")]
        public string ADDRESS { get; set; }
        /// <summary>
        /// 联系电话
        /// 长度：22
        /// </summary>
        [DisplayName("PHONE1")]
        public string PHONE1 { get; set; }
        /// <summary>
        /// /证件起始日（必填）
        /// 长度：10
        ///  YYYY-MM-DD
        /// </summary>
        [DisplayName("NOTES4")]
        public string NOTES4 { get; set; }
    }
}
