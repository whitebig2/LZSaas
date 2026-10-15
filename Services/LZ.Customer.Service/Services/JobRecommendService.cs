using LZ.Core;
using LZ.Core.Interface;
using LZ.Customer.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.PagedModel;
using Newtonsoft.Json;
using SqlSugar;
using System.Linq.Expressions;

namespace LZ.Customer.Service.Services
{
    /// <summary>
    /// 人岗匹配推荐服务（规则 + 加权打分）。
    ///
    /// ① 用户画像（GetUserProfile）：三路行为信号带时间半衰期，聚合到岗位类目偏好
    ///    - Live_JobsVisit 浏览：按岗位去重，停留≥30s×1.5 / ≥5s×1.0，7天半衰期，30天窗口
    ///    - Live_JobsVisit 搜索词：命中类目名×2.0，14天半衰期，14天窗口
    ///    - Live_UserEntry 入职：在职/干满30天×8.0、3-30天×2.0、3天内离职×-3.0，30天半衰期，90天窗口；
    ///      面试/待入职等投递状态×4.0，面试未通过/未入职×-1.0
    ///    偏好沿 Live_JobClass PID 上卷（父类×0.5），岗位侧 Tabs 同样展开到祖先求和；
    /// ② 硬性过滤（SQL）：性别、年龄(按出生日期实时计算)、岗位到期；内存：在职岗位不重复推荐、
    ///    特种作业技工岗要求证书而工人无证时过滤(合规红线，开关 FilterNoCertJobs)；
    /// ③ 加权打分：0.30 类目偏好 + 0.25 工作地(期望工作地优先：区1.0/市0.6/省0.3；
    ///    无期望时用进小程序坐标按距离分档 ≤5km 1.0 / ≤10km 0.8 / ≤20km 0.55 / ≤30km 0.35 / 其他0.15，
    ///    无坐标退化为 MiniDistrict/MiniCity 行政区匹配)
    ///    + 0.20 期望薪资(区间重叠度，岗位薪资缺省按 3000/6000 兜底)
    ///    + 0.15 期望岗位(User.Jobs vs Tabs，含父类命中半分)
    ///    + 0.10 特种证书(技工岗要求且持证 1.0 / 技工岗未识别要求 0.5~0.8 / 普工岗 0.6)；
    ///    冷启动(无任何行为)时类目偏好回退为期望岗位命中(与期望维合并权重 0.45)；
    /// ④ 重排：UpTime 新鲜度加分(≤0.05,30天半衰期) + IsQuality 加分(0.03) + 头部池打散
    ///    (query.SID 有值时固定种子，翻页顺序稳定)；
    /// ⑤ 召回上限 MaxCandidates 条，只 SELECT 打分所需列（视图列多，避免重蹈 V_UserEntry 全列慢查询）。
    /// </summary>
    public class JobRecommendService(LZ.Core.Redis.IRedisService redisService) : ServicesBase<Live_Jobs>, IJobRecommendService, IScopedDependency
    {
        // ===== 行为权重 =====
        private const double VisitWeight = 1.0;              // 有效浏览(≥5s)
        private const double VisitLongWeight = 1.5;          // 认真浏览(≥30s)
        private const double SearchWeight = 2.0;             // 搜索词命中类目
        private const double ApplyWeight = 4.0;              // 面试/待入职等投递信号
        private const double EntryLongWeight = 8.0;          // 干满30天（最强正样本）
        private const double EntryShortWeight = 2.0;         // 3~30天离职（弱正样本）
        private const double EntryQuickLeaveWeight = -3.0;   // 3天内离职（负样本）
        private const double ApplyFailedWeight = -1.0;       // 面试未通过/通知未入职（弱负样本）

        // ===== 时间窗口 / 半衰期 =====
        private const int VisitWindowDays = 30;
        private const int SearchWindowDays = 14;
        private const int EntryWindowDays = 90;
        private const double VisitHalfLifeDays = 7.0;
        private const double SearchHalfLifeDays = 14.0;
        private const double EntryHalfLifeDays = 30.0;
        private const double FreshHalfLifeDays = 30.0;

        // ===== 打分维度权重 =====（工种主轴 Live_EntryJob + 类目辅助轴 JobClass；User.Jobs=JobClass ID 仅用于期望岗位维度）
        private const double WeightWorkType = 0.25;  // 工种偏好（入职 WorkName / 客户 CustomerJobs，主轴）
        private const double WeightCate = 0.10;      // 类目偏好（Tabs→JobClass，辅助轴）
        private const double WeightGeo = 0.25;       // 工作地匹配
        private const double WeightSalary = 0.15;    // 期望薪资区间重叠
        private const double WeightExpect = 0.15;    // 期望岗位（User.Jobs vs Tabs，JobClass 体系）
        private const double WeightCert = 0.10;      // 特种证书/技能

        /// <summary>中性分：该维度无信息时的默认值</summary>
        private const double NeutralScore = 0.5;

        // ===== 重排参数 =====
        private const double FreshnessBonusMax = 0.05;  // UpTime 新鲜度加分上限
        private const double QualityBonus = 0.03;       // 优质岗位加分

        // ===== 类目上卷 / 召回规模 =====
        private const double ParentRollupFactor = 0.5;  // 偏好沿 JobClass PID 上卷系数
        private const int MaxClassDepth = 5;            // 类目树上卷最大深度（防环）
        private const int MaxCandidates = 1000;         // 岗位召回上限（只取打分所需列）
        private const int MaxCandidateUsers = 500;      // 候选人活跃召回上限（按 LastTime 活跃倒序）
        private const int MaxAffinityUsers = 500;       // 候选人类目亲和召回上限（近期看过/干过同类岗位）
        private const int MaxRelatedJobs = 1000;        // 亲和召回时同类目岗位数上限

        // ===== 浏览有效性 =====
        private const int ValidVisitSeconds = 5;        // 停留≥5s 才算有效浏览
        private const int LongVisitSeconds = 30;        // 停留≥30s 算认真浏览

        // ===== 离线预计算参数 =====
        private const int MaxPoolPerJob = 200;          // 每岗位物化候选人上限（TopN）
        private const int PrecomputeUserBatch = 200;    // 画像全量分批大小（游标循环，全部用户每日重算）
        private const int PoolInsertBatch = 500;        // 清单批量插入大小
        private const int StableStayDays = 90;          // 稳定型判定：平均在岗≥90天
        private const int MobileStayDays = 30;          // 流动型判定：平均在岗<30天
        private const int MaxCompanyCandidates = 5000;  // 单公司建池候选人上限（按 LastTime 取）

        // ===== 清单写入锁（防夜批/MQ 并发写同岗位撞主键）=====
        private static readonly TimeSpan NightLockTtl = TimeSpan.FromMinutes(30);   // 夜批防重入锁 TTL（跑批时长×2 兜底）
        private static readonly TimeSpan JobLockTtl = TimeSpan.FromSeconds(60);     // 岗位级写锁 TTL（单岗位事务秒级）

        /// <summary>夜批防重入锁 key（按天，跨天自然失效）</summary>
        private static string NightLockKey(DateTime now) => $"job:pool:night:{now:yyyyMMdd}";

        /// <summary>岗位清单写锁 key</summary>
        private static string JobLockKey(int jobId) => $"job:pool:job:{jobId}";

        /// <summary>SETNX 获取锁：true=拿到；false=他人持有（跳过）</summary>
        private async Task<bool> AcquireLock(string key, TimeSpan ttl)
        {
            return await redisService.SetAsync(key, Guid.NewGuid().ToString("N"), ttl, CSRedis.RedisExistence.Nx);
        }

        /// <summary>释放锁</summary>
        private async Task ReleaseLock(string key)
        {
            await redisService.DeleteAsync(key);
        }

        /// <summary>特种作业证书字典：证书名 → 岗位文本关键词（岗位侧暂无结构化证书要求字段，用 Title/JobSkills/WorkContent 识别）</summary>
        private static readonly (string Cert, string[] Keywords)[] CertKeywords =
        [
            ("电工证", ["电工", "电气", "配电"]),
            ("焊工证", ["焊工", "电焊", "气焊", "氩弧焊"]),
            ("叉车证", ["叉车"]),
            ("高处作业证", ["高空", "登高", "架子工", "外墙"]),
            ("制冷证", ["制冷", "冷库"]),
            ("电梯证", ["电梯"]),
        ];

        /// <summary>特种作业岗无证直接过滤（安全生产合规红线，默认开启）</summary>
        private const bool FilterNoCertJobs = true;

        /// <summary>投递/面试中状态（视为中等强度正信号）</summary>
        private static readonly HashSet<int> ApplyStatuses =
        [
            (int)UserStatus.Interviewed,      // 已面试
            (int)UserStatus.WaitEntry,        // 待入职
            (int)UserStatus.InterviewTrack,   // 面试跟踪中
            (int)UserStatus.WaitInterview,    // 待面试
            (int)UserStatus.NoticeEntry,      // 通知填写入职信息
            (int)UserStatus.WaitScanCode,     // 待扫码
        ];

        #region 对外入口

        public async Task<List<Live_V_Jobs>> Recommend(Expression<Func<Live_V_Jobs, bool>> exp, JobPagedModel query, int userId)
        {
            var now = DateTime.Now;
            var expable = Expressionable.Create<Live_V_Jobs>();
            if (exp != null)
            {
                expable.And(exp);
            }

            // 岗位有效期（过期岗位不推荐；列表侧 VerifyJob 有自动续期逻辑兜底）
            expable.And(it => it.EndTime == null || it.EndTime >= now);

            // 用户信息 + 个人硬性过滤（性别/年龄）
            var user = userId > 0
                ? await Context.Queryable<Live_User>()
                    .Where(it => it.UserID == userId)
                    .Select(it => new UserInfo
                    {
                        Sex = it.Sex,
                        BirthDate = it.BirthDate,
                        Age = it.Age,
                        ExpectProvince = it.Province,
                        ExpectCity = it.City,
                        ExpectDistrict = it.District,
                        MiniCity = it.MiniCity,
                        MiniDistrict = it.MiniDistrict,
                        Latitude = it.Latitude,
                        Longitude = it.Longitude,
                        MinMoney = it.MinMoney,
                        MaxMoney = it.MaxMoney,
                        Jobs = it.Jobs,
                        SpecialCertificate = it.SpecialCertificate
                    })
                    .FirstAsync()
                : null;
            ApplyHardFilters(expable, user, now);

            // 召回：只取打分所需列（视图 JOIN 多表，勿全列查询），按 ID 倒序取上限
            var candidates = await Context.Queryable<Live_V_Jobs>()
                .Where(expable.ToExpression())
                .OrderByDescending(it => it.ID)
                .Take(MaxCandidates)
                .Select(it => new CandidateJob
                {
                    ID = it.ID,
                    Title = it.Title,
                    CustomerID = it.CustomerID,
                    Tabs = it.Tabs,
                    Sex = it.Sex,
                    MinAge = it.MinAge,
                    MaxAge = it.MaxAge,
                    MinMoney = it.MinMoney,
                    MaxMoney = it.MaxMoney,
                    Province = it.Province,
                    City = it.City,
                    District = it.District,
                    JobProvince = it.JobProvince,
                    JobCity = it.JobCity,
                    JobDistrict = it.JobDistrict,
                    Latitude = it.Latitude,
                    Longitude = it.Longitude,
                    WorkContent = it.WorkContent,
                    SkillType = it.SkillType,
                    JobSkills = it.JobSkills,
                    IsQuality = it.IsQuality,
                    IsDayOver = it.IsDayOver,
                    UpTime = it.UpTime
                })
                .ToListAsync();

            if (candidates.Count == 0)
            {
                query.Count = 0;
                return [];
            }

            // 匹配字典（JobClass 树 + EntryJob 工种字典 + 客户工种关联），小表整表加载
            var dicts = await LoadDictionaries();

            // 技能文本（Title + JobSkills + WorkContent）用于证书关键词匹配；
            // 岗位工种集合 = 所属客户关联的工种（Live_CustomerJobs → Live_EntryJob）
            foreach (var job in candidates)
            {
                job.SkillText = string.Concat(job.Title, " ", job.JobSkills, " ", job.WorkContent);
                job.EntryJobIds = job.CustomerID.HasValue
                    ? dicts.CustomerEntryJobs.GetValueOrDefault(job.CustomerID.Value) ?? []
                    : [];
            }

            // 用户画像（浏览/搜索/入职 → 工种+类目偏好；在职岗位集合）
            var profile = await GetUserProfile(userId, now, dicts);

            // 在职岗位不再推荐
            if (profile.CurrentJobIds.Count > 0)
            {
                candidates = candidates.Where(it => !profile.CurrentJobIds.Contains(it.ID)).ToList();
            }

            // 特种作业岗无证硬过滤（合规红线）
            if (FilterNoCertJobs)
            {
                var userCerts = user?.SpecialCertificate;
                candidates = candidates.Where(it => !IsMissingRequiredCert(it, userCerts)).ToList();
            }

            // 地区强关联精滤：SQL 单向 Contains 的漏网在此按双向包含复核
            if (user != null && HasValue(user.ExpectCity))
            {
                var expectCity = user.ExpectCity;
                candidates = candidates
                    .Where(it => !RegionMismatch(expectCity, FirstText(it.JobCity, it.City)))
                    .ToList();
            }

            if (candidates.Count == 0)
            {
                query.Count = 0;
                return [];
            }

            var ranked = Rank(candidates, profile, user, query, now);
            query.Count = ranked.Count;

            // 内存分页（Page==0 返回全量，沿用现有约定；召回本身有上限保护）
            List<int> pageIds;
            if (query.Page == 0)
            {
                pageIds = ranked.Select(it => it.ID).ToList();
            }
            else
            {
                var page = query.Page < 1 ? 1 : query.Page;
                var pageSize = query.PageSize < 1 ? 15 : query.PageSize;
                pageIds = ranked.Skip((page - 1) * pageSize).Take(pageSize).Select(it => it.ID).ToList();
            }
            if (pageIds.Count == 0)
            {
                return [];
            }

            var models = await Context.Queryable<Live_V_Jobs>()
                .Where(it => pageIds.Contains(it.ID))
                .ToListAsync();

            // 有实时坐标时回填距离展示字段
            if (user is { Latitude: not null, Longitude: not null })
            {
                foreach (var model in models)
                {
                    if (model.Latitude.HasValue && model.Longitude.HasValue)
                    {
                        var km = GetDistanceKm(user.Latitude.Value, user.Longitude.Value, model.Latitude.Value, model.Longitude.Value);
                        model.Distance = FormatDistance(km);
                    }
                }
            }

            return pageIds
                .Select(id => models.Find(m => m.ID == id))
                .Where(it => it != null)
                .Select(it => it!)
                .ToList();
        }

        // 浏览/搜索埋点写入已迁至 JobVisitService（VisitController 调用），本服务只读 Live_JobsVisit 做画像聚合。

        /// <summary>
        /// 反向人岗匹配：给岗位找候选人（经纪人派单场景）。
        ///
        /// ① 岗位要求硬过滤（SQL）：性别（未知性别保留，便于后续人工确认）、年龄、平台公司范围；
        ///    内存：黑名单/禁用已在 SQL 过滤，特种作业岗无证过滤（合规）；
        /// ② 召回上限 MaxCandidateUsers 条，按 LastTime 活跃度倒序取近活跃用户；
        /// ③ 打分 = 0.25 岗位偏好亲和（候选人对该岗位 Tabs 类目的浏览/入职行为，带半衰期）
        ///    + 0.20 到岗率信誉（入职率 / 秒退惩罚，Live_UserEntry 全量聚合）
        ///    + 0.20 地理（期望工作地→实时位置 vs 岗位工作地/坐标）
        ///    + 0.15 薪资匹配 + 0.10 期望岗位 + 0.10 特种证书；
        /// ④ 头部池打散（SID 稳定翻页），内存分页后回表取整行。
        /// </summary>
        public async Task<List<MatchedUserModel>> RecommendUsers(int jobId, PagedModelBase query, int companyId)
        {
            // ── 优先读物化清单（岗位有效性在线校验，防展示已下架岗位）──
            var jobView = await Context.Queryable<Live_V_Jobs>()
                .Where(it => it.ID == jobId)
                .FirstAsync();
            if (jobView == null || (jobView.EndTime.HasValue && jobView.EndTime < DateTime.Now))
            {
                query.Count = 0;
                return [];
            }

            var poolModels = await ReadPoolPage(jobId, query);
            if (poolModels.Count > 0)
            {
                // 哨兵行（User.UserID=0）的 Rank = 清单总数，取完移除
                query.Count = poolModels[^1].User is { UserID: 0 } ? poolModels[^1].Rank : poolModels.Count;
                if (poolModels[^1].User is { UserID: 0 })
                {
                    poolModels.RemoveAt(poolModels.Count - 1);
                }
                return poolModels;
            }

            // ── 物化 miss（夜批未跑/岗位刚发布实时计算未完成）→ 实时兜底 ──
            return await RecommendUsersRealtime(jobId, query, companyId, jobView);
        }

        /// <summary>读物化清单一页：覆盖索引范围读（JobID+Rank），附匹配信息；空=miss 需走实时兜底。
        /// 返回列表末尾附一个 User.UserID=0 的哨兵行，其 Rank 字段携带总数（分页元数据），调用方取 PoolTotal。</summary>
        private async Task<List<MatchedUserModel>> ReadPoolPage(int jobId, PagedModelBase query)
        {
            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize < 1 ? 15 : query.PageSize;

            var count = await Context.Queryable<Live_JobCandidatePool>()
                .Where(it => it.JobID == jobId)
                .CountAsync();
            if (count == 0)
            {
                return [];
            }

            var rows = query.Page == 0
                ? await Context.Queryable<Live_JobCandidatePool>()
                    .Where(it => it.JobID == jobId)
                    .OrderBy(it => it.Rank)
                    .Take(query.Top > 0 ? query.Top : MaxPoolPerJob)
                    .ToListAsync()
                : await Context.Queryable<Live_JobCandidatePool>()
                    .Where(it => it.JobID == jobId)
                    .OrderBy(it => it.Rank)
                    .ToOffsetPageAsync(page, pageSize);
            if (rows.Count == 0)
            {
                return [];
            }

            // 回表取用户（校验仍启用且非黑名单）
            var userIds = rows.Select(it => it.UserID).ToList();
            var users = await Context.Queryable<Live_User>()
                .Where(it => userIds.Contains(it.UserID) && it.Enabled && !it.IsBlack)
                .ToListAsync();

            var result = new List<MatchedUserModel>();
            foreach (var row in rows)
            {
                var user = users.Find(u => u.UserID == row.UserID);
                if (user == null)
                {
                    continue;
                }
                result.Add(new MatchedUserModel
                {
                    User = user,
                    Score = row.Score,
                    Rank = row.Rank,
                    WorkTypeHit = row.WorkTypeHit ?? string.Empty,
                    Credibility = row.CredibilityScore,
                    SeekStatus = row.SeekStatus,
                    Distance = row.DistanceText ?? string.Empty,
                    LastEntryWorkName = row.LastEntryWorkName ?? string.Empty,
                    FromPool = true
                });
            }
            // 总数以哨兵行带回（User.UserID=0，Rank=总数），调用方据此设置 query.Count
            result.Add(new MatchedUserModel
            {
                User = new Live_User { UserID = 0 },
                Rank = count,
                FromPool = true
            });
            return result;
        }

        /// <summary>实时兜底：原实时计算路径（硬过滤召回+打分），匹配信息部分缺省</summary>
        private async Task<List<MatchedUserModel>> RecommendUsersRealtime(int jobId, PagedModelBase query, int companyId, Live_V_Jobs jobView)
        {
            var now = DateTime.Now;
            var jobCandidate = new CandidateJob
            {
                ID = jobView.ID,
                Title = jobView.Title,
                Tabs = jobView.Tabs,
                MinMoney = jobView.MinMoney,
                MaxMoney = jobView.MaxMoney,
                Province = jobView.Province,
                City = jobView.City,
                District = jobView.District,
                JobProvince = jobView.JobProvince,
                JobCity = jobView.JobCity,
                JobDistrict = jobView.JobDistrict,
                Latitude = jobView.Latitude,
                Longitude = jobView.Longitude,
                WorkContent = jobView.WorkContent,
                SkillType = jobView.SkillType,
                SkillText = string.Concat(jobView.Title, " ", jobView.JobSkills, " ", jobView.WorkContent)
            };
            var requiredCerts = RequiredCerts(jobCandidate.SkillText);

            // 匹配字典（JobClass 树 + EntryJob 工种字典 + 客户工种关联）
            var dicts = await LoadDictionaries();

            // 岗位工种集合 = 所属客户关联的工种（Live_CustomerJobs → Live_EntryJob，反向匹配主轴）
            jobCandidate.EntryJobIds = jobView.CustomerID.HasValue
                ? dicts.CustomerEntryJobs.GetValueOrDefault(jobView.CustomerID.Value) ?? []
                : [];

            // ① 召回：启用 + 非黑名单 + 平台公司范围 + 性别/年龄粗滤（按 LastTime 活跃倒序取上限）
            var scopeCompany = companyId > 0 ? companyId : jobView.CompanyID;
            var userExp = Expressionable.Create<Live_User>();
            userExp.And(it => it.Enabled && !it.IsBlack);
            if (scopeCompany > 0)
            {
                userExp.And(it => it.CompanyID == scopeCompany);
            }
            if (!string.IsNullOrEmpty(jobView.Sex) && jobView.Sex != "不限")
            {
                var jobSex = jobView.Sex;
                userExp.And(it => it.Sex == null || it.Sex == "" || it.Sex == "不限" || it.Sex == jobSex);
            }
            if (jobView.MinAge.HasValue || jobView.MaxAge.HasValue)
            {
                var minAge = jobView.MinAge ?? 0;
                var maxAge = jobView.MaxAge ?? 200;
                userExp.And(it => it.Age == null || (it.Age >= minAge && it.Age <= maxAge));
            }

            // 召回 A：最近活跃（LastTime 倒序取上限）
            var users = await Context.Queryable<Live_User>()
                .Where(userExp.ToExpression())
                .OrderByDescending(it => it.LastTime)
                .Take(MaxCandidateUsers)
                .Select(CandidateUserSelect)
                .ToListAsync();

            // 召回 B：工种/类目亲和——近期入职过目标客户工种、或浏览/入职过同类目岗位的用户。
            // 活跃召回会漏掉"匹配度高但最近没打开小程序"的人，而这恰是派单最不该漏的候选。
            users = users.Concat(await RecallAffinityUsers(userExp, users, jobCandidate, dicts, now)).ToList();

            if (users.Count == 0)
            {
                query.Count = 0;
                return [];
            }

            // 内存精滤：出生日期实时算年龄（Age 冗余字段缺失时兜底）；特种作业岗无证过滤
            if (requiredCerts.Count > 0 && FilterNoCertJobs)
            {
                users = users.Where(u => !string.IsNullOrEmpty(u.SpecialCertificate)
                    && requiredCerts.Any(cert => u.SpecialCertificate!.Contains(cert))).ToList();
            }
            if (jobView.MinAge.HasValue || jobView.MaxAge.HasValue)
            {
                users = users.Where(u =>
                {
                    var age = GetUserAge(u, now);
                    return age.HasValue
                        && (jobView.MinAge == null || age >= jobView.MinAge)
                        && (jobView.MaxAge == null || age <= jobView.MaxAge);
                }).ToList();
            }

            // 地区强关联（市级硬过滤，与正向对称）：期望工作地与岗位城市不符的候选人排除
            var jobCityText = FirstText(jobCandidate.JobCity, jobCandidate.City);
            if (HasValue(jobCityText))
            {
                users = users.Where(u => !RegionMismatch(u.ExpectCity, jobCityText)).ToList();
            }
            if (users.Count == 0)
            {
                query.Count = 0;
                return [];
            }

            // ② 岗位工种/类目亲和 + 到岗率信誉（批量拉候选人行为）
            var userIds = users.Select(u => u.UserID).ToList();
            var affinity = await GetJobAffinity(userIds, jobCandidate, dicts, now);
            var credibility = await GetUserCredibility(userIds, now);

            // ③ 打分
            var ranked = new List<RankedUser>(users.Count);
            foreach (var u in users)
            {
                var info = new UserInfo
                {
                    Sex = u.Sex,
                    BirthDate = u.BirthDate,
                    Age = u.Age,
                    ExpectProvince = u.ExpectProvince,
                    ExpectCity = u.ExpectCity,
                    ExpectDistrict = u.ExpectDistrict,
                    MiniCity = u.MiniCity,
                    MiniDistrict = u.MiniDistrict,
                    Latitude = u.Latitude,
                    Longitude = u.Longitude,
                    MinMoney = u.MinMoney,
                    MaxMoney = u.MaxMoney,
                    Jobs = u.Jobs,
                    SpecialCertificate = u.SpecialCertificate
                };

                // 岗位偏好亲和（工种直配 + 类目重叠行为强度），归一到 [0,1]
                var aff = 0d;
                var affMax = affinity.Values.Count > 0 ? affinity.Values.Max() : 0;
                if (affMax > 0)
                {
                    aff = Math.Min(affinity.GetValueOrDefault(u.UserID) / affMax, 1.0);
                }

                // 到岗率信誉：入职率 + 秒退惩罚 + 平均在岗天数加成
                var cred = credibility.GetValueOrDefault(u.UserID);

                var geo = ScoreGeo(jobCandidate, info);
                var salary = ScoreSalary(jobCandidate, info);

                // 期望岗位命中
                var expect = 0d;
                if (!string.IsNullOrEmpty(u.Jobs))
                {
                    var expectIds = new HashSet<int>();
                    foreach (var id in ParseIds(u.Jobs))
                    {
                        foreach (var expanded in ExpandIds(id, dicts.ClassMap))
                        {
                            expectIds.Add(expanded);
                        }
                    }
                    expect = ScoreExpect(jobCandidate, expectIds, dicts.ClassMap);
                }
                else
                {
                    expect = NeutralScore;
                }

                var cert = ScoreCert(jobCandidate, info);

                var score = 0.25 * aff
                    + 0.20 * cred.Score
                    + 0.20 * geo
                    + 0.15 * salary
                    + 0.10 * expect
                    + 0.10 * cert;

                ranked.Add(new RankedUser { UserID = u.UserID, Score = score });
            }

            var ordered = ranked
                .OrderByDescending(it => it.Score)
                .ThenBy(it => it.UserID)
                .ToList();
            var shuffled = ShuffleRanked(ordered, query.PageSize, query.SID);
            query.Count = shuffled.Count;

            // 内存分页（Page==0 全量，沿用现有约定）
            List<int> pageIds;
            if (query.Page == 0)
            {
                pageIds = shuffled.Select(it => it.UserID).ToList();
            }
            else
            {
                var page = query.Page < 1 ? 1 : query.Page;
                var pageSize = query.PageSize < 1 ? 15 : query.PageSize;
                pageIds = shuffled.Skip((page - 1) * pageSize).Take(pageSize).Select(it => it.UserID).ToList();
            }
            if (pageIds.Count == 0)
            {
                return [];
            }

            var scoreRank = new Dictionary<int, (double score, int rank)>();
            for (var i = 0; i < ordered.Count; i++)
            {
                scoreRank[ordered[i].UserID] = (ordered[i].Score, i + 1);
            }

            var models = await Context.Queryable<Live_User>()
                .Where(it => pageIds.Contains(it.UserID))
                .ToListAsync();
            return pageIds
                .Select(id => models.Find(m => m.UserID == id))
                .Where(it => it != null)
                .Select(it => it!)
                .Select(u =>
                {
                    var sr = scoreRank.GetValueOrDefault(u.UserID);
                    var cred = credibility.GetValueOrDefault(u.UserID);
                    return new MatchedUserModel
                    {
                        User = u,
                        Score = (decimal)sr.score,
                        Rank = sr.rank,
                        Credibility = (decimal)cred.Score,
                        FromPool = false
                    };
                })
                .ToList();
        }

        /// <summary>
        /// 会员查看推荐岗位：返回该会员在各岗位物化清单（Live_JobCandidatePool）中的匹配信息。
        /// jobId>0 时只查该岗位；否则查该会员全部匹配岗位（按匹配分降序）。
        /// </summary>
        public async Task<List<JobUserMatchModel>> GetUserMatches(int userId, int jobId = 0)
        {
            if (userId <= 0)
            {
                return [];
            }

            var qb = Context.Queryable<Live_JobCandidatePool>()
                .Where(it => it.UserID == userId);
            if (jobId > 0)
            {
                qb.Where(it => it.JobID == jobId);
            }

            var rows = await qb
                .OrderByDescending(it => it.Score)
                .Take(MaxPoolPerJob)
                .Select(it => new JobUserMatchModel
                {
                    JobID = it.JobID,
                    Score = it.Score,
                    Rank = it.Rank,
                    WorkTypeHit = it.WorkTypeHit,
                    DistanceText = it.DistanceText,
                    LastEntryWorkName = it.LastEntryWorkName,
                    CalcTime = it.CalcTime
                })
                .ToListAsync();

            if (rows.Count == 0)
            {
                return [];
            }

            // 过滤失效岗位（下架/过期），防展示不可投递岗位
            var jobIds = rows.Select(it => it.JobID).Distinct().ToList();
            var activeJobs = await Context.Queryable<Live_Jobs>()
                .Where(it => jobIds.Contains(it.ID)
                    && (it.EndTime == null || it.EndTime >= DateTime.Now))
                .Select(it => it.ID)
                .ToListAsync();
            var activeIds = activeJobs.ToHashSet();
            return rows.Where(it => activeIds.Contains(it.JobID)).ToList();
        }
        // 画像列表/详情查询已迁至会员服务（LZ.Users.Service/UserProfileService，Users.API/UserProfileController）。

        /// <summary>
        /// 岗位亲和（工种直配 + 类目重叠）：候选人近期浏览(30天)/入职(90天)行为与目标岗位的关联强度，
        /// userId → 亲和分（浏览×1.0 认真×1.5，入职按在岗天数 8.0/2.0/-3.0，带半衰期）。
        /// 工种直配（主路）：入职工种 WorkName ∈ 目标客户工种集合（Live_EntryJob，不依赖 JID/Tabs）；
        /// 类目重叠（辅路）：行为岗位 Tabs 与目标岗位 Tabs（叶子+祖先）重合，或行为岗位客户的工种集合与目标重合。
        /// </summary>
        private async Task<Dictionary<int, double>> GetJobAffinity(List<int> userIds, CandidateJob job, MatchDictionaries dicts, DateTime now)
        {
            var result = new Dictionary<int, double>();
            var jobTabIds = ExpandIds(ParseIds(job.Tabs), dicts.ClassMap);
            var targetWorkIds = job.EntryJobIds.ToHashSet();
            if ((jobTabIds.Count == 0 && targetWorkIds.Count == 0) || userIds.Count == 0)
            {
                return result;
            }

            var visitFrom = now.AddDays(-VisitWindowDays);
            var entryFrom = now.AddDays(-EntryWindowDays);

            // 近期浏览的岗位
            var visits = await Context.Queryable<Live_JobsVisit>()
                .Where(it => userIds.Contains(it.UserID.Value) && it.JobID != null && it.JobID > 0
                    && (it.StartTime >= visitFrom || it.CreateTime >= visitFrom))
                .Select(it => new { it.UserID, it.JobID, it.StartTime, it.CreateTime, it.Seconds })
                .ToListAsync();

            // 近期入职记录（工种直配不要求 JID，历史/导入数据全覆盖）
            var entries = await Context.Queryable<Live_UserEntry>()
                .Where(it => userIds.Contains(it.UserID.Value)
                    && (it.EntryTime >= entryFrom || it.CreateTime >= entryFrom))
                .Select(it => new { it.UserID, it.JID, it.Status, it.EntryTime, it.LeaveTime, it.Days, it.CreateTime, it.WorkName })
                .ToListAsync();

            var relatedJobIds = visits.Select(it => it.JobID!.Value)
                .Concat(entries.Select(it => it.JID ?? 0))
                .Where(v => v > 0)
                .Distinct()
                .ToList();
            var tabsExpanded = new Dictionary<int, HashSet<int>>();
            var jobCustomerMap = new Dictionary<int, int?>();
            if (relatedJobIds.Count > 0)
            {
                var tabsRows = await Context.Queryable<Live_Jobs>()
                    .Where(it => relatedJobIds.Contains(it.ID))
                    .Select(it => new { it.ID, it.Tabs, it.CustomerID })
                    .ToListAsync();
                tabsExpanded = tabsRows.ToDictionary(
                    kv => kv.ID,
                    kv => ExpandIds(ParseIds(kv.Tabs), dicts.ClassMap));
                jobCustomerMap = tabsRows.ToDictionary(it => it.ID, it => it.CustomerID);
            }

            // 岗位与目标关联：Tabs 类目重叠，或所属客户工种集合与目标工种集合重叠
            bool Overlaps(int jobId2)
            {
                if (jobTabIds.Count > 0 && tabsExpanded.TryGetValue(jobId2, out var ids) && ids.Any(jobTabIds.Contains))
                {
                    return true;
                }
                if (targetWorkIds.Count > 0 && jobCustomerMap.TryGetValue(jobId2, out var customer) && customer.HasValue)
                {
                    var workIds = dicts.CustomerEntryJobs.GetValueOrDefault(customer.Value);
                    return workIds != null && workIds.Any(targetWorkIds.Contains);
                }
                return false;
            }

            // 入职工种直配（主路）：WorkName 反查 EntryJob ID ∈ 目标客户工种集合
            bool DirectWorkTypeMatch(string? workName)
            {
                return !string.IsNullOrEmpty(workName)
                    && dicts.EntryJobIdsByName.TryGetValue(workName, out var wid)
                    && targetWorkIds.Contains(wid);
            }

            foreach (var group in visits.GroupBy(it => (it.UserID!.Value, it.JobID!.Value)))
            {
                if (!Overlaps(group.Key.Item2))
                {
                    continue;
                }
                var latest = group.OrderByDescending(v => v.StartTime ?? v.CreateTime ?? now).First();
                var seconds = group.Max(v => v.Seconds ?? 0);
                var time = latest.StartTime ?? latest.CreateTime ?? now;
                var decay = Math.Pow(0.5, (now - time).TotalDays / VisitHalfLifeDays);
                var weight = seconds >= LongVisitSeconds ? VisitLongWeight
                    : seconds >= ValidVisitSeconds ? VisitWeight
                    : seconds <= 0 && group.All(v => !v.Seconds.HasValue) ? VisitWeight * 0.6
                    : 0;
                if (weight > 0)
                {
                    result[group.Key.Item1] = result.GetValueOrDefault(group.Key.Item1) + weight * decay;
                }
            }

            foreach (var entry in entries)
            {
                var jid = entry.JID ?? 0;
                // 工种直配命中，或行为岗位与目标关联（类目/客户工种重叠）
                if (!DirectWorkTypeMatch(entry.WorkName) && (jid <= 0 || !Overlaps(jid)))
                {
                    continue;
                }
                var status = entry.Status ?? 0;
                if (status != (int)UserStatus.Entry && status != (int)UserStatus.Leave && !ApplyStatuses.Contains(status))
                {
                    continue;
                }
                var weight = status == (int)UserStatus.Entry ? EntryLongWeight
                    : status == (int)UserStatus.Leave
                        ? (entry.Days >= 30 ? EntryLongWeight : entry.Days >= 3 ? EntryShortWeight : EntryQuickLeaveWeight)
                    : ApplyWeight;
                var time = status == (int)UserStatus.Leave ? (entry.LeaveTime ?? entry.EntryTime ?? entry.CreateTime)
                    : (entry.EntryTime ?? entry.CreateTime);
                var decay = Decay(time, now, EntryHalfLifeDays);
                result[entry.UserID!.Value] = result.GetValueOrDefault(entry.UserID!.Value) + weight * decay;
            }

            return result;
        }

        /// <summary>
        /// 到岗率信誉（全量聚合，不设时间窗——历史爽约是长期信号）：
        /// score = 0.4 基础分 + 0.6 × min(1, 入职数/投递总数) - 0.3 × 秒退占比，平均在岗≥30天再加成 0.1。
        /// </summary>
        private async Task<Dictionary<int, UserCredibility>> GetUserCredibility(List<int> userIds, DateTime now)
        {
            var result = new Dictionary<int, UserCredibility>();
            if (userIds.Count == 0)
            {
                return result;
            }

            var entries = await Context.Queryable<Live_UserEntry>()
                .Where(it => userIds.Contains(it.UserID.Value) && it.JID != null && it.JID > 0)
                .Select(it => new { it.UserID, it.Status, it.Days })
                .ToListAsync();

            var stats = entries.GroupBy(it => it.UserID!.Value).ToDictionary(
                g => g.Key,
                g => new
                {
                    Hired = g.Count(e => e.Status == (int)UserStatus.Entry || e.Status == (int)UserStatus.Leave),
                    Applied = g.Count(e => ApplyStatuses.Contains(e.Status ?? 0)),
                    QuickLeave = g.Count(e => e.Status == (int)UserStatus.Leave && (e.Days ?? 0) < 3),
                    AvgDays = g.Where(e => e.Status == (int)UserStatus.Leave && e.Days > 0)
                        .Select(e => e.Days!.Value)
                        .DefaultIfEmpty(0)
                        .Average()
                });

            foreach (var userId in userIds)
            {
                if (!stats.TryGetValue(userId, out var s))
                {
                    // 无任何记录：中性偏保守
                    result[userId] = new UserCredibility { Score = 0.4, Hired = 0, Applied = 0, QuickLeave = 0 };
                    continue;
                }
                var total = s.Hired + s.Applied;
                var score = 0.4;
                if (s.Hired > 0)
                {
                    score += 0.6 * Math.Min(1d, (double)s.Hired / Math.Max(total, 1));
                    score -= 0.3 * ((double)s.QuickLeave / s.Hired);
                    if (s.AvgDays >= 30)
                    {
                        score += 0.1;
                    }
                }
                result[userId] = new UserCredibility
                {
                    Score = Math.Clamp(score, 0, 1),
                    Hired = s.Hired,
                    Applied = s.Applied,
                    QuickLeave = s.QuickLeave,
                    AvgStayDays = (double)s.AvgDays
                };
            }
            return result;
        }

        #region 离线预计算（夜间批 + 岗位发布实时触发）

        /// <summary>
        /// 夜间离线预计算入口：
        /// ① 增量刷新用户画像快照（Live_UserProfile，当天有 Visit/Entry 变化的用户）；
        /// ② 全量重建岗位候选人清单（Live_JobCandidatePool）：TRUNCATE 清场后逐岗位
        ///    [岗位锁 → 事务(DELETE+INSERT)] 原地替换（单岗位失败不拖垮整批）。
        ///
        /// 写入安全设计（防主键冲突）：
        /// - 夜批入口 SETNX 防重入（TTL 30min 兜底进程崩溃）；
        /// - 每岗位独立事务删旧插新——任何时刻单岗位行不存在跨批次共存；
        /// - 夜批优先：MQ 实时触发拿不到岗位锁即跳过（夜批结果即最新）；
        /// - TRUNCATE 开场，历史残留/半批数据天然归零；在线接口 miss 走实时兜底。
        /// </summary>
        public async Task<Dictionary<string, int>> PrecomputeNightly()
        {
            var now = DateTime.Now;
            var stats = new Dictionary<string, int>();

            // ⓪ 夜批防重入（调度重复触发/上次未跑完）：拿不到锁直接返回 skipped
            if (!await AcquireLock(NightLockKey(now), NightLockTtl))
            {
                stats["skipped"] = 1;
                return stats;
            }

            try
            {
                // ① 画像增量（清单依赖画像，必须先跑）
                stats["profilesUpdated"] = await PrecomputeProfiles(now);

                // ② 内存建池（逻辑不变，含全部去重）
                var rows = (await BuildAllJobPools(now))
                    .GroupBy(it => new { it.JobID, it.UserID })
                    .Select(g => g.OrderBy(it => it.Rank).First())
                    .ToList();
                var rowsByJob = rows
                    .GroupBy(it => it.JobID)
                    .ToDictionary(g => g.Key, g => g.ToList());
                var batchId = await GetNextPoolBatchId();

                // ③ 清场：TRUNCATE 秒级清空，历史残留/半批数据归零（在线接口走实时兜底）
                await Context.Ado.ExecuteCommandAsync("TRUNCATE TABLE Live_JobCandidatePool");

                // ④ 逐岗位落库：岗位锁 + 事务（DELETE+INSERT 原子替换）
                var poolJobs = 0;
                var poolRows = 0;
                var failedJobs = 0;
                foreach (var (jobId, jobRows) in rowsByJob)
                {
                    if (!await AcquireLock(JobLockKey(jobId), JobLockTtl))
                    {
                        // MQ 实时触发正在重算该岗位：让位跳过（夜批优先策略下极少发生）
                        continue;
                    }
                    try
                    {
                        await itenant.BeginTranAsync();
                        await Context.Deleteable<Live_JobCandidatePool>().Where(it => it.JobID == jobId).ExecuteCommandAsync();
                        foreach (var row in jobRows)
                        {
                            row.BatchID = batchId;
                        }
                        await Context.Insertable(jobRows).ExecuteCommandAsync();
                        await itenant.CommitTranAsync();
                        poolJobs++;
                        poolRows += jobRows.Count;
                    }
                    catch (Exception ex)
                    {
                        itenant.RollbackTran();
                        failedJobs++;
                        Common.StaticLog.Error($"夜批建池岗位{jobId}落库失败：{ex.Message}");
                    }
                    finally
                    {
                        await ReleaseLock(JobLockKey(jobId));
                    }
                }

                // ⑤ 漏网清理：本次未处理的岗位（下架但 MQ 失效事件丢失等）清空清单
                if (rowsByJob.Count > 0)
                {
                    var keepIds = rowsByJob.Keys.ToList();
                    await Context.Deleteable<Live_JobCandidatePool>().Where(it => !keepIds.Contains(it.JobID)).ExecuteCommandAsync();
                }

                stats["poolJobs"] = poolJobs;
                stats["poolRows"] = poolRows;
                stats["failedJobs"] = failedJobs;
                return stats;
            }
            finally
            {
                await ReleaseLock(NightLockKey(now));
            }
        }

        /// <summary>
        /// 单岗位候选人清单实时重算（岗位新增/修改影响匹配字段后触发）：
        /// 岗位锁 + 事务原地替换该 JobID 的清单行（BatchID=0, SourceType=1）。
        /// 夜批优先：拿不到岗位锁（夜批正在写该岗位）返回 -1，调用方记日志跳过、不重试——夜批结果即最新。
        /// 岗位不存在/已过期则清空并返回 0。
        /// </summary>
        public async Task<int> PrecomputeJob(int jobId)
        {
            var now = DateTime.Now;
            if (jobId <= 0)
            {
                return 0;
            }

            // 岗位写锁：夜批持锁时让位（-1，跳过不重试）
            if (!await AcquireLock(JobLockKey(jobId), JobLockTtl))
            {
                return -1;
            }

            try
            {
                var jobView = await Context.Queryable<Live_V_Jobs>()
                    .Where(it => it.ID == jobId)
                    .FirstAsync();
                if (jobView == null || (jobView.EndTime.HasValue && jobView.EndTime < now))
                {
                    // 岗位不存在/已过期：事务清空清单
                    await itenant.BeginTranAsync();
                    await Context.Deleteable<Live_JobCandidatePool>().Where(it => it.JobID == jobId).ExecuteCommandAsync();
                    await itenant.CommitTranAsync();
                    return 0;
                }

                var dicts = await LoadDictionaries();
                var job = ToCandidateJob(jobView, dicts);
                var candidates = await LoadCompanyCandidates(jobView.CompanyID ?? 0, dicts);

                // 该岗位在职名单（排除已在该岗位的人）
                var currentUsers = await Context.Queryable<Live_UserEntry>()
                    .Where(it => it.JID == jobId && it.Status == (int)UserStatus.Entry)
                    .Select(it => it.UserID ?? 0)
                    .ToListAsync();
                var currentAtJob = currentUsers.Select(uid => (uid, jobId)).ToHashSet();

                var rows = BuildPoolRowsForJob(job, candidates, currentAtJob, dicts, now, sourceType: 1)
                    .GroupBy(it => new { it.JobID, it.UserID })
                    .Select(g => g.OrderBy(it => it.Rank).First())
                    .ToList();

                await itenant.BeginTranAsync();
                await Context.Deleteable<Live_JobCandidatePool>().Where(it => it.JobID == jobId).ExecuteCommandAsync();
                if (rows.Count > 0)
                {
                    await Context.Insertable(rows).ExecuteCommandAsync();
                }
                await itenant.CommitTranAsync();
                return rows.Count;
            }
            catch
            {
                itenant.RollbackTran();
                throw;
            }
            finally
            {
                await ReleaseLock(JobLockKey(jobId));
            }
        }

        /// <summary>
        /// 画像全量刷新：对全部 Live_User 逐批重算画像快照落库（删旧插新）。
        /// 全量而非增量的原因：画像状态字段（SeekStatus 求职状态等）是时间的函数——
        /// 无新行为的用户，其"近30天活跃"判定也会随时间流逝而过期，需每日重新判定。
        /// 分批游标循环（按 UserID 升序，每批 PrecomputeUserBatch），单批失败不影响整体。
        /// </summary>
        private async Task<int> PrecomputeProfiles(DateTime now)
        {
            var dicts = await LoadDictionaries();
            var updated = 0;
            var cursor = 0;   // UserID 游标（Live_User.UserID 自增，>0）

            while (true)
            {
                // 一批用户（游标分页，稳定不漏不重）
                var batch = await Context.Queryable<Live_User>()
                    .Where(it => it.UserID > cursor)
                    .OrderBy(it => it.UserID)
                    .Take(PrecomputeUserBatch)
                    .Select(it => it.UserID)
                    .ToListAsync();
                if (batch.Count == 0)
                {
                    break;
                }

                try
                {
                    updated += await PrecomputeProfileBatch(batch, dicts, now);
                }
                catch (Exception ex)
                {
                    // 单批失败继续下一批（画像停留旧版，次日全量重算自愈）
                    Common.StaticLog.Error($"画像全量刷新批次失败(cursor={cursor}, {batch.Count}人)：{ex.Message}");
                }

                cursor = batch[^1];
                if (batch.Count < PrecomputeUserBatch)
                {
                    break;
                }
            }
            return updated;
        }

        /// <summary>一批用户的画像计算与落库（删旧插新）</summary>
        private async Task<int> PrecomputeProfileBatch(List<int> userIds, MatchDictionaries dicts, DateTime now)
        {
            // 用户基础信息（公司归属/GID；UserID 主键本应唯一，GroupBy 兜底防脏数据）
            var users = await Context.Queryable<Live_User>()
                .Where(it => userIds.Contains(it.UserID))
                .Select(it => new { it.UserID, it.CompanyID, it.UserGID })
                .ToListAsync();
            var companyMap = users
                .GroupBy(it => it.UserID)
                .ToDictionary(g => g.Key, g => g.First().CompanyID);
            var userGidMap = users
                .GroupBy(it => it.UserID)
                .ToDictionary(g => g.Key, g => g.First().UserGID);

            // 行为画像（复用在线画像逻辑：工种/类目偏好 + 在职岗位集合）
            var profiles = new Dictionary<int, UserProfile>();
            foreach (var uid in userIds)
            {
                profiles[uid] = await GetUserProfile(uid, now, dicts);
            }

            // 到岗率信誉（批量）
            var credibility = await GetUserCredibility(userIds, now);

            // 近 30 天行为计数（7/30 天）+ 最近行为时间（状态层判定输入）
            var behaviorFrom = now.AddDays(-30);
            var behaviors = await Context.Queryable<Live_JobsVisit>()
                .Where(it => userIds.Contains(it.UserID.Value)
                    && (it.StartTime >= behaviorFrom || it.CreateTime >= behaviorFrom))
                .Select(it => new { it.UserID, it.StartTime, it.CreateTime })
                .ToListAsync();
            var visit7 = new Dictionary<int, int>();
            var visit30 = new Dictionary<int, int>();
            var lastBehavior = new Dictionary<int, DateTime>();
            var from7 = now.AddDays(-7);
            foreach (var b in behaviors)
            {
                var uid = b.UserID ?? 0;
                if (uid <= 0)
                {
                    continue;
                }
                var time = b.StartTime ?? b.CreateTime ?? now;
                visit30[uid] = visit30.GetValueOrDefault(uid) + 1;
                if (time >= from7)
                {
                    visit7[uid] = visit7.GetValueOrDefault(uid) + 1;
                }
                if (time > lastBehavior.GetValueOrDefault(uid))
                {
                    lastBehavior[uid] = time;
                }
            }

            // 近 90 天入职附加信息：工种名/风险级/薪资锚点
            var entryFrom = now.AddDays(-EntryWindowDays);
            var entries = await Context.Queryable<Live_UserEntry>()
                .Where(it => userIds.Contains(it.UserID.Value)
                    && (it.EntryTime >= entryFrom || it.CreateTime >= entryFrom))
                .Select(it => new { it.UserID, it.JID, it.WorkName, it.WorkClass, it.EntryTime, it.CreateTime })
                .ToListAsync();

            // 行为岗位的薪资（入职 JID + 有效浏览 JobID → 岗位薪资中值，入职权重 8 / 浏览权重 1）
            var behaviorJobIds = entries.Select(it => it.JID ?? 0).Where(v => v > 0).Distinct().ToList();
            var salaryFrom = now.AddDays(-VisitWindowDays);
            var visitedJobIds = await Context.Queryable<Live_JobsVisit>()
                .Where(it => userIds.Contains(it.UserID.Value) && it.JobID != null && it.JobID > 0
                    && it.Seconds >= ValidVisitSeconds
                    && (it.StartTime >= salaryFrom || it.CreateTime >= salaryFrom))
                .Select(it => it.JobID.Value)
                .Distinct()
                .ToListAsync();
            var salaryJobIds = behaviorJobIds.Concat(visitedJobIds).Distinct().ToList();
            var salaryMap = new Dictionary<int, double>();
            if (salaryJobIds.Count > 0)
            {
                var salaryRows = await Context.Queryable<Live_Jobs>()
                    .Where(it => salaryJobIds.Contains(it.ID))
                    .Select(it => new { it.ID, it.MinMoney, it.MaxMoney })
                    .ToListAsync();
                foreach (var row in salaryRows)
                {
                    var min = row.MinMoney is > 0 ? row.MinMoney.Value : 3000;
                    var max = row.MaxMoney is > 0 ? row.MaxMoney.Value : 6000;
                    if (max < min)
                    {
                        max = min;
                    }
                    salaryMap[row.ID] = (min + max) / 2.0;
                }
            }

            var rows = new List<Live_UserProfile>(userIds.Count);
            foreach (var uid in userIds)
            {
                var profile = profiles.GetValueOrDefault(uid);
                if (profile == null)
                {
                    continue;
                }
                var cred = credibility.GetValueOrDefault(uid);

                // 工种偏好 JSON + Top3 工种名
                var workJson = profile.WorkTypePreference.Count > 0
                    ? JsonConvert.SerializeObject(profile.WorkTypePreference)
                    : null;
                var workTop = profile.WorkTypePreference.Count > 0
                    ? string.Join(",", profile.WorkTypePreference
                        .OrderByDescending(it => it.Value)
                        .Take(3)
                        .Select(it => dicts.EntryJobNames.GetValueOrDefault(it.Key, it.Key.ToString())))
                    : null;
                var cateJson = profile.CatePreference.Count > 0
                    ? JsonConvert.SerializeObject(profile.CatePreference)
                    : null;

                // 薪资心理价位：入职岗位中值×8 + 浏览岗位中值×1 的加权均值
                decimal? salaryLevel = null;
                var salaryEntries = entries.Where(it => (it.UserID ?? 0) == uid && (it.JID ?? 0) > 0 && salaryMap.ContainsKey(it.JID!.Value)).ToList();
                var salaryVisits = visitedJobIds.Select(jid => (jid, salaryMap.GetValueOrDefault(jid))).ToList();
                if (salaryEntries.Count > 0)
                {
                    var weighted = 0d;
                    var weights = 0d;
                    foreach (var entry in salaryEntries)
                    {
                        weighted += salaryMap[entry.JID!.Value] * EntryLongWeight;
                        weights += EntryLongWeight;
                    }
                    foreach (var visit in salaryVisits.Where(v => v.Item2 > 0))
                    {
                        weighted += visit.Item2 * VisitWeight;
                        weights += VisitWeight;
                    }
                    if (weights > 0)
                    {
                        salaryLevel = (decimal)(weighted / weights);
                    }
                }

                // 风险级众数 + 最近入职工种
                var userEntries = entries.Where(it => (it.UserID ?? 0) == uid).ToList();
                var riskMode = userEntries
                    .Where(it => !string.IsNullOrEmpty(it.WorkClass))
                    .GroupBy(it => it.WorkClass)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();
                var lastEntry = userEntries
                    .OrderByDescending(it => it.EntryTime ?? it.CreateTime ?? DateTime.MinValue)
                    .FirstOrDefault();

                // 求职状态：在职(看近7天浏览量) / 求职中(近30天有活跃) / 沉默 / 未知
                byte seekStatus;
                if (profile.CurrentJobIds.Count > 0)
                {
                    seekStatus = visit7.GetValueOrDefault(uid) >= 3 ? (byte)2 : (byte)1;
                }
                else if (visit30.GetValueOrDefault(uid) > 0 || userEntries.Count > 0)
                {
                    seekStatus = 3;
                }
                else if (lastBehavior.ContainsKey(uid) || profile.HasBehavior)
                {
                    seekStatus = 4;
                }
                else
                {
                    seekStatus = 0;
                }

                // 流动性：平均在岗天数
                byte mobility = 0;   // 0=未知
                if (cred.AvgStayDays >= StableStayDays)
                {
                    mobility = 1;
                }
                else if (cred.AvgStayDays > 0 && cred.AvgStayDays < MobileStayDays)
                {
                    mobility = 2;
                }

                rows.Add(new Live_UserProfile
                {
                    UserID = uid,
                    UserGID = userGidMap.GetValueOrDefault(uid),
                    CompanyID = companyMap.GetValueOrDefault(uid),
                    WorkTypePrefs = workJson,
                    WorkTypeTop = workTop,
                    CatePrefs = cateJson,
                    SalaryLevel = salaryLevel,
                    CredibilityScore = (decimal)cred.Score,
                    HiredCount = cred.Hired,
                    AppliedCount = cred.Applied,
                    QuickLeaveCount = cred.QuickLeave,
                    AvgStayDays = cred.AvgStayDays > 0 ? (decimal)cred.AvgStayDays : null,
                    RiskClassMode = riskMode,
                    SeekStatus = seekStatus,
                    MobilityType = mobility,
                    LastBehaviorTime = lastBehavior.GetValueOrDefault(uid),
                    LastEntryWorkName = lastEntry?.WorkName,
                    LastCalcTime = now,
                    CalcVersion = 0
                });
            }

            if (rows.Count == 0)
            {
                return 0;
            }
            await Context.Deleteable<Live_UserProfile>().Where(it => userIds.Contains(it.UserID)).ExecuteCommandAsync();
            await Context.Insertable(rows).ExecuteCommandAsync();
            return rows.Count;
        }

        /// <summary>全量岗位建池：按公司分组加载候选人与画像快照，逐岗位打分取 TopN</summary>
        private async Task<List<Live_JobCandidatePool>> BuildAllJobPools(DateTime now)
        {
            var dicts = await LoadDictionaries();

            // 在招岗位（到期有效）；视图可能因 JOIN 翻倍返回同一岗位多行，GroupBy 去重防重复建池
            var jobViews = (await Context.Queryable<Live_V_Jobs>()
                .Where(it => it.EndTime == null || it.EndTime >= now)
                .Select(it => new
                {
                    it.ID, it.Title, it.CustomerID, it.Tabs, it.Sex, it.MinAge, it.MaxAge,
                    it.MinMoney, it.MaxMoney, it.Province, it.City, it.District,
                    it.JobProvince, it.JobCity, it.JobDistrict, it.Latitude, it.Longitude,
                    it.WorkContent, it.SkillType, it.JobSkills, it.CompanyID
                })
                .ToListAsync())
                .GroupBy(it => it.ID)
                .Select(g => g.First())
                .ToList();
            if (jobViews.Count == 0)
            {
                return [];
            }

            // 在职映射（岗位,UserID）：排除已在该岗位的人
            var currentEntries = await Context.Queryable<Live_UserEntry>()
                .Where(it => it.Status == (int)UserStatus.Entry && it.JID != null && it.JID > 0)
                .Select(it => new { it.UserID, it.JID })
                .ToListAsync();
            var currentAtJob = currentEntries
                .Select(it => (it.UserID ?? 0, it.JID!.Value))
                .Where(it => it.Item1 > 0)
                .ToHashSet();

            var allRows = new List<Live_JobCandidatePool>();
            foreach (var companyGroup in jobViews.GroupBy(it => it.CompanyID ?? 0))
            {
                var candidates = await LoadCompanyCandidates(companyGroup.Key, dicts);
                if (candidates.Users.Count == 0)
                {
                    continue;
                }
                foreach (var jobView in companyGroup)
                {
                    var job = new CandidateJob
                    {
                        ID = jobView.ID,
                        Title = jobView.Title,
                        CustomerID = jobView.CustomerID,
                        Tabs = jobView.Tabs,
                        Sex = jobView.Sex,
                        MinAge = jobView.MinAge,
                        MaxAge = jobView.MaxAge,
                        MinMoney = jobView.MinMoney,
                        MaxMoney = jobView.MaxMoney,
                        Province = jobView.Province,
                        City = jobView.City,
                        District = jobView.District,
                        JobProvince = jobView.JobProvince,
                        JobCity = jobView.JobCity,
                        JobDistrict = jobView.JobDistrict,
                        Latitude = jobView.Latitude,
                        Longitude = jobView.Longitude,
                        WorkContent = jobView.WorkContent,
                        SkillType = jobView.SkillType,
                        JobSkills = jobView.JobSkills,
                        SkillText = string.Concat(jobView.Title, " ", jobView.JobSkills, " ", jobView.WorkContent),
                        EntryJobIds = jobView.CustomerID.HasValue
                            ? dicts.CustomerEntryJobs.GetValueOrDefault(jobView.CustomerID.Value) ?? []
                            : []
                    };
                    allRows.AddRange(BuildPoolRowsForJob(job, candidates, currentAtJob, dicts, now, sourceType: 0));
                }
            }
            return allRows;
        }

        /// <summary>加载一个公司的候选人与画像快照（公司内所有岗位共用，避免逐岗位重复查询）</summary>
        private async Task<CompanyCandidates> LoadCompanyCandidates(int companyId, MatchDictionaries dicts)
        {
            // 候选人（Enabled + 非黑名单 + 公司范围；companyId=0 时不限公司），按 UserID 去重防重复行
            var users = (await Context.Queryable<Live_User>()
                .Where(it => it.Enabled && !it.IsBlack && (companyId == 0 || it.CompanyID == companyId))
                .OrderByDescending(it => it.LastTime)
                .Take(MaxCompanyCandidates)
                .Select(CandidateUserSelect)
                .ToListAsync())
                .GroupBy(it => it.UserID)
                .Select(g => g.First())
                .ToList();

            // 画像快照（解析偏好 JSON，缓存归一化分母）
            var snapshots = new Dictionary<int, UserSnapshot>();            if (users.Count > 0)
            {
                var userIds = users.Select(it => it.UserID).ToList();
                var profileRows = await Context.Queryable<Live_UserProfile>()
                    .Where(it => userIds.Contains(it.UserID))
                    .ToListAsync();
                var profileMap = profileRows
                    .GroupBy(it => it.UserID)
                    .Select(g => g.OrderByDescending(p => p.LastCalcTime).First())
                    .ToDictionary(p => p.UserID);
                foreach (var user in users)
                {
                    var info = ToUserInfo(user);
                    var expectIds = new HashSet<int>();
                    if (!string.IsNullOrEmpty(user.Jobs))
                    {
                        foreach (var id in ParseIds(user.Jobs))
                        {
                            foreach (var expanded in ExpandIds(id, dicts.ClassMap))
                            {
                                expectIds.Add(expanded);
                            }
                        }
                    }

                    var snapshot = new UserSnapshot
                    {
                        UserID = user.UserID,
                        Info = info,
                        ExpectIds = expectIds,
                        Credibility = 0.4,               // 无画像记录的中性信誉
                        SeekStatus = 0
                    };
                    if (profileMap.TryGetValue(user.UserID, out var profile))
                    {
                        snapshot.WorkPrefs = ParsePrefsJson(profile.WorkTypePrefs);
                        snapshot.WorkMax = snapshot.WorkPrefs.Count > 0 ? snapshot.WorkPrefs.Values.Max() : 0;
                        snapshot.CatePrefs = ParsePrefsJson(profile.CatePrefs);
                        snapshot.CateMax = snapshot.CatePrefs.Count > 0 ? snapshot.CatePrefs.Values.Max() : 0;
                        snapshot.Credibility = (double)(profile.CredibilityScore ?? 0.4m);
                        snapshot.SeekStatus = profile.SeekStatus;
                        snapshot.LastWorkName = profile.LastEntryWorkName;
                    }
                    snapshots[user.UserID] = snapshot;
                }
            }

            return new CompanyCandidates { Users = users, Snapshots = snapshots };
        }

        /// <summary>单岗位建池：硬过滤（性别/年龄/证书/在职排除/地区强关联）+ 全维度打分 + TopN 截断，返回带名次的清单行</summary>
        private static List<Live_JobCandidatePool> BuildPoolRowsForJob(CandidateJob job, CompanyCandidates candidates, HashSet<(int UserID, int JobID)> currentAtJob, MatchDictionaries dicts, DateTime now, byte sourceType)
        {
            var requiredCerts = RequiredCerts(job.SkillText);
            var jobTabIds = ExpandIds(ParseIds(job.Tabs), dicts.ClassMap);
            var jobCityText = FirstText(job.JobCity, job.City);

            var scored = new List<(UserSnapshot snapshot, double score, string workHit, string distance)>();
            var seenUsers = new HashSet<int>();
            foreach (var user in candidates.Users)
            {
                // 同一用户只入池一次（候选人列表已去重，此处最终兜底）
                if (!seenUsers.Add(user.UserID))
                {
                    continue;
                }

                // 在该岗位在职的不入池
                if (currentAtJob.Contains((user.UserID, job.ID)))
                {
                    continue;
                }

                // 硬过滤：地区强关联（与正向对称，市级）
                if (RegionMismatch(user.ExpectCity, jobCityText))
                {
                    continue;
                }

                // 硬过滤：性别（未知保留）                if (!string.IsNullOrEmpty(job.Sex) && job.Sex != "不限")
                {
                    var known = !string.IsNullOrEmpty(user.Sex) && user.Sex != "不限";
                    if (known && user.Sex != job.Sex)
                    {
                        continue;
                    }
                }
                // 硬过滤：年龄（出生日期实时算）
                if (job.MinAge.HasValue || job.MaxAge.HasValue)
                {
                    var age = GetUserAge(user, now);
                    if (!age.HasValue
                        || (job.MinAge.HasValue && age < job.MinAge)
                        || (job.MaxAge.HasValue && age > job.MaxAge))
                    {
                        continue;
                    }
                }                // 硬过滤：特种作业岗无证（合规红线）
                if (requiredCerts.Count > 0 && FilterNoCertJobs)
                {
                    if (string.IsNullOrEmpty(user.SpecialCertificate)
                        || !requiredCerts.Any(cert => user.SpecialCertificate!.Contains(cert)))
                    {
                        continue;
                    }
                }

                var snapshot = candidates.Snapshots.GetValueOrDefault(user.UserID);                if (snapshot == null)
                {
                    continue;
                }

                // 亲和（读画像快照）：工种命中 0.7 + 类目命中 0.3，各自按本人偏好最大值归一
                var aff = 0d;
                var workHit = string.Empty;
                if (snapshot.WorkMax > 0 && job.EntryJobIds.Count > 0)
                {
                    var wtSum = 0d;
                    var best = 0d;
                    foreach (var workId in job.EntryJobIds)
                    {
                        var value = snapshot.WorkPrefs.GetValueOrDefault(workId);
                        if (value > 0)
                        {
                            wtSum += value;
                            if (value > best)
                            {
                                best = value;
                                workHit = dicts.EntryJobNames.GetValueOrDefault(workId, string.Empty);
                            }
                        }
                    }
                    aff += 0.7 * Math.Min(wtSum / snapshot.WorkMax, 1.0);
                }
                if (snapshot.CateMax > 0 && jobTabIds.Count > 0)
                {
                    var cateSum = 0d;
                    foreach (var id in jobTabIds)
                    {
                        cateSum += snapshot.CatePrefs.GetValueOrDefault(id);
                    }
                    aff += 0.3 * Math.Min(cateSum / snapshot.CateMax, 1.0);
                }

                var geo = ScoreGeo(job, snapshot.Info);
                var salary = ScoreSalary(job, snapshot.Info);
                var expect = snapshot.ExpectIds.Count > 0 ? ScoreExpect(job, snapshot.ExpectIds, dicts.ClassMap) : NeutralScore;
                var cert = ScoreCert(job, snapshot.Info);

                var score = 0.25 * aff
                    + 0.20 * snapshot.Credibility
                    + 0.20 * geo
                    + 0.15 * salary
                    + 0.10 * expect
                    + 0.10 * cert;

                // 距离快照（双方有坐标时）
                string distance = string.Empty;
                if (snapshot.Info.Latitude.HasValue && snapshot.Info.Longitude.HasValue
                    && job.Latitude.HasValue && job.Longitude.HasValue)
                {
                    var km = GetDistanceKm(snapshot.Info.Latitude.Value, snapshot.Info.Longitude.Value, job.Latitude.Value, job.Longitude.Value);
                    distance = FormatDistance(km);
                }

                scored.Add((snapshot, score, workHit, distance));
            }

            return scored
                .OrderByDescending(it => it.score)
                .ThenBy(it => it.snapshot.UserID)
                .Take(MaxPoolPerJob)
                .Select((it, index) => new Live_JobCandidatePool
                {
                    JobID = job.ID,
                    UserID = it.snapshot.UserID,
                    Score = (decimal)it.score,
                    Rank = index + 1,
                    ScoreDetail = JsonConvert.SerializeObject(new { aff = Math.Round(it.score, 4) }),
                    WorkTypeHit = it.workHit,
                    CredibilityScore = (decimal)it.snapshot.Credibility,
                    SeekStatus = it.snapshot.SeekStatus,
                    DistanceText = it.distance,
                    LastEntryWorkName = it.snapshot.LastWorkName,
                    CertMatch = requiredCerts.Count > 0 ? (byte)1 : (byte)0,
                    BatchID = 0,
                    CalcTime = now,
                    SourceType = sourceType
                })
                .ToList();
        }

        /// <summary>下一个夜批批次号 = 当前最大 + 1</summary>
        private async Task<long> GetNextPoolBatchId()
        {
            var lastRow = await Context.Queryable<Live_JobCandidatePool>()
                .Where(it => it.BatchID > 0)
                .OrderByDescending(it => it.BatchID)
                .FirstAsync();
            return (lastRow?.BatchID ?? 0) + 1;
        }

        /// <summary>Live_V_Jobs 视图行 → 候选岗位（含客户工种集合）</summary>
        private static CandidateJob ToCandidateJob(Live_V_Jobs jobView, MatchDictionaries dicts)
        {
            return new CandidateJob
            {
                ID = jobView.ID,
                Title = jobView.Title,
                CustomerID = jobView.CustomerID,
                Tabs = jobView.Tabs,
                Sex = jobView.Sex,
                MinAge = jobView.MinAge,
                MaxAge = jobView.MaxAge,
                MinMoney = jobView.MinMoney,
                MaxMoney = jobView.MaxMoney,
                Province = jobView.Province,
                City = jobView.City,
                District = jobView.District,
                JobProvince = jobView.JobProvince,
                JobCity = jobView.JobCity,
                JobDistrict = jobView.JobDistrict,
                Latitude = jobView.Latitude,
                Longitude = jobView.Longitude,
                WorkContent = jobView.WorkContent,
                SkillType = jobView.SkillType,
                JobSkills = jobView.JobSkills,
                SkillText = string.Concat(jobView.Title, " ", jobView.JobSkills, " ", jobView.WorkContent),
                EntryJobIds = jobView.CustomerID.HasValue
                    ? dicts.CustomerEntryJobs.GetValueOrDefault(jobView.CustomerID.Value) ?? []
                    : []
            };
        }

        /// <summary>CandidateUser → UserInfo（打分输入）</summary>
        private static UserInfo ToUserInfo(CandidateUser user)
        {
            return new UserInfo
            {
                Sex = user.Sex,
                BirthDate = user.BirthDate,
                Age = user.Age,
                ExpectProvince = user.ExpectProvince,
                ExpectCity = user.ExpectCity,
                ExpectDistrict = user.ExpectDistrict,
                MiniCity = user.MiniCity,
                MiniDistrict = user.MiniDistrict,
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                MinMoney = user.MinMoney,
                MaxMoney = user.MaxMoney,
                Jobs = user.Jobs,
                SpecialCertificate = user.SpecialCertificate
            };
        }

        /// <summary>偏好 JSON 解析（容错：坏 JSON 返回空字典）</summary>
        private static Dictionary<int, double> ParsePrefsJson(string? json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return [];
            }
            try
            {
                var raw = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);
                if (raw == null)
                {
                    return [];
                }
                return raw
                    .Where(it => int.TryParse(it.Key, out _))
                    .ToDictionary(it => int.Parse(it.Key), it => it.Value);
            }
            catch
            {
                return [];
            }
        }

        #endregion

        /// <summary>候选人召回列投影（活跃召回与亲和召回共用）</summary>
        private static readonly Expression<Func<Live_User, CandidateUser>> CandidateUserSelect = it => new CandidateUser
        {
            UserID = it.UserID,
            Sex = it.Sex,
            BirthDate = it.BirthDate,
            Age = it.Age,
            ExpectProvince = it.Province,
            ExpectCity = it.City,
            ExpectDistrict = it.District,
            MiniCity = it.MiniCity,
            MiniDistrict = it.MiniDistrict,
            Latitude = it.Latitude,
            Longitude = it.Longitude,
            MinMoney = it.MinMoney,
            MaxMoney = it.MaxMoney,
            Jobs = it.Jobs,
            SpecialCertificate = it.SpecialCertificate
        };

        /// <summary>
        /// 亲和召回（召回 B），两条路：
        /// ① 工种直配（主路）：近期入职工种 WorkName ∈ 目标客户工种集合（Live_EntryJob），SQL IN 直接召回；
        /// ② 类目重叠（辅路）：近期浏览/入职过与目标岗位同类目（Tabs 重叠，叶子+祖先展开）岗位的用户。
        /// 召回后套与活跃召回相同的 SQL 硬过滤，按 LastTime 倒序补入候选池（排除已召回者，上限 MaxAffinityUsers）。
        /// 解决活跃召回漏掉"匹配度高但最近不活跃"候选人的盲区。
        /// </summary>
        private async Task<List<CandidateUser>> RecallAffinityUsers(Expressionable<Live_User> userExp, List<CandidateUser> activeUsers, CandidateJob job, MatchDictionaries dicts, DateTime now)
        {
            var visitFrom = now.AddDays(-VisitWindowDays);
            var entryFrom = now.AddDays(-EntryWindowDays);
            var known = activeUsers.Select(it => it.UserID).ToHashSet();
            // ① 工种直配（主路）：近期入职工种 ∈ 目标客户工种集合
            List<int?> workNameUsers = [];
            var targetWorkNames = job.EntryJobIds
                .Where(dicts.EntryJobNames.ContainsKey)
                .Select(id => dicts.EntryJobNames[id])
                .Distinct()
                .ToList();
            if (targetWorkNames.Count > 0)
            {
                workNameUsers = await Context.Queryable<Live_UserEntry>()
                    .Where(it => targetWorkNames.Contains(it.WorkName)
                        && (it.EntryTime >= entryFrom || it.CreateTime >= entryFrom))
                    .Select(it => it.UserID)
                    .Distinct()
                    .ToListAsync();
            }

            // ② 类目重叠（辅路）：与目标岗位类目重叠的岗位（全表取 ID+Tabs 内存比对，岗位量级小）
            var tabIds = ExpandIds(ParseIds(job.Tabs), dicts.ClassMap);
            List<int?> visitUsers = [];
            List<int?> entryUsers = [];
            if (tabIds.Count > 0)
            {
                var allJobs = await Context.Queryable<Live_Jobs>()
                    .Select(it => new { it.ID, it.Tabs })
                    .ToListAsync();
                var relatedJobIds = allJobs
                    .Where(j =>
                    {
                        var ids = ExpandIds(ParseIds(j.Tabs), dicts.ClassMap);
                        return ids.Count > 0 && ids.Any(tabIds.Contains);
                    })
                    .Select(j => j.ID)
                    .Take(MaxRelatedJobs)
                    .ToList();

                if (relatedJobIds.Count > 0)
                {
                    // 近期对这些同类岗位有行为的用户（浏览 + 入职，SQL 侧去重）
                    visitUsers = await Context.Queryable<Live_JobsVisit>()
                        .Where(it => it.JobID != null && it.JobID > 0
                            && relatedJobIds.Contains(it.JobID.Value)
                            && (it.StartTime >= visitFrom || it.CreateTime >= visitFrom))
                        .Select(it => it.UserID)
                        .Distinct()
                        .ToListAsync();
                    entryUsers = await Context.Queryable<Live_UserEntry>()
                        .Where(it => it.JID != null && it.JID > 0
                            && relatedJobIds.Contains(it.JID.Value)
                            && (it.EntryTime >= entryFrom || it.CreateTime >= entryFrom))
                        .Select(it => it.UserID)
                        .Distinct()
                        .ToListAsync();
                }
            }

            // 排除活跃召回已覆盖的，限量后按同样硬过滤取回
            var extraIds = workNameUsers
                .Concat(visitUsers)
                .Concat(entryUsers)
                .Where(it => it.HasValue && it.Value > 0)
                .Select(it => it.Value)
                .Where(it => !known.Contains(it))
                .Distinct()
                .Take(MaxAffinityUsers)
                .ToList();
            if (extraIds.Count == 0)
            {
                return [];
            }

            return await Context.Queryable<Live_User>()
                .Where(userExp.ToExpression())
                .Where(it => extraIds.Contains(it.UserID))
                .OrderByDescending(it => it.LastTime)
                .Take(extraIds.Count)
                .Select(CandidateUserSelect)
                .ToListAsync();
        }



        /// <summary>候选人年龄：出生日期实时计算优先，退化 Age 字段</summary>
        private static int? GetUserAge(CandidateUser user, DateTime now)
        {
            if (user.BirthDate.HasValue)
            {
                var birth = user.BirthDate.Value;
                var age = now.Year - birth.Year;
                if (now < birth.AddYears(age))
                {
                    age--;
                }
                return age > 0 ? age : null;
            }
            return user.Age is > 0 ? user.Age : null;
        }

        #endregion

        #region 硬性过滤

        /// <summary>个人硬性过滤（SQL 侧）：性别、年龄、地区强关联。岗位侧空值/"不限"视为不限制。</summary>
        private static void ApplyHardFilters(Expressionable<Live_V_Jobs> expable, UserInfo user, DateTime now)        {
            if (user == null)
            {
                return;
            }

            // 性别：岗位限制了性别且用户性别已知时过滤
            if (!string.IsNullOrEmpty(user.Sex) && user.Sex != "不限")
            {
                var sex = user.Sex;
                expable.And(it => it.Sex == null || it.Sex == "" || it.Sex == "不限" || it.Sex == sex);
            }

            // 年龄：BirthDate 实时计算优先，无出生日期退化为 Age 冗余字段
            var age = GetAge(user, now);
            if (age.HasValue)
            {
                var ageValue = age.Value;
                expable.And(it => (it.MinAge == null || it.MinAge <= ageValue) && (it.MaxAge == null || it.MaxAge >= ageValue));
            }

            // 地区强关联（市级硬过滤）：有期望工作地时，岗位工作地(JobCity)/所在城市(City)
            // 与期望市不符的不再召回（城市未知/为空的岗位保留，不做误杀）。
            if (HasValue(user.ExpectCity))
            {
                var expectCity = user.ExpectCity;
                expable.And(it => (it.JobCity == null || it.JobCity == "" || it.JobCity.Contains(expectCity))
                    && (it.City == null || it.City == "" || it.City.Contains(expectCity)));
            }
        }

        /// <summary>地区强关联不符判定：期望市与岗位城市均有值且互不包含（双向包含容错"苏州"/"苏州市"）。任一方无值=无法判定=不排除。</summary>
        private static bool RegionMismatch(string? userExpectCity, string? jobCity)
        {
            if (!HasValue(userExpectCity) || !HasValue(jobCity))
            {
                return false;
            }
            return !TextMatch(jobCity, userExpectCity);
        }


        /// <summary>特种作业技工岗要求证书而工人无证/缺证 → true（应过滤）。无证上岗是安全生产红线。</summary>
        private static bool IsMissingRequiredCert(CandidateJob job, string? userCerts)
        {
            if (job.SkillType != 2)
            {
                return false;
            }
            var required = RequiredCerts(job.SkillText);
            if (required.Count == 0)
            {
                return false;
            }
            return string.IsNullOrEmpty(userCerts) || !required.Any(cert => userCerts.Contains(cert));
        }

        /// <summary>从岗位文本(Title/JobSkills/WorkContent)识别要求的特种证书</summary>
        private static List<string> RequiredCerts(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return [];
            }
            return CertKeywords
                .Where(ck => ck.Keywords.Any(k => text.Contains(k)))
                .Select(ck => ck.Cert)
                .ToList();
        }

        #endregion

        #region 用户画像

        /// <summary>
        /// 用户行为画像：工种偏好（主轴，Live_EntryJob）+ 类目偏好（辅助轴，Live_JobClass）+ 在职岗位集合。
        /// 工种信号：入职工种 WorkName（主路，历史数据全覆盖，不依赖 JID/岗位存在）
        ///         + 浏览/入职岗位所属客户的工种集合 + 搜索词命中工种名。
        /// </summary>
        private async Task<UserProfile> GetUserProfile(int userId, DateTime now, MatchDictionaries dicts)
        {
            var profile = new UserProfile
            {
                ClassMap = dicts.ClassMap
            };
            if (userId <= 0)
            {
                return profile;
            }

            var visitFrom = now.AddDays(-VisitWindowDays);
            var searchFrom = now.AddDays(-SearchWindowDays);
            var entryFrom = now.AddDays(-EntryWindowDays);

            // ① 近 30 天岗位浏览（按 JobID 去重：权重取停留档位、时间取最近一次）
            var visits = await Context.Queryable<Live_JobsVisit>()
                .Where(it => it.UserID == userId && it.JobID != null && it.JobID > 0
                    && (it.StartTime >= visitFrom || it.CreateTime >= visitFrom))
                .Select(it => new { it.JobID, it.StartTime, it.CreateTime, it.Seconds })
                .ToListAsync();

            if (visits.Count > 0)
            {
                var visitedJobIds = visits.Select(it => it.JobID!.Value).Distinct().ToList();
                var tabsRows = await Context.Queryable<Live_Jobs>()
                    .Where(it => visitedJobIds.Contains(it.ID))
                    .Select(it => new { it.ID, it.Tabs, it.CustomerID })
                    .ToListAsync();
                var tabsMap = tabsRows.ToDictionary(it => it.ID, it => it.Tabs);
                var visitCustomerMap = tabsRows.ToDictionary(it => it.ID, it => it.CustomerID);

                foreach (var group in visits.GroupBy(it => it.JobID!.Value))
                {
                    var latest = group.OrderByDescending(v => v.StartTime ?? v.CreateTime ?? now).First();
                    var seconds = group.Max(v => v.Seconds ?? 0);
                    var time = latest.StartTime ?? latest.CreateTime ?? now;
                    var decay = Math.Pow(0.5, (now - time).TotalDays / VisitHalfLifeDays);

                    // 停留时长档位：≥30s 认真看 / ≥5s 有效 / 无时长记录保守降档 / <5s 无效不计
                    double weight;
                    if (seconds >= LongVisitSeconds)
                    {
                        weight = VisitLongWeight;
                    }
                    else if (seconds >= ValidVisitSeconds)
                    {
                        weight = VisitWeight;
                    }
                    else if (seconds <= 0 && group.All(v => !v.Seconds.HasValue))
                    {
                        weight = VisitWeight * 0.6;    // 历史无时长记录，保守降档
                    }
                    else
                    {
                        continue;                       // 划过的无效浏览
                    }

                    AddCatePreference(profile, tabsMap.GetValueOrDefault(group.Key), weight * decay);

                    // 工种偏好：浏览的岗位 → 所属客户关联的工种集合（均摊）
                    var visitCustomer = visitCustomerMap.GetValueOrDefault(group.Key);
                    if (visitCustomer.HasValue)
                    {
                        var workIds = dicts.CustomerEntryJobs.GetValueOrDefault(visitCustomer.Value);
                        if (workIds is { Count: > 0 })
                        {
                            foreach (var workId in workIds)
                            {
                                AddWorkTypePreference(profile, workId, weight * decay / workIds.Count);
                            }
                        }
                    }
                }
            }

            // ② 近 14 天搜索词 → 命中工种名(主) / 类目名(辅)（主动搜索是强意图信号）
            var searches = await Context.Queryable<Live_JobsVisit>()
                .Where(it => it.UserID == userId
                    && ((it.Keyword != null && it.Keyword != "") || (it.Search != null && it.Search != ""))
                    && (it.StartTime >= searchFrom || it.CreateTime >= searchFrom))
                .Select(it => new { it.Keyword, it.Search, it.StartTime, it.CreateTime })
                .ToListAsync();

            foreach (var row in searches)
            {
                var text = string.Concat(row.Keyword, " ", row.Search);
                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }
                var time = row.StartTime ?? row.CreateTime ?? now;
                var decay = Math.Pow(0.5, (now - time).TotalDays / SearchHalfLifeDays);

                // 命中工种名（Live_EntryJob，更具体的意图，主轴），最多记 3 个防误伤
                var workHits = dicts.EntryJobIdsByName
                    .Where(kv => kv.Key.Length >= 2 && text.Contains(kv.Key))
                    .OrderByDescending(kv => kv.Key.Length)
                    .Take(3)
                    .ToList();
                foreach (var hit in workHits)
                {
                    AddWorkTypePreference(profile, hit.Value, SearchWeight * decay);
                }

                // 命中类目名（名字长的更具体，优先），单条搜索最多记 3 个类目防误伤
                var hits = profile.ClassMap.Values
                    .Where(c => !string.IsNullOrEmpty(c.Name) && c.Name.Length >= 2 && text.Contains(c.Name))
                    .OrderByDescending(c => c.Name.Length)
                    .Take(3)
                    .ToList();
                foreach (var hit in hits)
                {
                    AddCatePreference(profile, hit.ID.ToString(), SearchWeight * decay);
                }
            }

            // ③ 近 90 天入职记录（正负样本 + 在职岗位 + 工种信号）
            // 注意：不要求 JID 有值——工种信号走 WorkName 反查，历史/导入数据无 JID 也能聚合
            var entries = await Context.Queryable<Live_UserEntry>()
                .Where(it => it.UserID == userId
                    && (it.EntryTime >= entryFrom || it.CreateTime >= entryFrom))
                .Select(it => new { it.JID, it.Status, it.EntryTime, it.LeaveTime, it.Days, it.CreateTime, it.WorkName })
                .ToListAsync();

            if (entries.Count > 0)
            {
                var entryJobIds = entries.Select(it => it.JID ?? 0).Where(v => v > 0).Distinct().ToList();
                Dictionary<int, string?> tabsMap = [];
                Dictionary<int, int?> entryCustomerMap = [];
                if (entryJobIds.Count > 0)
                {
                    var tabsRows = await Context.Queryable<Live_Jobs>()
                        .Where(it => entryJobIds.Contains(it.ID))
                        .Select(it => new { it.ID, it.Tabs, it.CustomerID })
                        .ToListAsync();
                    tabsMap = tabsRows.ToDictionary(it => it.ID, it => it.Tabs);
                    entryCustomerMap = tabsRows.ToDictionary(it => it.ID, it => it.CustomerID);
                }

                foreach (var entry in entries)
                {
                    var jid = entry.JID ?? 0;
                    var status = entry.Status ?? 0;

                    double weight;
                    DateTime? signalTime;
                    if (status == (int)UserStatus.Entry)
                    {
                        // 在职：不再推荐该岗位；计最强正样本
                        if (jid > 0)
                        {
                            profile.CurrentJobIds.Add(jid);
                        }
                        weight = EntryLongWeight;
                        signalTime = entry.EntryTime ?? entry.CreateTime;
                    }
                    else if (status == (int)UserStatus.Leave)
                    {
                        // 按在岗天数分桶：≥30 天正样本 / 3~30 天弱正 / <3 天负样本
                        var days = entry.Days ?? (entry.LeaveTime.HasValue && entry.EntryTime.HasValue
                            ? (decimal?)Math.Max(0, (entry.LeaveTime.Value - entry.EntryTime.Value).TotalDays)
                            : null);
                        weight = days >= 30 ? EntryLongWeight : days >= 3 ? EntryShortWeight : EntryQuickLeaveWeight;
                        signalTime = entry.LeaveTime ?? entry.EntryTime ?? entry.CreateTime;
                    }
                    else if (ApplyStatuses.Contains(status))
                    {
                        weight = ApplyWeight;
                        signalTime = entry.CreateTime;
                    }
                    else if (status == (int)UserStatus.InterviewFailed || status == (int)UserStatus.NotEntry)
                    {
                        weight = ApplyFailedWeight;
                        signalTime = entry.CreateTime;
                    }
                    else
                    {
                        continue;
                    }

                    var halfLife = status == (int)UserStatus.Entry || status == (int)UserStatus.Leave
                        ? EntryHalfLifeDays
                        : SearchHalfLifeDays;
                    var w = weight * Decay(signalTime, now, halfLife);

                    if (jid > 0)
                    {
                        AddCatePreference(profile, tabsMap.GetValueOrDefault(jid), w);
                    }

                    // 工种偏好（主路）：入职工种名反查 EntryJob ID；反查失败经岗位客户的工种集合兜底
                    AddEntryWorkTypePreference(profile, dicts, entry.WorkName, entryCustomerMap.GetValueOrDefault(jid), w);
                }
            }

            return profile;
        }

        /// <summary>入职工种偏好累加：WorkName 反查 EntryJob ID（入职时选定，历史数据全覆盖）；缺失时经岗位客户工种集合均摊兜底</summary>
        private static void AddEntryWorkTypePreference(UserProfile profile, MatchDictionaries dicts, string? workName, int? jobCustomerId, double weight)
        {
            if (weight == 0)
            {
                return;
            }
            if (!string.IsNullOrEmpty(workName) && dicts.EntryJobIdsByName.TryGetValue(workName, out var entryJobId))
            {
                AddWorkTypePreference(profile, entryJobId, weight);
                return;
            }
            if (jobCustomerId.HasValue)
            {
                var workIds = dicts.CustomerEntryJobs.GetValueOrDefault(jobCustomerId.Value);
                if (workIds is { Count: > 0 })
                {
                    foreach (var workId in workIds)
                    {
                        AddWorkTypePreference(profile, workId, weight / workIds.Count);
                    }
                }
            }
        }

        /// <summary>工种偏好累加（EntryJob 扁平字典，无上卷）</summary>
        private static void AddWorkTypePreference(UserProfile profile, int entryJobId, double weight)
        {
            if (weight == 0 || entryJobId <= 0)
            {
                return;
            }
            profile.WorkTypePreference[entryJobId] = profile.WorkTypePreference.GetValueOrDefault(entryJobId) + weight;
            profile.HasWorkTypeBehavior = true;
        }

        /// <summary>类目偏好累加：岗位 Tabs 打散到类目后均摊，并沿 JobClass PID 上卷（父类×0.5）</summary>
        private static void AddCatePreference(UserProfile profile, string? tabs, double weight)
        {
            if (weight == 0)
            {
                return;
            }
            var ids = ParseIds(tabs);
            if (ids.Count == 0)
            {
                return;
            }
            var share = weight / ids.Count;
            foreach (var id in ids)
            {
                var current = id;
                var factor = 1.0;
                var depth = 0;
                while (current > 0 && depth < MaxClassDepth)
                {
                    profile.CatePreference[current] = profile.CatePreference.GetValueOrDefault(current) + share * factor;
                    current = profile.ClassMap.GetValueOrDefault(current)?.PID ?? 0;
                    factor *= ParentRollupFactor;
                    depth++;
                }
            }
            profile.HasBehavior = true;
        }

        /// <summary>
        /// 匹配字典（小表整表加载，每请求一次）：
        /// - ClassMap：Live_JobClass 树（类目辅助轴，期望岗位/列表筛选体系）
        /// - EntryJobNames / EntryJobIdsByName：Live_EntryJob 工种字典（主轴；含停用工种——历史 WorkName 反查需要）
        /// - CustomerEntryJobs：Live_CustomerJobs 客户→可用工种集合（岗位侧工种来源，IsDefault 不区分）
        /// </summary>
        private async Task<MatchDictionaries> LoadDictionaries()
        {
            var classes = await Context.Queryable<Live_JobClass>()
                .Select(it => new JobClassNode { ID = it.ID, PID = it.PID, Name = it.Name })
                .ToListAsync();
            var entryJobs = await Context.Queryable<Live_EntryJob>()
                .Select(it => new { it.ID, it.Name })
                .ToListAsync();
            var customerJobs = await Context.Queryable<Live_CustomerJobs>()
                .Select(it => new { it.CustomerID, it.EntryJobID })
                .ToListAsync();
            return new MatchDictionaries
            {
                ClassMap = classes.ToDictionary(it => it.ID),
                EntryJobNames = entryJobs.ToDictionary(it => it.ID, it => it.Name),
                EntryJobIdsByName = entryJobs
                    .GroupBy(it => it.Name)
                    .ToDictionary(g => g.Key, g => g.First().ID),
                CustomerEntryJobs = customerJobs
                    .GroupBy(it => it.CustomerID)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.EntryJobID).Distinct().ToList())
            };
        }

        private static double Decay(DateTime? time, DateTime now, double halfLifeDays)
        {
            if (!time.HasValue)
            {
                return 0.3;   // 时间未知按旧数据保守处理
            }
            return Math.Pow(0.5, (now - time.Value).TotalDays / halfLifeDays);
        }

        #endregion

        #region 打分与排序

        private static List<RankedJob> Rank(List<CandidateJob> candidates, UserProfile profile, UserInfo? user, JobPagedModel query, DateTime now)
        {
            // 期望岗位 ID（含祖先展开）
            HashSet<int> expectIds = [];
            bool hasExpect = false;
            if (user != null && !string.IsNullOrEmpty(user.Jobs))
            {
                foreach (var id in ParseIds(user.Jobs))
                {
                    hasExpect = true;
                    foreach (var expanded in ExpandIds(id, profile.ClassMap))
                    {
                        expectIds.Add(expanded);
                    }
                }
            }

            double maxCate = profile.CatePreference.Count > 0 ? profile.CatePreference.Values.Max() : 0;
            double maxWorkType = profile.WorkTypePreference.Count > 0 ? profile.WorkTypePreference.Values.Max() : 0;

            var ranked = new List<RankedJob>(candidates.Count);
            foreach (var job in candidates)
            {
                // 工作地：期望工作地优先，无期望用实时位置
                var geo = ScoreGeo(job, user);

                // 期望薪资：区间重叠度
                var salary = ScoreSalary(job, user);

                // 期望岗位命中：叶子命中 1.0 / 仅父类命中 0.5 / 无期望中性 0.5
                var expect = hasExpect ? ScoreExpect(job, expectIds, profile.ClassMap) : NeutralScore;

                // 类目偏好（辅助轴）：无行为(冷启动)时回退为期望岗位命中
                double cate;
                if (profile.HasBehavior && maxCate > 0)
                {
                    var ids = ExpandIds(ParseIds(job.Tabs), profile.ClassMap);
                    var sum = 0d;
                    foreach (var id in ids)
                    {
                        sum += profile.CatePreference.GetValueOrDefault(id);
                    }
                    cate = Math.Min(sum / maxCate, 1.5);
                }
                else
                {
                    cate = expect;
                }

                // 工种偏好（主轴）：岗位工种集合（客户关联 Live_EntryJob）vs 用户工种偏好分
                double workType;
                if (profile.HasWorkTypeBehavior && maxWorkType > 0)
                {
                    var wtSum = 0d;
                    foreach (var id in job.EntryJobIds)
                    {
                        wtSum += profile.WorkTypePreference.GetValueOrDefault(id);
                    }
                    workType = Math.Min(wtSum / maxWorkType, 1.5);
                }
                else
                {
                    workType = NeutralScore;   // 无工种信号（冷启动）中性
                }

                // 证书/技能
                var cert = ScoreCert(job, user);

                var score = WeightWorkType * workType
                    + WeightCate * cate
                    + WeightGeo * geo
                    + WeightSalary * salary
                    + WeightExpect * expect
                    + WeightCert * cert
                    + FreshnessBonus(job.UpTime, now)
                    + (job.IsQuality ? QualityBonus : 0);

                ranked.Add(new RankedJob { ID = job.ID, Score = score });
            }

            var ordered = ranked
                .OrderByDescending(it => it.Score)
                .ThenBy(it => it.ID)
                .ToList();

            return ShuffleRanked(ordered, query);
        }

        /// <summary>工作地匹配：有期望工作地(区1.0/市0.6/省0.3)；无期望用进小程序的实时坐标分档距离，无坐标退化为行政区匹配。</summary>
        private static double ScoreGeo(CandidateJob job, UserInfo? user)
        {
            if (user == null)
            {
                return NeutralScore;
            }

            var jobProvince = FirstText(job.JobProvince, job.Province);
            var jobCity = FirstText(job.JobCity, job.City);
            var jobDistrict = FirstText(job.JobDistrict, job.District);

            // ① 期望工作地优先
            var hasExpect = HasValue(user.ExpectCity) || HasValue(user.ExpectDistrict);
            if (hasExpect)
            {
                if (HasValue(user.ExpectDistrict) && TextMatch(jobDistrict, user.ExpectDistrict))
                {
                    return 1.0;
                }
                if (HasValue(user.ExpectCity) && TextMatch(jobCity, user.ExpectCity))
                {
                    return 0.6;
                }
                if (HasValue(user.ExpectProvince) && TextMatch(jobProvince, user.ExpectProvince))
                {
                    return 0.3;
                }
                return 0;
            }

            // ② 实时坐标 → 距离分档
            if (user.Latitude.HasValue && user.Longitude.HasValue && job.Latitude.HasValue && job.Longitude.HasValue)
            {
                var km = GetDistanceKm(user.Latitude.Value, user.Longitude.Value, job.Latitude.Value, job.Longitude.Value);
                return km <= 5 ? 1.0
                    : km <= 10 ? 0.8
                    : km <= 20 ? 0.55
                    : km <= 30 ? 0.35
                    : 0.15;
            }

            // ③ 无坐标 → 行政区匹配
            if (HasValue(user.MiniDistrict) && TextMatch(jobDistrict, user.MiniDistrict))
            {
                return 1.0;
            }
            if (HasValue(user.MiniCity) && TextMatch(jobCity, user.MiniCity))
            {
                return 0.6;
            }
            return 0.1;
        }

        /// <summary>期望薪资区间重叠度。岗位薪资缺省按 3000/6000 兜底（与列表 VerifyJob 逻辑一致）；低于期望按缺口衰减（2000 元以上归零）。</summary>
        private static double ScoreSalary(CandidateJob job, UserInfo? user)
        {
            if (user == null || (user.MinMoney is null or 0 && user.MaxMoney is null or 0))
            {
                return NeutralScore;   // 未填期望薪资 → 中性
            }

            var jobMin = job.MinMoney is > 0 ? job.MinMoney.Value : 3000;
            var jobMax = job.MaxMoney is > 0 ? job.MaxMoney.Value : 6000;
            if (jobMax < jobMin)
            {
                jobMax = jobMin;
            }

            var userMin = (double)user.MinMoney.GetValueOrDefault(0);
            var userMax = user.MaxMoney is > 0 ? (double)user.MaxMoney.Value : jobMax;   // 只填下限视为不封顶
            if (userMax < userMin)
            {
                userMax = userMin;
            }

            var overlap = Math.Min(jobMax, userMax) - Math.Max(jobMin, userMin);
            if (overlap >= 0)
            {
                var range = userMax - userMin;
                return range <= 0 ? 1.0 : Math.Min(overlap / range, 1.0);
            }

            // 不重叠：岗位整体低于期望
            return Math.Max(0, 1 - (userMin - jobMax) / 2000);
        }

        /// <summary>期望岗位命中：Tabs 叶子命中 1.0，仅父类命中 0.5</summary>
        private static double ScoreExpect(CandidateJob job, HashSet<int> expectIds, Dictionary<int, JobClassNode> classMap)
        {
            var tabIds = ParseIds(job.Tabs);
            if (tabIds.Count == 0)
            {
                return 0;
            }
            foreach (var id in tabIds)
            {
                if (expectIds.Contains(id))
                {
                    return 1.0;
                }
            }
            foreach (var id in tabIds)
            {
                var parent = classMap.GetValueOrDefault(id)?.PID ?? 0;
                if (parent > 0 && expectIds.Contains(parent))
                {
                    return 0.5;
                }
            }
            return 0;
        }

        /// <summary>证书/技能维度：普工岗中性 0.6；技工岗要求证书且持证 1.0 / 未识别出证书要求 0.5~0.8</summary>
        private static double ScoreCert(CandidateJob job, UserInfo? user)
        {
            if (job.SkillType != 2)
            {
                return 0.6;
            }
            var required = RequiredCerts(job.SkillText);
            var userCerts = user?.SpecialCertificate;
            if (required.Count == 0)
            {
                return string.IsNullOrEmpty(userCerts) ? 0.5 : 0.8;
            }
            if (string.IsNullOrEmpty(userCerts))
            {
                return 0;   // 硬过滤开关关闭时才会到这里
            }
            return required.Any(cert => userCerts.Contains(cert)) ? 1.0 : 0;
        }

        /// <summary>UpTime 新鲜度加分（≤0.05，30 天半衰期）</summary>
        private static double FreshnessBonus(DateTime? upTime, DateTime now)
        {
            if (!upTime.HasValue)
            {
                return 0;
            }
            var bonus = FreshnessBonusMax * Math.Pow(0.5, (now - upTime.Value).TotalDays / FreshHalfLifeDays);
            return Math.Max(bonus, 0);
        }

        /// <summary>
        /// 头部高分池随机打散：取前 pageSize×4（至少 40）个高分岗位叠加随机扰动后重排，
        /// 每次刷新顺序不同但整体质量不变。query.SID 有值时固定种子（翻页顺序稳定）。
        /// </summary>
        private static List<RankedJob> ShuffleRanked(List<RankedJob> ranked, JobPagedModel query)
        {
            return ShuffleRanked(ranked, query.PageSize, query.SID);
        }

        /// <summary>头部池打散通用实现（sid 有值时固定种子，翻页顺序稳定）</summary>
        private static List<T> ShuffleRanked<T>(List<T> ranked, int pageSize, int? sid) where T : IRankedScore
        {
            if (ranked.Count <= 1)
            {
                return ranked;
            }

            var size = pageSize < 1 ? 15 : pageSize;
            var poolSize = Math.Min(ranked.Count, Math.Max(size * 4, 40));
            var pool = ranked.Take(poolSize).ToList();
            var rest = ranked.Skip(poolSize).ToList();

            var seed = sid ?? Random.Shared.Next();
            var rng = new Random(seed);
            var min = pool.Min(it => it.Score);
            var max = pool.Max(it => it.Score);
            var spread = Math.Max(max - min, 1);

            var shuffled = pool
                .Select(it => new
                {
                    Item = it,
                    Order = it.Score + rng.NextDouble() * spread * 0.65   // 扰动不超过分差的 65%，高分仍倾向靠前
                })
                .OrderByDescending(it => it.Order)
                .Select(it => it.Item)
                .ToList();
            shuffled.AddRange(rest);
            return shuffled;
        }

        #endregion

        #region 工具方法

        private static int? GetAge(UserInfo user, DateTime now)
        {
            if (user.BirthDate.HasValue)
            {
                var birth = user.BirthDate.Value;                var age = now.Year - birth.Year;
                if (now < birth.AddYears(age))
                {
                    age--;
                }
                return age > 0 ? age : null;
            }
            return user.Age is > 0 ? user.Age : null;
        }

        private static double GetDistanceKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double earthRadiusKm = 6371;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLng = (lng2 - lng1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                  + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                  * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static string FormatDistance(double km)
        {
            var meters = km * 1000;
            if (meters < 1000)
            {
                return $"{Math.Max(1, (int)Math.Round(meters))}米";
            }
            return km < 10 ? $"{Math.Round(km, 1)}公里" : $"{Math.Round(km, 0)}公里";
        }

        /// <summary>逗号分隔 ID 串解析（Live_Jobs.Tabs / Live_User.Jobs 同格式）</summary>
        private static List<int> ParseIds(string? ids)
        {
            var result = new List<int>();
            if (!string.IsNullOrEmpty(ids))
            {
                foreach (var part in ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (int.TryParse(part, out var id) && id > 0)
                    {
                        result.Add(id);
                    }
                }
            }
            return result.Distinct().ToList();
        }

        /// <summary>类目 ID 展开到自身+祖先（打分侧对称展开，与偏好上卷匹配）</summary>
        private static HashSet<int> ExpandIds(List<int> ids, Dictionary<int, JobClassNode> classMap)
        {
            var set = new HashSet<int>();
            foreach (var id in ids)
            {
                foreach (var expanded in ExpandIds(id, classMap))
                {
                    set.Add(expanded);
                }
            }
            return set;
        }

        private static IEnumerable<int> ExpandIds(int id, Dictionary<int, JobClassNode> classMap)
        {
            var current = id;
            var depth = 0;
            while (current > 0 && depth < MaxClassDepth)
            {
                yield return current;
                current = classMap.GetValueOrDefault(current)?.PID ?? 0;
                depth++;
            }
        }

        /// <summary>地区文本匹配：双向包含（"苏州" vs "苏州市"）</summary>
        private static bool TextMatch(string? jobText, string? userText)
        {
            if (string.IsNullOrEmpty(jobText) || string.IsNullOrEmpty(userText))
            {
                return false;
            }
            return jobText.Contains(userText) || userText.Contains(jobText);
        }

        /// <summary>地区值有效（非空且非"不限"）</summary>
        private static bool HasValue(string? text)
        {
            return !string.IsNullOrEmpty(text) && text != "不限" && text != "全部";
        }

        private static string? FirstText(params string?[] texts)
        {
            foreach (var text in texts)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
            }
            return null;
        }

        #endregion

        #region 内部模型

        /// <summary>用户画像：工种偏好 + 类目偏好 + 在职岗位集合 + 行为标记</summary>
        private sealed class UserProfile
        {
            /// <summary>工种偏好分：EntryJobID → 偏好强度（主轴，扁平字典无上卷）</summary>
            public Dictionary<int, double> WorkTypePreference { get; } = [];

            /// <summary>类目偏好分：JobClassID → 偏好强度（辅助轴，含父类上卷）</summary>
            public Dictionary<int, double> CatePreference { get; } = [];

            /// <summary>在职岗位 ID（不再重复推荐）</summary>
            public HashSet<int> CurrentJobIds { get; } = [];

            /// <summary>类目字典</summary>
            public Dictionary<int, JobClassNode> ClassMap { get; set; } = [];

            /// <summary>是否有任何类目行为信号（冷启动判断）</summary>
            public bool HasBehavior { get; set; }

            /// <summary>是否有任何工种信号（冷启动判断）</summary>
            public bool HasWorkTypeBehavior { get; set; }
        }

        /// <summary>候选岗位（只取打分所需列）</summary>
        private sealed class CandidateJob
        {
            public int ID { get; set; }
            public string Title { get; set; } = string.Empty;
            public int? CustomerID { get; set; }

            /// <summary>岗位工种集合：所属客户关联的 Live_EntryJob ID（匹配主轴）</summary>
            public List<int> EntryJobIds { get; set; } = [];
            public string? Tabs { get; set; }
            public string? Sex { get; set; }
            public int? MinAge { get; set; }
            public int? MaxAge { get; set; }
            public int? MinMoney { get; set; }
            public int? MaxMoney { get; set; }
            public string? Province { get; set; }
            public string? City { get; set; }
            public string? District { get; set; }
            public string? JobProvince { get; set; }
            public string? JobCity { get; set; }
            public string? JobDistrict { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string? WorkContent { get; set; }
            public bool IsQuality { get; set; }
            public bool IsDayOver { get; set; }
            public DateTime? UpTime { get; set; }

            // 技能（视图 Live_V_Jobs 已含）
            public int? SkillType { get; set; }
            public string? JobSkills { get; set; }
            public string? SkillText { get; set; }
        }

        /// <summary>排序结果</summary>
        private sealed class RankedJob : IRankedScore
        {
            public int ID { get; set; }
            public double Score { get; set; }
        }

        /// <summary>候选人排序结果</summary>
        private sealed class RankedUser : IRankedScore
        {
            public int UserID { get; set; }
            public double Score { get; set; }
        }

        /// <summary>打散排序项约束</summary>
        private interface IRankedScore
        {
            double Score { get; }
        }

        /// <summary>候选人（召回所需列）</summary>
        private sealed class CandidateUser
        {
            public int UserID { get; set; }
            public string? Sex { get; set; }
            public DateTime? BirthDate { get; set; }
            public int? Age { get; set; }
            public string? ExpectProvince { get; set; }
            public string? ExpectCity { get; set; }
            public string? ExpectDistrict { get; set; }
            public string? MiniCity { get; set; }
            public string? MiniDistrict { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public decimal? MinMoney { get; set; }
            public decimal? MaxMoney { get; set; }
            public string? Jobs { get; set; }
            public string? SpecialCertificate { get; set; }
        }

        /// <summary>到岗率信誉</summary>
        private sealed class UserCredibility
        {
            /// <summary>信誉分 [0,1]</summary>
            public double Score { get; set; }
            /// <summary>入职次数（在职+已离职）</summary>
            public int Hired { get; set; }
            /// <summary>投递未入职次数</summary>
            public int Applied { get; set; }
            /// <summary>秒退次数（<3天）</summary>
            public int QuickLeave { get; set; }
            /// <summary>平均在岗天数（已离职记录均值，无记录 0）</summary>
            public double AvgStayDays { get; set; }
        }

        /// <summary>候选人画像快照（建池用，偏好 JSON 已解析并缓存归一化分母）</summary>
        private sealed class UserSnapshot
        {
            public int UserID { get; set; }
            /// <summary>打分输入（期望地/坐标/薪资/证书等）</summary>
            public UserInfo Info { get; set; } = new();
            /// <summary>期望岗位 JobClass ID（含祖先展开）</summary>
            public HashSet<int> ExpectIds { get; set; } = [];
            /// <summary>工种偏好（Live_UserProfile.WorkTypePrefs 解析）</summary>
            public Dictionary<int, double> WorkPrefs { get; set; } = [];
            public double WorkMax { get; set; }
            /// <summary>类目偏好（Live_UserProfile.CatePrefs 解析）</summary>
            public Dictionary<int, double> CatePrefs { get; set; } = [];
            public double CateMax { get; set; }
            /// <summary>到岗率信誉快照（无画像记录 0.4 中性）</summary>
            public double Credibility { get; set; }
            /// <summary>求职状态快照</summary>
            public byte SeekStatus { get; set; }
            /// <summary>最近入职工种名</summary>
            public string? LastWorkName { get; set; }
        }

        /// <summary>一个公司的建池上下文（候选人与画像快照，公司内所有岗位共用）</summary>
        private sealed class CompanyCandidates
        {
            public List<CandidateUser> Users { get; set; } = [];
            public Dictionary<int, UserSnapshot> Snapshots { get; set; } = [];
        }

        /// <summary>用户信息（只取匹配所需列）</summary>
        private sealed class UserInfo
        {
            public string? Sex { get; set; }
            public DateTime? BirthDate { get; set; }
            public int? Age { get; set; }

            // 期望工作地（注册/完善资料时填写）
            public string? ExpectProvince { get; set; }
            public string? ExpectCity { get; set; }
            public string? ExpectDistrict { get; set; }

            // 进小程序时的实时位置
            public string? MiniCity { get; set; }
            public string? MiniDistrict { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }

            // 期望薪资
            public decimal? MinMoney { get; set; }
            public decimal? MaxMoney { get; set; }

            // 期望岗位（逗号分隔 JobClass ID）
            public string? Jobs { get; set; }

            // 特种作业操作证
            public string? SpecialCertificate { get; set; }
        }

        /// <summary>岗位类目节点</summary>
        private sealed class JobClassNode
        {
            public int ID { get; set; }
            public int? PID { get; set; }
            public string? Name { get; set; }
        }

        /// <summary>匹配字典集合（每请求加载一次，小表整表）</summary>
        private sealed class MatchDictionaries
        {
            /// <summary>类目树（Live_JobClass，辅助轴）</summary>
            public Dictionary<int, JobClassNode> ClassMap { get; init; } = [];

            /// <summary>工种 ID → 名称（Live_EntryJob，主轴）</summary>
            public Dictionary<int, string> EntryJobNames { get; init; } = [];

            /// <summary>工种名 → ID（历史 WorkName 反查，含停用工种）</summary>
            public Dictionary<string, int> EntryJobIdsByName { get; init; } = [];

            /// <summary>客户 → 可用工种集合（Live_CustomerJobs，岗位侧工种来源）</summary>
            public Dictionary<int, List<int>> CustomerEntryJobs { get; init; } = [];
        }

        #endregion
    }
}
