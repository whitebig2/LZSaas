using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.InsuranceApi.ThirtySix
{


 
        /// <summary>
        /// 客户替换请求根对象
        /// </summary>
        public class ThirtySixReplaceRequest
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
            /// 流程ID
            /// </summary>
            public string FlowId { get; set; }

            /// <summary>
            /// 操作时间，格式：yyyy-MM-dd
            /// </summary>
            public string OperatorDate { get; set; }

            /// <summary>
            /// 代理编码
            /// </summary>
            public string PartnerAccountCode { get; set; }

            /// <summary>
            /// 请求类型，例如：COMMON_REPLACE_CUSTOMER
            /// </summary>
            public string RequestType { get; set; }

            /// <summary>
            /// 替换人员信息列表
            /// </summary>
            public List<ThirtySixReplaceRecord> RequestJson { get; set; }
        }

     

}
