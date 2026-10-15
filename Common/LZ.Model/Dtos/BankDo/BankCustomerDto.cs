using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.BankDo
{
    public class BankCustomerDto
    {
        public Guid ID { get; set; }
        public string CustomerName { get; set; }


        public string CustomerNo { get; set; }

	
        public string SignUnitName { get; set; }
	
        public string SignUnitNo { get; set; }

	
        public bool IsEnable { get; set; }
  
    }
}
