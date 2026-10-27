using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    /// <summary>
    ///  大地财险导入模板
    /// </summary>
    public class DdcxExcelTemplate
    {
        [DisplayName("姓名*")]
        public string Name { get; set; }
        [DisplayName("证件号码*")]
        public string IdCardNo { get; set; }
        [DisplayName("备注")]
        public string Remark { get; set; }
        [DisplayName("保险起期")]
        public DateTime StartDate { get; set; }
        [DisplayName("保险止期")]
        public DateTime EndDate { get; set; }
        [DisplayName("职业类别")]
        public string JobClass { get; set; }
        public string WorkClass => this.JobClass + "类";
        public string WorkName { get; set; }
        public string CustomerName { get; set; }

        public void Count()
        {

            if (!string.IsNullOrWhiteSpace(this.Remark))
            {
                var index = this.Remark.IndexOf(")");
                var workName = this.Remark.Substring(1, index - 1);
                var customerName = this.Remark.Substring(index + 1);
                this.CustomerName=customerName;
                this.WorkName=workName;
            }


        }
    }
}
