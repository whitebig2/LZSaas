using LZ.Model.Enums;
using LZ.Model;

namespace LZ.Core.Interface
{
    public interface IHttpService
    {

        Task<string> GetAsync(string url, Dictionary<string, string> data = null, Dictionary<string, string> headers = null);

        string GetQueryString(Dictionary<string, string> formData);

        string GetServicePath(string name);
        Task<RequestResultModel> DeleteAsync(string serviceName, string path, Dictionary<string, string> data);
        Task<RequestResultModel> GetAsync(string serviceName, string path, Dictionary<string, string> data = null);
        Task<T> GetAsync<T>(string serviceName, string path, Dictionary<string, string> data = null,bool isItem=false);
        Task<string> PostJsonAsync(string url, string data, Dictionary<string, string> headers = null);
        Task<RequestResultModel> PostAsync(string serviceName, string path, string data);
        Task<RequestResultModel> PostAsync(string serviceName, string path, Dictionary<string, string> data);
        Task<RequestResultObject<T>> PostAsync<T>(string serviceName, string path, string data);
        Task<RequestResultObject<T>> PostAsync<T>(string serviceName, string path, Dictionary<string, string> data);
        Task<string> PostFormAsync(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null);
        Task<string> PutJsonAsync(string url, string data, Dictionary<string, string> headers = null);
        Task<string> PutFormAsync(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null);
        Task<RequestResultModel> PutAsync(string serviceName, string path, string data);
        Task<RequestResultModel> PutAsync(string serviceName, string path, Dictionary<string, string> data);
        Task<RequestResultModel> PostModelAsync(string serviceName, string path, string data);
    }
}
