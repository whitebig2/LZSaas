using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class BatchSearchStatusResponse
    {

        public string Msg { get; set; }
        public int Code { get; set; }
        public BatchSearchStatusData Data { get; set; }
        
    }

    public class BatchSearchStatusData 
    {
        public string Msg { get; set; }
        public string InsureId { get; set; }
        public string IndecaseId { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public List<BatchEmployeeFailItem> FailList { get; set; }


    }
}
