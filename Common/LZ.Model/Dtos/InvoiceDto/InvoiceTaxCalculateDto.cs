using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.InvoiceDto
{
    public class InvoiceTaxCalculateDto
    {
  
        public decimal InvoiceAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdditionTaxSumAmount { get; set; }
        public decimal AddedTaxSumAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public List<InvoiceTaxRecordDto> TaxRecords { get; set; }




        public void CalculateTax()
        {
        

            this.ChangeAmount = decimal.Round(this.ChangeAmount, 2, MidpointRounding.AwayFromZero);
            if (TaxRecords!=null&& TaxRecords.Any())
            {
                foreach (var item in TaxRecords)
                {
                    item.CalculateTax();
                }
                this.InvoiceAmount = this.TaxRecords.Sum(f => f.BaseAmount);
                this.AdditionTaxSumAmount = this.TaxRecords.Sum(f => f.AdditionalTaxAmount);
                this.AddedTaxSumAmount = this.TaxRecords.Sum(f => f.ValueAddedTaxAmount);
            }
            this.TotalAmount = this.InvoiceAmount + this.AdditionTaxSumAmount + this.AddedTaxSumAmount + this.ChangeAmount;
            //this.ValueAddedTaAmount = this.BaseAmount * this.ValueAddedTaxPointPercent / 100;
            //this.AdditionalTaxAmount = this.BaseAmount * this.AdditionalTaxPointPercent / 100;
            //this.ValueAddedTaAmount = decimal.Round(this.ValueAddedTaAmount, 2, MidpointRounding.AwayFromZero);
            //this.AdditionalTaxAmount = decimal.Round(this.AdditionalTaxAmount, 2, MidpointRounding.AwayFromZero);
            //this.TotalAmount = this.ValueAddedTaAmount + this.AdditionalTaxAmount;
        }

    }



}
