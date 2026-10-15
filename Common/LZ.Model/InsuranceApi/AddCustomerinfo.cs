using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class AddCustomerinfo
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string Custemername { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string Custemernum { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Custemersex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public string Custemerage { get; set; }
        /// <summary>
        /// 雇佣工种
        /// </summary>
        public string Guyuangongzhong { get; set; }
        /// <summary>
        /// 实际用工单位
        /// </summary>
        public string Sjygdw { get; set; }
        /// <summary>
        /// 劳动合同单位
        /// </summary>
        public string Ldhtdw { get; set; }
        /// <summary>
        /// 职业类别
        /// </summary>
        public string Zhiyeleibei { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string Custemertype { get; set; }
    }
}
