using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Response
{
    public class XfzPageResponse<T>
    {
        /// <summary>
        /// 当前页标
        /// </summary>
        public int PageNo { get; set; } = 1;
        public int TotalCount { get; set; } = 1;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; } = 6;
        /// <summary>
        /// 数据总数
        /// </summary>
        public int DataCount { get; set; } = 0;
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { set; get; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> Data { get; set; }
    }
}
