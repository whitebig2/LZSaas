using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.InsuranceApi.ThirtySix
{
    /// <summary>
    /// 替换人员详细信息
    /// </summary>
    public class ThirtySixReplaceRecord
    {
        /// <summary>
        /// 新被保险人姓名
        /// </summary>
        public string InsuredName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 实际用工单位
        /// </summary>
        public string ActualInsuredCustomers { get; set; }

        /// <summary>
        /// 保单号
        /// </summary>
        public string PolicyNumber { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string PlanInfoName { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        public string Profession { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public string EffectiveDate { get; set; }

        /// <summary>
        /// 是否参保工伤保险：0-否，1-是（或其他约定值）
        /// </summary>
        public string HasSocialS { get; set; }

        /// <summary>
        /// 是否涉高：0-否，1-是（或其他约定值）
        /// </summary>
        public string RelatedHeightS { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 离职/被替换被保险人姓名
        /// </summary>
        public string AttritionInsuredName { get; set; }

        /// <summary>
        /// 离职/被替换被保险人证件号
        /// </summary>
        public string AttritionIdNumber { get; set; }
    }
}
