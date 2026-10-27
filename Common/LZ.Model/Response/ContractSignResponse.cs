using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Response
{
    public class ContractRecordSignResponse
    {
        public string KeyID { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public string Phone { get; set; }
        public string IdCardNo { get; set; }
        public string CustomerName { get; set; }
        public string SignUnitName { get; set; }
        public string EntryDate { get; set; }
        public string LeaveDate { get; set; }
        public string SmUrl { get; set; }
        public string SmBase64 { get; set; }
        public string HtBase64 { get; set; }
        public string HtUrl { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string StatusName { get; set; }
        public string ContractTypeName { get; set; }
    }
}
