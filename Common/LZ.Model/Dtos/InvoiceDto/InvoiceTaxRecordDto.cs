using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.InvoiceDto
{
    public class InvoiceTaxRecordDto
    {

        public string ID { get; set; }
        public InvoiceTaxDataTypeEnumType DataType { get; set; }
        public decimal BaseAmount { get; set; } = 0;
        public decimal ValueAddedTaxPointPercent { get; set; } = 0;
        public decimal AdditionalTaxPointPercent { get; set; } = 0;

        public decimal ValueAddedTaxAmount { get; set; } = 0;
        public decimal AdditionalTaxAmount{ get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;

        public void CalculateTax()
        {

            if (string.IsNullOrWhiteSpace(this.ID))
            {
                ID = Guid.NewGuid().ToString();
            }
            else
            {
                if (!Guid.TryParse(this.ID, out Guid existID))
                {
                    ID = Guid.NewGuid().ToString();
                }
            }

            this.ValueAddedTaxAmount= this.BaseAmount*this.ValueAddedTaxPointPercent/100;
            this.AdditionalTaxAmount = this.BaseAmount * this.AdditionalTaxPointPercent / 100;
            this.ValueAddedTaxAmount = decimal.Round(this.ValueAddedTaxAmount, 2, MidpointRounding.AwayFromZero);
            this.AdditionalTaxAmount = decimal.Round(this.AdditionalTaxAmount, 2, MidpointRounding.AwayFromZero);
            this.TotalAmount = this.ValueAddedTaxAmount + this.AdditionalTaxAmount;
        }
    }
}
