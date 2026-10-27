using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Payroll
{
    public class PayrollQueryDetailResultBody: IPayrollResultBody
    {
        /// <summary>
        /// 返回码类型
        /// 返回码类型，表示服务的执行状态，取值范围如下： E –错误    S –成功
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 业务编码
        /// 唯一表示一种确定业务状态的8位数字编码，由服务方定义，提供给调用方使用
        ///（UNP30103：批次号不允许为空
        /// UNP30104：上送的批次号查询数据为空
        /// UNP30105：该批次数据在代扣中，请稍后再试
        /// UNP33005：合作方代发结果查询成功
        /// UNP00001：访问服务系统异常 ）
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 业务信息
        /// 客户易于识别的文字信息，调用方颗直接显示给客户，也可以根据code 自行组织提示信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 文件名
        /// 暂不使用
        /// </summary>
        public string OutFileName { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchId { get; set; }
        /// <summary>
        /// 交易总笔数
        /// </summary>
        public string TotalQt { get; set; }
        /// <summary>
        /// 交易总金额
        /// </summary>
        public string TotalAt { get; set; }
        /// <summary>
        /// 成功笔数
        /// </summary>
        public string SuccQt { get; set; }
        /// <summary>
        /// 成功金额
        /// </summary>
        public string SuccAt { get; set; }
        /// <summary>
        /// 失败笔数
        /// </summary>
        public string FailQt { get; set; }
        /// <summary>
        /// 失败金额
        /// </summary>
        public string FailAt { get; set; }
        /// <summary>
        /// 代扣失败明细
        /// 子格式
        /// </summary>
        public IEnumerable<PayrollQueryDetailFailItem> OutList { get; set; }
    }

    public class PayrollQueryDetailFailItem
    {
        /// <summary>
        /// 明细序号
        /// </summary>
        public string RowNum { get; set; }
        /// <summary>
        /// 超网行号
        /// </summary>
        public string SuperAccNo { get; set; }
        /// <summary>
        /// 帐号/卡号
        /// </summary>
        public string AccId { get; set; }
        /// <summary>
        /// 录入交易金额
        /// </summary>
        public string TranAt { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public string ActAt { get; set; }
        /// <summary>
        /// 账户户名
        /// </summary>
        public string AccName { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 响应描述
        /// </summary>
        public string AnsTx { get; set; }
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string TradSerilNo { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string OthMsg4Tx { get; set; }
    }
}
