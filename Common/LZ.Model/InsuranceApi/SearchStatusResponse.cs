using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class SearchStatusResponse:EmpapiResponse
    {

        public SearchStatusData Data { get; set; }

    }
    public class SearchStatusData
    {
        public string Enddate { get; set; }
        public string Insureid { get; set; }
        public string Startdate { get; set; }
        public string Indecaseid { get; set; }
    }

}
