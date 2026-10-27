using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.BankDo
{
    public class BankUserCompanyReqDto
    {

        public Guid BankUserID { get; set; }
        public List<Guid> CompanyIds { get; set; }
    }
}
