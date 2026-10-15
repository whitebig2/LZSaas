using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Drawing;
using SqlSugar;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace LZ.Basic.API.Controllers
{
    [AllowAnonymous]
    public class ExternalAliYunAppController(IAliyunCommonService aliyunCommonService, IAliyunOSSService aliyunOSSService, IAliyunImageIdentifyService aliyunImageIdentifyService,ICompanyConfigService companyConfigService, IAliSendInfo aliSendInfo, ISettingsService settingsService, IRedisService redisService) : BasicControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> BankInfo([FromForm]string bank)
        {
            var res = await aliyunCommonService.BankCardInfo(bank);
            return Ok(res);
        }
        [HttpPost]
        public async Task<IActionResult> SendSMS([FromForm] string phone)
        {
            Random random = new Random();
            int code = random.Next(10000, 100000);
            var gid = Guid.NewGuid().ToString();
            await redisService.SetAsync(string.Format(CoreRedisConstants.SmsVerify, gid), code, TimeSpan.FromMinutes(15));
            var settings = await settingsService.GetMasterSettings();
            var res = aliSendInfo.SendAliSms(phone, settings.AliSmsVerifyTempCode, "{\"code\":\"" + code + "\"}", settings);
            return Ok(new { code = gid,number=code });
        }
        /// <summary>
        /// 识别图片
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> IdentifyImage([FromForm] string fileType)
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadImage(files[0], fileType);
                //Common.StaticLog.Debug(JsonSerializer.Serialize(res));
                if (res.Code == 0)
                {
                    if (fileType == "idcard")
                    {
                        var model = await aliyunImageIdentifyService.IdcardIdentify(res.Data.ToString());
                        model.image = res.Data.ToString();
                        return Ok(model);
                    }
                    else
                    {
                        var model = await aliyunImageIdentifyService.BankIdentify(res.Data.ToString());
                        model.image = res.Data.ToString();
                        return Ok(model);
                    }

                }
                else
                {
                    throw new ErrorCodeException(-1, res.Message);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "请选择文件");
            }
        }
        /// <summary>
        /// 识别图片
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> IdPhotoImage()
        {
            var files = Request.Form.Files;
            var guid = HttpContext.Request.Headers["company-guid"].ToString();
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadImage(files[0], "idphoto");
                Common.StaticLog.Debug(JsonSerializer.Serialize(res));
                if (res.Code == 0)
                {
                    Model.CompanyConfig companyConfig = null;
                    if (!string.IsNullOrEmpty(guid))
                    {
                       var companyModel =  await companyConfigService.Get(new Guid(guid));
                        companyConfig = companyModel.Config;
                    }
                    else
                    {
                        companyConfig = new Model.CompanyConfig();
                    }
                    var model = await aliyunCommonService.IdPhoto(res.Data.ToString(), companyConfig);
                    if (model != null)
                    {
                        var imageBytes = Convert.FromBase64String(model.base64);
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                           var ossres = await aliyunOSSService.UploadFile(ms, "1.jpg", true, false, "idphoto");
                            return Ok(new { url = ossres.Data });
                        }
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, "生成电子证件照失败");
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, res.Message);
                }
            }
            else
            {
                throw new ErrorCodeException(-1, "请选择文件");
            }
        }
    }
}
