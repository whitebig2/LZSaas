using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    /// <summary>
    ///  大地财险推送
    /// </summary>
    public class DDCXPushExcelTemplate
    {
        /// <summary>
        /// 新增
        /// 删除
        /// </summary>
        [DisplayName("变更类型")]
        public string ChangeType { get; set; }
        [DisplayName("姓名")]
        public string Name { get; set; }
        [DisplayName("身份证号")]
        public string IdCardNo { get; set; }
        [DisplayName("年龄")]
        public string Age { get; set; }
        [DisplayName("工种")]
        public string WorkName { get; set; }
        [DisplayName("派遣单位")]
        public string CustomerName { get; set; }
        [DisplayName("生效日期")]
        public string StartDate { get; set; }
    }
}
