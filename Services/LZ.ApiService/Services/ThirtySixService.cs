using Aliyun.Acs.Core;
using Azure.Core;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model.Exceptions;
using LZ.Model.InsuranceApi.ThirtySix;
using LZ.Model.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NPOI.HSSF.Record;
using Org.BouncyCastle.Ocsp;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static LZ.ApiService.Interface.IThirtySixService;

namespace LZ.ApiService.Services
{
    public class ThirtySixService: IThirtySixService, IScopedDependency
    {
        private static readonly JsonSerializerOptions RecordJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IRedisService _redisService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ThirtySixOptions _thirtySixOptions;

        public ThirtySixService(IHttpClientFactory clientFactory, IRedisService redisService, IOptions<ThirtySixOptions> options)
        {
            _clientFactory = clientFactory;
            _redisService = redisService;
            _thirtySixOptions = options.Value;
        }

        private async Task<string> GetToken()
        {
            string cacheKey = "ThirtySix_Token";
            string accessToken = string.Empty;
            accessToken = await _redisService.GetAsync<string>(cacheKey);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                return accessToken;
            }

            TokenRecord req = new TokenRecord(_thirtySixOptions.Id, _thirtySixOptions.Account, _thirtySixOptions.Password);

            var client = _clientFactory.CreateClient("ThirtySix");
            //client.DefaultRequestHeaders.Add("Authorization", "Basic bGFtcF93ZWJfcGx1czpsYW1wX3dlYl9wbHVzX3NlY3JldA==");
            //client.DefaultRequestHeaders.Add("Tenant", "MDAwMA==");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(req, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync("/api/oauth/noToken/getOauthAccessToken", httpContent);
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<ThirtySixResponse<string>>();
                await _redisService.SetAsync(cacheKey, result.Data, 3600);

                accessToken = await _redisService.GetAsync<string>(cacheKey);
                return accessToken;
            }
            else
            {
                throw new ErrorCodeException(-1, "获取授权码失败");
            }
        }

        public async Task<ThirtySixCusResponse> PushAdd(List<ThirtySixAddRecord> records)
        {
            try
            {
                string token = await GetToken();
                var client = _clientFactory.CreateClient("ThirtySix");
                client.DefaultRequestHeaders.Add("Token", token);

                // 客户单位名称统一格式化

                foreach (var item in records)
                {
                    if (!item.ActualInsuredCustomers.EndsWith("公司"))
                    {
                        item.ActualInsuredCustomers = item.ActualInsuredCustomers.Split('-')[0];
                        item.ActualInsuredCustomers = item.ActualInsuredCustomers.Split('(')[0];
                        item.ActualInsuredCustomers = item.ActualInsuredCustomers.Split('（')[0];
                    }
                }


                ThirtySixAddRequest req = new ThirtySixAddRequest()
                {
                    RequestJson = records,
                    FlowId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    Id = _thirtySixOptions.Id,
                    UserId = _thirtySixOptions.UserId,
                    OperatorDate = DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"),
                    PartnerAccountCode = _thirtySixOptions.PartnerAccountCode,
                    //RequestType = "COMMON_BATCH_CUSTOMER",
                    RequestType = "COMMON_BATCH_CUSTOMER_URL",
                    PaymentType = "1",
                    Premium = "0"
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(req, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync("/api/basis/forward/commonRequest", httpContent);
                res.EnsureSuccessStatusCode();
                var result = await res.Content.ReadFromJsonAsync<ThirtySixResponse<object>>();
                ThirtySixCusResponse cusResponse = new ThirtySixCusResponse()
                {
                    Code = result.Code,
                    ConfirmUrl = string.Empty,
                    ErrorMsg = result.Msg,
                    Extra = result.Extra,
                    IsSuccess = result.IsSuccess,
                    Msg = result.Msg,
                    Path = result.Path,
                    Timestamp = result.Timestamp,
                };
                if (result.Code == 0 || result.Code == 200)
                {
                    cusResponse.ConfirmUrl = result.Data.ToString();
                    cusResponse.Records = new List<ThirtySixResponseRecord>();
                }
                else
                {
                    cusResponse.ConfirmUrl = string.Empty;
                    cusResponse.Records = ToResponseRecords(result.Data);
                }
                return cusResponse;


             
            }
            catch (Exception ex)
            {
                ThirtySixCusResponse cusResponse = new ThirtySixCusResponse()
                {
                    Code = -1,
                    ConfirmUrl = string.Empty,
                    ErrorMsg = ex.Message,
                    Extra = null,
                    IsSuccess = false,
                    Msg = "推送接口请求失败",
                    Path = string.Empty,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    Records = null
                };
                return cusResponse;
            }
        }


        public async Task<ThirtySixCusResponse> PushReplace(List<ThirtySixReplaceRecord> recordList)
        {

            try
            {

                foreach (var item in recordList)
                {
                    if (!item.ActualInsuredCustomers.EndsWith("公司"))
                    {
                        item.ActualInsuredCustomers = item.ActualInsuredCustomers.Split('-')[0];
                        item.ActualInsuredCustomers = item.ActualInsuredCustomers.Split('(')[0];
                        item.ActualInsuredCustomers = item.ActualInsuredCustomers.Split('（')[0];
                    }
                }
                ThirtySixReplaceRequest req = new ThirtySixReplaceRequest()
                {
                    RequestJson = recordList,
                    FlowId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    Id = _thirtySixOptions.Id,
                    UserId = _thirtySixOptions.UserId,
                    OperatorDate = DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"),
                    PartnerAccountCode = _thirtySixOptions.PartnerAccountCode,
                    //RequestType = "COMMON_REPLACE_CUSTOMER"
                    RequestType = "COMMON_REPLACE_CUSTOMER_URL"
                };

                string token = await GetToken();
                var client = _clientFactory.CreateClient("ThirtySix");
                client.DefaultRequestHeaders.Add("Token", token);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(req, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore

                });
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync("/api/basis/forward/commonRequest", httpContent);

                res.EnsureSuccessStatusCode();
                var result = await res.Content.ReadFromJsonAsync<ThirtySixResponse<object>>();
                ThirtySixCusResponse cusResponse = new ThirtySixCusResponse()
                {
                    Code = result.Code,
                    ConfirmUrl = string.Empty,
                    ErrorMsg = result.Msg,
                    Extra = result.Extra,
                    IsSuccess = result.IsSuccess,
                    Msg = result.Msg,
                    Path = result.Path,
                    Timestamp = result.Timestamp,
                };
                if (result.Code == 0 || result.Code == 200)
                {
                    cusResponse.ConfirmUrl = result.Data.ToString();
                    cusResponse.Records = new List<ThirtySixResponseRecord>();
                }
                else
                {
                    cusResponse.ConfirmUrl = string.Empty;
                    cusResponse.Records = ToResponseRecords(result.Data);
                }
                return cusResponse;
            }
            catch (Exception ex)
            {

                ThirtySixCusResponse cusResponse = new ThirtySixCusResponse()
                {
                    Code = -1,
                    ConfirmUrl = string.Empty,
                    ErrorMsg = ex.Message,
                    Extra = null,
                    IsSuccess = false,
                    Msg = $"推送接口请求失败:{ex.Message}",
                    Path = string.Empty,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    Records = null
                };
                return cusResponse;
            }
        }


        public async Task<ThirtySixResponse<List<ThirtySixResponseRecord>>> QueryDetail()
        {
            string token = await GetToken();
            var client = _clientFactory.CreateClient("ThirtySix");
            client.DefaultRequestHeaders.Add("Token", token);


            ThirtySixeQueryRequest req = new ThirtySixeQueryRequest() {
                FlowId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                Id = _thirtySixOptions.Id,
                UserId = _thirtySixOptions.UserId,
                OperatorDate = DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss"),
                PartnerAccountCode = _thirtySixOptions.PartnerAccountCode,
                RequestType = "COMMON_POLICY_URL",
            };


            var json = Newtonsoft.Json.JsonConvert.SerializeObject(req, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore

            });
            StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync("/api/basis/forward/commonRequest", httpContent);
            var jsonss = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<ThirtySixResponse<List<ThirtySixResponseRecord>>>();
                return result;
            }
            else
            {
                throw new ErrorCodeException(-1, "推送失败");
            }
        }

        private static List<ThirtySixResponseRecord> ToResponseRecords(object data)
        {
            if (data is not JsonElement element)
                return new List<ThirtySixResponseRecord>();

            return element.Deserialize<List<ThirtySixResponseRecord>>(RecordJsonOptions)
                   ?? new List<ThirtySixResponseRecord>();
        }
    }
}
