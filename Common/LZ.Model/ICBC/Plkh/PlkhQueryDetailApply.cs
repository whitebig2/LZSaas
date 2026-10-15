using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Plkh
{

    /// <summary>
    /// 合作方开卡结果查询
    /// </summary>
    public class PlkhQueryDetailApply
    {

        /// <summary>
        /// 时间戳（必填）
        ///长度13
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 合作方代码（必填）
        /// </summary>
        public string AppCode { get; set; }
        /// <summary>
        /// 姓名（必填）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证（必填）
        /// </summary>
        public string Lgldocno { get; set; }

        /// <summary>
        /// 开卡发送日期
        /// </summary>
        public string Senddate { get; set; }
        /// <summary>
        /// 接口标识码（必填）
        /// </summary>
        public string Id { get; set; }
    }
}
