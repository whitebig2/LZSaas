using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{



    public class PrepaidApplyRecordExcelTemplate
    {

        [DisplayName("账户服务商")]
        public string AccountPartnerName { get; set; }

        //[DisplayName("客户服务商")]
        //public string PartnerName { get; set; }

        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("签章单位")]
        public string SigningUnitName { get; set; }
        [DisplayName("状态")]
        public string StatusName { get; set; }

        [DisplayName("现金签署状态")]

        public string CashStatusName { get; set; }

        [DisplayName("还款状态")]
        public string RepayStatusName { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }
        [DisplayName("手机号码")]
        public string Phone { get; set; }
        [DisplayName("银行卡所属人")]
        public string BankUserName { get; set; }
        [DisplayName("银行卡所属行")]
        public string Bank { get; set; }
        [DisplayName("银行卡号")]
        public string BankNo { get; set; }
        [DisplayName("申请金额")]
        public decimal ApplyAmount { get; set; }
        [DisplayName("发放方式")]
        public string SendTypeName { get; set; }
        [DisplayName("现金发放")]
        public string IsCashSend { get; set; }

        [DisplayName("发放时间")]
        public string SendTime { get; set; }
        [DisplayName("申请时间")]
        public string CreateTime { get; set; }
        [DisplayName("手动还款状态")]
        public string BadRepayStatusName { get; set; }
        [DisplayName("手动还款金额")]
        public decimal BadRepayAmount { get; set; }
        [DisplayName("手动还款时间")]
        public string BadRepayTime { get; set; }





    }
  
}
