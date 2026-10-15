namespace LZ.Model.PagedModel
{
    public class PagedModelBase
    {
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; } = 15;

        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 
        /// </summary>
        public Guid? PID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; } = 0;
        public int Top { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public Guid? GID { get; set; }
        public int? CID { get; set; }
        public int? ParentID { get; set; }
        public int? SID { get; set; }
        public bool? Enabled { get; set; }
        public bool? Main { get; set; }

        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? CompanyID { get; set; }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword { get; set; } = string.Empty;

        public string Types { get; set; } = string.Empty;
        public string IDS { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string Order { get; set; } = string.Empty;
        public string OrderField { get; set; } = string.Empty;
    }
}
