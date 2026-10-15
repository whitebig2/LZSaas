using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class EntryBusInsuranceRecordExcelTemp
    {
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        [DisplayName("签章单位")]
        public string SignUnitName { get; set; }
        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("身份号码")]
        public string CardID { get; set; }
        [DisplayName("状态")]
        public string Status { get; set; }

        [DisplayName("参保状态")]
        public string PlanStatus { get; set; }

        [DisplayName("商保流程状态")]
        public string ProcessStatus { get; set; }


        [DisplayName("保险公司")]
        public string InsuranceCompany { get; set; }
        [DisplayName("订单号")]
        public string OrderNo { get; set; }
  
        [DisplayName("工种")]
        public string WorkName { get; set; }

        [DisplayName("类别")]
        public string WorkClass { get; set; }

        [DisplayName("参保方案")]
        public string PlanName { get; set; }
        [DisplayName("是否手参")]
        public string IsManuallyInsurance { get; set; }
        [DisplayName("入职日期")]
        public string EntryDate { get; set; }
        [DisplayName("离职日期")]
        public string LeaveDate { get; set; }
    }
}
