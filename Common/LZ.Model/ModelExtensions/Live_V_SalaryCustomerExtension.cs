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
    public class Live_V_SalaryCustomerExtension:Live_SalaryCustomer
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_SalaryRecordExtension.SalaryCustomerID))]
        public List<Live_SalaryRecordExtension> Records { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Live_SalaryDelaySendRecord.SalaryCustomerID))]
        public List<Live_V_SalaryDelaySendRecord> DelaySendRecords { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Live_SalaryCashSendRecord.SalaryCustomerID))]
        public List<Live_V_SalaryCashSendRecord> CashSendRecords { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Live_Invoice.SalaryCustomerID))]
        public List<Live_Invoice> Invoices { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(Live_Settlement.SalaryCustomerID))]
        public List<Live_Settlement> Settlements { get; set; }



    }
}
