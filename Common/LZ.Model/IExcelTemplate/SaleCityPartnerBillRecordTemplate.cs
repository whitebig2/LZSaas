using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public partial class SaleCityPartnerBillRecordTemplate
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public string FullName { get; set; }
        /// <summary>
        /// 城市合伙人
        /// </summary>
        [DisplayName("城市合伙人")]
        public string CityPartnerName { get; set; }
        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        /// <summary>
        /// 当面状态
        /// </summary>
        [DisplayName("当面状态")]
        public string StatusName { get; set; }
       
        /// <summary>
        /// 金额
        /// </summary>
        [DisplayName("金额")]
        public int Money { get; set; }
        /// <summary>
        /// 工资日期
        /// </summary>
        [DisplayName("创建时间")]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd")]
        public string CreateTime { get; set; }

    }
}
