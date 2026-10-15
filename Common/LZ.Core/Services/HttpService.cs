
using LZ.Model;
using LZ.Core.Interface;
using LZ.Common;
using System.Text;
using Consul;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LZ.Core.Services
{
    public class HttpService(IHttpClientFactory httpClientFactory, IConfiguration configuration,ISettingsService settingsService) : IHttpService,IScopedDependency
    {
        public async Task<string> GetAsync(string url, Dictionary<string, string> data = null, Dictionary<string, string> headers = null)
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);//验证请求头赋值
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                if (data != null)
                {
                    url += "?" + GetQueryString(data);
                }
                HttpResponseMessage response = await httpClient.GetAsync(url);
                //response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return "error";
            }
        }
        public string GetQueryString(Dictionary<string, string> formData)
        {
            string str;
            if ((formData == null ? false : formData.Count != 0))
            {
                StringBuilder stringBuilder = new StringBuilder();
                int num = 0;
                foreach (KeyValuePair<string, string> formDatum in formData)
                {
                    num++;
                    stringBuilder.AppendFormat("{0}={1}", formDatum.Key, formDatum.Value);
                    if (num < formData.Count)
                    {
                        stringBuilder.Append("&");
                    }
                }
                str = stringBuilder.ToString();
            }
            else
            {
                str = "";
            }
            return str;
        }
        public string GetServicePath(string name)
        {
            var consulUri = "http://" + configuration["Consul:ConsulIP"]+":"+ configuration["Consul:ConsulPort"];
            ConsulClient client = new ConsulClient(x =>
            {
                x.Address = new Uri(consulUri);
            });
            var consulServiceList = client.Agent.Services().Result.Response;
            var serviceList = consulServiceList.Where(it => it.Value.Service == name).ToArray();
            var service = serviceList[new Random().Next(serviceList.Length)].Value;
            return "http://" + service.Address + ":" + service.Port;
        }
        public async Task<RequestResultModel> GetAsync(string serviceName,string path, Dictionary<string, string> data = null)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");
                var url = GetServicePath(serviceName) + path;
                var res = await GetAsync(url, data, headers);
                //StaticLog.Debug(url+";res:"+res);
                if (res != "error") {
                    var obj = JsonConvert.DeserializeObject<RequestResultModel>(res);
                    if (obj == null)
                    {
                        return new RequestResultModel(500, "转换发生错误", res);
                    }
                    else
                    {
                        return obj;
                    }
                }
                else
                {
                    return new RequestResultModel(500,"发生错误", null);
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(500, ex.Message, null);
            }
        }

        public async Task<T> GetAsync<T>(string serviceName, string path, Dictionary<string, string> data = null,bool isItem=false)
        {
            var res = await GetAsync(serviceName, path, data);
            if (res.Code == 200 && res.Data != null)
            {
                if (isItem)
                {
                    var items = JsonConvert.DeserializeObject(res.Data.ToString()) as JObject;
                    return JsonConvert.DeserializeObject<T>(items["items"].ToString());
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(res.Data.ToString());
                }
            }
            else
            {
                StaticLog.Error(JsonConvert.SerializeObject(res));
            }
            return default(T);
        }
        public async Task<string> PostJsonAsync(string url, string data, Dictionary<string, string> headers = null)
        {
            try
            {
                StaticLog.Debug(url);
                var httpClient = httpClientFactory.CreateClient();
                HttpContent httpContent = new StringContent(data, Encoding.UTF8, "application/json");
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
                //response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return "error";
            }
        }
        public async Task<RequestResultModel> PostAsync(string serviceName, string path, string data)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");
                var url = GetServicePath(serviceName) + path;
                var res = await PostJsonAsync(url, data, headers);
                StaticLog.Debug(res);
                if (res != "error")
                {
                    return JsonConvert.DeserializeObject<RequestResultModel>(res);
                }
                else
                {
                    return new RequestResultModel(500, "发生错误", null);
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(500, ex.Message, null);
            }
        }
        public async Task<RequestResultObject<T>> PostAsync<T>(string serviceName, string path, string data)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");
                var url = GetServicePath(serviceName) + path;
                var res = await PostJsonAsync(url, data, headers);
                
                if (res != "error")
                {
                    return JsonConvert.DeserializeObject<RequestResultObject<T>>(res);
                }
                else
                {
                    return new RequestResultObject<T>(500, "发生错误");
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultObject<T>(500, ex.Message);
            }
        }
        public async Task<RequestResultModel> PostAsync(string serviceName, string path, Dictionary<string, string> data)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");
                var url = GetServicePath(serviceName) + path;
                var res = await PostFormAsync(url, data, headers);
                if (res != "error")
                {
                    return JsonConvert.DeserializeObject<RequestResultModel>(res);
                }
                else
                {
                    return new RequestResultModel(500, "发生错误", null);
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(500, ex.Message, null);
            }
        }

        public async Task<RequestResultModel> DeleteAsync(string serviceName, string path, Dictionary<string, string> data)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");
                var url = GetServicePath(serviceName) + path;
                var res = await DeleteFormAsync(url, data, headers);
                if (res != "error")
                {
                    return JsonConvert.DeserializeObject<RequestResultModel>(res);
                }
                else
                {
                    return new RequestResultModel(500, "发生错误", null);
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(500, ex.Message, null);
            }
        }
        public async Task<string> DeleteFormAsync(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null)
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                HttpContent httpContent = new FormUrlEncodedContent(data);
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                if (data != null)
                {
                    url += "?" + GetQueryString(data);
                }
                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                //response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return ex.Message;
            }
        }
        public async Task<RequestResultObject<T>> PostAsync<T>(string serviceName, string path, Dictionary<string, string> data)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");
                var url = GetServicePath(serviceName) + path;
                var res = await PostFormAsync(url, data, headers);
                StaticLog.Debug(res);
                if (res != "error")
                {
                    return JsonConvert.DeserializeObject<RequestResultObject<T>>(res);
                }
                else
                {
                    return new RequestResultObject<T>(500, "发生错误");
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultObject<T>(500, ex.Message);
            }
        }
        public async Task<string> PostFormAsync(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null)
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                //Common.StaticLog.Debug(JsonConvert.SerializeObject(data));
                HttpContent httpContent = new FormUrlEncodedContent(data);
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                 HttpResponseMessage response = await httpClient.PostAsync(url, httpContent);
                //response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return ex.Message;
            }
        }
        public async Task<string> PutJsonAsync(string url, string data, Dictionary<string, string> headers = null)
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                HttpContent httpContent = new StringContent(data, Encoding.UTF8, "application/json");
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);//验证请求头赋值
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
                //response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return "error";
            }
        }
        public async Task<string> PutFormAsync(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null)
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient();
                HttpContent httpContent = new FormUrlEncodedContent(data);
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);//验证请求头赋值
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                HttpResponseMessage response = await httpClient.PutAsync(url, httpContent);
                //response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return ex.Message;
            }
        }
        public async Task<RequestResultModel> PutAsync(string serviceName, string path, string data)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");
                var url = GetServicePath(serviceName) + path;
                var res = await PutJsonAsync(url, data, headers);
                if (res != "error")
                {
                    return JsonConvert.DeserializeObject<RequestResultModel>(res);
                }
                else
                {
                    return new RequestResultModel(500, "发生错误", null);
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(500, ex.Message, null);
            }
        }
        public async Task<RequestResultModel> PutAsync(string serviceName, string path, Dictionary<string, string> data)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");
                var url = GetServicePath(serviceName) + path;
                var res = await PutFormAsync(url, data, headers);
                StaticLog.Debug(res);
                if (res != "error")
                {
                    return JsonConvert.DeserializeObject<RequestResultModel>(res);
                }
                else
                {
                    return new RequestResultModel(500, "发生错误", null);
                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(500, ex.Message, null);
            }
        }

        public async Task<RequestResultModel> PostModelAsync(string serviceName, string path, string data)
        {
            try
            {
                var headers = new Dictionary<string, string>();
                var setting = await settingsService.GetMasterSettings();
                headers.Add("Authorization", $"Basic {setting.ApiToken}");

                var url = GetServicePath(serviceName) + path;
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                        var response = await client.PostAsync(url, content);
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<RequestResultModel>(responseBody);

                }
            }
            catch (Exception ex)
            {
                StaticLog.Error(ex);
                return new RequestResultModel(500, ex.Message, null);
            }

        }

    }
}
