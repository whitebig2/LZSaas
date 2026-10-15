using LZ.Model;
using Microsoft.AspNetCore.Http;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System.Linq.Expressions;

namespace LZ.ApiService.Interface
{
    public interface IAliyunOSSService
    {
        Task<Model.RequestResultModel> UploadFile(IFormFile file, string path = "", bool compress = true);

        Task<Model.RequestResultModel> UploadImage(IFormFile file, string path = "pictures", int bigWidth = 1000, int smallWidth = 400,int quality = 100);
        Task<string> UploadFile(string file, string path);
        Task<RequestResultModel> UploadFile(Stream file, string fileName, bool uploadOss, bool uploadLocal,string path="files");
        Task<string> DownloadFile(string url);
        Task<RequestResultModel> UploadOssFile(Stream file, string objectKey);
        Task<bool> Exist(string objectKey);
        Task<bool> GetObject(string objectKey, Stream stream);
        Task<bool> Delete(string objectKey);
    }
}
