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
    /// 子女教育
    /// </summary>
    public class SpecialDeductionZNJYDto
    {
        [DisplayName("纳税人姓名")]
        public string Name { get; set; }
        [DisplayName("纳税人证件号码")]
        public string IdCadNo { get; set; }

        [DisplayName("本人扣除比例")]
        public string DeductionPercentage { get; set; } = "100%";
        [DisplayName("关系")]
        public string Relationship { get; set; }
        [DisplayName("子女姓名")]
        public string RelationName { get; set; }
        [DisplayName("子女证件号码")]
        public string RelationIdCardNo { get; set; }
        public DeductionEnumType DataType { get; set; } = DeductionEnumType.子女教育支出;
        [DisplayName("扣除有效期起")]
        public DateTime StartMonth { get; set; }
        [DisplayName("扣除有效期止")]
        public DateTime EndMonth { get; set; }
    }
}
