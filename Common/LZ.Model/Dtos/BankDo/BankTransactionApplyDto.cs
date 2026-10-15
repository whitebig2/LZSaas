using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.BankDo
{
    public class BankTransactionApplyDto
    {

        /// <summary>
        /// 交易记录标识
        /// </summary>
        public string TransactionID { get; set; }
        public string BusinessID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string SignUnitName { get; set; }
        public string SignUnitNo { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 期望支付时间
        /// </summary>
        public DateTime PayDate { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 用途
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// 附言
        /// </summary>
        public string Remark { get; set; }

        public List<BankTransactionRecordApplyDto> Records { get; set; }

        public BankTransactionApplyDto()
        {
            Records = new List<BankTransactionRecordApplyDto>();
        }
    }


    public class BankTransactionRecordApplyDto
    {

        public string RecordTransactionID { get; set; }
        public string RecordBusinessID { get; set; }
        public string Bank { get; set; }
        public string BankNo { get; set; }
        public string Name { get; set; }
        public string CardID { get; set; }
        public decimal TransAmount { get; set; }
    


    }
}
