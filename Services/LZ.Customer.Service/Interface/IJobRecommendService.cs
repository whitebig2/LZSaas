using LZ.Model;
using LZ.Model.PagedModel;
using System.Linq.Expressions;

namespace LZ.Customer.Service.Interface
{
    /// <summary>
    /// 人岗匹配推荐服务（规则 + 加权打分）。
    ///
    /// 行为信号：Live_JobsVisit 浏览(1.0/1.5,7天半衰期) + 搜索词(2.0,14天半衰期)
    ///           + Live_UserEntry 入职正负样本(干满30天×8.0 / 3-30天×2.0 / 3天内离职×-3.0,30天半衰期)，
    ///           均聚合到 Live_Jobs.Tabs 岗位类目(Live_JobClass，沿 PID 上卷)；
    /// 匹配维度：0.30 类目偏好 + 0.25 工作地(期望工作地优先，无期望用进小程序实时位置/坐标)
    ///           + 0.20 期望薪资区间重叠 + 0.15 期望岗位(User.Jobs) + 0.10 特种证书(SpecialCertificate)；
    /// 硬性过滤：性别、年龄、岗位到期、在职岗位不重复推荐、特种作业岗无证过滤(合规)；
    /// 重排：UpTime 新鲜度加分 + IsQuality 加分 + 头部池打散(SID 稳定翻页)。
    /// </summary>
    public interface IJobRecommendService
    {
        /// <summary>
        /// 个性化岗位推荐。exp 为列表可见性过滤(控制器构建)，服务内追加个人硬过滤后打分排序。
        /// </summary>
        Task<List<Live_V_Jobs>> Recommend(Expression<Func<Live_V_Jobs, bool>> exp, JobPagedModel query, int userId);

        /// <summary>
        /// 反向人岗匹配：给岗位找候选人（经纪人派单场景）。
        /// 优先读岗位候选人清单物化（Live_JobCandidatePool，按 Rank 排序分页），
        /// miss（无物化行）回退实时计算路径。返回候选人 + 匹配信息（工种命中/信誉/求职状态/距离）。
        /// </summary>
        Task<List<MatchedUserModel>> RecommendUsers(int jobId, PagedModelBase query, int companyId);

        /// <summary>
        /// 会员查看推荐岗位：返回该会员在各岗位物化清单中的匹配信息
        /// （Score/Rank/WorkTypeHit/DistanceText），用于推荐列表展示"匹配过的岗位信息"。
        /// jobId &gt; 0 时只返回该岗位的匹配记录。
        /// </summary>
        Task<List<JobUserMatchModel>> GetUserMatches(int userId, int jobId = 0);

        /// <summary>
        /// 夜间离线预计算：① 增量刷新用户画像快照（Live_UserProfile）
        /// ② 全量重建岗位候选人清单（Live_JobCandidatePool，新批次写入后清旧批次）。
        /// 由外部调度（Quartz/Live_Scheduler）经 HTTP 触发。返回统计：profilesUpdated/poolJobs/poolRows。
        /// </summary>
        Task<Dictionary<string, int>> PrecomputeNightly();

        /// <summary>
        /// 单岗位候选人清单实时重算（岗位新增/修改影响匹配字段后触发）：
        /// 原地替换该 JobID 的清单行（BatchID=0, SourceType=1），失败不影响线上（在线接口有实时兜底）。
        /// 返回写入行数；岗位不存在/已过期则清空该岗位清单并返回 0。
        /// </summary>
        Task<int> PrecomputeJob(int jobId);
    }

    /// <summary>候选人 + 匹配信息（派单列表展示）。读物化路径下列表末尾附一个
    /// User.UserID=0 的哨兵行，其 Rank 携带清单总数（分页元数据），调用方应忽略该行。</summary>
    public class MatchedUserModel
    {
        public Live_User User { get; set; }
        /// <summary>匹配总分（0~1）</summary>
        public decimal Score { get; set; }
        /// <summary>岗位内名次（哨兵行=清单总数）</summary>
        public int Rank { get; set; }
        /// <summary>命中的工种名（如"焊工"，空=未命中工种）</summary>
        public string WorkTypeHit { get; set; } = string.Empty;
        /// <summary>到岗率信誉快照</summary>
        public decimal? Credibility { get; set; }
        /// <summary>求职状态快照：0未知 1在职稳定 2在职看机会 3求职中 4沉默</summary>
        public byte? SeekStatus { get; set; }
        /// <summary>距离快照（如"2.3公里"，空=未计算）</summary>
        public string Distance { get; set; } = string.Empty;
        /// <summary>最近入职工种（如"电工"，空=无记录）</summary>
        public string LastEntryWorkName { get; set; } = string.Empty;
        /// <summary>是否来自物化清单（false=实时兜底计算，匹配信息部分缺省）</summary>
        public bool FromPool { get; set; }
    }

    /// <summary>会员在某岗位的匹配信息（推荐岗位列表展示）</summary>
    public class JobUserMatchModel
    {
        public int JobID { get; set; }
        /// <summary>匹配总分</summary>
        public decimal Score { get; set; }
        /// <summary>岗位内名次</summary>
        public int Rank { get; set; }
        /// <summary>命中的工种名</summary>
        public string WorkTypeHit { get; set; } = string.Empty;
        /// <summary>距离快照</summary>
        public string DistanceText { get; set; } = string.Empty;
        /// <summary>最近入职工种</summary>
        public string LastEntryWorkName { get; set; } = string.Empty;
        /// <summary>清单计算时间</summary>
        public DateTime? CalcTime { get; set; }
    }
}
