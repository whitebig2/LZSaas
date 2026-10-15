using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.InsuranceApi.ThirtySix
{
    public class ThirtySixeQueryRequest
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FlowId { get; set; }
        public string OperatorDate { get; set; }
        public string PartnerAccountCode { get; set; }
        public string RequestType { get; set; }
    }
}
