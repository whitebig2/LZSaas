namespace LZ.Model.Statistics
{

    public class EntryModel
    {
        /// <summary>
        /// 归属
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// 归属名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        public int? PartnerType { get; set; }
        public int? PartnerLevel { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 在职人数
        /// </summary>
        public int OnTheJobCount { get; set; }
        /// <summary>
        /// 今日入职人数
        /// </summary>
        public int ToDayEntryCount { get; set; }
        /// <summary>
        /// 今日离职人数
        /// </summary>
        public int ToDayLeaveCount { get; set; }
    }
}
