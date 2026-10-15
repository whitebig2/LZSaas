using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class DelEmpResponse:EmpapiResponse
    {
        /// <summary>
        /// 主订单号
        /// </summary>
        public string insureId { get; set; }
        /// <summary>
        /// 减员序号
        /// </summary>
        public string indecaseId { get; set; }
        /// <summary>
        /// 减员人员姓名
        /// </summary>
        public string custemername { get; set; }
        /// <summary>
        ///  减员人员身份证
        /// </summary>
        public string custemernum { get; set; }
         
 
 

    }
}
