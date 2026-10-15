using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Bank.CZY.BaseModels
{
    public class T0010NoteApply
    {
        /// <summary>
        /// 票据介质 默认1.电票
        /// </summary>
        public required string BillsProperty { get; set; }

        /// <summary>
        /// 票据类型 默认1.银行承兑汇票
        /// </summary>
        public required string BillsType { get; set; }

        /// <summary>
        /// 出票日期
        /// </summary>
        public required string BillsDate { get; set; }

        /// <summary>
        /// 到期日
        /// </summary>
        public required string BillsDueDate { get; set; }

        /// <summary>
        /// 能否转让 1.可转让 2.不得转让
        /// </summary>
        public required string Transfer { get; set; }

        /// <summary>
        /// 是否拆包 1.可拆包 2.不可拆包
        /// </summary>
        public required string CanSplitDraft { get; set; }
    }
}
