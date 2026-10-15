using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum ProductStatus
    {
        /// <summary>
        /// 下架中
        /// </summary>
        Delist = 1,
        /// <summary>
        /// 上架中
        /// </summary>
        List = 2,
        /// <summary>
        /// 仓库中
        /// </summary>
        Warehouse = 3
    }
}
