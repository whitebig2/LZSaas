using System.ComponentModel;

namespace LZ.Model.IExcelTemplate
{
    public class BusInsuranceApiPushDetailExcelTemplate
    {
        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("身份证号")]
        public string CardID { get; set; }

        [DisplayName("年龄")]
        public int Age { get; set; }

        [DisplayName("性别")]
        public string Sex { get; set; }

        [DisplayName("手机号")]
        public string Phone { get; set; }

        [DisplayName("客户单位")]
        public string CustomerName { get; set; }

        [DisplayName("保单号")]
        public string OrderNo { get; set; }

        [DisplayName("参保方案")]
        public string PlanName { get; set; }

        [DisplayName("工种")]
        public string WorkName { get; set; }

        [DisplayName("工种类别")]
        public string WorkClass { get; set; }

        [DisplayName("保险公司工种")]
        public string InsuranceWorkName { get; set; }

        [DisplayName("起保日期")]
        public string StartDate { get; set; }

        [DisplayName("是否有社保")]
        public string IsSocial { get; set; }

        [DisplayName("是否高处作业")]
        public string IsWorkHeight { get; set; }

        [DisplayName("替换姓名")]
        public string DelName { get; set; }

        [DisplayName("替换身份证")]
        public string DelCardID { get; set; }

        [DisplayName("状态")]
        public string Status { get; set; }

        [DisplayName("失败原因")]
        public string FailMsg { get; set; }

        [DisplayName("备注")]
        public string Remark { get; set; }
    }
}
