using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.SpecialDeductionDtos
{
    /// <summary>
    /// 赡养老人
    /// </summary>
    public class SpecialDeductionSYLRDto
    {
        [DisplayName("纳税人姓名")]
        public string Name { get; set; }
        [DisplayName("纳税人证件号码")]
        public string IdCadNo { get; set; }

        [DisplayName("本人扣除比例")]
        public string DeductionPercentage { get; set; } = "100%";
        [DisplayName("被赡养人信息|关系")]
        public string Relationship { get; set; }
        [DisplayName("被赡养人信息|姓名")]
        public string RelationName { get; set; }
        [DisplayName("被赡养人信息|证件号码")]
        public string RelationIdCardNo { get; set; }
        [DisplayName("本年度月扣除金额")]
        public decimal BaseAmount { get; set; }
        public DeductionEnumType DataType { get; set; } = DeductionEnumType.赡养老人支出;
        [DisplayName("被赡养人信息|扣除有效期起")]
        public DateTime StartMonth { get; set; }
        [DisplayName("被赡养人信息|扣除有效期止")]
        public DateTime EndMonth { get; set; }
    }
}
