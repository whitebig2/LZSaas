using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Payroll
{
    /// <summary>
    /// 合作方代发申请 响应
    /// </summary>
    public class PayrollSendApplyResultBody: IPayrollResultBody
    {
        /// <summary>
        /// 返回码类型
        /// 返回码类型，表示服务的执行状态，取值范围如下： E –错误    S –成功
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 业务编码
        /// 唯一表示一种确定业务状态的8位数字编码，由服务方定义，提供给调用方使用
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 业务信息
        /// 客户易于识别的文字信息，调用方颗直接显示给客户，也可以根据code 自行组织提示信息
        /// </summary>
        public string Message { get; set; }
    }
}
