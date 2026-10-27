using LZ.Model.Enums;
using SqlSugar;
using System.Collections.Generic;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_PaymentOrder")]
    public class Live_PaymentOrderExtension : Live_PaymentOrder
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_PaymentOrderDetail.PaymentOrderID))]
        public List<Live_PaymentOrderDetail> Records { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        [SqlSugar.SugarColumn(
                  IsEnableUpdateVersionValidation = true,//标识版本字段 
                  IsOnlyIgnoreInsert = true,//禁止插入
                  IsOnlyIgnoreUpdate = true,//禁止更新
                  ColumnDataType = "timestamp" //时间戳类型，有些库需要配置或者有差异
                   )]
        public new byte[] Timestamp { get; set; }

        public void SetCancel()
        {
            this.Status = PaymentOrderTransStatusEnumType.取消支付;
            this.UpdateTime = DateTime.Now;
            this.PushTime = null;
            this.PushCount = 0;
            if (Records == null || Records.Count == 0)
            {
                return;
            }
            foreach (var item in Records)
            {
                item.Status = PaymentOrderTransStatusEnumType.取消支付;
            }
        }


        public void SetTransFail()
        {
            this.Status = PaymentOrderTransStatusEnumType.支付失败;
            this.UpdateTime = DateTime.Now;
            this.PushTime = null;
            this.PushCount = 0;
            if (Records == null || Records.Count == 0)
            {
                return;
            }
            foreach (var item in Records)
            {
                item.Status = PaymentOrderTransStatusEnumType.支付失败;
            }
        }

        public void HandlePayment()
        {
            //this.Status = PaymentOrderTransStatusEnumType.支付成功;
            this.TransTime = DateTime.Now;
            this.UpdateTime = DateTime.Now;
            this.PushTime = null;
            this.PushCount = 0;
            if (Records == null || Records.Count == 0)
            {
                return;
            }
            bool isManual = false;
            foreach (var detail in Records)
            {
                if (detail.PayStatus == ((int)CzyPayStatusEnumType.支付成功).ToString())
                {
                    detail.TransTime = DateTime.Now;
                    detail.Status = PaymentOrderTransStatusEnumType.支付成功;
                }
                else if (detail.PayStatus == ((int)CzyPayStatusEnumType.支付失败).ToString())
                {
                    detail.Status = PaymentOrderTransStatusEnumType.支付失败;
                }
                else if (detail.PayStatus == ((int)CzyPayStatusEnumType.已退款).ToString() || detail.PayStatus == ((int)CzyPayStatusEnumType.部分退款).ToString() || detail.PayStatus == ((int)CzyPayStatusEnumType.部分支付成功).ToString())
                {
                    detail.Status = PaymentOrderTransStatusEnumType.待财务确认支付结果;
                    isManual = true;
                }
                else {
                    isManual = true;
                }
            }

            if (isManual)
            {
                this.Status = PaymentOrderTransStatusEnumType.待财务确认支付结果;
            }
            else
            {

                if (Records.Any(f => f.Status == PaymentOrderTransStatusEnumType.支付成功))
                {
                    // 标记为支付成功
                    this.Status = PaymentOrderTransStatusEnumType.支付成功;
                }
                else
                {
                    this.Status = PaymentOrderTransStatusEnumType.支付失败;
                }
            }



        }

    }
}
