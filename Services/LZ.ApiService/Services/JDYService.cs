using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Exceptions;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LZ.ApiService.Services
{
    public class JDYService(IHttpService httpService,IHttpClientFactory _httpClientFactory) : IJDYService, IScopedDependency
    {
        private string path = "https://api.jiandaoyun.com/api";
        public async Task<string> QueryMultipleFormData(Dictionary<string, object> data, CompanyConfig companyConfig)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {companyConfig.JDYApiKey}");
            var body = JsonConvert.SerializeObject(data);
            return await httpService.PostJsonAsync(path + "/v5/app/entry/data/list", body, headers);
        }
        public async Task<string> ChangeFormData(Dictionary<string, object> data, CompanyConfig companyConfig)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {companyConfig.JDYApiKey}");
            var body = JsonConvert.SerializeObject(data);
            return await httpService.PostJsonAsync(path + "/v5/app/entry/data/update", body, headers);
        }
        public async Task<string> CreateFormData(Dictionary<string, object> data, CompanyConfig companyConfig)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {companyConfig.JDYApiKey}");
            var body = JsonConvert.SerializeObject(data);
            return await httpService.PostJsonAsync(path + "/v5/app/entry/data/create", body, headers);
        }
        public async Task<string> GetUsers(Dictionary<string, object> data, CompanyConfig companyConfig)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {companyConfig.JDYApiKey}");
            var body = JsonConvert.SerializeObject(data);
            return await httpService.PostJsonAsync(path + "/v5/corp/department/user/list", body, headers);
        }
        public async Task<string> GetExternalUsers(CompanyConfig companyConfig)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {companyConfig.JDYApiKey}");
            var data = new Dictionary<string, string>();
            var body = JsonConvert.SerializeObject(data);
            return await httpService.PostJsonAsync(path + "/v5/corp/guest/user/list", body, headers);
        }
        public async Task<string> GetUploadToken(Dictionary<string, object> data, CompanyConfig companyConfig)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {companyConfig.JDYApiKey}");
            var body = JsonConvert.SerializeObject(data);
            Common.StaticLog.Debug(body);
            return await httpService.PostJsonAsync(path + "/v5/app/entry/file/get_upload_token", body, headers);
        }
        public async Task<string> UploadFileByTokenAsync(
                 string uploadUrl,
                 string token,
                 Stream fileStream,
                 string fileName,
                 string contentType)
        {
            if (string.IsNullOrWhiteSpace(uploadUrl))
                throw new ArgumentException("上传地址不能为空", nameof(uploadUrl));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("上传凭证不能为空", nameof(token));
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            using var content = new MultipartFormDataContent();
            // token 必须在 file 之前；file 作为最后一个参数
            content.Add(new StringContent(token), "token");

            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
            content.Add(streamContent, "file", fileName);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(uploadUrl, content);
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ErrorCodeException(-1, $"上传失败 HTTP {(int)response.StatusCode}: {responseStr}");
            }

            return responseStr;
        }
    }
}
