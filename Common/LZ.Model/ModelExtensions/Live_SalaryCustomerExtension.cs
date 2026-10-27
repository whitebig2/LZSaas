using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.ModelExtensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_SalaryCustomer")]
    public class Live_SalaryCustomerExtension:Live_SalaryCustomer
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_SalaryRecordExtension.SalaryCustomerID))]
        public List<Live_SalaryRecordExtension> Records { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Live_SalaryDelaySendRecord.SalaryCustomerID))]
        public List<Live_SalaryDelaySendRecord> DelaySendRecords { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Live_SalaryCashSendRecord.SalaryCustomerID))]
        public List<Live_SalaryCashSendRecord> CashSendRecords { get; set; }

        //[Navigate(NavigateType.OneToMany, nameof(Live_Settlement.SalaryCustomerID))]
        //public List<Live_Settlement> Settlements { get; set; }

        //[Navigate(NavigateType.OneToMany, nameof(Live_Invoice.SalaryCustomerID))]
        //public List<Live_Invoice> Invoices { get; set; }

        public void SalaryCount()
        {
            if (Records != null && Records.Any())
            {
                // 企业应发工资
                this.UnitSalarySumAmount = Records.Sum(f => f.UnitSalaryAmount);
                // 企业扣款
                this.UnitDeductionSumAmount = Records.Sum(f => f.UnitDeductionAmount);
                // 企业应发合计
                this.UnitSalaryTotalAmount = UnitSalarySumAmount - UnitDeductionSumAmount;
                // 服务费
                this.ServiceFeeSumAmount = Records.Sum(f => f.ServiceFeeAmount);
                //税后工资
                this.ActualSalarySumAmount = Records.Sum(f => f.ActualSalaryAmount);
                // 公司管理费
                UnitManageSumAmount = Records.Sum(f => f.UnitManageAmount);
                // 企业社保
                SocialUnitSumAmount = Records.Sum(f => f.SocialUnitSumAmount);
                this.BeforeTaxSalarySumAmount = Records.Sum(f => f.BeforeTaxSalaryAmount);
                // 应收账款
                ReceivableTotalAmount =0;
                if (this.SigningUnitCompanyName != this.CooperationUnitCompanyName)
                {
                    // 公司结算费用合计
                    //公司结算费用合计=企业应发工资+企业内部管理费+企业社保-企业扣款
                    this.UnitSettlementAmount = this.UnitSalarySumAmount + this.SocialUnitSumAmount + this.UnitManageSumAmount - this.UnitDeductionSumAmount;
                }
                else {
                    this.UnitSettlementAmount = 0;
                }
                RecordCount = Records.Count();
                ServiceRecordCount = Records.Count(f=>f.ServiceFeeAmount>0);
                ZeroSalaryCount = Records.Count(f=>f.SalaryRecordType== Enums.SalaryRecordTypeEnumType.补零工资);
                // 公司已过账
                IsCompanyPosted = false;
                // 客户已打款
                IsCustomerPaid = false;
            }

        }

        public void Reset()
        {
            SalaryBatchType = 0;
            Status = SalaryStatusEnumType.AwaitingCalculation;
            IsCombineSalary = false;
            if (Records != null && Records.Any())
            {
                Records.AsParallel().ForAll(r =>
                {
                    r.MonthNetSalaryAmount = 0;
                    r.TaxMonthAmount = 0;
                    r.LastBeforeTaxSalaryTotalAmount = 0;
                    r.LastSocialDaBingTotalAmount = 0;
                    r.LastSocialEmpTotalAmount = 0;
                    r.LastSocialGongJiJinTotalAmount = 0;
                    r.LastSocialShiYeTotalAmount = 0;
                    r.LastSocialYangLaoTotalAmount = 0;
                    r.LastSocialYiLiaoTotalAmount = 0;
                    r.LastTaxTotalAmount = 0;
                    r.TaxTimes = 0;
                    r.ActualSalaryAmount = 0;
                    r.ActualSalarySendCardAmount= 0;
                  

                    r.SpBasiclAmount = 0;
                    r.SpBasicTotalAmount = 0;

                    r.SpZiNvAmount = 0;
                    r.SpJiaoYuAmount = 0;
                    r.SpDaiKuanAmount = 0;
                    r.SpZuJinAmount = 0;
                    r.SpShanYangAmount = 0;
                    r.SpYingErAmount = 0;
                 
                    r.TaxSpZiNvTotalAmount = 0;
                    r.TaxSpJiaoYuTotalAmount = 0;
                    r.TaxSpDaiKuanTotalAmount = 0;
                    r.TaxSpZuJinTotalAmount = 0;
                    r.TaxSpShanYangTotalAmount = 0;
                    r.TaxSpYingErTotalAmount = 0;

                    r.SpSumAmount = 0;
                    r.SpTotalAmount = 0;
                    r.TaxTotalAmount = 0;
                    r.TaxMonthAmount = 0;
                    r.Status= Enums.SalaryRecordSendStatusEnumType.待处理;

                    r.PrepaidRepaymentAmount = 0;
                    //r.IsDelayPayment = false;
                    r.IsDelayPaymentSuc=false;
                    //r.IsCashSend = false;
                    r.IsCashSignSuc=false;
                    r.IsFailSalary = false;
                    r.IsFailSalarySuc = false;
                    r.IsSplitSend = false;
                    r.IsSplitSendSuc = false;
                    r.IsLocked = false;
                    r.IsPrepaidFail = false;
                    r.LockedTime = null;
                    r.SendTime = null;
                    r.IsAutoLocked = true;
                    r.SettlementID = null;
                   
                });
            }
        }

        /// <summary>
        /// 只处理批量发放记录
        /// </summary>
        public void SetBatchRecord()
        {
            if (this.Status == SalaryStatusEnumType.SucessSend)
            {
                throw new Exception("客户工资已发放，无法执行此操作");
            }
            foreach (var item in this.Records)
            {

                if (item.SalaryRecordType == Enums.SalaryRecordTypeEnumType.常规工资)
                {
                    if (!item.IsDelayPayment && !item.IsCashSend)
                    {
                        item.IsBatchSend = true;
                        item.IsSingleSend = false;
                        item.IsAutoLocked = true;
                        item.TransStatus = string.Empty;
                        item.TransStatusName = string.Empty;
                        item.TransMsg = string.Empty;
                        item.SendTime = null;
                        // 发放异常状态重置
                        item.IsFailSalary = false;
                        item.IsFailSalarySuc = false;
                        item.Status = Enums.SalaryRecordSendStatusEnumType.待发放;

                    }
                    else
                    {
                        item.IsBatchSend = false;
                        item.IsSingleSend = true;
                    }
                }
                else
                {
                    item.IsBatchSend = false;
                    item.IsSingleSend = false;
                    item.Status = Enums.SalaryRecordSendStatusEnumType.不发放;
                }
            }

        }

        public void SetPendingSend()
        {
            if (this.Status == SalaryStatusEnumType.SucessSend)
            {
                throw new Exception("客户工资已发放，无法执行此操作");
            }
            this.Status = SalaryStatusEnumType.PendingSend;
            this.IsPushToOrder = false;
            this.IsPushTrans = false;
            this.PushTransTime = null;
            this.TransMsg = string.Empty;
            this.TransStatus = string.Empty;
            this.TransStatusName = string.Empty;
            this.SendTime = null;
            this.PaymentStatus = SalaryPaymentStatusEnumType.无;
            foreach (var item in this.Records)
            {
                if (item.SalaryRecordType == Enums.SalaryRecordTypeEnumType.常规工资)
                {
                    if (!item.IsDelayPayment && !item.IsCashSend)
                    {
                        item.IsSendSuccess = false;
                        item.IsPushToOrder = false;

                        item.IsBatchSend = true;
                        item.IsSingleSend = false;
                        item.IsAutoLocked = true;
                        item.TransStatus = string.Empty;
                        item.TransStatusName = string.Empty;
                        item.TransMsg = string.Empty;
                        item.SendTime = null;
                        // 发放异常状态重置
                        item.IsFailSalary = false;
                        item.IsFailSalarySuc = false;
                        item.Status = Enums.SalaryRecordSendStatusEnumType.待发放;

                    }
                    else
                    {
                        item.IsBatchSend = false;
                        item.IsSingleSend = true;
                    }
                }
                else
                {
                    item.IsBatchSend = false;
                    item.IsSingleSend = false;
                    item.Status = Enums.SalaryRecordSendStatusEnumType.不发放;
                }
            }

        }
        public void SetBatchLocked(bool isLocked)
        {


            if (this.Status == SalaryStatusEnumType.SucessSend)
            {
                throw new Exception("客户工资已发放，无法执行此操作");
            }


            this.IsLocked = isLocked;
            this.LockedTime= isLocked?DateTime.Now:null;
            foreach (var item in this.Records)
            {
                if (item.SalaryRecordType == Enums.SalaryRecordTypeEnumType.常规工资)
                {
                    if (item.IsBatchSend && !item.IsSingleSend)
                    {
                        item.IsLocked = IsLocked;
                        item.LockedTime = IsLocked ? DateTime.Now : null;
                    }
                }
                else
                {
                    item.IsBatchSend = false;
                    item.IsSingleSend = false;
                    item.Status = Enums.SalaryRecordSendStatusEnumType.不发放;
                }
            }

        }
        /// <summary>
        ///  获取 推送记录
        ///  将记录标记为 已推送到订单
        /// </summary>
        /// <param name="isLocked"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Live_SalaryRecordExtension> SetPushOrder()
        {

            if (!this.IsLocked)
            {
                throw new ErrorCodeException(-1, "【未锁定】状态无法执行此操作");
            }
            if (this.Status != SalaryStatusEnumType.PendingSend)
            {
                throw new ErrorCodeException(-1, message: "非【待发放】状态无法执行此操作！");
            }
            if (this.IsCmbcSend || this.IsPushTrans)
            {
                throw new ErrorCodeException(-1, message: "检测到已标记为【已推送】记录，无法再次推送");
            }

            if (!this.IsCustomerPaid)
            {
                throw new ErrorCodeException(-1, "客户未打款，无法执行此功能！");
            }


            if (Records.Any(f=>f.Status== SalaryRecordSendStatusEnumType.待发放&&f.IsBatchSend&&f.IsLocked==false))
            {
                throw new Exception("批量发放存在【未锁定】记录");
            }
            var pushOrderRecords = Records.Where(f => 
            f.ActualSalarySendCardAmount > 0 
            && f.Status == Enums.SalaryRecordSendStatusEnumType.待发放 
            && f.IsLocked 
            &&f.IsBatchSend
           ).ToList();
            if (!pushOrderRecords.Any())
            {
                throw new Exception("没有符合条件的待发放记录");
            }


            if (pushOrderRecords.Any(f => string.IsNullOrWhiteSpace(f.Bank) || string.IsNullOrWhiteSpace(f.BankNo)))
            {
                throw new ErrorCodeException(-1, "代发记录存在银行卡信息缺失记录，操作失败");
            }
            foreach (var item in pushOrderRecords)
            {
                item.IsPushToOrder = true;
            }


            this.PushTransTime = DateTime.Now;
            this.IsPushToOrder = true;
            this.IsPushTrans = true;
            return pushOrderRecords;
        }

        /// <summary>
        ///  批量代发被退回或者 代发失败的记录，取消推送到订单
        /// </summary>
        public void SetPushOrderCancel(List<Guid> recordIds)
        {
            this.PushTransTime = null;
            this.IsPushToOrder = false;
            this.IsPushTrans = false;

            var pushOrderRecords = Records.Where(f => recordIds.Contains(f.ID)).ToList();

            if (pushOrderRecords.Any())
            {
                foreach (var item in pushOrderRecords)
                {
                    item.IsPushToOrder = false;
                }
            }
        }


    }
}
