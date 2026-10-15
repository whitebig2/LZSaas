using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Model.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LZ.ApiService.Services
{
    public class AliyunImageIdentifyService(ISettingsService settingsService,IHttpService httpService) : IAliyunImageIdentifyService, IScopedDependency
    {
        private string IdentifyServiceUrl = "https://idcardside.market.alicloudapi.com";
        private string IdentifyServiceUrl_SZ = "https://dfidcard.market.alicloudapi.com";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<Model.ApiModel.IdcradImageResultModel?> IdcardIdentify_SM(string imageUrl)
        {
            var url = IdentifyServiceUrl + "/idcard_ocr";
            var settings = await settingsService.GetMasterSettings();
            imageUrl = settings.AliBucketDomain + imageUrl;
            var paras = new Dictionary<string, string>();
            paras.Add("url", imageUrl);
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
            var res = await httpService.PostFormAsync(url, paras, headers);
            var model = System.Text.Json.JsonSerializer.Deserialize<Model.ApiModel.IdcradImageModel>(res);
            if (model != null)
            {
                if (model.success)
                {
                    return model.data.info;
                }
                else
                {
                    Common.StaticLog.Error(res);
                    throw new ErrorCodeException(-1, "身份证识别失败!");
                }
            }
            return null;
        }
        //深智身份证识别
        public async Task<Model.ApiModel.IdcradImageResultModel?> IdcardIdentify(string imageUrl)
        {
            var url = IdentifyServiceUrl_SZ + "/ocr/idcard";
            var settings = await settingsService.GetMasterSettings();
            imageUrl = settings.AliBucketDomain + imageUrl;
            var paras = new Dictionary<string, string>();
            paras.Add("image_url", imageUrl);
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
            var res = await httpService.PostFormAsync(url, paras, headers);
            var model = System.Text.Json.JsonSerializer.Deserialize<Model.ApiModel.IdcradImageModel_ZS>(res);
            if (model != null)
            {
                if (model.status == "OK")
                {
                    return model.result;
                }
                else
                {
                    Common.StaticLog.Error(res);
                    throw new ErrorCodeException(-1, "身份证识别失败!");
                }
            }
            return null;
        }
        public async Task<Model.ApiModel.BankImageResultModel?> BankIdentify(string imageUrl)
        {
            var url = IdentifyServiceUrl_SZ + "/ocr/bankcard_v2";
            var settings = await settingsService.GetMasterSettings();
            imageUrl = settings.AliBucketDomain + imageUrl;
            var paras = new Dictionary<string, string>();
            paras.Add("image_url", imageUrl);
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
            var res = await httpService.PostFormAsync(url, paras, headers);
            Common.StaticLog.Debug(res);
            var model = System.Text.Json.JsonSerializer.Deserialize<Model.ApiModel.BankImageModel>(res);
            if (model != null)
            {
                if (model.status == "OK")
                {
                    return model.result;
                }
                else
                {
                    Common.StaticLog.Error(res);
                    throw new ErrorCodeException(-1, "银行卡识别失败!");
                }
            }
            return null;
        }

        public async Task<Model.ApiModel.BusinessLicenseModel?> BusinessLicense(string imageUrl)
        {
            var url = IdentifyServiceUrl + "/business_license/ocr";
            var settings = await settingsService.GetMasterSettings();
            imageUrl = settings.AliBucketDomain + imageUrl;
            var paras = new Dictionary<string, string>();
            paras.Add("url", imageUrl);
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
            
            var res = await httpService.PostFormAsync(url, paras, headers);
            var result = JsonConvert.DeserializeObject(res) as JObject;
            if (result["code"].ToString() == "200")
            {
                var model = JsonConvert.DeserializeObject<Model.ApiModel.BusinessLicenseModel>(result["data"].ToString());
                model.image = imageUrl;
                return model;
            }
            else
            {
                return null;
            }

        }


        //public async Task<Model.ApiModel.InvoiceModel?> Invoice(string imageUrl)
        //{
        //    var url = "https://juminvoie.market.alicloudapi.com/ocr/invoice-v2";
        //    var settings = await settingsService.GetMasterSettings();
        //    imageUrl = settings.AliBucketDomain + imageUrl;
        //    var paras = new Dictionary<string, string>();
        //    paras.Add("url", imageUrl);
        //    var headers = new Dictionary<string, string>();
        //    headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
        //    var res = await httpService.PostFormAsync(url, paras, headers);
        //    var result = JsonConvert.DeserializeObject(res) as JObject;
        //    if (result["code"].ToString() == "200")
        //    {
        //        var model = JsonConvert.DeserializeObject<Model.ApiModel.InvoiceModel>(result["data"].ToString());
        //        model.image = imageUrl;
        //        return model;
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}

        public async Task<Model.ApiModel.InvoiceModel?> Invoice(string imageUrl)
        {
            var url = "https://juminvoie.market.alicloudapi.com/ocr/invoice-v2";
            var settings = await settingsService.GetMasterSettings();
            imageUrl = settings.AliBucketDomain + imageUrl;
            var paras = new Dictionary<string, string>();
            paras.Add("url", imageUrl);
            // string json = JsonConvert.SerializeObject(paras);
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
            var res = await httpService.PostFormAsync(url, paras, headers);
            var result = JsonConvert.DeserializeObject(res) as JObject;
            if (result["code"].ToString() == "200")
            {
                var model = JsonConvert.DeserializeObject<Model.ApiModel.InvoiceModel>(result["data"].ToString());
                model.image = imageUrl;
                return model;
            }
            else
            {
                return null;
            }

        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public async Task<string> InvoiceTest(string imageUrl)
        {
            String querys = "";
           
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
           

            var url = "https://juminvoie.market.alicloudapi.com/ocr/invoice-v2";
            var settings = await settingsService.GetMasterSettings();
            imageUrl = settings.AliBucketDomain + imageUrl;
            String bodys = "url=" + imageUrl;
            var paras = new Dictionary<string, string>();
            paras.Add("url", imageUrl);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));

            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            Console.WriteLine(httpResponse.StatusCode);
            Console.WriteLine(httpResponse.Method);
            Console.WriteLine(httpResponse.Headers);
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            Console.WriteLine(reader.ReadToEnd());
            Console.WriteLine("\n");


            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
            var res = await httpService.PostFormAsync(url, paras, headers);
            return imageUrl;

        }

    }
}
