using LZ.Model.Enums;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LZ.Model
{
    [SugarTable("Live_V_Invoice")]
    public class Live_V_InvoiceExtension : Live_V_Invoice
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_InvoiceAttachFile.InvoiceID), nameof(Live_V_Invoice.ID))]
        public List<Live_InvoiceAttachFile> AttachFiles { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Live_InvoiceTaxRecord.InvoiceID), nameof(Live_V_Invoice.ID))]
        public List<Live_InvoiceTaxRecord> TaxRecords { get; set; }
     
    }
}
