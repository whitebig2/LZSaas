using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class SearchStatusRequest
    {
        /// <summary>
        /// 订单ID(string) 主订单号(必填)
        /// </summary>
        public string InsureId { get; set; }
        /// <summary>
        /// 增减员订单号
        /// </summary>
        public string Indecaseid { get; set; }
        /// <summary>
        /// addemp：增员
        /// delemp：减员
        /// </summary>
        public string Type { get; set; }
    }
}
