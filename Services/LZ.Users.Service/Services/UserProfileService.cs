using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Newtonsoft.Json;
using SqlSugar;

namespace LZ.Users.Service.Services
{
    /// <summary>
    /// 用户画像服务（会员侧）：Live_UserProfile 快照的查询与展示。
    ///
    /// 画像数据由岗位匹配服务的夜间预计算写入（JobTaskController.JobRecommendPrecomputeJob）：
    /// 工种/类目偏好、到岗率信誉、求职状态、流动性、薪资心理价位等；
    /// 本服务只读，负责列表（分页筛选）与详情（偏好 JSON 解析为可读结构 + 岗位匹配记录）。
    /// 岗位匹配记录（JobMatches）读 Live_JobCandidatePool 物化清单，自动过滤下架/过期岗位。
    /// </summary>
    public class UserProfileService : ServicesBase<Live_UserProfile>, IUserProfileService, IScopedDependency
    {
        /// <summary>
        /// 画像列表：Live_UserProfile 分页，支持公司/求职状态/工种筛选，
        /// 工种筛选按 WorkTypePrefs JSON 匹配（画像表为快照表，量级=用户数，可接受）。
        /// </summary>
        public async Task<List<UserProfileListModel>> List(int companyId, byte? seekStatus, int entryJobId, PagedModelBase query)
        {
            var expable = Expressionable.Create<Live_UserProfile>();
            expable.AndIF(companyId > 0, it => it.CompanyID == companyId);
            expable.AndIF(seekStatus.HasValue, it => it.SeekStatus == seekStatus.Value);
            if (entryJobId > 0)
            {
                expable.And(it => it.WorkTypePrefs != null && it.WorkTypePrefs != ""
                    && it.WorkTypePrefs.Contains($"\"{entryJobId}\""));
            }

            var qb = Context.Queryable<Live_UserProfile>()
                .Where(expable.ToExpression())
                .OrderByDescending(it => it.LastCalcTime);

            List<Live_UserProfile> rows;
            if (query.Page == 0)
            {
                rows = await qb.Take(query.Top > 0 ? query.Top : 200).ToListAsync();
                query.Count = rows.Count;
            }
            else
            {
                var count = new RefAsync<int>();
                rows = await qb.ToOffsetPageAsync(query.Page, query.PageSize, count);
                query.Count = count.Value;
            }
            if (rows.Count == 0)
            {
                return [];
            }

            // 用户基础信息（姓名/手机号）
            var userIds = rows.Select(it => it.UserID).ToList();
            var users = await Context.Queryable<Live_User>()
                .Where(it => userIds.Contains(it.UserID))
                .Select(it => new { it.UserID, it.FullName, it.Phone })
                .ToListAsync();
            var userMap = users.ToDictionary(it => it.UserID);

            var result = new List<UserProfileListModel>(rows.Count);
            foreach (var row in rows)
            {
                var user = userMap.GetValueOrDefault(row.UserID);
                result.Add(new UserProfileListModel
                {
                    UserID = row.UserID,
                    FullName = user?.FullName ?? string.Empty,
                    Phone = user?.Phone ?? string.Empty,
                    WorkTypeTop = row.WorkTypeTop ?? string.Empty,
                    SeekStatus = row.SeekStatus,
                    MobilityType = row.MobilityType,
                    CredibilityScore = row.CredibilityScore,
                    HiredCount = row.HiredCount,
                    QuickLeaveCount = row.QuickLeaveCount,
                    AvgStayDays = row.AvgStayDays,
                    SalaryLevel = row.SalaryLevel,
                    RiskClassMode = row.RiskClassMode ?? string.Empty,
                    LastEntryWorkName = row.LastEntryWorkName ?? string.Empty,
                    LastBehaviorTime = row.LastBehaviorTime,
                    LastCalcTime = row.LastCalcTime
                });
            }
            return result;
        }

        /// <summary>画像详情（单人）：偏好 JSON 解析为明细 + 信誉 + 状态 + 岗位匹配记录。
        /// 优先按 userGID 查询（外部系统/前端常用 GID），未传时按 userID。</summary>
        public async Task<UserProfileDetailModel> Get(int userId, Guid? userGid = null)
        {
            var hasGid = userGid.HasValue && userGid.Value != Guid.Empty;
            if (!hasGid && userId <= 0)
            {
                return null;
            }

            var row = await Context.Queryable<Live_UserProfile>()
                .Where(it => hasGid ? it.UserGID == userGid : it.UserID == userId)
                .FirstAsync();
            if (row == null)
            {
                return null;
            }

            // 个人信息（GID/生日/身份证/籍贯(身份证地址)/学历/期望工作地/期望工资/实名/技能/证书等）
            var user = await Context.Queryable<Live_User>()
                .Where(it => it.UserID == row.UserID).FirstAsync();

            // 年龄：出生日期实时计算优先，退化 Age 冗余字段
            int? age = null;
            if (user?.BirthDate != null)
            {
                var a = DateTime.Now.Year - user.BirthDate.Value.Year;
                if (DateTime.Now < user.BirthDate.Value.AddYears(a))
                {
                    a--;
                }
                if (a > 0)
                {
                    age = a;
                }
            }
            else if (user?.Age is > 0)
            {
                age = user.Age;
            }

            // 工种偏好 → 名称（含停用工种：名称缺则显示 ID）
            var workPrefs = ParsePrefsJson(row.WorkTypePrefs);
            var workItems = new List<PrefItem>();
            if (workPrefs.Count > 0)
            {
                var entryJobs = await Context.Queryable<Live_EntryJob>()
                    .Where(it => workPrefs.Keys.Contains(it.ID))
                    .Select(it => new { it.ID, it.Name })
                    .ToListAsync();
                var nameMap = entryJobs.ToDictionary(it => it.ID, it => it.Name);
                workItems = workPrefs
                    .Select(kv => new PrefItem { ID = kv.Key, Name = nameMap.GetValueOrDefault(kv.Key, $"#{kv.Key}"), Score = Math.Round(kv.Value, 2) })
                    .OrderByDescending(it => it.Score)
                    .ToList();
            }

            // 类目偏好 → 名称
            var catePrefs = ParsePrefsJson(row.CatePrefs);
            var cateItems = new List<PrefItem>();
            if (catePrefs.Count > 0)
            {
                var classes = await Context.Queryable<Live_JobClass>()
                    .Where(it => catePrefs.Keys.Contains(it.ID))
                    .Select(it => new { it.ID, it.Name })
                    .ToListAsync();
                var nameMap = classes.ToDictionary(it => it.ID, it => it.Name);
                cateItems = catePrefs
                    .Select(kv => new PrefItem { ID = kv.Key, Name = nameMap.GetValueOrDefault(kv.Key, $"#{kv.Key}"), Score = Math.Round(kv.Value, 2) })
                    .OrderByDescending(it => it.Score)
                    .ToList();
            }

            return new UserProfileDetailModel
            {
                UserID = row.UserID,
                UserGID = user?.UserGID,
                FullName = user?.FullName ?? string.Empty,
                Phone = user?.Phone ?? string.Empty,

                // 个人信息
                BirthDate = user?.BirthDate,
                Age = age,
                Sex = user?.Sex ?? string.Empty,
                CardID = user?.CardID ?? string.Empty,
                NativePlace = user?.IdcardAddress ?? string.Empty,
                Education = user?.Education ?? string.Empty,
                IsCaAuth = user?.IsCaAuth ?? false,
                IsReal = user?.IsReal ?? false,
                IdPhoto = user?.IdPhoto ?? string.Empty,
                ExpectProvince = user?.Province ?? string.Empty,
                ExpectCity = user?.City ?? string.Empty,
                ExpectDistrict = user?.District ?? string.Empty,
                ExpectMinMoney = user?.MinMoney,
                ExpectMaxMoney = user?.MaxMoney,
                MiniProvince = user?.MiniProvince ?? string.Empty,
                MiniCity = user?.MiniCity ?? string.Empty,
                MiniDistrict = user?.MiniDistrict ?? string.Empty,
                SkillType = user?.SkillType,
                JobSkills = user?.JobSkills ?? string.Empty,
                SpecialCertificate = user?.SpecialCertificate ?? string.Empty,
                Tabs = user?.Tabs ?? string.Empty,
                SysTabs= user?.SysTabs ?? string.Empty,

                // 画像
                WorkTypePrefs = workItems,
                CatePrefs = cateItems,
                SalaryLevel = row.SalaryLevel,
                CredibilityScore = row.CredibilityScore,
                HiredCount = row.HiredCount,
                AppliedCount = row.AppliedCount,
                QuickLeaveCount = row.QuickLeaveCount,
                AvgStayDays = row.AvgStayDays,
                RiskClassMode = row.RiskClassMode ?? string.Empty,
                SeekStatus = row.SeekStatus,
                MobilityType = row.MobilityType,
                LastBehaviorTime = row.LastBehaviorTime,
                LastEntryWorkName = row.LastEntryWorkName ?? string.Empty,
                LastCalcTime = row.LastCalcTime,
                JobMatches = await GetUserMatches(row.UserID)
            };
        }

        /// <summary>会员的岗位匹配记录（读物化清单，过滤下架/过期岗位）</summary>
        private async Task<List<JobUserMatchModel>> GetUserMatches(int userId)
        {
            var rows = await Context.Queryable<Live_JobCandidatePool>()
                .Where(it => it.UserID == userId)
                .OrderByDescending(it => it.Score)
                .Take(200)
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
    }
}
