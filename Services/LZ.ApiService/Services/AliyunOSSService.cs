using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.AccessControl;
using System.Drawing;
using System.IO.Pipes;
using Microsoft.AspNetCore.Http.HttpResults;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using MathNet.Numerics.Statistics;

namespace LZ.ApiService.Services
{
    public class AliyunOSSService(ISettingsService settingsService, IHttpClientFactory httpClientFactory) : IAliyunOSSService, IScopedDependency
    {




        private string endpoint = "https://oss-cn-shanghai.aliyuncs.com";
        //private string domainName = "https://lz-uplod.oss-cn-shanghai.aliyuncs.com/";
        private string domainName = "/";
        public async Task<Model.RequestResultModel> UploadFile(IFormFile file, string path = "", bool compress = true)
        {
            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var fileExtension = Path.GetExtension(fileName).ToLower();
                string fileContentType = file.ContentType;
                bool hasImage = fileContentType == "image/bmp" || fileContentType == "image/gif" || fileContentType == "image/jpeg" || fileContentType == "image/png";
                var timeName = DateTime.Now.ToString("yyyy/MM/dd");
                if (string.IsNullOrEmpty(path))
                {
                    path = hasImage ? "pictures" : "files";
                }
                if (hasImage && compress)
                {
                    return await UploadImage(file, path);
                }
                else
                {
                    var aliPath = path + "/" + timeName + "/";
                    var filePath = Directory.GetCurrentDirectory() + "/uploadfiles/" + aliPath;
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    var upFileName = Guid.NewGuid().ToString("n") + fileExtension;
                    var localPath = filePath + upFileName;
                    var aliFilePath = aliPath + upFileName;
                    //using (var stream = new FileStream(localPath, FileMode.Create))
                    //{
                    //    await file.CopyToAsync(stream);
                    //}
                    //string domainPath = await UploadFile(localPath, aliFilePath);
                    string domainPath =domainName+ aliFilePath;
                    using (var ms = file.OpenReadStream())
                    {
                        await UploadFile(ms, aliFilePath);
                    }
                    return new Model.RequestResultModel(0, "上传成功", domainPath);
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return new Model.RequestResultModel(-1, ex.Message);
            }
        }
        public async  Task<Model.RequestResultModel> UploadImage(IFormFile file, string path = "pictures", int bigWidth = 1000, int smallWidth = 400,int quality = 100)
        {
            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var fileExtension = Path.GetExtension(fileName).ToLower();
                string fileContentType = file.ContentType;
                bool hasImage = fileContentType == "image/bmp" || fileContentType == "image/gif" || fileContentType == "image/jpeg" || fileContentType == "image/png";
                if (!hasImage)
                {
                    return new Model.RequestResultModel(-1, "该文件不是图片类型");
                }
                var timeName = DateTime.Now.ToString("yyyy/MM/dd");
                var aliPath = path + "/" + timeName + "/";
                var picPath= Directory.GetCurrentDirectory()+"/uploadfiles/"+ aliPath;
                if (!Directory.Exists(picPath))
                {
                    Directory.CreateDirectory(picPath);
                }
                var imageName= Guid.NewGuid().ToString("n") + fileExtension;
                var imagePath = picPath + imageName;
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var bigPath= picPath + "liveb_" + imageName;
                await Common.ImageSharp.ImageResize(imagePath, bigWidth, bigPath, quality);
                var bigAliPath = aliPath + "liveb_" + imageName;
                await UploadFile(bigPath, bigAliPath);
                if (smallWidth > 0)
                {
                    var smallPath = picPath + "lives_" + imageName;
                    var smallAliPath = aliPath + "lives_" + imageName;
                    await Common.ImageSharp.ImageResize(imagePath, smallWidth, smallPath);
                    await UploadFile(smallPath, smallAliPath);
                }
                return new Model.RequestResultModel(0, "上传成功", domainName + bigAliPath);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return new Model.RequestResultModel(-1, ex.Message);
            }
        }


        public async Task<Model.RequestResultModel> UploadFile(Stream file, string fileName,bool uploadOss,bool uploadLocal,string path="files")
        {
            try
            {
                var fileExtension = Path.GetExtension(fileName).ToLower();
                var timeName = DateTime.Now.ToString("yyyy/MM/dd");
                var aliPath = path + "/" + timeName + "/";
                var filePath = Directory.GetCurrentDirectory() + "/uploadfiles/" + aliPath;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var upFileName = Guid.NewGuid().ToString("n") + fileExtension;
                var localPath = filePath + upFileName;
                var aliFilePath = aliPath + upFileName;
                if (uploadLocal)
                {
                    using (var stream = new FileStream(localPath, FileMode.Create))
                    {
                        file.Position = 0;
                        await file.CopyToAsync(stream);
                    }
                }
                if (uploadOss)
                {
                    file.Position = 0;
                    await UploadFile(file, aliFilePath);
                }

       
                return new Model.RequestResultModel(0, "上传成功", domainName + aliFilePath);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return new Model.RequestResultModel(-1, ex.Message);
            }
        }


        /// <summary>
        /// 上传oss
        /// </summary>
        /// <param name="file"></param>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        public async Task<Model.RequestResultModel> UploadOssFile(Stream file, string objectKey)
        {
            file.Position = 0;

            var settings = await settingsService.GetMasterSettings();
            var accessKeyId = settings.AliAccessID;
            var accessKeySecret = settings.AliAccessSecret;
            var bucketName = settings.AliBucketName;
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var result = client.PutObject(bucketName, objectKey, file);
                return new Model.RequestResultModel(0, "上传成功", objectKey);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return new Model.RequestResultModel(-1, ex.Message);
            }
        }


        private async Task UploadFile(Stream file, string path)
        {
            file.Position = 0;
            var settings = await settingsService.GetMasterSettings();
            var accessKeyId = settings.AliAccessID;
            var accessKeySecret = settings.AliAccessSecret;
            var bucketName = settings.AliBucketName;
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var result = client.PutObject(bucketName, path, file);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
        }
        public async Task<string> UploadFile(string file, string path)
        {
            var settings = await settingsService.GetMasterSettings();
            var accessKeyId = settings.AliAccessID;
            var accessKeySecret = settings.AliAccessSecret;
            var bucketName = settings.AliBucketName;
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var result = client.PutObject(bucketName, path, file);
                return domainName + path;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return "error";
            }
        }

        public async Task<string> DownloadFile(string url)
        {
            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                var contentDisposition = response.Content.Headers.ContentDisposition?.ToString();
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileBase64 = Convert.ToBase64String(fileBytes);
                return fileBase64;
            }
            return null;

        }

        public async Task<bool> Exist(string objectKey)
        {
            var settings = await settingsService.GetMasterSettings();
            var accessKeyId = settings.AliAccessID;
            var accessKeySecret = settings.AliAccessSecret;
            var bucketName = settings.AliBucketName;
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var result = client.DoesObjectExist(bucketName, objectKey);
                return result;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return false;
            }
        }

        public async Task<bool> GetObject(string objectKey, Stream stream)
        {
            var settings = await settingsService.GetMasterSettings();
            var accessKeyId = settings.AliAccessID;
            var accessKeySecret = settings.AliAccessSecret;
            var bucketName = settings.AliBucketName;
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var result = client.GetObject(new GetObjectRequest(bucketName, objectKey), stream);
                return true;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return false;
            }
        }

        public async Task<bool> Delete(string objectKey)
        {
            var settings = await settingsService.GetMasterSettings();
            var accessKeyId = settings.AliAccessID;
            var accessKeySecret = settings.AliAccessSecret;
            var bucketName = settings.AliBucketName;
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
             client.DeleteObject(bucketName, objectKey);
                return true;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return false;
            }
        }
    }
}
