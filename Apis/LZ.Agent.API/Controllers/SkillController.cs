using LZ.Agent.Service.Interface;
using LZ.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LZ.Agent.API
{
    public class SkillController(IQuerySkillService querySkillService) : AgentControllerBase
    {
        [HttpGet]
        public IActionResult List()
        {
            return Ok(querySkillService.List());
        }

        [HttpPost]
        public async Task<IActionResult> Query([FromBody] SkillQueryRequest? request)
        {
            if (!TryReadArguments(request?.Arguments, out var arguments, out var error))
            {
                return Ok(new RequestResultModel(StatusCodes.Status400BadRequest, error));
            }
            var result = await querySkillService.QueryAsync(request?.Name, arguments);
            return Ok(result);
        }

        private static bool TryReadArguments(Dictionary<string, JsonElement>? source, out Dictionary<string, string?> arguments, out string error)
        {
            arguments = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            error = "";
            if (source == null)
            {
                return true;
            }
            foreach (var pair in source)
            {
                if (pair.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array)
                {
                    error = "查询条件必须是单个值";
                    return false;
                }
                arguments[pair.Key] = pair.Value.ValueKind switch
                {
                    JsonValueKind.String => pair.Value.GetString(),
                    JsonValueKind.Number => pair.Value.GetRawText(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    _ => null
                };
            }
            return true;
        }
    }

    public class SkillQueryRequest
    {
        public string? Name { get; set; }
        public Dictionary<string, JsonElement>? Arguments { get; set; }
    }
}
