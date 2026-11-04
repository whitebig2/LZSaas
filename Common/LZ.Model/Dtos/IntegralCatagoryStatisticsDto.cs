using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    /// <summary>
    /// 积分分类统计
    /// </summary>
    public class IntegralCatagoryStatisticsDto
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
    }

    public class IntegralCatagoryStatisticsResultDto
    {
        [JsonIgnore]
        public int IntegralType { get; set; }
        /// <summary>
        /// 积分类别
        /// </summary>
        public string Catagory { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
    }
}
