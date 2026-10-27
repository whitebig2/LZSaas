using LZ.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class BusInsuranceDayMatchRecordExcelTemplate
    {
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        [DisplayName("签章单位")]
        public string SignUnitName { get; set; }

        [DisplayName("订单号")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName( "保险公司")]
        public string InsuranceCompany { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("工种")]
        public string WorkName { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        [DisplayName("类别")]
        public string WorkClass { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("参保方案")]
        public string PlanName { get; set; }

       

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



        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("是否增员")]
        public string IsAddRecord { get; set; }

   
        [DisplayName("状态")]
        public BusInsuranceDayMatchRecordStatusEnumType Status { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("记录日期")]
        public string RecordDate { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        [DisplayName("备注")]
        public string Remark { get; set; }


        [DisplayName("起保日期")]
        public string StartDate { get; set; }

        	
        [DisplayName("终保日期")]
        public string EndDate { get; set; }


        [DisplayName("创建时间")]
        public string CreateTime { get; set; }

       	
        [DisplayName("年龄")]
        public int Age { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("性别")]
        public string Sex { get; set; }

        /// <summary>
        /// 
        /// </summary>		
        [DisplayName("推送结果")]
        public string PushResult { get; set; }

   
    }
}
