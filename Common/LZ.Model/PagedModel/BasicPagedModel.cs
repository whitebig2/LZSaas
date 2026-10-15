namespace LZ.Model.PagedModel
{
    public class BasicPagedModel : PagedModelBase
    {
        /// <summary>
        /// 多个id
        /// </summary>
        public string Ids { get; set; } = string.Empty;
        public string ProvinceCode { get; set; } = string.Empty;

        public string CityCode { get; set; } = string.Empty;
        public string BillMonth { get; set; } = string.Empty;
        public bool? Enable { get; set; }
        public bool? IsCombine { get; set; }
        public int? Status { get; set; }
        public int? CompanyID { get; set; }
        public string CardID { get; set; } = string.Empty;
        public int? Month { get; set; }
        public int? GID { get; set; }
        public string MonthType { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Title { get; set; } = string.Empty;

    }
}
