using System;
using System.Collections.Generic;
using System.Text;

namespace LZ.Model.InsuranceApi
{
    public class AddEmpRequest
    {
        /// <summary>
        /// 增员人员姓名(必填) 
        /// </summary>
        public string Custemername { get; set; }
        /// <summary>
        /// 身份证/非身份证(必填) 
        /// </summary>
        public string Custemertype { get; set; }
        /// <summary>
        /// 增员人员的证件号(必填) 
        /// </summary>
        public string Custemernum { get; set; }
        /// <summary>
        /// 增员人员的性别(非必填) 
        /// </summary>
        public string? Custemersex { get; set; }
        /// <summary>
        /// 增员人员的年龄(非必填) 
        /// </summary>
        public int? Custemerage { get; set; }
        /// <summary>
        /// 增员人员的岗位名称(必填) 
        /// </summary>
        public string Guyuangongzhong { get; set; }
        /// <summary>
        /// 增员人员的实际工作单位全称(必填)
        /// </summary>
        public string Sjygdw { get; set; }
        /// <summary>
        /// 与增员人员签署劳动合同 或其他劳动形式协议的单 位全称(必填)
        /// </summary>
        public string Ldhtdw { get; set; }
        /// <summary>
        /// 增员人员岗位所属的职业类别，为单个数字：1、2、3、4、5 (必填)
        /// </summary>
        public string Zhiyeleibei { get; set; }
        /// <summary>
        /// 增员对应的订单号(必填) 
        /// </summary>
        public string InsureId { get; set; }
        /// <summary>
        /// 增员对应订单号的保障方案(必填)
        /// </summary>
        public string GnteeId { get; set; }
        /// <summary>
        /// 增员方案名称(非必填) 
        /// </summary>
        public string GnteeName { get; set; }
    }
}
