using LZ.Model.Enums;

namespace LZ.Agent.Service.Skills
{
    public enum QuerySkillValueType
    {
        String,
        Int,
        Guid,
        DateTime,
        Bool,
        Number
    }

    public sealed class QuerySkillParameter
    {
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";
        public QuerySkillValueType Type { get; init; }
        public bool Required { get; init; }
    }

    /// <summary>
    /// 只读查询技能。下游路径只在执行器内部使用。
    /// </summary>
    public sealed class QuerySkillDefinition
    {
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";
        public string ServiceName { get; init; } = "";
        public string Path { get; init; } = "";
        public bool CheckCompany { get; init; }
        public PlatformType[] Platforms { get; init; } = [];
        public QuerySkillParameter[] Parameters { get; init; } = [];
    }

    public sealed class QuerySkillSummary
    {
        public string Name { get; init; } = "";
        public string Description { get; init; } = "";
        public string[] Platforms { get; init; } = [];
        public QuerySkillParameter[] Parameters { get; init; } = [];
    }
}
