using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum SalaryFailTypeEnumType
    {
        /// <summary>
        /// （1）银行卡号错误
        /// </summary>
        卡号异常 = 0,
        /// <summary>
        /// （2）姓名跟银行卡号不一致
        /// </summary>
        信息不一致 = 1,
        /// <summary>
        /// （3）需要具体支行名称。
        /// </summary>
        需要具体支行名称 = 2,
        /// <summary>
        /// （4）收款账户为二/三类卡，有限额，转账失败。
        /// </summary>
        限额 = 3,
        /// <summary>
        ///（5）其他。（选择其他时支持财务手动输入）
        /// </summary>
        其他 = 0
    }
}
