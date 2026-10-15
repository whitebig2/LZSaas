using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;

namespace LZ.Basic.API.Controllers
{
    public class AliYunAppController(IAliyunCommonService aliyunCommonService, IAliyunOSSService aliyunOSSService, IAliyunImageIdentifyService aliyunImageIdentifyService,ICompanyConfigService companyConfigService,IAliSendInfo aliSendInfo,ISettingsService settingsService, IRedisService redisService) : BasicControllerBase
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
            var gid=Guid.NewGuid().ToString();
            await redisService.SetAsync(string.Format(CoreRedisConstants.SmsVerify, gid), code, TimeSpan.FromMinutes(15));
            var settings=await settingsService.GetMasterSettings();
            var res = aliSendInfo.SendAliSms(phone, settings.AliSmsVerifyTempCode, code.ToString(), settings);
            return Ok(new { code=gid});
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
        /// 营业执照识别图片
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [AllowAnonymous]
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> BusinessLicenseImage()
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadImage(files[0]);
                //Common.StaticLog.Debug(JsonSerializer.Serialize(res));
                if (res.Code == 0)
                {
                    var model = await aliyunImageIdentifyService.BusinessLicense(res.Data.ToString());
                    return Ok(model);

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
        /// 发票识别图片
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [AllowAnonymous]
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> InvoiceImage()
        {
            var files = Request.Form.Files;
            if (files.Count > 0)
            {
                var res = await aliyunOSSService.UploadImage(files[0]);
                //Common.StaticLog.Debug(JsonSerializer.Serialize(res));
                if (res.Code == 0)
                {
                    var model = await aliyunImageIdentifyService.Invoice(res.Data.ToString());
                    return Ok(new { data = model });

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

        
        [AllowAnonymous]
        [HttpGet]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Test()
        {
            var json = await aliyunImageIdentifyService.Invoice("pictures/2026/08/31/liveb_4c3c8554cbca4d45bb6c3ff317d2fb7a.jpg");
            return Ok();
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

        /// <summary>
        /// 逆地址解析
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Geocoder([FromForm]string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                throw new ErrorCodeException(-1, "坐标错误");
            }
            var res = await aliyunCommonService.Geocoder(location);
           return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> ExpressList(string expressNo, string expressType)
        {
            if (string.IsNullOrEmpty(expressNo) && string.IsNullOrEmpty(expressType))
            {
                throw new ErrorCodeException(-1, "快递信息错误");
            }
            var res = await aliyunCommonService.ExpressList(expressNo, expressType);
            return Ok(res);
        }
    }
}
