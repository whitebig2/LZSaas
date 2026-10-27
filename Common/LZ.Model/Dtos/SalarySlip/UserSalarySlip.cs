using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.SalarySlip
{
    public class UserSalarySlip
    {
        public Guid ID { get; set; }
        public DateTime SalaryMonth { get; set; }
        public int SalaryBatchType { get; set; }
        public string CustomerName { get; set; }
        public List<UserSalarySlipItem> Records { get; set; }
    }
}
