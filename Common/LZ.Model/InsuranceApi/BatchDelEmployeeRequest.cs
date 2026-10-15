using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class BatchDelEmployeeRequest
    {
        public string InsureId { get; set; }
        public List<DelCustomerinfo> CustomerList { get; set; }
    }
}
