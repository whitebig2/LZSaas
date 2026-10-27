using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class OrderDetailResponse:EmpapiResponse
    {
        public OrderDetailData Data { get; set; }
    }

    public class OrderDetailData
    {
        public string enddate { get; set; }
        public string insureid { get; set; }
        public string startdate { get; set; }
        public List<GnteeItem> gnteeList { get; set; }
        public List<BaoquanItem> baoquanlist { get; set; }
    }

    public class GnteeItem
    {
        public string GnteeId { get; set; }
        public string GnteeName { get; set; }
    }

    public class BaoquanItem
    {
        /// <summary>
        /// 人员起保日期
        /// </summary>
        public DateTime Czbrq { get; set; }
        /// <summary>
        /// 保费
        /// </summary>
        public decimal Cbf { get; set; }
        /// <summary>
        ///参保方案
        /// </summary>
        public string Geentname { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Cname { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Proname { get; set; }
        /// <summary>
        /// 雇佣工种
        /// </summary>
        public string Cgygz { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string Cidnum { get; set; }
        /// <summary>
        /// 实际用工单位
        /// </summary>
        public string Csjygdw { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public string Cage { get; set; }
        /// <summary>
        /// 职业类别
        /// </summary>
        public string Czylb { get; set; }
        /// <summary>
        /// 人员终保日
        /// </summary>
        public DateTime Cqbrq { get; set; }
        /// <summary>
        /// 参保方案标识
        /// </summary>
        public string Geentid { get; set; }
        /// <summary>
        ///  劳动合同单位
        /// </summary>
        public string Cldhtdw { get; set; }

        /// <summary>
        ///  劳动合同单位
        /// </summary>
        public string InsuranceId { get; set; }
    }
}
