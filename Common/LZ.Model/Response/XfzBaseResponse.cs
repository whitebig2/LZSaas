using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class XfzBaseResponse<T> 
    {

        public int Code { get; set; } = 200;

        /// <summary>
        /// 状态码
        /// </summary>
        public bool Success => Code == 200;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T Result { get; set; }
    }
}
