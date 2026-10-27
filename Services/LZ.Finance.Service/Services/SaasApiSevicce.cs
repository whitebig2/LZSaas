//using LZ.Core.Interface;
//using LZ.Finance.Service.Interface;
//using LZ.Model;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json.Linq;
//using Senparc.CO2NET.Helpers.Serializers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace LZ.Finance.Service.Services
//{
//    public class SaasApiSevicce : ISaasApiSevicce,IScopedDependency
//    {

//        private readonly IHttpClientFactory _httpClientFactory;

//        private ILogger<SaasApiSevicce> _logger;

//        public SaasApiSevicce(IHttpClientFactory httpClientFactory, ILogger<SaasApiSevicce> logger)
//        {
//            _httpClientFactory = httpClientFactory;
//            _logger = logger;
//        }

//        public async Task<List<Live_Customer>> GetCustomers(string authorization)
//        {
//            using (var httpClient = _httpClientFactory.CreateClient())
//            {

//                httpClient.DefaultRequestHeaders.Add("Authorization", authorization);
//                // 发送 HTTP 请求
//                var response = await httpClient.GetAsync("http://localhost:8862/api/customer/customer/list");
//                response.EnsureSuccessStatusCode();
//                // 读取响应内容
//                var content = await response.Content.ReadAsStringAsync();
//                Console.WriteLine(content);
//                JObject jobject = JObject.Parse(content);
//                var itemsJson = jobject["data"]["items"].ToString();
//                var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Live_Customer>>(itemsJson);
//                return items;
//            }
//        }

//        public async Task<List<Live_Customer>> GetCompas(string authorization)
//        {
//            using (var httpClient = _httpClientFactory.CreateClient())
//            {

//                httpClient.DefaultRequestHeaders.Add("Authorization", authorization);
//                // 发送 HTTP 请求
//                var response = await httpClient.GetAsync("http://localhost:8862/api/company/company/list");
//                response.EnsureSuccessStatusCode();
//                // 读取响应内容
//                var content = await response.Content.ReadAsStringAsync();
//                Console.WriteLine(content);
//                JObject jobject = JObject.Parse(content);
//                var itemsJson = jobject["data"]["items"].ToString();
//                var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Live_Customer>>(itemsJson);
//                return items;
//            }
//        }
//    }
//}
