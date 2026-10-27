using LZ.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SqlSugar.SugarTable("Live_PrepaidPartnerAccount")]
    public class Live_PrepaidPartnerAccountExtension:Live_PrepaidPartnerAccount
    {

        [SqlSugar.SugarColumn(
                   IsEnableUpdateVersionValidation = true,//标识版本字段 
                   IsOnlyIgnoreInsert = true,//禁止插入
                   IsOnlyIgnoreUpdate = true,//禁止更新
                   ColumnDataType = "timestamp" //时间戳类型，有些库需要配置或者有差异
                    )]
        public new byte[]  Timestamp { get; set; }


        /// <summary>
        /// 发放成功
        /// </summary>
        /// <param name="amount"></param>
        public void SendSuccess(decimal amount)
        {
            if (amount > 0)
            {
                this.UsedAmount += amount;
                this.AvailableAmount = this.TotalAmount - this.UsedAmount;
            }
        }


        /// <summary>
        /// 现金充值
        /// </summary>
        /// <param name="amount"></param>
        public void CashRecharge(decimal amount)
        {
            if (amount > 0 )
            {
                this.CashAmount += amount;
                this.TotalAmount += amount;
                this.AvailableAmount = this.TotalAmount - this.UsedAmount;
            }
        }



        /// <summary>
        /// 现金提现
        /// </summary>
        /// <param name="amount"></param>
        public void CashDeduct(decimal amount)
        {
            if (amount > 0&& AvailableAmount>= amount)
            {
                this.CashAmount -= amount;
                this.TotalAmount -= amount;
                this.AvailableAmount = this.TotalAmount - this.UsedAmount;
            }
        }
    }
}
