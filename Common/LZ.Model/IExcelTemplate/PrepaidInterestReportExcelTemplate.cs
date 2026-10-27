using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class PrepaidInterestReportExcelTemplate
    {
        [DisplayName("服务商")]
        public string PartnerName { get; set; }
        [DisplayName("预发工资垫付累计额（年度）")]
        public decimal ApplyYearAmount { get; set; }
        [DisplayName("预发工资月垫付额")]
        public decimal ApplyMonthAmount { get; set; }


        [DisplayName("年累计服务费")]
        public decimal InterestYearAmount { get; set; }
        [DisplayName("月服务费")]
        public decimal InterestMonthAmount { get; set; }

        [DisplayName("服务费比例%")]
        public decimal BasePercent { get; set; }
        [DisplayName("浮动%")]
        public decimal FloatPercent { get; set; }
        [DisplayName("账单月")]
        public string BillMonth { get; set; }


        //[DisplayName("预发垫付待还款累计额（年）")]
        //public decimal PendingYearAmount { get; set; }

        //[DisplayName("预发工资月垫付额")]
        //public decimal PendingMonthAmount { get; set; }



    }

    public class PrepaidInterestRecordExcelTemplate
    {
        [DisplayName("服务商")]
        public string PartnerName { get; set; }
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("垫付预发金额")]
        public decimal ApplyAmount { get; set; }

        [DisplayName("预发发放日期")]
        public string SendDate { get; set; }
        [DisplayName("服务费比例%")]
        public decimal BasePercent { get; set; }
        [DisplayName("浮动%")]
        public decimal FloatPercent { get; set; }
        [DisplayName("确定比例%")]
        public decimal ActualPercent { get; set; }


        [DisplayName("结算金额")]
        public decimal MainAmount { get; set; }

        [DisplayName("还款日期")]
        public string RepaymentDate { get; set; }

        [DisplayName("结算日期起")]
        public string StartDate { get; set; }

        [DisplayName("结算日期止")]
        public string EndDate { get; set; }

        [DisplayName("结算天数")]
        public int Days { get; set; }

        [DisplayName("服务费金额")]
        public decimal MainInterestAmount { get; set; }
        [DisplayName("账单月")]
        public string BillMonth { get; set; }







    }


    public class PrepaidInterestApplyRecordExcelTemplate
    {
        [DisplayName("服务商")]
        public string PartnerName { get; set; }
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }

        [DisplayName("垫付预发金额")]
        public decimal ApplyAmount { get; set; }

        [DisplayName("预发发放日期")]
        public string SendDate { get; set; }
        [DisplayName("预发工资状态")]
        public string Status { get; set; }



      






    }
}
