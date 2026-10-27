using LZ.Agent.Service.Interface;
using LZ.Agent.Service.Skills;
using LZ.Common;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LZ.Agent.Service.Services
{
    public class QuerySkillService(IHttpService httpService, ICurrentUser currentUser) : IQuerySkillService, IScopedDependency
    {
        private const int MaxPageSize = 50;

        private static readonly HashSet<string> DeniedArguments = new(StringComparer.OrdinalIgnoreCase)
        {
            "userId", "userID", "userGid", "userGID",
            "companyId", "companyID", "companyGid", "companyGID",
            "shopId", "shopID", "platformType", "platform",
            "token", "authorization", "company-guid"
        };

        public IReadOnlyList<QuerySkillSummary> List()
        {
            return QuerySkillCatalog.All.Select(skill => new QuerySkillSummary
            {
                Name = skill.Name,
                Description = skill.Description,
                Platforms = skill.Platforms.Length == 0
                    ? ["已登录即可"]
                    : skill.Platforms.Select(QuerySkillCatalog.PlatformName).ToArray(),
                Parameters = skill.Parameters
            }).ToList();
        }

        public async Task<RequestResultModel> QueryAsync(string? name, IDictionary<string, string?>? arguments)
        {
            if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.Token))
            {
                return new RequestResultModel(StatusCodes.Status401Unauthorized, "请先登录");
            }
            if (currentUser.PlatformType == PlatformType.Api)
            {
                return new RequestResultModel(StatusCodes.Status403Forbidden, "智能体不使用 API 平台身份");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                return new RequestResultModel(StatusCodes.Status400BadRequest, "请指定查询技能");
            }

            var skill = QuerySkillCatalog.All.FirstOrDefault(it => string.Equals(it.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
            if (skill == null)
            {
                return new RequestResultModel(StatusCodes.Status400BadRequest, "没有这个查询技能");
            }
            if (skill.Platforms.Length > 0 && !skill.Platforms.Contains(currentUser.PlatformType))
            {
                return new RequestResultModel(StatusCodes.Status403Forbidden, "当前账号无权使用该查询");
            }

            var input = arguments ?? new Dictionary<string, string?>();
            var denied = input.Keys.FirstOrDefault(key => DeniedArguments.Contains(key));
            if (denied != null)
            {
                return new RequestResultModel(StatusCodes.Status400BadRequest, "不能传入身份字段");
            }

            var query = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in input)
            {
                if (string.IsNullOrWhiteSpace(pair.Key) || string.IsNullOrWhiteSpace(pair.Value))
                {
                    continue;
                }
                var parameter = skill.Parameters.FirstOrDefault(it => string.Equals(it.Name, pair.Key, StringComparison.OrdinalIgnoreCase));
                if (parameter == null)
                {
                    return new RequestResultModel(StatusCodes.Status400BadRequest, "不支持的查询条件：" + pair.Key);
                }
                if (!TryNormalize(parameter, pair.Value, out var value, out var error))
                {
                    return new RequestResultModel(StatusCodes.Status400BadRequest, error);
                }
                query[parameter.Name] = value;
            }

            foreach (var parameter in skill.Parameters.Where(it => it.Required))
            {
                if (!query.ContainsKey(parameter.Name))
                {
                    return new RequestResultModel(StatusCodes.Status400BadRequest, "缺少查询条件：" + parameter.Name);
                }
            }

            if (query.TryGetValue("pageSize", out var pageSizeText) && int.TryParse(pageSizeText, out var pageSize) && pageSize > MaxPageSize)
            {
                query["pageSize"] = MaxPageSize.ToString();
            }

            string serviceUrl;
            try
            {
                serviceUrl = httpService.GetServicePath(skill.ServiceName);
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(StatusCodes.Status500InternalServerError, "查询服务暂不可用");
            }

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = currentUser.Token
            };
            if (currentUser.CompanyGID.HasValue && currentUser.CompanyGID.Value != Guid.Empty)
            {
                headers["company-guid"] = currentUser.CompanyGID.Value.ToString();
            }

            StaticLog.Debug($"skill:{skill.Name},user:{currentUser.UserID}");
            var body = await httpService.GetAsync(serviceUrl + AppendQuery(skill.Path, query), data: null!, headers);
            if (string.IsNullOrWhiteSpace(body) || body == "error")
            {
                return new RequestResultModel(StatusCodes.Status500InternalServerError, "查询失败");
            }

            RequestResultModel? result;
            try
            {
                result = JsonConvert.DeserializeObject<RequestResultModel>(body);
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(StatusCodes.Status500InternalServerError, "查询结果无法解析");
            }
            if (result == null)
            {
                return new RequestResultModel(StatusCodes.Status500InternalServerError, "查询结果为空");
            }
            if (skill.CheckCompany && result.Code == StatusCodes.Status200OK && result.Data is JToken data && BelongsToAnotherCompany(data, currentUser))
            {
                return new RequestResultModel(StatusCodes.Status403Forbidden, "这条记录不属于当前公司");
            }
            return result;
        }

        private static bool TryNormalize(QuerySkillParameter parameter, string raw, out string value, out string error)
        {
            value = raw.Trim();
            error = "";
            switch (parameter.Type)
            {
                case QuerySkillValueType.Int:
                    if (!int.TryParse(value, out var number) || (parameter.Name is "page" or "pageSize" && number < 1))
                    {
                        error = parameter.Description + "不正确";
                        return false;
                    }
                    value = number.ToString();
                    return true;
                case QuerySkillValueType.Guid:
                    if (!Guid.TryParse(value, out var id))
                    {
                        error = parameter.Description + "不正确";
                        return false;
                    }
                    value = id.ToString();
                    return true;
                case QuerySkillValueType.DateTime:
                    if (!DateTime.TryParse(value, out var time))
                    {
                        error = parameter.Description + "不正确";
                        return false;
                    }
                    value = time.ToString("yyyy-MM-dd HH:mm:ss");
                    return true;
                case QuerySkillValueType.Bool:
                    if (!bool.TryParse(value, out var flag))
                    {
                        error = parameter.Description + "不正确";
                        return false;
                    }
                    value = flag ? "true" : "false";
                    return true;
                case QuerySkillValueType.Number:
                    if (!double.TryParse(value, out var numeric))
                    {
                        error = parameter.Description + "不正确";
                        return false;
                    }
                    value = numeric.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return true;
                default:
                    return true;
            }
        }

        private static bool BelongsToAnotherCompany(JToken data, ICurrentUser user)
        {
            if (user.PlatformType == PlatformType.Admin)
            {
                return false;
            }
            if (string.Equals(user.CompanyType, CompanyType.Party.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return HasOtherCompany(data, user.CompanyID);
        }

        private static bool HasOtherCompany(JToken token, int companyId)
        {
            if (token is JArray array)
            {
                return array.Any(item => HasOtherCompany(item, companyId));
            }
            if (token is not JObject obj)
            {
                return false;
            }
            if (ReadCompanyId(obj, out var recordCompany) && recordCompany != companyId)
            {
                return true;
            }
            var model = obj["model"] ?? obj["Model"];
            return model != null && HasOtherCompany(model, companyId);
        }

        private static bool ReadCompanyId(JObject obj, out int companyId)
        {
            companyId = 0;
            var token = obj["companyID"] ?? obj["CompanyID"];
            return token != null && token.Type != JTokenType.Null && int.TryParse(token.ToString(), out companyId) && companyId > 0;
        }

        private static string AppendQuery(string path, IReadOnlyDictionary<string, string> query)
        {
            if (query.Count == 0)
            {
                return path;
            }
            var parts = query.Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}");
            return path + "?" + string.Join("&", parts);
        }
    }
}
