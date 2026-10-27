using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ModelExtensions
{
    [SugarTable("Live_ProfitSigningUnitReport")]
    public class Live_ProfitSigningUnitReportExtension : Live_ProfitSigningUnitReport
    {
        [Navigate(NavigateType.OneToOne, nameof(Live_ProfitSigningUnitReportExtension.RiskInvoiceID))]
        public Live_Invoice RiskInvoice { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(Live_ProfitSigningUnitReportExtension.BusinessInvoiceID))]
        public Live_Invoice BusinessInvoice { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(Live_ProfitSigningUnitReportExtension.BusInsuranceSheetID))]
        public Live_Settlement BusInsuranceSheet { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(Live_ProfitSigningUnitReportExtension.RiskSheetID))]
        public Live_Settlement RiskSheet { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(Live_ProfitSigningUnitReportExtension.BusinessSheetID))]
        public Live_Settlement BusinessSheet { get; set; }
    }
}
