using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate.profit
{
    public class ProfitBusInsuranceChangeExcelDto
    {
        [DisplayName("申请日期")]
        public string ApplyDate { get; set; }

        [DisplayName("增员")]
        public int AddCount { get; set; }
        [DisplayName("减员")]
        public int DelCount { get; set; }
        [DisplayName("批改人数")]
        public int ChangeCount => this.AddCount - this.DelCount;
        [DisplayName("批改费")]
        public decimal ChangeAmount { get; set; }
    }
}
