namespace LZ.Model.PagedModel
{
    public class TempPagedModel : PagedModelBase
    {

        /// <summary>
        /// 
        /// </summary>
        public int? NoticeID { get; set; }
        public string Type { get; set; } = string.Empty;
        public string NoticeIDS { get; set; } = string.Empty;
    }
}
