using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public class CashSignRecordExportDto
    {
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("签章单位")]
        public string SignUnitName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string CardID { get; set; }
        [DisplayName("性别")]
        public string Sex { get; set; }
        [DisplayName("手机号码")]
        public string Phone { get; set; }
        [DisplayName("现金类型")]
        public string CashType { get; set; }
        [DisplayName("过期日期")]
        public string ExpirationDate { get; set; }
        [DisplayName("状态")]
        public CashSignStatusEnumType Status { get; set; }
        [DisplayName("已签署文件")]
        public string SignedPath  { get; set; }
        [DisplayName("签署时间")]
        public string SignTime { get; set; }
    }
}
