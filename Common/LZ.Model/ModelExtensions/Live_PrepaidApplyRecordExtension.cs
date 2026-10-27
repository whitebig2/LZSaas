using LZ.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SqlSugar.SugarTable("Live_PrepaidApplyRecord")]
    public class Live_PrepaidApplyRecordExtension : Live_PrepaidApplyRecord
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_PrepaidRepaymentRecord.PrepaidApplyRecordID))]
        public List<Live_PrepaidRepaymentRecord> RepaymentRecords { get; set; }

        public Live_PrepaidRepaymentRecord AddRepaymentRecord(Guid salaryCustomerID,Guid salaryRecordID, int userEntryID,decimal repaymentAmount,string remark)
        {
            Live_PrepaidRepaymentRecord repaymentRecord = new Live_PrepaidRepaymentRecord()
            {
                CreateTime = DateTime.Now,
                RepaymentTime = DateTime.MinValue,
                IsRetrunToAccount = false,
                IsSuc = false,
                Remark = remark,
                PrepaidApplyRecordID = this.ID,
                SalaryCustomerID = salaryCustomerID,
                SalaryRecordID = salaryRecordID,
                TotalAmount = repaymentAmount,
                UserEntryID = userEntryID
            };
            if (repaymentRecord.Remark== "客服人工处理还款")
            {
                repaymentRecord.RepaymentTime = DateTime.Now;
            }

            if (RepaymentRecords == null)
            {
                RepaymentRecords = new List<Live_PrepaidRepaymentRecord>();
            }

            RepaymentRecords.Add(repaymentRecord);
            this.RepayTotalAmount = RepaymentRecords.Sum(x => x.TotalAmount);

            if (this.RepayTotalAmount >= this.ApplyAmount)
            {
                this.RepayStatus = PrepaidRepayStatusEnumType.还款中;
            }
            else {
                this.RepayStatus = PrepaidRepayStatusEnumType.还款失败;
            }

            return repaymentRecord;
        }

        public void Count()
        {

            if (RepaymentRecords != null && RepaymentRecords.Count > 0)
            {
                this.RepayTotalAmount = RepaymentRecords.Sum(x => x.TotalAmount);
                if (this.RepayTotalAmount >= this.ApplyAmount)
                {
                    this.RepayStatus = PrepaidRepayStatusEnumType.还款中;
                }
                else
                {
                    if (this.RepayTotalAmount > 0)
                    {
                        this.RepayStatus = PrepaidRepayStatusEnumType.还款失败;

                    }
                    else
                    {
                        this.RepayStatus = PrepaidRepayStatusEnumType.待还款;
                    }
                }
            }
            else {
                this.RepayTotalAmount = 0;
                this.RepayStatus = PrepaidRepayStatusEnumType.待还款;

            }


        }
    }
}
