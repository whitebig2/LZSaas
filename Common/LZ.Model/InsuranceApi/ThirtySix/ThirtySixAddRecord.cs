using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.InsuranceApi.ThirtySix
{

    public class ThirtySixAddRecord
    {
        /// <summary>
        /// 姓名
        /// 如果是替换操作，此处通常指“新被保险人”姓名；如果是纯减员，指“原被保险人”姓名。
        /// 需结合接口整体定义确认是输入新人的信息还是旧人的信息。
        /// </summary>
        public string InsuredName { get; set; }

        /// <summary>
        /// 证件类型
        /// 枚举值示例：身份证、护照、港澳居民来往内地通行证、台湾居民来往大陆通行证
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// 证件号
        /// 用于唯一标识被保险人
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 实际用工单位
        /// 若存在劳务派遣或多主体投保，需指定具体用工单位
        /// </summary>
        public string ActualInsuredCustomers { get; set; }

        /// <summary>
        /// 保单号
        /// 关键索引字段，指定操作所属的保单
        /// </summary>
        public string PolicyNumber { get; set; }

        /// <summary>
        /// 方案名称
        /// 业务规则：
        /// 1. 不填写时，默认读取原减员方案或系统默认方案。
        /// 2. 在替换场景中，若新旧方案价格相同（如同属A/B/C类同价方案），可不填以简化操作。
        /// 3. 若价格不同或风险类别变更，建议明确填写。
        /// </summary>
        public string PlanInfoName { get; set; }

        /// <summary>
        /// 工种/职业
        /// 影响费率的关键因素，需确保与方案匹配
        /// </summary>
        public string Profession { get; set; }

        /// <summary>
        /// 生效日期
        /// 格式通常为 yyyy-MM-dd。
        /// 减员通常即时生效或指定日期生效；替换通常无缝衔接。
        /// </summary>
        public string EffectiveDate { get; set; }

        /// <summary>
        /// 是否参保工伤保险
        /// 0: 否
        /// 1: 是
        /// </summary>
        public string HasSocialS { get; set; }
        /// <summary>
        /// 是否涉高作业
        /// 0: 否
        /// 1: 是
        /// 涉高作业通常属于高风险工种，可能触发额外核保或保费调整
        /// </summary>
        public string RelatedHeightS { get; set; }

        /// <summary>
        /// 备注
        /// 可选填，用于记录特殊操作原因或说明
        /// </summary>
        public string Remark { get; set; }
    }

  
}
