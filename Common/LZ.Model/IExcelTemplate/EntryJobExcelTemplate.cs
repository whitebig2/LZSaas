using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.IExcelTemplate
{
    public class EntryJobExcelTemplate
    {
        /// <summary>
        /// 工种名称
        /// </summary>
        [DisplayName("工种名称")]
        public string Name { get; set; }
        /// <summary>
        /// 工种类型
        /// </summary>
        [DisplayName("工种类型")]
        public string Type { get; set; }
       
    }
}
