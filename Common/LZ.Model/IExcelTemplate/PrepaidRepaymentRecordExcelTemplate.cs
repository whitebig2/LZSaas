using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class PrepaidRepaymentRecordExcelTemplate
    {

        [DisplayName("账户服务商")]
        public string AccountPartnerName { get; set; }


        [DisplayName("客户单位")]
        public string CustomerName { get; set; }


        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }



        [DisplayName("申请金额")]
        public decimal ApplyAmount { get; set; }


        [DisplayName("还款金额")]
        public decimal RepaymentAmount { get; set; }

        [DisplayName("发放时间")]
        public string SendTime { get; set; }

        [DisplayName("还款状态")]
        public string RepayStatusName { get; set; }

        [DisplayName("还款时间")]
        public string RepaymentTime { get; set; }

    }
}
