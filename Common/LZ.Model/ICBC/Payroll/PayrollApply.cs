using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Payroll
{
    public class PayrollApply<T> where T : IPayrollApplyBody
    {
        /// <summary>
        /// 交易码（必填）
        /// 根据不通交易上送指定的交易码
        /// 交易码	描述
        ///A00001 合作方代发申请
        ///A00002 合作方代发结果查询
        ///A00003 合作方上送员工信息的新增
        ///A00004 合作方上送员工信息的修改

        /// </summary>
        public string TransCode { get; set; }
        /// <summary>
        /// 应用代码（必填）
        /// 用于平台登记的应用代码
        /// </summary>
        public string AppCode { get; set; }
        /// <summary>
        /// 合作方代码（必填）
        /// 用于平台登记的合作方代码
        /// </summary>
        public string PartnerCode { get; set; }
        /// <summary>
        /// 请求方流水号（必填）
        /// 要求全程唯一，规则为：应用代码 + 14交易时间（yyyyMMddHHmmss）+3位随机数
        /// </summary>
        public string ReqSeq { get; set; }
        /// <summary>
        /// 交易发起时间戳（必填）
        /// 定长，交易发起时时间戳，Timestamp格式yyyyMMddHHmmssSSS
        /// </summary>
        public string ReqTime { get; set; }
        /// <summary>
        /// 发送方交易日期（必填）
        /// 定长，交易发起日期，Date格式yyyyMMdd
        /// </summary>
        public string TransDate { get; set; }
        /// <summary>
        /// 发送方交易时间（必填）
        /// 定长，交易发起日期，格式HHmmssSSS
        /// </summary>
        public string TransTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        public string Reserve { get; set; }
        /// <summary>
        /// json（必填）
        /// 对应交易报文部分，包含各个交易专用报文
        /// </summary>
        public T Body { get; set; }
    }
}
