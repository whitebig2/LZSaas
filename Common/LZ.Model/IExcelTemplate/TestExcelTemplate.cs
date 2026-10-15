using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class TestExcelTemplate
    {
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("证件号")]
        public string IdCardNo { get; set; }
        [DisplayName("雇员工种")]
        public string WorkName { get; set; }
        [DisplayName("实际用工单位")]
        public string CustomerName { get; set; }
    }

    public class UserEntryExcelTestTemplate
    {
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }
        [DisplayName("入职时间")]
        public string EntryDate { get; set; }

        [DisplayName("退休日期")]
        public string RetirDate { get; set; }
        [DisplayName("初次合同日期")]
        public string ContractDate { get; set; }
        [DisplayName("合同开始日期")]
        public string StartDate { get; set; }
        [DisplayName("合同结束日期")]
        public string EndDate { get; set; }

    }


   
}
