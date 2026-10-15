using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class DelEmpRequest
    {
        /// <summary>
        /// 证件号(string) 减员人员的证件号(必填)
        /// </summary>
        public string Custemernum { get; set; }
        /// <summary>
        ///  姓名(string) 减员人员姓名(必填)
        /// </summary>
        public string Custemername { get; set; }
        /// <summary>
        /// 订单ID(string) 减员操作对应的订单编号(必填)
        /// </summary>
        public string InsureId { get; set; }
    }
}
