namespace LZ.Model.PagedModel
{
    /// <summary>
    /// 岗位访问记录查询参数（Live_JobsVisit）
    /// </summary>
    public class JobVisitPagedModel : PagedModelBase
    {
        /// <summary>
        /// 岗位ID
        /// </summary>
        public int? JobID { get; set; }

        /// <summary>
        /// 来源类型（VisitFromType：2=搜索 3=岗位浏览）
        /// </summary>
        public int? Type { get; set; }
    }
}
