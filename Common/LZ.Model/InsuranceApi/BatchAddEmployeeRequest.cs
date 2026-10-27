using LZ.Model.InsuranceApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class BatchAddEmployeeRequest
    {
        public string InsureId { get; set; }
        public int GnteeId { get; set; }
        public string GnteeName { get; set; }
        public List<AddCustomerinfo> CustomerList { get; set; }
    }
}
