using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class DeathInsuranceExcelTemp
    {
        public string Title { get; set; } = "保全变更人员名单";
        [DisplayName("被保险人：")]
        public string CompanyName { get; set; } = "上海正联成网络科技有限公司（宁波莱博）";
        [DisplayName("险种：")]
        public string PlanName { get; set; } = "医疗健康险";
        [DisplayName("保单号：")]
        public string PolicyNum { get; set; } = "保全变更人员名单";
        [DisplayName("起保日期：")]
        public string StartDate { get; set; } = "2026年05月01日零时";
        [DisplayName("截止日期：")]
        public string EndDate { get; set; } = "2027年04月30日二十四时";
        [DisplayName("申请日期：")]
        public string ApplyDate { get; set; }
        [DisplayName("增加人数：")]
        public int AddCount { get; set; }
        [DisplayName("减少人数：")]
        public int DelCount { get; set; }
    }

    public class DeathInsuranceItemExcelTemp
    {
      
        [DisplayName("投保方案")]
        public string PlanName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证")]
        public string CardID { get; set; }
        [DisplayName("护照")]
        public string Passport { get; set; }
        /// <summary>
        /// 批增
        /// 批减
        /// </summary>
        [DisplayName("变更类型")]
        public string ChangeType { get; set; }
        [DisplayName("生效日期")]
        public string StartDate { get; set; }
        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("有无社保")]
        public string IsHaveSocialInsurance { get; set; }
        [DisplayName("用工单位")]
        public string CompanyName { get; set; }
        [DisplayName("用工地址")]
        public string CompanyAddress { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }
 

    }
}
