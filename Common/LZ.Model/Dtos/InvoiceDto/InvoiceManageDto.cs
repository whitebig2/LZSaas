using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.InvoiceDto
{
    public class InvoiceManageDto
    {

      	
        public Guid? ID { get; set; }
        public Guid CustomerGID { get; set; }
        public string CustomerName { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public string IssuerName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string IssuerCompanyNo { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string IssuerPhone { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string IssuerAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string IssuerBank { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string IssuerBankNo { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string RecipientName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string RecipientCompanyNo { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string RecipientPhone { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string RecipientAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string RecipientBank { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string RecipientBankNo { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public int InvoiceType { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public int BusinessType { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public bool IsAddedTax { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public bool IsAdditionalTax { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal AddedTaxAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal AdditionalTaxAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal AddedTaxDiscountAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal AdditionalTaxDiscountAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal ChangeAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public bool IsLocked { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public bool IsIssued { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public int RecordCount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal ServiceFeeAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public string Remark { get; set; }

  

        /// <summary>
        /// 
        /// </summary>		

        public DateTime? SalaryMonth { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public Guid IssuerUnitID { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public Guid RecipientUnitID { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal AddedTaxPointAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal AdditionalTaxPointAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal InvoiceAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal AddedTaxSumAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
       
        public decimal AdditionTaxSumAmount { get; set; }
        public decimal SfAddedTaxAmount { get; set; }
        public decimal SfAddedTaxPointAmount { get; set; }
        public decimal SfPersonalTaxAmount { get; set; }
        public decimal SfPersonalTaxPointAmount { get; set; }
        public bool IsSf { get; set; }
        public decimal SfOtherPaidAmount { get; set; }
        public decimal SfSocialAmount { get; set; }
        public decimal SfTaxRefundAmount { get; set; }

        public List<Guid> AttachFiles { get; set; }

        public List<InvoiceTaxRecordDto> TaxRecords { get; set; }

    }
}
