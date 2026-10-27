using LZ.Agent.Service.Skills;
using LZ.Model;

namespace LZ.Agent.Service.Interface
{
    public interface IQuerySkillService
    {
        IReadOnlyList<QuerySkillSummary> List();

        Task<RequestResultModel> QueryAsync(string? name, IDictionary<string, string?>? arguments);
    }
}
