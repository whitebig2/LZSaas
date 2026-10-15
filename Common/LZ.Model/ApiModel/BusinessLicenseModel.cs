using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ApiModel
{
    public class BusinessLicenseModel
    {
        public string order_no { get; set; } = string.Empty;//订单号
        public string legal_person { get; set; } = string.Empty;//法人
        public string capital { get; set; } = string.Empty;//注册资本
        public string valid_date { get; set; } = string.Empty;//有效期
        public string address { get; set; } = string.Empty;//地址
        public string reg_num { get; set; } = string.Empty;//统一社会信用代码
        public string company { get; set; } = string.Empty; //公司名称
        public string establish_date { get; set; } = string.Empty;//注册日期
        public string business { get; set; } = string.Empty;//经营内容
        public string type { get; set; } = string.Empty;//公司类型
        public string image { get; set; } = string.Empty;
    }
}
