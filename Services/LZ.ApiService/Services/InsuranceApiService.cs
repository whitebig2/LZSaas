using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model.InsuranceApi;
using LZ.Model.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Senparc.CO2NET.Extensions;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Services
{
    public class InsuranceApiService: IInsuranceApiService,IScopedDependency
    {

        private readonly InsuranceOptions _insuranceOptions;
        private readonly IRedisService _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;

        public InsuranceApiService(IOptions<InsuranceOptions> insuranceOptions, IRedisService memoryCache, IHttpClientFactory httpClientFactory)
        {

            _insuranceOptions = insuranceOptions.Value;
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
        }

        private async Task<string> GetAccessToken()
        {
            string key = _insuranceOptions.CacheKey;
            string accessToken = string.Empty;
            accessToken = await _memoryCache.GetAsync<string>(key);

            //Common.StaticLog.Debug($"商保配置信息:用户名{_insuranceOptions.UserName}————密码：{_insuranceOptions.Password}");
            //Common.StaticLog.Debug($"商保获取授权码:{accessToken}");
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                string nonce = Guid.NewGuid().ToString("N");
                //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();


                Common.StaticLog.Debug($"商保获取授权码，时间戳:{timestamp}");


                Dictionary<string, string> dss = new Dictionary<string, string>();
                dss.Add("account", _insuranceOptions.UserName);
                dss.Add("password", _insuranceOptions.Password);
                dss.Add("nonce", nonce);
                dss.Add("timestamp", timestamp);

           

                string signatureStr = _insuranceOptions.SecretKey + nonce + timestamp;


                Common.StaticLog.Debug($"商保签名字符串signatureStr:{signatureStr}");
                var signature = ComputeMd5Hash(signatureStr);
                dss.Add("signature", signature);



                string bw = Newtonsoft.Json.JsonConvert.SerializeObject(dss);

                Common.StaticLog.Debug($"商保获取授权码请求保温，Json:{bw}");

                var client = _httpClientFactory.CreateClient("InsuranceApi");
                var stringContent = new StringContent(bw, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync("/empapi/verify", stringContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        Common.StaticLog.Debug($"商保获取授权码请求响应:{responseContent}");

                        var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<EmpapiVerifyResponse>(responseContent);
                        if (resultData != null && resultData.Code == 200)
                        {
                            TimeSpan timeSpan = new TimeSpan(1, 0, 0);
                            await _memoryCache.SetAsync(key, resultData.Data.Token, timeSpan);
                        }
                    }
                    else
                    {


                        Common.StaticLog.Debug($"商保获取授权码请求失败：{response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Common.StaticLog.Debug($"商保请求异常:{ex.Message}",ex);
                    throw ex;
                }

              
            }
            accessToken=await _memoryCache.GetAsync<string>(key);
            
            return accessToken;


        }


        public async Task<OrderDetailResponse> GetOrderDetail(string insureId)
        {
            Common.StaticLog.Debug($"商保获取授权码");
            var accessToken = await GetAccessToken();
            OrderDetailRequest orderDetailRequest = new OrderDetailRequest()
            {
                InsureId = insureId
            };
            string urlPath = "/empapi/order_details";
            string json = await Execute(urlPath, orderDetailRequest);
            OrderDetailResponse resp = null;
            if (json.IsNullOrWhiteSpace())
            {
                return new OrderDetailResponse()
                {
                    Code = 500,
                    Msg = "接口请求异常！"
                };
            }
            else
            {
                resp = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderDetailResponse>(json);
            }
            return resp;
        }

        /// <summary>
        /// 批量增员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BatchAddEmployeeResponse> BatchAddEmployee(BatchAddEmployeeRequest request)
        {
            try
            {
                BatchAddEmployeeResponse resp = null;
                string urlPath = "/empapi/list_add_emp";
                string json = await Execute(urlPath, request);
                if (json.IsNullOrWhiteSpace())
                {
                    return new BatchAddEmployeeResponse()
                    {
                        Code = 500,
                        Msg = "接口请求异常！"
                    };
                }
                else
                {
                    resp = Newtonsoft.Json.JsonConvert.DeserializeObject<BatchAddEmployeeResponse>(json);
                }
                return resp;
            }
            catch (Exception ex)
            {

                return new BatchAddEmployeeResponse()
                {
                    Code = 500,
                    Msg = $"{ex.Message}"
                };
            }
        }


        public async Task<BatchDelEmployeeResponse> BatchDelEmployee(BatchDelEmployeeRequest request)
        {

            try
            {
                BatchDelEmployeeResponse resp = null;
                string urlPath = "/empapi/list_del_emp";
                string json = await Execute(urlPath, request);
                if (json.IsNullOrWhiteSpace())
                {
                    resp= new BatchDelEmployeeResponse()
                    {
                        Code = 500,
                        Msg = "接口请求异常！"
                    };
                }
                else
                {
                    resp = Newtonsoft.Json.JsonConvert.DeserializeObject<BatchDelEmployeeResponse>(json);
                }
                return resp;
            }
            catch (Exception ex)
            {

                return new BatchDelEmployeeResponse()
                {
                    Code = 500,
                    Msg = $"{ex.Message}！"
                };

            }
         
        }


        public async Task<BatchSearchStatusResponse> BatchSearchStatus(BatchSearchStatusRequest request)
        {
            BatchSearchStatusResponse resp = null;
            string urlPath = "/empapi/list_search_status";
            string json = await Execute(urlPath, request);
            if (json.IsNullOrWhiteSpace())
            {
                return new BatchSearchStatusResponse()
                {
                    Code = 500,
                    Msg = "接口请求异常！"
                };
            }
            else
            {
                resp = Newtonsoft.Json.JsonConvert.DeserializeObject<BatchSearchStatusResponse>(json);
            }
            return resp;
        }

        public async Task<string> Execute<T>(string url,T request)
        {
            var accessToken = await GetAccessToken();
            var client = _httpClientFactory.CreateClient("InsuranceApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(request, new Newtonsoft.Json.JsonSerializerSettings()
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }) ;
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url, stringContent);
            if (res.IsSuccessStatusCode)
            {
                var responseStr = await res.Content.ReadAsStringAsync();
               return responseStr;
            }
            else
            {
              return string.Empty;
            }
        }

        private string ComputeMd5Hash(string input)
        {
            // 创建一个MD5哈希算法实例  
            using (MD5 md5Hash = MD5.Create())
            {
                // 将输入字符串转换为字节数组  
                byte[] data = Encoding.UTF8.GetBytes(input);

                // 计算MD5哈希值  
                byte[] hash = md5Hash.ComputeHash(data);

                // 将哈希值转换为十六进制字符串  
                StringBuilder hexString = new StringBuilder();
                foreach (byte b in hash)
                {
                    hexString.Append(b.ToString("x2"));
                }
                return hexString.ToString();
            }
        }

      


        
    }
}
