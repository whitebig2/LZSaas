using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class BatchAddEmployeeResponse
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public int FailNum { get; set; }
        public int SuccessNum { get; set; }
        public string InsureId { get; set; }
        public string IndecaseId { get; set; }
        public List<BatchEmployeeFailItem> FailList { get; set; }
    }



}
