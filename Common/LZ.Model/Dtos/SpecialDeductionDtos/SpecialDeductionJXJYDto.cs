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
    /// 继续教育-学历学位
    /// </summary>
    public class SpecialDeductionJXJYDto
    {
        [DisplayName("纳税人姓名")]
        public string Name { get; set; }
        [DisplayName("纳税人证件号码")]
        public string IdCadNo { get; set; }
        [DisplayName("学历（学位）继续教育情况|扣除有效期起")]
        public DateTime StartMonth { get; set; }
        [DisplayName("学历（学位）继续教育情况|扣除有效期止")]
        public DateTime EndMonth { get; set; }
    }


}
