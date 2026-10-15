using LZ.ApiService.Helper;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model.BaiWangModel;
using LZ.Model.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace LZ.ApiService.Services
{
    public class BaiWangService : IBaiWangService, IScopedDependency
    {
        private readonly BaiWangOption _option;
        private readonly IRedisService _redisService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BaiWangService> _logger;

        public BaiWangService(
            IOptions<BaiWangOption> option,
            IRedisService redisService,
            IHttpClientFactory httpClientFactory,
            ILogger<BaiWangService> logger)
        {
            _option = option.Value;
            _redisService = redisService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public Task<BaiWangResponse<BaiWangAddPreInvoiceResponse>> AddPreInvoiceAsync(BaiWangAddPreInvoiceRequest request)
        {
            return ExecuteAsync<BaiWangAddPreInvoiceRequest, BaiWangAddPreInvoiceResponse>(_option.AddPreInvoiceMethod, request);
        }

        public Task<BaiWangResponse<BaiWangQueryPreInvoiceResponse>> QueryPreInvoiceAsync(BaiWangQueryPreInvoiceRequest request)
        {
            return ExecuteAsync<BaiWangQueryPreInvoiceRequest, BaiWangQueryPreInvoiceResponse>(_option.QueryPreInvoiceMethod, request);
        }

        public Task<BaiWangResponse<BaiWangQueryInvoiceResponse>> QueryInvoiceAsync(BaiWangQueryInvoiceRequest request)
        {
            return ExecuteAsync<BaiWangQueryInvoiceRequest, BaiWangQueryInvoiceResponse>(_option.QueryInvoiceMethod, request);
        }

        public Task<BaiWangResponse<BaiWangIssueInvoiceResponse>> IssueInvoiceAsync(BaiWangIssueInvoiceRequest request)
        {
            return ExecuteAsync<BaiWangIssueInvoiceRequest, BaiWangIssueInvoiceResponse>(_option.IssueInvoiceMethod, request);
        }

        public async Task<BaiWangTokenResponse> GetTokenAsync(bool cacheToken = false)
        {
            if (cacheToken)
            {
                var cachedToken = await _redisService.GetAsync<string>(_option.CacheKey);
                if (!string.IsNullOrWhiteSpace(cachedToken))
                {
                    return new BaiWangTokenResponse
                    {
                        access_token = cachedToken,
                        token_type = "bearer"
                    };
                }
            }

            var tokenResponse = await RequestTokenByPostAsync();
            if (cacheToken)
            {
                await CacheTokenAsync(tokenResponse);
            }

            return tokenResponse;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var accessToken = await _redisService.GetAsync<string>(_option.CacheKey);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                return accessToken;
            }

            var tokenResponse = await RequestTokenByPostAsync();
            await CacheTokenAsync(tokenResponse);
            return tokenResponse.access_token;
        }

        private async Task<BaiWangTokenResponse> RequestTokenByPostAsync()
        {
            var jsonData = new Dictionary<string, string>
            {
                ["client_secret"] = _option.AppSecret,
                ["username"] = _option.UserName,
                ["password"] = BaiWangHelper.EncryptPassword(_option.Password, _option.Salt)
            };
            var tokenUrl = GetApiRootUrl();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            dic.Add("method", "baiwang.oauth.token");
            dic.Add("grant_type", "password");
            dic.Add("version", _option.Version);
            dic.Add("client_id", _option.AppKey);
            dic.Add("encryptScope", "00");
            dic.Add("encryptType", "NONE");
            var query = string.Join("&", dic.Select(item =>
                $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value)}"));
            var url = $"{GetApiRootUrl()}?{query}";
            //var url = tokenUrl + "?" + DictionaryToQueryString(dic);
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(jsonData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("百望获取Token请求 Url={Url}，Response={Response}", tokenUrl, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("百望获取Token失败，StatusCode:{StatusCode}，Response:{Response}", response.StatusCode, responseContent);
                throw new InvalidOperationException($"百望获取Token失败：{responseContent}");
            }
            var result = JsonConvert.DeserializeObject(responseContent) as JObject;
            var tokenResponse = JsonConvert.DeserializeObject<BaiWangTokenResponse>(result["response"].ToString());
            if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.access_token))
            {
                var errorMessage = tokenResponse?.error_description ?? tokenResponse?.error ?? responseContent;
                throw new InvalidOperationException($"百望获取Token失败：{errorMessage}");
            }

            return tokenResponse;
        }



        private async Task CacheTokenAsync(BaiWangTokenResponse tokenResponse)
        {
            var cacheSeconds = tokenResponse.expires_in > 120 ? tokenResponse.expires_in - 60 : tokenResponse.expires_in;
            await _redisService.SetAsync(_option.CacheKey, tokenResponse.access_token, TimeSpan.FromSeconds(cacheSeconds));
        }

        private string GetApiRootUrl()
        {
            var baseUrl = _option.BaseUrl.TrimEnd('/');
            const string routerSuffix = "/router/rest";
            return baseUrl+ routerSuffix;
        }

        public static string DictionaryToQueryString(Dictionary<string, string> param)
        {
            if (param == null || param.Count == 0)
                return string.Empty;

            var builder = new StringBuilder();
            foreach (var entry in param)
            {
                // 跳过空键或空值的项（按需保留）
                if (string.IsNullOrWhiteSpace(entry.Key) || entry.Value == null)
                    continue;

                builder.Append(Uri.EscapeDataString(entry.Key));
                builder.Append('=');
                builder.Append(Uri.EscapeDataString(entry.Value));
                builder.Append('&');
            }

            // 去掉末尾多余的 '&'
            return builder.ToString().TrimEnd('&');
        }



        private async Task<BaiWangResponse<TResponse>> ExecuteAsync<TRequest, TResponse>(string method, TRequest request)
        {
            var accessToken = await GetAccessTokenAsync();
            var bodyJson = BaiWangHelper.ToJson(request);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            var signParams = new Dictionary<string, string>
            {
                ["method"] = method,
                ["version"] = _option.Version,
                ["appKey"] = _option.AppKey,
                ["format"] = "json",
                ["timestamp"] = timestamp,
                ["token"] = accessToken,
                ["type"] = "sync",
                ["encryptScope"] = "00",
                ["encryptType"] = "NONE"
            };

            var sign = BaiWangHelper.Sign(signParams, bodyJson, _option.AppSecret);
            var query = string.Join("&", signParams.Select(item =>
                $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value)}"));
            var url = $"{GetApiRootUrl()}?{query}&sign={Uri.EscapeDataString(sign)}";

            var client = _httpClientFactory.CreateClient();
            using var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("百望接口请求 method={Method}，Response={Response}", method, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                return new BaiWangResponse<TResponse>
                {
                    success = false,
                    method = method,
                    errorResponse = new BaiWangErrorResponse
                    {
                        code = ((int)response.StatusCode).ToString(),
                        message = $"百望接口请求异常，HttpStatusCode:{response.StatusCode}"
                    }
                };
            }

            var result = JsonConvert.DeserializeObject<BaiWangResponse<TResponse>>(responseContent);
            return result ?? new BaiWangResponse<TResponse>
            {
                success = false,
                method = method,
                errorResponse = new BaiWangErrorResponse
                {
                    code = "-1",
                    message = "百望接口响应解析失败"
                }
            };
        }
    }
}
