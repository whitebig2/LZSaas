using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.InsuranceApi.ThirtySix
{
    
        /// <summary>
        /// 投保请求根对象
        /// </summary>
        public class ThirtySixAddRequest
        {
            /// <summary>
            /// 客户ID
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 用户ID
            /// </summary>
            public string UserId { get; set; }

            /// <summary>
            /// 流程ID - 投保雇员的
            /// </summary>
            public string FlowId { get; set; }

            /// <summary>
            /// 总保费（不含服务费）
            /// </summary>
            public string Premium { get; set; }

            /// <summary>
            /// 付款方式：1-扫码，2-公户
            /// </summary>
            public string PaymentType { get; set; }

            /// <summary>
            /// 操作时间，格式：yyyy-MM-dd hh:mm:ss
            /// </summary>
            public string OperatorDate { get; set; }

            /// <summary>
            /// 代理编码
            /// </summary>
            public string PartnerAccountCode { get; set; }

            /// <summary>
            /// 请求类型，例如：COMMON_SUBMIT_CUSTOMER
            /// </summary>
            public string RequestType { get; set; }

            /// <summary>
            /// 投保人员信息列表
            /// </summary>
            public List<ThirtySixAddRecord> RequestJson { get; set; }

        }

      
}
