using LZ.Core.Services;
using LZ.Model.Enums;

namespace LZ.Agent.Service.Skills
{
    /// <summary>
    /// 第一批只读查询。入参只有业务条件，身份字段由执行器从当前 Token 带上。
    /// </summary>
    public static class QuerySkillCatalog
    {
        private static QuerySkillParameter Text(string name, string description, bool required = false) => new()
        {
            Name = name,
            Description = description,
            Type = QuerySkillValueType.String,
            Required = required
        };

        private static QuerySkillParameter Int(string name, string description, bool required = false) => new()
        {
            Name = name,
            Description = description,
            Type = QuerySkillValueType.Int,
            Required = required
        };

        private static QuerySkillParameter Date(string name, string description, bool required = false) => new()
        {
            Name = name,
            Description = description,
            Type = QuerySkillValueType.DateTime,
            Required = required
        };

        private static QuerySkillParameter Flag(string name, string description) => new()
        {
            Name = name,
            Description = description,
            Type = QuerySkillValueType.Bool
        };

        private static QuerySkillParameter Num(string name, string description, bool required = false) => new()
        {
            Name = name,
            Description = description,
            Type = QuerySkillValueType.Number,
            Required = required
        };

        private static readonly QuerySkillParameter[] Page =
        [
            Int("page", "页码，从 1 开始"),
            Int("pageSize", "每页条数，最大 50")
        ];

        private static readonly PlatformType[] JobPlatforms = [PlatformType.Admin, PlatformType.MiniAdmin, PlatformType.LZMini];
        private static readonly PlatformType[] EntryPlatforms = [PlatformType.Admin, PlatformType.MiniAdmin, PlatformType.LZMini, PlatformType.XFZMini];
        private static readonly PlatformType[] BackOffice = [PlatformType.Admin, PlatformType.MiniAdmin];

        public static IReadOnlyList<QuerySkillDefinition> All { get; } =
        [
            new()
            {
                Name = "current-user",
                Description = "查看当前登录用户的资料",
                ServiceName = ServiceNames.UsersService,
                Path = "/api/User/Info"
            },
            new()
            {
                Name = "my-entry",
                Description = "员工查看自己的入职信息",
                ServiceName = ServiceNames.UsersService,
                Path = "/api/Entry/UserEntryInfo",
                Platforms = [PlatformType.XFZMini]
            },
            new()
            {
                Name = "customer-search",
                Description = "按名称、城市合伙人或业务员查询当前公司可见的客户",
                ServiceName = ServiceNames.CustomerService,
                Path = "/api/Customer/List",
                Platforms = [PlatformType.Admin, PlatformType.MiniAdmin, PlatformType.LZMini],
                Parameters = Concat(Page,
                    Text("name", "客户名称，模糊匹配"),
                    Text("cityPartnerTitle", "城市合伙人名称"),
                    Text("salesman", "业务员"),
                    Flag("enabled", "是否启用"))
            },
            new()
            {
                Name = "customer-detail",
                Description = "查看客户详情。编号必须来自本次 customer-search 的结果",
                ServiceName = ServiceNames.CustomerService,
                Path = "/api/Customer/GetInfo",
                CheckCompany = true,
                Platforms = BackOffice,
                Parameters = [Int("id", "客户编号", true)]
            },
            new()
            {
                Name = "job-search",
                Description = "按标题、城市、客户或状态查询岗位",
                ServiceName = ServiceNames.CustomerService,
                Path = "/api/Jobs/List",
                Platforms = JobPlatforms,
                Parameters = Concat(Page,
                    Text("name", "岗位标题"),
                    Text("city", "城市"),
                    Text("customerName", "客户名称"),
                    Int("status", "岗位状态"))
            },
            new()
            {
                Name = "job-nearby",
                Description = "按当前位置查询附近岗位，半径单位为公里，最大 100",
                ServiceName = ServiceNames.CustomerService,
                Path = "/api/Jobs/Nearby",
                Platforms = JobPlatforms,
                Parameters = Concat(Page,
                    Num("latitude", "纬度", true),
                    Num("longitude", "经度", true),
                    Num("radius", "搜索半径，公里"),
                    Text("name", "岗位标题"),
                    Text("city", "城市"),
                    Text("customerName", "客户名称"),
                    Int("status", "岗位状态"))
            },
            new()
            {
                Name = "job-recommend",
                Description = "按当前登录用户推荐岗位，筛选条件与岗位查询相同",
                ServiceName = ServiceNames.CustomerService,
                Path = "/api/Jobs/Recommend",
                Platforms = JobPlatforms,
                Parameters = Concat(Page,
                    Text("name", "岗位标题"),
                    Text("city", "城市"),
                    Text("customerName", "客户名称"),
                    Int("status", "岗位状态"))
            },
            new()
            {
                Name = "job-candidates",
                Description = "给指定岗位查找候选人。岗位编号必须来自本次 job-search 的结果",
                ServiceName = ServiceNames.CustomerService,
                Path = "/api/Jobs/RecommendUsers",
                Platforms = JobPlatforms,
                Parameters = Concat(Page, Int("id", "岗位编号", true))
            },
            new()
            {
                Name = "my-job-matches",
                Description = "查看当前用户与岗位的匹配记录。可只看某一个岗位",
                ServiceName = ServiceNames.CustomerService,
                Path = "/api/Jobs/UserMatches",
                Platforms = JobPlatforms,
                Parameters = [Int("jobID", "岗位编号，不传则返回全部匹配")]
            },
            new()
            {
                Name = "entry-search",
                Description = "按姓名、手机、身份证、客户、状态或入离职时间查询入职记录",
                ServiceName = ServiceNames.UsersService,
                Path = "/api/Entry/List",
                Platforms = EntryPlatforms,
                Parameters = Concat(Page,
                    Text("name", "姓名"),
                    Text("keyword", "姓名或手机号"),
                    Text("phone", "手机号"),
                    Text("cardID", "身份证号"),
                    Text("customerName", "客户名称"),
                    Int("status", "入职状态"),
                    Date("entryStartTime", "入职开始时间"),
                    Date("entryEndTime", "入职结束时间"),
                    Date("leaveStartTime", "离职开始时间"),
                    Date("leaveEndTime", "离职结束时间"))
            },
            new()
            {
                Name = "entry-roster",
                Description = "查询某个时点仍在职的花名册。不传时间则按当天",
                ServiceName = ServiceNames.UsersService,
                Path = "/api/Entry/Roster",
                Platforms = EntryPlatforms,
                Parameters = [Date("entryStartTime", "在职时点")]
            },
            new()
            {
                Name = "contract-search",
                Description = "查询入职或离职合同及签署状态",
                ServiceName = ServiceNames.CustomerService,
                Path = "/api/ContractRecord/PageList",
                Parameters = Concat(Page,
                    Text("name", "员工姓名"),
                    Text("customerName", "客户名称"),
                    Text("idCardNo", "身份证号"),
                    Int("status", "签署状态"),
                    Int("contractType", "合同类型"),
                    Flag("isTimeout", "入职合同是否超时"))
            },
            new()
            {
                Name = "salary-batch-search",
                Description = "查询工资批次",
                ServiceName = ServiceNames.FinanceService,
                Path = "/api/SalaryCustomer/PageList",
                Parameters = Concat(Page,
                    Text("customerName", "客户名称"),
                    Text("signingUnitName", "签约单位"),
                    Date("salaryMonth", "工资所属月"),
                    Date("taxMonth", "税款所属月"),
                    Int("status", "批次状态"),
                    Date("sendTime", "发放日期"))
            },
            new()
            {
                Name = "salary-batch-detail",
                Description = "查看工资批次详情。编号必须来自本次 salary-batch-search 的结果",
                ServiceName = ServiceNames.FinanceService,
                Path = "/api/SalaryCustomer/Get",
                CheckCompany = true,
                Parameters = [Id("id", "工资批次编号")]
            },
            new()
            {
                Name = "salary-record-search",
                Description = "查询工资明细和发放状态",
                ServiceName = ServiceNames.FinanceService,
                Path = "/api/SalaryRecord/PageList",
                Parameters = Concat(Page,
                    Text("name", "姓名"),
                    Text("idCardNo", "身份证号"),
                    Text("customerName", "客户名称"),
                    Text("signingUnitName", "签约单位"),
                    Date("salaryMonth", "工资所属月"),
                    Date("taxMonth", "税款所属月"),
                    Int("status", "发放状态"))
            },
            new()
            {
                Name = "salary-record-detail",
                Description = "查看一条工资明细。编号必须来自本次 salary-record-search 的结果",
                ServiceName = ServiceNames.FinanceService,
                Path = "/api/SalaryRecord/Get",
                CheckCompany = true,
                Parameters = [Id("id", "工资明细编号")]
            },
            new()
            {
                Name = "invoice-search",
                Description = "查询发票",
                ServiceName = ServiceNames.FinanceService,
                Path = "/api/Invoice/PageList",
                Parameters = Concat(Page,
                    Text("customerName", "客户名称"),
                    Text("issuerName", "开票方"),
                    Text("recipientName", "受票方"),
                    Int("status", "发票状态"),
                    Date("createMonth", "开票月份"))
            },
            new()
            {
                Name = "invoice-detail",
                Description = "查看发票详情。编号必须来自本次 invoice-search 的结果",
                ServiceName = ServiceNames.FinanceService,
                Path = "/api/Invoice/GetView",
                CheckCompany = true,
                Parameters = [Id("id", "发票编号")]
            },
            new()
            {
                Name = "receivable-search",
                Description = "查询应收",
                ServiceName = ServiceNames.FinanceService,
                Path = "/api/AccountReceivable/PageList",
                Parameters = Concat(Page,
                    Text("customerName", "客户名称"),
                    Text("issuerName", "开票方"),
                    Text("partnerName", "合伙人"),
                    Int("status", "应收状态"))
            },
            new()
            {
                Name = "receivable-detail",
                Description = "查看应收详情。编号必须来自本次 receivable-search 的结果",
                ServiceName = ServiceNames.FinanceService,
                Path = "/api/AccountReceivable/Get",
                CheckCompany = true,
                Parameters = [Id("id", "应收编号")]
            },
            new()
            {
                Name = "admin-home",
                Description = "查看后台首页汇总",
                ServiceName = ServiceNames.StatisticsService,
                Path = "/api/AdminStatistics/HomeStatistics",
                Platforms = BackOffice
            }
        ];

        public static string PlatformName(PlatformType platform) => platform switch
        {
            PlatformType.Admin => "总后台",
            PlatformType.MiniAdmin => "商家后台",
            PlatformType.XFZMini => "员工端",
            PlatformType.LZMini => "驻厂端",
            PlatformType.LWMini => "劳务",
            _ => platform.ToString()
        };

        private static QuerySkillParameter Id(string name, string description) => new()
        {
            Name = name,
            Description = description,
            Type = QuerySkillValueType.Guid,
            Required = true
        };

        private static QuerySkillParameter[] Concat(QuerySkillParameter[] page, params QuerySkillParameter[] extra)
        {
            var list = new QuerySkillParameter[extra.Length + page.Length];
            extra.CopyTo(list, 0);
            page.CopyTo(list, extra.Length);
            return list;
        }
    }
}
