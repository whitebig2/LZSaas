namespace LZ.Model.PagedModel
{
    public class JobPagedModel : PagedModelBase
    {

        /// <summary>
        /// 
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Money { get; set; } = string.Empty;
        public string Tabs { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public bool? XFZShow { get; set; }
        public bool? LWShow { get; set; }
        public bool? IsJoint { get; set; }
        public bool? IsDayOver { get; set; }
        public int? Status { get; set; }
        public int? JCID { get; set; }
        public int? CustomerID { get; set; }
        public int? AttendanceType { get; set; }
        public int? PartnerID { get; set; }
        /// <summary>
        /// 当前纬度
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// 当前经度
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// 搜索半径，单位公里，默认10
        /// </summary>
        public double? Radius { get; set; }
    }
}
