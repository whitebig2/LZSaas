using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class ContractRecordExcelTemplate
    {
        [DisplayName("员工姓名")]
        public string EmpName { get; set; }
        [DisplayName("身份证号码")]
        public string IdCardNo { get; set; }
        [DisplayName("客户单位")]
        public string CustomerName { get; set; }
        [DisplayName("签章单位")]
        public string SigningUnitName { get; set; }
        [DisplayName("合同类型")]
        public string ContractTypeName { get; set; }
        [DisplayName("声明模板")]
        public string SmTemplateFilePath { get; set; }
        [DisplayName("合同模板")]
        public string TemplateFilePath { get; set; }
        [DisplayName("待签合同")]
        public string PdfFilePath { get; set; }
        [DisplayName("已签合同")]
        public string SignedFilePath { get; set; }

        [DisplayName("验证码")]
        public string SmsCode { get; set; }
        [DisplayName("离职通知书")]
        public string AutoLeaveFilePath { get; set; }
        [DisplayName("合同有效期")]
        public string ContractStartToEndDate { get; set; }
        [DisplayName("签署状态")]
        public string StatusName { get; set; }
        [DisplayName("签署时间")]
        public string SignedTime { get; set; }
        [DisplayName("计划签署日期")]
        public string ReadySigningDate { get; set; }
        [DisplayName("创建时间")]
        public string CreateTime { get; set; }
    
    }
}
