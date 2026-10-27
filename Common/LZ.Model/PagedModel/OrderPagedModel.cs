namespace LZ.Model.PagedModel
{
    public class OrderPagedModel : PagedModelBase
    {

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Money { get; set; } = string.Empty;
        public string OrderID { get; set; } = string.Empty;
        public string ReturnID { get; set; } = string.Empty;
        public int? OrderType { get; set; }
        public int? UserID { get; set; }
        public DateTime? Time { get; set; }
        public bool? Enable { get; set; }
        public int? Status { get; set; }
        public int? PartnerID { get; set; }
        public int PartnerType { get; set; }
        public string FromType { get; set; } = string.Empty;
        public string CardID { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public string PersonnelName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public bool? Mark { get; set; }
        public string TimeType { get; set; } = string.Empty;
        public string PayType { get; set; } = string.Empty;
        public string ExpressNo { get; set; } = string.Empty;
        public int? AttendanceType { get; set; }
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
        public bool? Assess { get; set; }
    }
}
