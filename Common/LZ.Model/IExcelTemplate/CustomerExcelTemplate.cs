using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class CustomerExcelTemplate
    {
        [DisplayName("服务商")]
        public string PartnerName { get; set; }
        [DisplayName("客户名称")]
        public string CustomerName { get; set; }
        [DisplayName("所在地")]
        public string ProviceAndCity { get; set; }
        [DisplayName("公司地址")]
        public string CompanyAddress { get; set; }
        [DisplayName("统一社会信用代码")]
        public string CompanyNo { get; set; }
        [DisplayName("商务合作单位")]
        public string CooperationUnitName { get; set; }
        [DisplayName("所属服务商")]
        public string SigningUnitName { get; set; }
    }
}
