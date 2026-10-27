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
    public class BusInsuranceRecordTemplate
    {


        [DisplayName("签章单位")]	
        public string SignUnitName { get; set; }

        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("性别")]
        public string Sex { get; set; }

        [DisplayName("年龄")]
        public int Age { get; set; }

        [DisplayName("身份证号码")]
        public string CardID { get; set; }

        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("类别")]
        public string WorkClass { get; set; }


        [DisplayName("保单号")]
        public string PolicyNum { get; set; }


        /// <summary>
        /// 订单号
        /// </summary>
        [DisplayName(displayName: "订单号")]
        public string OrderNo { get; set; }


        [DisplayName(displayName: "参保方案")]
        public string PlanName { get; set; }




     	
        [DisplayName(displayName: "保险公司")]
        public string InsuranceCompany { get; set; }

        [DisplayName(displayName: "开始时间")]

        public string StartDate { get; set; }

        [DisplayName(displayName: "结束时间")]
        public string EndDate { get; set; }



        [DisplayName(displayName: "状态")]
        public string Status { get; set; }

        [DisplayName(displayName: "参保月份")]
        public string InsuranceMonth { get; set; }

        [DisplayName(displayName: "创建时间")]
        public string  CreateTime { get; set; }
        [DisplayName(displayName: "匹配日期")]
        public string MatchDate { get; set; }
    }
}
