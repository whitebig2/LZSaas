
using LZ.Model;

namespace LZ.ApiService.Interface
{
    public interface IJDYService
    {
        Task<string> QueryMultipleFormData(Dictionary<string, object> data, CompanyConfig companyConfig);
        Task<string> ChangeFormData(Dictionary<string, object> data, CompanyConfig companyConfig);
        Task<string> CreateFormData(Dictionary<string, object> data, CompanyConfig companyConfig);
        Task<string> GetUsers(Dictionary<string, object> data, CompanyConfig companyConfig);
        Task<string> GetExternalUsers(CompanyConfig companyConfig);
        Task<string> GetUploadToken(Dictionary<string, object> data, CompanyConfig companyConfig);
        Task<string> UploadFileByTokenAsync(
                 string uploadUrl,
                 string token,
                 Stream fileStream,
                 string fileName,
                 string contentType);
    }
}
