using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
   public class AddEmpResponse:EmpapiResponse
    {
        /// <summary>
        /// 主订单号
        /// </summary>
        public string InsureId { get; set; }
        /// <summary>
        /// 增员序号
        /// </summary>
        public string IndecaseId { get; set; }
        /// <summary>
        /// 增员人员姓名
        /// </summary>
        public string Custemername { get; set; }
        /// <summary>
        /// 增员人员身份证
        /// </summary>
        public string Custemernum { get; set; }
    }
}
