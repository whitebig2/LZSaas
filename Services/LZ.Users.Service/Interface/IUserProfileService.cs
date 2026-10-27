using LZ.Model.PagedModel;

namespace LZ.Users.Service.Interface
{
    /// <summary>
    /// 用户画像服务（会员侧）：离线物化画像（Live_UserProfile）的查询与展示。
    /// 画像的计算与落库在岗位匹配服务（JobRecommendService 夜间预计算），
    /// 本服务只负责读取：列表（分页筛选）与详情（偏好解析 + 岗位匹配记录）。
    /// </summary>
    public interface IUserProfileService
    {
        /// <summary>
        /// 画像列表：分页 + 公司/求职状态/工种筛选，按计算时间倒序，附用户姓名手机号。
        /// </summary>
        Task<List<UserProfileListModel>> List(int companyId, byte? seekStatus, int entryJobId, PagedModelBase query);

        /// <summary>
        /// 画像详情（单人）：工种/类目偏好 JSON 解析为明细 + 信誉 + 状态 + 岗位匹配记录。
        /// 优先按 userGID 查询（外部系统/前端常用 GID），未传时按 userID。
        /// 无画像记录返回 null。
        /// </summary>
        Task<UserProfileDetailModel> Get(int userId, Guid? userGid = null);
    }

    /// <summary>用户画像列表行（附用户基础信息）</summary>
    public class UserProfileListModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        /// <summary>Top 工种偏好（如"焊工,电工,普工"）</summary>
        public string WorkTypeTop { get; set; } = string.Empty;
        /// <summary>求职状态：0未知 1在职稳定 2在职看机会 3求职中 4沉默</summary>
        public byte SeekStatus { get; set; }
        /// <summary>流动性：0未知 1稳定型 2流动型</summary>
        public byte MobilityType { get; set; }
        /// <summary>到岗率信誉分 0~1</summary>
        public decimal? CredibilityScore { get; set; }
        /// <summary>入职次数</summary>
        public int HiredCount { get; set; }
        /// <summary>秒退次数</summary>
        public int QuickLeaveCount { get; set; }
        /// <summary>平均在岗天数</summary>
        public decimal? AvgStayDays { get; set; }
        /// <summary>行为推断薪资心理价位</summary>
        public decimal? SalaryLevel { get; set; }
        /// <summary>常干工种风险级众数（3类/4类/5类）</summary>
        public string RiskClassMode { get; set; } = string.Empty;
        /// <summary>最近入职工种</summary>
        public string LastEntryWorkName { get; set; } = string.Empty;
        /// <summary>最近行为时间</summary>
        public DateTime? LastBehaviorTime { get; set; }
        /// <summary>画像计算时间</summary>
        public DateTime? LastCalcTime { get; set; }
    }

    /// <summary>用户画像详情（单人，偏好 JSON 已解析为可读结构）</summary>
    public class UserProfileDetailModel
    {
        public int UserID { get; set; }
        public Guid? UserGID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // ===== 个人信息（取自 Live_User）=====
        /// <summary>生日</summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>年龄（出生日期实时计算，缺失退化 Age 字段）</summary>
        public int? Age { get; set; }
        /// <summary>性别</summary>
        public string Sex { get; set; } = string.Empty;
        /// <summary>身份证号</summary>
        public string CardID { get; set; } = string.Empty;
        /// <summary>籍贯/身份证地址（用户表无独立籍贯字段，取 IdcardAddress）</summary>
        public string NativePlace { get; set; } = string.Empty;
        /// <summary>学历</summary>
        public string Education { get; set; } = string.Empty;
        /// <summary>是否 CA 实名认证</summary>
        public bool? IsCaAuth { get; set; }
        /// <summary>是否身份验证（实名）</summary>
        public bool? IsReal { get; set; }
        /// <summary>证件照</summary>
        public string IdPhoto { get; set; } = string.Empty;
        /// <summary>期望工作地（省/市/区）</summary>
        public string ExpectProvince { get; set; } = string.Empty;
        public string ExpectCity { get; set; } = string.Empty;
        public string ExpectDistrict { get; set; } = string.Empty;
        /// <summary>期望工资区间</summary>
        public decimal? ExpectMinMoney { get; set; }
        public decimal? ExpectMaxMoney { get; set; }
        /// <summary>最近小程序登录地（进入小程序时的省/市/区）</summary>
        public string MiniProvince { get; set; } = string.Empty;
        public string MiniCity { get; set; } = string.Empty;
        public string MiniDistrict { get; set; } = string.Empty;
        /// <summary>技能类型（1普工 2技工）</summary>
        public int? SkillType { get; set; }
        /// <summary>技能</summary>
        public string JobSkills { get; set; } = string.Empty;
        /// <summary>特种作业操作证</summary>
        public string SpecialCertificate { get; set; } = string.Empty;
        /// <summary>标签</summary>
        public string Tabs { get; set; } = string.Empty;
        /// <summary>系统标签</summary>
        public string SysTabs { get; set; } = string.Empty;
        // ===== 画像（取自 Live_UserProfile）=====
        /// <summary>工种偏好明细：EntryJobID + 名称 + 偏好分，按分值降序</summary>
        public List<PrefItem> WorkTypePrefs { get; set; } = [];
        /// <summary>类目偏好明细：JobClassID + 名称 + 偏好分，按分值降序</summary>
        public List<PrefItem> CatePrefs { get; set; } = [];

        public decimal? SalaryLevel { get; set; }
        public decimal? CredibilityScore { get; set; }
        public int HiredCount { get; set; }
        public int AppliedCount { get; set; }
        public int QuickLeaveCount { get; set; }
        public decimal? AvgStayDays { get; set; }
        public string RiskClassMode { get; set; } = string.Empty;
        public byte SeekStatus { get; set; }
        public byte MobilityType { get; set; }
        public DateTime? LastBehaviorTime { get; set; }
        public string LastEntryWorkName { get; set; } = string.Empty;
        public DateTime? LastCalcTime { get; set; }
        /// <summary>该会员的岗位匹配记录（读 Live_JobCandidatePool，过滤下架/过期岗位）</summary>
        public List<JobUserMatchModel> JobMatches { get; set; } = [];
    }

    /// <summary>偏好明细项</summary>
    public class PrefItem
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Score { get; set; }
    }

    /// <summary>会员在某岗位的匹配信息（来自岗位候选人清单物化）</summary>
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
