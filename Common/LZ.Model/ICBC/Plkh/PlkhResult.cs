using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Plkh
{
    /// <summary>
    /// 批量开户响应
    /// </summary>
    public class PlkhResult
    {
        /// <summary>
        ///接口标识码
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 返回编码
        /// 0000 – 成功
        /// 其他 - 失败
        /// </summary>
        public string Retcode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Retmsg { get; set; }
    }
}
