using LZ.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SqlSugar.SugarTable("Live_Invoice")]
    public class Live_InvoiceExtension:Live_Invoice
    {

        [Navigate(NavigateType.OneToMany, nameof(Live_InvoiceAttachFile.InvoiceID))]
        public List<Live_InvoiceAttachFile> AttachFiles { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(Live_InvoiceTaxRecord.InvoiceID), nameof(Live_Invoice.ID))]
        public List<Live_InvoiceTaxRecord> TaxRecords { get; set; }

        public void AddTaxRecord(decimal baseAmount, InvoiceTaxDataTypeEnumType dataType, decimal valueAddedTaxPoint, decimal additionalTaxPoint, string remark = "")
        {
            if (TaxRecords == null)
            {
                TaxRecords = new List<Live_InvoiceTaxRecord>();
            }
            Live_InvoiceTaxRecord record = new Live_InvoiceTaxRecord()
            {
                DataType = dataType,
                BaseAmount = baseAmount,
                ID = Guid.NewGuid(),
                InvoiceID = this.ID,
                Remark = remark,
                TaxType = 0,
                TotalAmount = 0,
                AdditionalTaxPointPercent = additionalTaxPoint,
                AdditionalTaxAmount = additionalTaxPoint,
                ValueAddedTaxPointPercent = valueAddedTaxPoint,
                ValueAddedTaxAmount = 0,
            };
            record.ValueAddedTaxAmount = record.BaseAmount * record.ValueAddedTaxPointPercent / 100;
            record.ValueAddedTaxAmount = decimal.Round(record.ValueAddedTaxAmount, 2, MidpointRounding.AwayFromZero);
            record.AdditionalTaxAmount = record.BaseAmount * record.AdditionalTaxPointPercent / 100;
            record.AdditionalTaxAmount = decimal.Round(record.AdditionalTaxAmount, 2, MidpointRounding.AwayFromZero);
            record.TotalAmount = record.AdditionalTaxAmount + record.ValueAddedTaxAmount;

            TaxRecords.Add(record);
        }
        public void CalculationTax()
        {


            if (TaxRecords!=null&& TaxRecords.Any())
            {
                foreach (var item in TaxRecords)
                {
                    item.ValueAddedTaxAmount = item.BaseAmount * item.ValueAddedTaxPointPercent / 100;
                    item.AdditionalTaxAmount = item.BaseAmount * item.AdditionalTaxPointPercent / 100;
                    item.ValueAddedTaxAmount = decimal.Round(item.ValueAddedTaxAmount, 2, MidpointRounding.AwayFromZero);
                    item.AdditionalTaxAmount = decimal.Round(item.AdditionalTaxAmount, 2, MidpointRounding.AwayFromZero);
                    item.TotalAmount = item.ValueAddedTaxAmount + item.AdditionalTaxAmount;
                }
            }
            // 增值税
            if (this.IsAddedTax)
            {
                this.AddedTaxAmount = this.InvoiceAmount * this.AddedTaxPointAmount/100;
                this.AddedTaxAmount = decimal.Round(this.AddedTaxAmount, 2, MidpointRounding.AwayFromZero);
                this.AddedTaxSumAmount = this.AddedTaxAmount - this.AddedTaxDiscountAmount;
                if (AddedTaxSumAmount < 0)
                {
                    AddedTaxSumAmount = 0;
                }

              
            }
            // 附加税
            if (this.IsAdditionalTax)
            {
                this.AdditionalTaxAmount = this.InvoiceAmount * this.AdditionalTaxPointAmount/100;
                this.AdditionalTaxAmount = decimal.Round(this.AdditionalTaxAmount, 2, MidpointRounding.AwayFromZero);
                this.AdditionTaxSumAmount = this.AdditionalTaxAmount - this.AdditionalTaxDiscountAmount;
                if (AdditionTaxSumAmount<0)
                {
                    AdditionTaxSumAmount = 0;
                }
            }


            if (this.TaxRecords != null && this.TaxRecords.Any())
            {
                this.AddedTaxSumAmount = this.TaxRecords.Sum(f => f.ValueAddedTaxAmount);
                this.AdditionTaxSumAmount = this.TaxRecords.Sum(f => f.AdditionalTaxAmount);
                this.InvoiceAmount = this.TaxRecords.Sum(f => f.BaseAmount);
            }

            this.TotalAmount=this.InvoiceAmount+this.AdditionTaxSumAmount+this.AddedTaxSumAmount+this.ChangeAmount;
        }


        public void AddAttachFile(string fileName,string fileOssUrl) {

            if (AttachFiles==null)
            {
                AttachFiles=new List<Live_InvoiceAttachFile> ();
            }
            AttachFiles.Add(new Live_InvoiceAttachFile()
            {
                CreateTime = DateTime.Now,
                FileName = fileName,
                FileOssUrl = fileOssUrl,
                FilePath = fileOssUrl,
                ID = Guid.NewGuid(),
            });

        }
    }
}
