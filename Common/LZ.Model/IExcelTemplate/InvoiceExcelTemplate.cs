using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class InvoiceExcelTemplate
    {
        [DisplayName("收票公司名称")]
        public string RecipientName { get; set; }
        [DisplayName("客户公司名称")]
        public string CustomerName { get; set; }
        [DisplayName(displayName: "开票公司名称")]
        public string IssuerName { get; set; }
        [DisplayName(displayName: "发票类别")]
        public string BusinessTypeName { get; set; }
        [DisplayName(displayName: "发票类型")]
        public string InvoiceTypeName { get; set; }
        //[DisplayName(displayName: "开票金额")]
        //public decimal InvoiceAmount { get; set; }
        [DisplayName(displayName: "开票金额")]
        public decimal TotalAmount { get; set; }
        [DisplayName(displayName: "员工工资")]
        public decimal SalaryAmount { get; set; }
        [DisplayName(displayName: "实收服务费")]
        public decimal ServiceFeeAmount { get; set; }
        [DisplayName(displayName: "人数")]
        public int RecordCount { get; set; }
        [DisplayName(displayName: "工资人数")]
        public int SalaryRecordCount { get; set; }
        [DisplayName(displayName: "服务费人数")]
        public int SerivceFeeRecordCount { get; set; }
        [DisplayName(displayName: "工资服务费人数")]
        public int BothRecordCount { get; set; }
        [DisplayName(displayName: "开票名称")]
        public string Title { get; set; }
        [DisplayName(displayName: "实际包含内容")]
        public string Description { get; set; }
        [DisplayName(displayName: "纳税人识别号")]
        public string RecipientCompanyNo { get; set; }
        [DisplayName(displayName: "状态")]
        public string StatusName { get; set; }
        [DisplayName(displayName: "申请人")]
        public string ApplyUserFullName { get; set; }
        [DisplayName(displayName: "锁定时间")]
        public string LockedTime { get; set; }
        [DisplayName(displayName: "开具时间")]
        public string IssuedTime { get; set; }
        [DisplayName(displayName: "创建时间")]
        public string CreateTime { get; set; }
    }
}
