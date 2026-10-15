using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class SalaryRecordExcelDto
    {

        /// <summary>
        /// 客户单位
        /// </summary>
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 签章单位
        /// </summary>
        [DisplayName("签章单位")]
        public string SigningUnitName { get; set; }
        /// <summary>
        /// 工资所属月
        /// </summary>
        [DisplayName(displayName: "工资所属月")]
        public string BillMonth { get; set; }
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


        [DisplayName("状态")]
        public string StatusName { get; set; }


        [DisplayName("是否暂缓")]
        public string IsDelayPayment { get; set; }

        [DisplayName("是否现金")]
        public string IsCash { get; set; }

        [DisplayName("是否异常")]
        public string IsFailSalary { get; set; }

        [DisplayName("是否部分发放")]
        public string IsSplitSend { get; set; }




        [DisplayName("银行卡所属人")]
        public string BankUserName { get; set; }

        [DisplayName("银行卡所属行")]
        public string Bank {get; set; }
        [DisplayName(displayName: "银行卡号")]
        public string BankNo { get; set; }
        [DisplayName(displayName: "客服扣/补金额")]
        public decimal ChangeAmount { get; set; }
        [DisplayName(displayName: "客服扣/补备注")]
        public string ChangeRemark { get; set; }
        [DisplayName(displayName: "应发金额")]
        public decimal UnitSalaryAmount { get; set; }

        [DisplayName(displayName: "实发金额")]
        public decimal TotalAmount { get; set; }

        [DisplayName(displayName: "部分发放成功金额")]
        public decimal SplitSendSucAmount { get; set; }
        [DisplayName(displayName: "备注")]
        public string Remark { get; set; }

        [DisplayName(displayName: "发放时间")]
        public string SendTime { get; set; }

        [DisplayName(displayName: "创建时间")]
        public string CreateTime { get; set; }

       
    }
}
