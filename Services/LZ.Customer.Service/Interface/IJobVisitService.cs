using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Customer.Service.Interface
{
    /// <summary>
    /// 岗位访问记录服务（Live_JobsVisit），逻辑对齐商城 LZ.ShopBasic.Service 的 IVisitService：
    /// 岗位详情浏览(Add/Leave，服务端计算停留秒数)、搜索词记录与关键词提取、后台查询/删除。
    /// </summary>
    public interface IJobVisitService
    {
        Task<List<Model.Live_JobsVisit>> List(Expression<Func<Model.Live_JobsVisit, bool>> exp, PagedModelBase query);

        Task<int> Add(Model.Live_JobsVisit model);

        Task<bool> Leave(int id);

        Task<bool> Delete(int id);

        Task<Model.Live_JobsVisit> Get(int id);

        /// <summary>
        /// 从搜索内容提取岗位关键词（命中岗位标题/类目名），写回 model.Keyword，
        /// 唯一命中且无 JobID 时回填 model.JobID。对齐商城 ExtractProductKeywords。
        /// </summary>
        Task ExtractJobKeywords(Model.Live_JobsVisit model);

        Task<List<VisitSearchStat>> SearchStats(Expression<Func<Model.Live_JobsVisit, bool>> exp);
    }
}
