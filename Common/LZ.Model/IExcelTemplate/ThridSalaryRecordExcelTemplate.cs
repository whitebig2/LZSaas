using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class ThridSalaryRecordExcelTemplate
    {

        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [DisplayName("身份证号码")]
        public string IdCardNo { get; set; }

        [DisplayName("车间(非必填项)")]
        public string Workshop { get; set; }
        [DisplayName(displayName: "工资日期")]
        public DateTime SalaryMonth { get; set; }
        [DisplayName(displayName: "服务费")]
        public decimal ServiceFeeAmount { get; set; }
        [DisplayName(displayName: "企业应发工资")]
        public decimal UnitSalaryAmount { get; set; }
        [DisplayName(displayName: "住宿费")]
        public decimal ZhuSuAmount { get; set; }
        [DisplayName(displayName: "水电费")]
        public decimal ShuiDianAmount { get; set; }
        [DisplayName(displayName: "商保费")]
        public decimal ShangBaoAmunt { get; set; }
        [DisplayName(displayName: "体检费")]
        public decimal TiJianAmunt { get; set; }
        [DisplayName(displayName: "线下预支")]
        public decimal XianXiaYuZhiAmount { get; set; }
        [DisplayName(displayName: "其他扣除")]
        public decimal QiTaKouChuAmount { get; set; }
        [DisplayName(displayName: "备注")]
        public string Remark { get; set; }
        [DisplayName(displayName: "银行卡所属行")]
        public string Bank { get; set; }
        [DisplayName(displayName: "银行卡卡号")]
        public string BankNo { get; set; }
        [DisplayName(displayName: "银行卡所属人")]
        public string BankUserName { get; set; }

    }
}
