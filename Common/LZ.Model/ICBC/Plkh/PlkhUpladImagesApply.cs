using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ICBC.Plkh
{
    public class PlkhUpladImagesApply
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
        /// 合作方开卡文件名称（必填）
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// 开卡文件路径（必填）
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 接口标识码（必填）
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 开卡日期（必填）
        /// 格式：yyyymmdd
        /// </summary>
        public string Kkrq { get; set; }
    }
}
