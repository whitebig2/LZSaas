using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.BankDo
{
    public class BankCompanyDto
    {

        public Guid ID { get; set; }


        public string Name { get; set; }


        public string CompanyNo { get; set; }

    
        public bool IsEnable { get; set; }

    }
}
