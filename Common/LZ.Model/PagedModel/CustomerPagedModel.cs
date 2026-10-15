namespace LZ.Model.PagedModel
{
    public class CustomerPagedModel : PagedModelBase
    {

        public bool? IsPerfect { get; set; }
        public int SaleID { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceName { get; set; }
        public string ReceiptName { get; set; }
        public string PartnerName{ get; set; }
        public int PartnerID { get; set; }
        public int? AttendanceType { get; set; }
        public string CooperationUnitGID { get; set; }
        public DateTime? SalaryMonth { get; set; }
        public string TimeType { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
