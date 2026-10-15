using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class DaDiInsuranceExcelTemp
    {
        /// <summary>
        /// 变更类型
        /// 删除、新增
        /// </summary>
        [DisplayName("变更类型")]
        public string  PushType { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string CardID { get; set; }
        [DisplayName("年龄")]
        public int Age { get; set; }
        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("雇主单位")]
        public string SignUnitName { get; set; }
        [DisplayName("派遣单位")]
        public string CustomerName { get; set; }
        [DisplayName("生效日期")]
        public string StartDate { get; set; }
   
    }
}
