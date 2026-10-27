using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Payroll
{
    public class PayrollApplyResult<T>
    {
        /// <summary>
        /// 交易码（必填）
        /// 返回通用输入信息对应值
        /// </summary>
        public string TransCode { get; set; }

        /// <summary>
        /// 请求方流水号（必填）
        /// 返回通用输入信息对应值
        /// </summary>
        public string ReqSeq { get; set; }
        /// <summary>
        /// 响应方流水号（必填）
        /// 业务受理方流水号，可以不记录
        /// </summary>
        public string RespSeq { get; set; }
        /// <summary>
        /// 响应时间（必填）
        /// yyyyMMddHHmmss
        /// </summary>
        public string RespTime { get; set; }
        /// <summary>
        /// 响应结果状态（必填）
        /// 响应结果状态（S:成功，E:失败）
        /// </summary>
        public string RespStatus { get; set; }
        /// <summary>
        /// 响应错误码
        /// </summary>
        public string RespCode { get; set; }
        /// <summary>
        /// 响应错误信息
        /// </summary>
        public string RespDesc { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        public string Reserve { get; set; }

        /// <summary>
        /// 交易报文
        /// 对应交易报文部分，包含各个交易专用报文
        /// </summary>
        public string Body { get; set; }
    }
}
