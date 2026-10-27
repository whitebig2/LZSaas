using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Model
{
    public class CustomerAuthModel
    {
        public int CustomerID { get; set; } //客户单位id
        public int CompanyID { get; set; }//人力公司id 保存合同用，不需要更改
        public Guid? CompanyGID { get; set; }//人力公司gid
        public string CustomerName { get; set; } = string.Empty;//公司全称
        public string CompanyNo { get; set; } = string.Empty;//统一信用代码
        public string VerifyName { get; set; } = string.Empty;//认证人姓名
        public string VerifyCardID { get; set; } = string.Empty;//认证人身份证
        public string LegalName { get; set; } = string.Empty;//法人姓名
        public bool IsAuth { get; set; }//是否企业认证
        public bool IsReal { get; set; }//是否实名认证
        public string Address { get; set; } = string.Empty;//地址
        public string AuthorizeFilePath { get; set; } = string.Empty;//上传书授权书
        public string VerifyAmount { get; set; } = string.Empty;//签约人验证金额
        public string VerifyCode { get; set; } = string.Empty;//签约人验证码
        public string BankName { get; set; } = string.Empty;//账号名称
        public string Bank { get; set; } = string.Empty;//银行名称
        public string BankAccount { get; set; } = string.Empty;//银行账号
        public int VerifyType { get; set; } //1,法人认证，2，签约人认证
    }
}
