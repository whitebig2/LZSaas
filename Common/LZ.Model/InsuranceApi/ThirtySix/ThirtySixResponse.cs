using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.InsuranceApi.ThirtySix
{


    /// <summary>
    /// API 通用响应对象
    /// </summary>
    public class ThirtySixResponse<T>

    {
        /// <summary>
        /// 响应状态码，0 通常表示成功
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 响应消息，例如 "ok"
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误消息详情
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 路径信息
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 额外信息
        /// </summary>
        public object Extra { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 数据列表，包含客户/被保险人详细信息
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 客户/被保险人详细信息对象
    /// </summary>
    public class ThirtySixResponseRecord
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 错误信息，若无错误则为空
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 被保险人姓名
        /// </summary>
        public string InsuredName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 实际用工单位
        /// </summary>
        public string ActualInsuredCustomers { get; set; }

        /// <summary>
        /// 保单号或投保人信息
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// 保证人/担保人信息 (Assured)
        /// </summary>
        public string Assured { get; set; }

        /// <summary>
        /// 被保险人 (The Insured)，可能与 InsuredName 重复或指代不同主体
        /// </summary>
        public string TheInsured { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string PlanInfoName { get; set; }

        /// <summary>
        /// 工种/职业
        /// </summary>
        public string Profession { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public string EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期/到期日期
        /// </summary>
        public string ExpirationDate { get; set; }

        /// <summary>
        /// 是否参保工伤保险：0-否，1-是
        /// </summary>
        public string HasSocial { get; set; }

        /// <summary>
        /// 是否涉高作业：0-否，1-是
        /// </summary>
        public string RelatedHeight { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 离职/被替换人员的姓名（用于批增批减或替换场景）
        /// </summary>
        public string AttritionInsuredName { get; set; }

        /// <summary>
        /// 离职/被替换人员的证件号（用于批增批减或替换场景）
        /// </summary>
        public string AttritionIdNumber { get; set; }

        /// <summary>
        /// 核保结论或核保信息
        /// </summary>
        public int Underwriting { get; set; }
    }




    public class ThirtySixCusResponse
    {



        /// <summary>
        /// 响应状态码，0 通常表示成功
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 响应消息，例如 "ok"
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误消息详情
        /// </summary>
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 路径信息
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 额外信息
        /// </summary>
        public object Extra { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }


        public List<ThirtySixResponseRecord> Records { get; set; }
        public string ConfirmUrl { get; set; }
    }

}
