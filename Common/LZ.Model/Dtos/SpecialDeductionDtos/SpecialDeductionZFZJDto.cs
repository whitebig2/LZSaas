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
    public class SpecialDeductionZFZJDto
    {
        [DisplayName("纳税人姓名")]
        public string Name { get; set; }
        [DisplayName("纳税人证件号码")]
        public string IdCadNo { get; set; }
        [DisplayName("工作城市")]
        public string WorkCity { get; set; }
        [DisplayName("扣除有效期起")]
        public DateTime StartMonth { get; set; }
        [DisplayName("扣除有效期止")]
        public DateTime EndMonth { get; set; }
    }
}
