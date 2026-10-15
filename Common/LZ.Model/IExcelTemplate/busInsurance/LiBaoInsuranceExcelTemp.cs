using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class AnZeInsuranceExcelTemp
    {
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号码")]
        public string CardID { get; set; }
    
        [DisplayName("被派遣单位（可选）")]
        public string CustomerName { get; set; }
        [DisplayName("工种(可选,必须为\r\n被派遣单位下存在的工种,否则无效)")]
        public string WorkName { get; set; }
        [DisplayName("签章单位")]
        public string SignUnitName { get; set; }
        /// <summary>
        /// 加、减
        /// </summary>
        [DisplayName("操作加减")]
        public string PushType { get; set; }
    }
}
