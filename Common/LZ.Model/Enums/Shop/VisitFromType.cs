using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum VisitFromType
    {
        /// <summary>
        /// 商品访问
        /// </summary>
        Product = 1,
        /// <summary>
        /// 搜索访问
        /// </summary>
        Search = 2,
        /// <summary>
        /// 岗位访问（Live_JobsVisit，岗位详情页浏览）
        /// </summary>
        Job = 3
    }
}
