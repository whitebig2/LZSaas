using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LZ.ApiService.Interface;
using LZ.Core.Interface;
using System.Text.Json;
using LZ.Model.Exceptions;
using LZ.Model.ApiModel;
using System.Text.Json.Serialization;
using LZ.Model.Dtos;

namespace LZ.ApiService.Services
{
    public class AliyunCommonService(ISettingsService settingsService,IHttpService httpService) : IAliyunCommonService, IScopedDependency
    {
        private string JuMeiServiceUrl = "https://jumnritv.market.alicloudapi.com";
        private string JuMeiZSServiceUrl = "https://jmzjzzz.market.alicloudapi.com";
        public async Task<BankInfoResultModel?> BankCardInfo(string number)
        {
            var url = JuMeiServiceUrl + "/bankcard/info";
            var settings = await settingsService.GetMasterSettings();
            var paras = new Dictionary<string, string>();
            paras.Add("bankcard", number);
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
            var res = await httpService.PostFormAsync(url, paras, headers);
            var model = JsonSerializer.Deserialize<Model.ApiModel.BankInfoModel>(res);
            if (model != null)
            {
                if (model.code == 200)
                {
                    return model.data;
                }
                else
                {
                    Common.StaticLog.Error(res);
                    throw new ErrorCodeException(-1, model.msg);
                }
            }
            return null;
        }
        public async Task<IdPhotoModelResultModel?> IdPhoto(string path, Model.CompanyConfig config)
        {
            try
            {
                Common.StaticLog.Debug(path);
                var url = JuMeiZSServiceUrl + "/id-phote/create";
                var settings = await settingsService.GetMasterSettings();
                var paras = new Dictionary<string, string>();
                paras.Add("url", settings.AliBucketDomain + path);
                paras.Add("spec", config.UserIdPhontSpecID.ToString());
                paras.Add("bgColor", config.UserIdPhontColor.ToString());
                paras.Add("beautyDegree", config.UserIdPhontBeautyDegree.ToString());
                var headers = new Dictionary<string, string>();
                headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
                var res = await httpService.PostFormAsync(url, paras, headers);
                Common.StaticLog.Debug(res);
                var options = new JsonSerializerOptions
                {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };
                var model = JsonSerializer.Deserialize<Model.ApiModel.IdPhotoModel>(res, options);
                if (model != null)
                {
                    if (model.code == 200)
                    {
                        return model.data;
                    }
                    else
                    {
                        Common.StaticLog.Error(res);
                        throw new ErrorCodeException(-1, model.msg);
                    }
                }
                return null;
            }
            catch(Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "生成电子证件照错误");
                return null;
            }
            
        }
        public async Task<string> Geocoder(string location)
        {
            try
            {
                var url = "https://jmregeocd.market.alicloudapi.com/geocode/regeo/query";
                var settings = await settingsService.GetMasterSettings();
                var paras = new Dictionary<string, string>();
                paras.Add("location", location);
                var headers = new Dictionary<string, string>();
                headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
                var res = await httpService.PostFormAsync(url, paras, headers);
                return res;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "坐标错误");
            }

        }

        public async Task<string> ExpressList(string expressNo, string expressType)
        {
            try
            {
                var url = "https://wuliu.market.alicloudapi.com/kdi";
                var settings = await settingsService.GetMasterSettings();
                var paras = new Dictionary<string, string>();
                paras.Add("no", expressNo);
                paras.Add("type", expressType);
                var headers = new Dictionary<string, string>();
                headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
                var res = await httpService.GetAsync(url, paras, headers);
                return res;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "获取快递错误");
            }

        }

        /// <summary>
        /// 快递物流轨迹订阅（聚美智数/阿里云市场）
        /// </summary>
        public async Task<bool> ExpressSubscribe(string expressNo, string expressCode, string callBackUrl, string mobile = null)
        {
            try
            {
                var url = "https://jumexpress.market.alicloudapi.com/express/logistics/subscribe";
                var settings = await settingsService.GetMasterSettings();
                var paras = new Dictionary<string, string>();
                paras.Add("number", expressNo);
                paras.Add("callBackUrl", callBackUrl);
                if (!string.IsNullOrEmpty(expressCode))
                {
                    paras.Add("expressCode", expressCode);
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    paras.Add("mobile", mobile);
                }
                var headers = new Dictionary<string, string>();
                headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
                var res = await httpService.PostFormAsync(url, paras, headers);
                Common.StaticLog.Debug(res);
                var options = new JsonSerializerOptions
                {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                    PropertyNameCaseInsensitive = true
                };
                var model = JsonSerializer.Deserialize<ExpressSubscribeResultModel>(res, options);
                if (model != null && (model.code == 200 || model.success))
                {
                    return true;
                }
                Common.StaticLog.Error(res);
                return false;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 二要素认证
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cardID"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<CheckbodyResultRecord> IdentityAuthentication2(string name,string cardID)
        {
            try
            {
                var url = "https://eid.shumaidata.com/eid/checkbody";
                var settings = await settingsService.GetMasterSettings();
                var paras = new Dictionary<string, string>();
                paras.Add("name", name);
                paras.Add("idcard", cardID);
                var headers = new Dictionary<string, string>();
                headers.Add("Authorization", "APPCODE " + settings.AliAppCode);
                var res = await httpService.PostFormAsync(url, paras, headers);
                try
                {
                    var reader = new Newtonsoft.Json. JsonTextReader(new System.IO.StringReader(res));
                    if (reader.Read() && reader.TokenType == Newtonsoft.Json.JsonToken.StartObject)
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<CheckbodyResultRecord>(res);
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, res);
                    }
                }
                catch (Exception ex)
                {
                    throw new ErrorCodeException(-1, ex.Message);
                }


            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, "坐标错误");
            }

        }
    }
}
