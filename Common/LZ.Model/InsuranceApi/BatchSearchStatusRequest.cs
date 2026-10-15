using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class BatchSearchStatusRequest
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string InsureId { get; set; }
        /// <summary>
        /// 增减员序号
        /// </summary>
        public string Indecaseid { get; set; }
        /// <summary>
        /// apply：申请投保
        /// addemp：增员
        /// delemp：减员
        /// </summary>
        public string Type { get; set; }
    }
}
