using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.Enums;

namespace LZ.Model.IExcelTemplate
{
    public class InsuranceOrderNameRecordExcelTemplate
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [DisplayName("年龄")]
        public int Age { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [DisplayName("雇员工种")]
        public string JobType { get; set; }

        /// <summary>
        /// 职业类被
        /// </summary>
        [DisplayName("职业类别")]
        public int JobLevelType { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        [DisplayName("证件类型")]
        public string CertificateType { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        [DisplayName("劳动合同单位")]
        public string SigningUnitName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        [DisplayName("实际用工单位")]
        public string JobCompanyName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        [DisplayName("保障方案")]
        public string PlanName { get; set; }

        /// <summary>
        /// 工资日期
        /// </summary>
        [DisplayName("起保日期")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime InsuracneStartDate { get; set; }
        /// <summary>
        /// 工资日期
        /// </summary>
        [DisplayName("终保日期")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public DateTime InsuracneEndDate { get; set; }


        /// <summary>
        /// 工资日期
        /// </summary>
        [DisplayName("保费")]
        public decimal InsuranceAmount { get; set; }

        /// <summary>
        /// 工资日期
        /// </summary>
        [DisplayName("投保方式")]
        public string MethodTypeName { get; set; }
        public InsuranceMethodEnumType MethodType  => Enum.Parse<InsuranceMethodEnumType>(this.MethodTypeName);
    }
}
