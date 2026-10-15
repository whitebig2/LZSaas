using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos.SettlementDtos
{
    public class SettlementManageDto
    {

     
        public Guid? ID { get; set; }
        public Guid CustomerGID { get; set; }
        public string CustomerName { get; set; }

        /// <summary>
        /// 
        /// </summary>		
     
        public Guid IssuerUnitID { get; set; }

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
     
        public Guid? RecipientUnitID { get; set; }

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
     
        public int BusinessType { get; set; }

        /// <summary>
        /// 
        /// </summary>		
     
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>		
     
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>		
     
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>		
     
        public DateTime? ApplyDate { get; set; }

        /// <summary>
        /// 
        /// </summary>		
     
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>		
     
        public DateTime? SalaryMonth { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public int? SettlementType { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public string Remark { get; set; }
    }
}
