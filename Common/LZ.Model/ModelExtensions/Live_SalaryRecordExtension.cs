using Microsoft.AspNetCore.Http.Metadata;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_SalaryRecord")]
    public class Live_SalaryRecordExtension : Live_SalaryRecord
    {
        public void CalculationTax()
        {

            var today = DateTime.Today;
         
            // 税前工资
            this.BeforeTaxSalaryAmount=this.UnitSalaryAmount+this.SubsidyAmount;

            this.BeforeTaxSalaryTotalAmount=this.LastBeforeTaxSalaryTotalAmount+this.BeforeTaxSalaryAmount;
            // 实收企业工资合计
            this.CompanySalarySumAmount=this.UnitSalaryAmount-this.UnitDeductionAmount;
            // 社保个人部分金额
            this.SocialEmpSumAmount =
                this.SocialYangLaoAmount
                + this.SocialShiYeAmount
                + this.SocialYiLiaoAmount
                + this.SocialDaBingAmount
                + this.SocialGongJiJinAmount;

            this.SocialYangLaoTotalAmount=this.LastSocialYangLaoTotalAmount+this.SocialYangLaoAmount;
            this.SocialShiYeTotalAmount=this.LastSocialShiYeTotalAmount+this.SocialShiYeAmount;
            this.SocialYiLiaoTotalAmount=this.LastSocialYiLiaoTotalAmount+this.SocialYiLiaoAmount;
            this.SocialDaBingTotalAmount=this.LastSocialDaBingTotalAmount+this.SocialDaBingAmount;
            this.SocialGongJiJinTotalAmount=this.LastSocialGongJiJinTotalAmount+this.SocialGongJiJinAmount;
            this.SocialEmpTotalAmount=this.LastSocialEmpTotalAmount+this.SocialEmpSumAmount;
            // 每月5000免税额度
            this.SpBasicTotalAmount = 5000;
            this.SpBasicTotalAmount = this.TaxTimes * 5000;



            //
            this.TaxSocialTotalAmount =
                this.SocialYangLaoTotalAmount
                + this.SocialShiYeTotalAmount
                + this.SocialYiLiaoTotalAmount
                + this.SocialDaBingTotalAmount
                + this.SocialGongJiJinTotalAmount;


            // 专项扣除汇总
            this.TaxSpTotalAmount =
                this.TaxSpZiNvTotalAmount
                + this.TaxSpJiaoYuTotalAmount
                + this.TaxSpDaiKuanTotalAmount
                + this.TaxSpZuJinTotalAmount
                + this.TaxSpShanYangTotalAmount
                + this.TaxSpYingErTotalAmount;

            // 需要缴纳个税的金额
            this.TaxSalaryTotalAmount =
                this.BeforeTaxSalaryTotalAmount
                - this.SpBasicTotalAmount
                - this.TaxSpTotalAmount
                - this.TaxSocialTotalAmount;
            if (TaxSalaryTotalAmount > 0)
            {
                if (this.TaxSalaryTotalAmount <= 36000)
                {
                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.03");
                }
                else if (this.TaxSalaryTotalAmount <= 144000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 36000) * decimal.Parse("0.1");
                }
                else if (this.TaxSalaryTotalAmount <= 300000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 144000) * decimal.Parse("0.2");
                }
                else if (this.TaxSalaryTotalAmount <= 420000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (300000 - 144000) * (decimal)0.2;
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 300000) * decimal.Parse("0.25");
                }
                else if (this.TaxSalaryTotalAmount <= 660000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (300000 - 144000) * (decimal)0.2;
                    TaxTotalAmount += (420000 - 300000) * decimal.Parse("0.25");
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 420000) * decimal.Parse("0.30");
                }
                else if (this.TaxSalaryTotalAmount <= 960000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (300000 - 144000) * (decimal)0.2;
                    TaxTotalAmount += (420000 - 300000) * decimal.Parse("0.25");
                    TaxTotalAmount += (660000 - 420000) * decimal.Parse("0.30"); ;
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 660000) * decimal.Parse("0.35");
                }
                else
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (300000 - 144000) * (decimal)0.2;
                    TaxTotalAmount += (420000 - 300000) * decimal.Parse("0.25");
                    TaxTotalAmount += (660000 - 420000) * decimal.Parse("0.30"); ;
                    TaxTotalAmount += (960000 - 660000) * decimal.Parse("0.35");
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 960000) * decimal.Parse("0.45");
                }
            }
            else
            {

                if (today.Year < 2026)
                {
                    TaxSalaryTotalAmount = 0;
                }
                TaxTotalAmount = 0;
            }

            TaxTotalAmount = decimal.Round(TaxTotalAmount, 2, MidpointRounding.AwayFromZero);

            if (TaxTotalAmount >= LastTaxTotalAmount)
            {
                TaxMonthAmount = TaxTotalAmount - LastTaxTotalAmount;
            }
            else
            {
                TaxMonthAmount = 0;
                TaxTotalAmount = LastTaxTotalAmount;
            }
            TaxMonthAmount = decimal.Round(TaxMonthAmount, 2, MidpointRounding.AwayFromZero);
            // 实发工资
            this.ActualSalaryAmount =
                UnitSalaryAmount // 企业应发工资
                +SubsidyAmount // 税前补贴
                - TaxMonthAmount// 当月个税
                - UnitDeductionAmount// 企业扣款
                - DeductionNoTaxAmount// 税后扣除
                -SocialEmpSumAmount // 减去社保个人部分
                + SubsidyNoTaxAmount;// 税后补贴


            this.ActualSalarySendCardAmount = this.ActualSalaryAmount;


            //if (this.IsDelayPayment)
            //{
            //    this.DelayPaymentAmount = this.ActualSalarySendCardAmount;
            //    this.ActualSalarySendCardAmount = 0;
            //}
            //else {

            //    this.DelayPaymentAmount = 0;
            //}


            if (this.SalaryRecordType == Enums.SalaryRecordTypeEnumType.常规工资)
            {

                if (this.LeaveDate != null && this.LeaveDate.Value.Year > 2000)
                {
                    if (this.EntryDate.Value.Year == this.LeaveDate.Value.Year && this.EntryDate.Value.Month == this.LeaveDate.Value.Month)
                    {
                        this.MonthWorkDays = this.LeaveDate.Value.Day - this.EntryDate.Value.Day;
                    }
                    else
                    {
                        this.MonthWorkDays = this.LeaveDate.Value.Day - 1;
                    }
                }
                else
                {
                    this.MonthWorkDays = DateTime.DaysInMonth(this.SalaryMonth.Value.Year, this.SalaryMonth.Value.Month);
                    // 首月入职
                    if (this.SalaryMonth.Value.Year == this.EntryDate.Value.Year && this.SalaryMonth.Value.Month == this.EntryDate.Value.Month)
                    {
                        this.MonthWorkDays = this.MonthWorkDays - this.EntryDate.Value.Day + 1;
                    }
                }
            }
        }

        public void CalculationNoTax()
        {

            this.BeforeTaxSalaryAmount = this.UnitSalaryAmount + this.SubsidyAmount;
            this.TaxTotalAmount = 0;
            this.TaxMonthAmount = 0;
            this.ActualSalaryAmount = this.BeforeTaxSalaryAmount - this.TaxSocialTotalAmount;
            // 扣除个税、延迟发放、还款、税后扣除
        



            // 社保个人部分金额
            this.SocialEmpSumAmount =
                this.SocialYangLaoAmount
                + this.SocialShiYeAmount
                + this.SocialYiLiaoAmount
                + this.SocialDaBingAmount
                + this.SocialGongJiJinAmount;



            this.ActualSalaryAmount =
                 UnitSalaryAmount // 企业应发工资
                 - TaxMonthAmount// 当月个税
                 - UnitDeductionAmount// 企业扣款
                 - DeductionNoTaxAmount// 税后扣除
                 - SocialEmpSumAmount // 减去社保个人部分
                 + SubsidyNoTaxAmount;// 税后补贴


            this.ActualSalarySendCardAmount = this.ActualSalaryAmount;

        }

        public void CalculationSalaryYearPersionalTax()
        {
            // 税前工资
            this.BeforeTaxSalaryAmount = this.UnitSalaryAmount + this.SubsidyAmount;

            this.BeforeTaxSalaryTotalAmount = this.LastBeforeTaxSalaryTotalAmount + this.BeforeTaxSalaryAmount;
            // 实收企业工资合计
            this.CompanySalarySumAmount = this.UnitSalaryAmount - this.UnitDeductionAmount;
            // 社保个人部分金额
            this.SocialEmpSumAmount = 0;



            this.SocialYangLaoTotalAmount = 0;
            this.SocialShiYeTotalAmount = 0;
            this.SocialYiLiaoTotalAmount = 0;
            this.SocialDaBingTotalAmount = 0;
            this.SocialGongJiJinTotalAmount = 0;


            this.SocialEmpTotalAmount = 0;



            // 每月5000免税额度
            this.SpBasicTotalAmount = 0;
            this.SpBasicTotalAmount = 0;



            //
            this.TaxSocialTotalAmount =
                this.SocialYangLaoTotalAmount
                + this.SocialShiYeTotalAmount
                + this.SocialYiLiaoTotalAmount
                + this.SocialDaBingTotalAmount
                + this.SocialGongJiJinTotalAmount;


            // 专项扣除汇总
            this.TaxSpTotalAmount =
                this.TaxSpZiNvTotalAmount
                + this.TaxSpJiaoYuTotalAmount
                + this.TaxSpDaiKuanTotalAmount
                + this.TaxSpZuJinTotalAmount
                + this.TaxSpShanYangTotalAmount
                + this.TaxSpYingErTotalAmount;

            // 需要缴纳个税的金额
            this.TaxSalaryTotalAmount = this.BeforeTaxSalaryTotalAmount;
            
            if (TaxSalaryTotalAmount > 0)
            {
                if (this.TaxSalaryTotalAmount <= 36000)
                {
                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.03");
                }
                else if (this.TaxSalaryTotalAmount <= 144000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 36000) * decimal.Parse("0.1");
                }
                else if (this.TaxSalaryTotalAmount <= 300000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 144000) * decimal.Parse("0.2");
                }
                else if (this.TaxSalaryTotalAmount <= 420000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (300000 - 144000) * (decimal)0.2;
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 300000) * decimal.Parse("0.25");
                }
                else if (this.TaxSalaryTotalAmount <= 660000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (300000 - 144000) * (decimal)0.2;
                    TaxTotalAmount += (420000 - 300000) * decimal.Parse("0.25");
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 420000) * decimal.Parse("0.30");
                }
                else if (this.TaxSalaryTotalAmount <= 960000)
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (300000 - 144000) * (decimal)0.2;
                    TaxTotalAmount += (420000 - 300000) * decimal.Parse("0.25");
                    TaxTotalAmount += (660000 - 420000) * decimal.Parse("0.30"); ;
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 660000) * decimal.Parse("0.35");
                }
                else
                {
                    TaxTotalAmount = 36000 * (decimal)0.03;
                    TaxTotalAmount += (144000 - 36000) * (decimal)0.1;
                    TaxTotalAmount += (300000 - 144000) * (decimal)0.2;
                    TaxTotalAmount += (420000 - 300000) * decimal.Parse("0.25");
                    TaxTotalAmount += (660000 - 420000) * decimal.Parse("0.30"); ;
                    TaxTotalAmount += (960000 - 660000) * decimal.Parse("0.35");
                    TaxTotalAmount += (this.TaxSalaryTotalAmount - 960000) * decimal.Parse("0.45");
                }
            }
            else
            {
                TaxSalaryTotalAmount = 0;
                TaxTotalAmount = 0;
            }


            TaxTotalAmount = decimal.Round(TaxTotalAmount, 2, MidpointRounding.AwayFromZero);
            if (TaxTotalAmount >= LastTaxTotalAmount)
            {
                TaxMonthAmount = TaxTotalAmount - LastTaxTotalAmount;
            }
            else
            {
                TaxMonthAmount = 0;
                TaxTotalAmount = LastTaxTotalAmount;
            }
            // 实发工资
            this.ActualSalaryAmount =
                UnitSalaryAmount // 企业应发工资
                  + SubsidyAmount // 税前补贴
                - TaxMonthAmount// 当月个税
                - UnitDeductionAmount// 企业扣款
                - DeductionNoTaxAmount// 税后扣除
                - SocialEmpSumAmount// 税后扣除
                + SubsidyNoTaxAmount;// 税后补贴
            this.ActualSalarySendCardAmount = this.ActualSalaryAmount;
            //if (this.IsDelayPayment)
            //{
            //    this.DelayPaymentAmount = this.ActualSalaryAmount;
            //    this.ActualSalarySendCardAmount = 0;
            //}
        }


        // 年终奖新的计算方式
        public void CalculationSalaryYearPersionalTaxNew()
        {
            // 税前工资
            this.BeforeTaxSalaryAmount = this.UnitSalaryAmount + this.SubsidyAmount;

            this.BeforeTaxSalaryTotalAmount = this.LastBeforeTaxSalaryTotalAmount + this.BeforeTaxSalaryAmount;
            // 实收企业工资合计
            this.CompanySalarySumAmount = this.UnitSalaryAmount - this.UnitDeductionAmount;
            // 社保个人部分金额
            this.SocialEmpSumAmount = 0;



            this.SocialYangLaoTotalAmount = 0;
            this.SocialShiYeTotalAmount = 0;
            this.SocialYiLiaoTotalAmount = 0;
            this.SocialDaBingTotalAmount = 0;
            this.SocialGongJiJinTotalAmount = 0;


            this.SocialEmpTotalAmount = 0;



            // 每月5000免税额度
            this.SpBasicTotalAmount = 0;
            this.SpBasicTotalAmount = 0;



            //
            this.TaxSocialTotalAmount =
                this.SocialYangLaoTotalAmount
                + this.SocialShiYeTotalAmount
                + this.SocialYiLiaoTotalAmount
                + this.SocialDaBingTotalAmount
                + this.SocialGongJiJinTotalAmount;


            // 专项扣除汇总
            this.TaxSpTotalAmount =
                this.TaxSpZiNvTotalAmount
                + this.TaxSpJiaoYuTotalAmount
                + this.TaxSpDaiKuanTotalAmount
                + this.TaxSpZuJinTotalAmount
                + this.TaxSpShanYangTotalAmount
                + this.TaxSpYingErTotalAmount;

            // 需要缴纳个税的金额
            this.TaxSalaryTotalAmount = this.BeforeTaxSalaryTotalAmount;

            if (TaxSalaryTotalAmount > 0)
            {
                if (this.TaxSalaryTotalAmount <= 36000)
                {
                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.03");
                }
                else if (this.TaxSalaryTotalAmount <= 144000)
                {

                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.1");
                    this.TaxTotalAmount -= 210;
                }
                else if (this.TaxSalaryTotalAmount <= 300000)
                {
                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.2");
                    this.TaxTotalAmount -= 1410;
                  
                }
                else if (this.TaxSalaryTotalAmount <= 420000)
                {
                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.25");
                    this.TaxTotalAmount -= 2660;
                }
                else if (this.TaxSalaryTotalAmount <= 660000)
                {
                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.30");
                    this.TaxTotalAmount -= 4410;
                }
                else if (this.TaxSalaryTotalAmount <= 960000)
                {
                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.35");
                    this.TaxTotalAmount -= 7160;
                }
                else
                {
                    this.TaxTotalAmount = this.TaxSalaryTotalAmount * decimal.Parse("0.45");
                    this.TaxTotalAmount -= 15160;
                }
            }
            else
            {
                TaxSalaryTotalAmount = 0;
                TaxTotalAmount = 0;
            }
            TaxTotalAmount = decimal.Round(TaxTotalAmount, 2, MidpointRounding.AwayFromZero);
            if (TaxTotalAmount >= LastTaxTotalAmount)
            {
                TaxMonthAmount = TaxTotalAmount - LastTaxTotalAmount;
            }
            else
            {
                TaxMonthAmount = 0;
                TaxTotalAmount = LastTaxTotalAmount;
            }
            // 实发工资
            this.ActualSalaryAmount =
                UnitSalaryAmount // 企业应发工资
                  + SubsidyAmount // 税前补贴
                - TaxMonthAmount// 当月个税
                - UnitDeductionAmount// 企业扣款
                - DeductionNoTaxAmount// 税后扣除
                - SocialEmpSumAmount// 税后扣除
                + SubsidyNoTaxAmount;// 税后补贴
            this.ActualSalarySendCardAmount = this.ActualSalaryAmount;
            //if (this.IsDelayPayment)
            //{
            //    this.DelayPaymentAmount = this.ActualSalaryAmount;
            //    this.ActualSalarySendCardAmount = 0;
            //}
        }

        // 预发工资还款
        public decimal PrepaidRepayment(decimal repaymentAmount)
        {

            if (!this.ChangeAmount.HasValue)
            {
                this.ChangeAmount = 0;
            }

            var salary=this.ActualSalaryAmount-this.PrepaidRepaymentAmount+this.ChangeAmount.Value;

            
            if (salary > 0)
            {
                // 实际到手工资
                if (salary >= repaymentAmount)
                {
                    this.PrepaidRepaymentAmount += repaymentAmount;
                    this.ActualSalarySendCardAmount = this.ActualSalaryAmount - this.PrepaidRepaymentAmount+this.ChangeAmount.Value ;
                    this.IsPrepaidFail = false;
                    return repaymentAmount;
                }
                else
                {
                    decimal repaymentPartAmount = salary;
                    this.PrepaidRepaymentAmount += salary;
                    this.ActualSalarySendCardAmount = this.ActualSalaryAmount - this.PrepaidRepaymentAmount + this.ChangeAmount.Value ;
                    // 本次工资未足额完成预发还款
                    this.IsPrepaidFail = true;
                    return salary;
                }
            }
            else {

                return 0;
            }
        }

        public void SetSend()
        {
            bool isCanSend = true;
            if (this.IsDelayPayment && !this.IsDelayPaymentSuc)
            {
                isCanSend = false;
            }
            if (isCanSend && this.IsCashSend && !this.IsCashSignSuc)
            {
                isCanSend = false;
            }
            this.Status = Enums.SalaryRecordSendStatusEnumType.待发放;
            this.IsLocked = false;
        }


        public void SetLocked(bool isLocked)
        {
            if (isLocked)
            {
                this.IsLocked = true;
                this.LockedTime = DateTime.Now;
                this.Status = Enums.SalaryRecordSendStatusEnumType.待发放;
            }
            else
            {
                this.IsLocked = false;
                this.LockedTime = null;
            }
        }

    
    }
}
