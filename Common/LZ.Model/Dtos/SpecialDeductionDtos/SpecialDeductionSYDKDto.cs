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
    /// 住房贷款利息支出-商业贷款信息
    /// </summary>
    public class SpecialDeductionSYDKDto
    {
        [DisplayName("纳税人姓名")]
        public string Name { get; set; }
        [DisplayName("纳税人证件号码")]
        public string IdCadNo { get; set; }
        [DisplayName("本人扣除比例")]
        public string DeductionPercentage { get; set; } = "100%";
        [DisplayName("商业贷款信息|扣除有效期起")]
        public DateTime StartMonth { get; set; }
        [DisplayName("商业贷款信息|扣除有效期止")]
        public DateTime EndMonth { get; set; }
    }


}
