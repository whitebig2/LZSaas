using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ApiModel
{
    public class InvoiceModel
    {
        public string amountInFiguers { get; set; } = string.Empty;////价税合计(小写)
        public string invoiceNum { get; set; } = string.Empty;//发票号码
        public string totalTax { get; set; } = string.Empty;//合计税额
        public string image { get; set; } = string.Empty;//图片保存地址
    }
}
