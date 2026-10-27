using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Dtos.OpenAccountDtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.ICBC.Plkh;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.Util;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace LZ.Bank.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class BankOpenAccountController : ApiControllerBase
    {

        private readonly IFxqService _fxqService;
        private readonly IEntryService _entryService;
        private readonly IWebHostEnvironment _env;
        private readonly IAliyunOSSService _ossService;
        private readonly ISettingsService _settingsService;
 
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBankOpenAccountService _service;
        private readonly ILogger<BankOpenAccountController> _logger;

        public BankOpenAccountController(IBankOpenAccountService service, ILogger<BankOpenAccountController> logger, IHttpClientFactory clientFactory, ISettingsService settingsService, IAliyunOSSService ossService, IWebHostEnvironment env, IEntryService entryService, IFxqService fxqService)
        {
            _service = service;
            _logger = logger;
            _clientFactory = clientFactory;
            _settingsService = settingsService;
            _ossService = ossService;
            _env = env;
            _entryService = entryService;
            _fxqService = fxqService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string companyName, string name,string cardID, OpenAccountStatusEnumType? status,[FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_BankOpenAccount>();
            exp.AndIF(!string.IsNullOrWhiteSpace(companyName), f => f.CompanyName.Contains(companyName.Trim()));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(cardID), f => f.CardID.Contains(cardID));
            exp.AndIF(status.HasValue, f => f.Status== status.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
     
        [HttpPost]
        public async Task<IActionResult> TestRecord()
        {

           var model=await _service.Get(f => f.ID == Guid.Parse("5DCF178D-1BF9-4E90-854F-8FB6CDAC577E"));
         
            var config = await _settingsService.GetMasterSettings();

            config.AliBucketDomain = config.AliBucketDomain.TrimEnd('/');
            string fontPath = Path.Combine(_env.WebRootPath, "Fonts", "msyh.ttf");
            var client = _clientFactory.CreateClient();
            string frontPath = string.Empty;
            string backPath = string.Empty;


            if (model.CardFrontPath.StartsWith("http"))
            {
                frontPath = model.CardFrontPath;
            }
            else
            {
                frontPath = $"{config.AliBucketDomain.TrimEnd('/')}/{model.CardFrontPath.TrimStart('/')}";
            }

            if (model.CardBackPath.StartsWith("http"))
            {
                backPath = model.CardBackPath;
            }
            else
            {
                backPath = $"{config.AliBucketDomain.TrimEnd('/')}/{model.CardBackPath.TrimStart('/')}";
            }

            if (!string.IsNullOrWhiteSpace(frontPath))
            {
                var frontResp = client.GetAsync(frontPath, HttpCompletionOption.ResponseHeadersRead).Result;
                if (!frontResp.IsSuccessStatusCode)
                {
                    throw new FileNotFoundException($"证件照正面图片下载失败： {frontPath}。 Status: {frontResp.StatusCode}");
                }
                var extName = System.IO.Path.GetExtension(frontPath);
                var stream = await frontResp.Content.ReadAsStreamAsync();
                using var outputStream = new MemoryStream();
                ImageSharp.AddTextWatermark(
                 stream,
                 outputStream,
                 "本人授权单位使用本人信息代申领工商银行借记卡",
                 fontPath,
                 fontSize: 30,
                 colorHex: "#80FFFFFF");

                var frontObjectKey = $"ICBC/Plkh/{model.CardID}/front{extName}";

                bool frontExistKey = await _ossService.Exist(frontObjectKey);
                if (!frontExistKey)
                {
                    var frontOssRes = await _ossService.UploadOssFile(outputStream, frontObjectKey);
                    if (frontOssRes.Code == 0)
                    {
                        model.CardFrontPath = frontOssRes.Data.ToString();
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(backPath))
            {
                var backResp = client.GetAsync(backPath, HttpCompletionOption.ResponseHeadersRead).Result;
                if (!backResp.IsSuccessStatusCode)
                {
                    throw new FileNotFoundException($"证件照正面图片下载失败： {backPath}。 Status: {backResp.StatusCode}");
                }
                var extName = System.IO.Path.GetExtension(backPath);
                var stream = await backResp.Content.ReadAsStreamAsync();
                using var outputStream = new MemoryStream();
                ImageSharp.AddTextWatermark(
                 stream,
                 outputStream,
                 "本人授权单位使用本人信息代申领工商银行借记卡",
                 fontPath,
                 fontSize: 30,
                 colorHex: "#80FFFFFF");

                var backObjectKey = $"ICBC/Plkh/{model.CardID}/back{extName}";

                bool frontExistKey = await _ossService.Exist(backObjectKey);
                if (!frontExistKey)
                {
                    var frontOssRes = await _ossService.UploadOssFile(outputStream, backObjectKey);
                    if (frontOssRes.Code == 0)
                    {
                        model.CardBackPath = frontOssRes.Data.ToString();
                    }
                }
            }
            await _service.Update(model);
            return Ok();
        }
        [HttpGet("download-zip")]
        public async Task<IActionResult> DownloadZip()
        {

            var model = await _service.Get(f => f.ID == Guid.Parse("5DCF178D-1BF9-4E90-854F-8FB6CDAC577E"));
            var memoryStream = new MemoryStream();

            // 创建 ZipArchive，模式为 Create，leaveOpen: true 确保流在 using 结束后不被关闭
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var fileEntry1 = archive.CreateEntry($"{model.CardID}/front.jpg", CompressionLevel.Optimal);
                using (var entryStream = fileEntry1.Open())
                {
                    await _ossService.GetObject(model.CardFrontPath, entryStream);
                }
                var fileEntry2 = archive.CreateEntry($"{model.CardID}/back.jpg", CompressionLevel.Optimal);
                using (var entryStream = fileEntry2.Open())
                {
                    await _ossService.GetObject(model.CardBackPath, entryStream);
                }
            }

            // 重置流位置到开头
            memoryStream.Seek(0, SeekOrigin.Begin);

            // 返回文件结果
            return File(memoryStream, "application/zip", "archive.zip");
        }
        /// <summary>
        /// 获取开户基础信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpGet]
        public async Task<IActionResult> GetByUserID(int userID)
        {
            //Live_V_UserEntryInfo
            var model = await _entryService.GetUserEntryInfo(f => f.UserID == userID && f.Status == 2);
            if (model == null)
            {
                model = await _entryService.GetUserEntryInfo(f => f.UserID == userID && f.Status == 4);
            }

            if (model == null)
            {
                throw new ErrorCodeException(-1, "未获取到已入职记录！");
            }



            PreOpenAccountRecordDto preOpenAccountRecordDto = null;
            var openCardModel = await _service.Get(f => f.CardID == model.CardID);

          
            if (openCardModel != null)
            {
                preOpenAccountRecordDto = new PreOpenAccountRecordDto()
                {
                    ID = openCardModel.ID,
                    Address = openCardModel.Address,
                    UserID = openCardModel.UserID,
                    BirthDate = openCardModel.BirthDate.Value,
                    CardBackPath = openCardModel.CardBackPath,
                    CardFrontPath = openCardModel.CardFrontPath,
                    CardID = openCardModel.CardID,
                    CardEndDate = openCardModel.CardEndDate,
                    CardStartDate = openCardModel.CardStartDate,
                    CustomerName = openCardModel.CustomerName,
                    Name = openCardModel.Name,
                    Phone = openCardModel.Phone,
                    Sex = openCardModel.Sex,
                    UserEntryID = openCardModel.UserEntryID,
                };
            }
            else {

                var cardInfo = IdCardHelper.GetAgeAndGenderAndBirthDate(model.CardID);
                preOpenAccountRecordDto = new PreOpenAccountRecordDto()
                {
                    ID = null,
                    Address = model.HomeAddress,
                    UserID = model.UserID.Value,
                    BirthDate = cardInfo.birthDate,
                    CardBackPath = model.IdcardBackImage,
                    CardFrontPath = model.IdcardFrontImage,
                    CardID = model.CardID,
                    CardEndDate = null,
                    CardStartDate = null,
                    CustomerName = model.CustomerName,
                    Name = model.Name,
                    Phone = model.Phone,
                    Sex = cardInfo.Gender,
                    UserEntryID= model.ID

                };

                if (!string.IsNullOrWhiteSpace(model.CardStartDate))
                {
                    if (DateTime.TryParseExact(model.CardStartDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime outDate))
                    {
                        preOpenAccountRecordDto.CardStartDate = outDate;
                    }
                }
                if (!string.IsNullOrWhiteSpace(model.CardEndDate))
                {
                    if (DateTime.TryParseExact(model.CardEndDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime outDate))
                    {
                        preOpenAccountRecordDto.CardEndDate = outDate;
                    }
                }
            }
            return Ok(preOpenAccountRecordDto);
        }

        /// <summary>
        /// 申请开卡
        /// 添加或更新记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        public async Task<IActionResult> ApplyRecord(PreOpenAccountRecordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CardFrontPath))
            {
                throw new ErrorCodeException(-1, $"证件照正面图片路径参数缺失！");
            }
            if (string.IsNullOrWhiteSpace(dto.CardBackPath))
            {
                throw new ErrorCodeException(-1, $"证件照反面图片路径参数缺失！");
            }

            if (string.IsNullOrWhiteSpace(dto.Phone))
            {
                throw new ErrorCodeException(-1, $"请填写手机号码！");
            }

            if (dto.CardStartDate==null||dto.CardStartDate.Value.Year<1900)
            {
                throw new ErrorCodeException(-1, $"证件开始日期错误！");
            }

            if (dto.CardEndDate == null || dto.CardEndDate.Value.Year < 1900)
            {
                throw new ErrorCodeException(-1, $"证件截止日期错误！");
            }

            var userInfo=await _entryService.GetUserEntryInfo(f => f.ID == dto.UserEntryID);
            if (userInfo==null)
            {
                throw new ErrorCodeException(-1, $"入职记录信息获取失败！");
            }
            


            if (dto.ID != null && dto.ID != Guid.Empty)
            {
                var entity = await _service.Get(f => f.ID == dto.ID);
                if (entity == null)
                {
                    throw new ErrorCodeException(-1, $"开户信息不存在，请重新提交！");
                }

                if (entity.Status== OpenAccountStatusEnumType.待开户)
                {
                    throw new ErrorCodeException(-1, $"待开户状态无法修改！");
                }
                //entity.Name = dto.Name;
                //entity.CardID = dto.CardID;
                entity.CardStartDate = dto.CardStartDate;
                entity.CardEndDate = dto.CardEndDate;
                entity.Phone = dto.Phone;
                //entity.BirthDate = dto.BirthDate;
                entity.Address = dto.Address ?? string.Empty;
                entity.CardFrontPath = dto.CardFrontPath;
                entity.CardBackPath = dto.CardBackPath;

                await _service.Update(entity);
            }
            else {
                var exist = await _service.Get(f => f.CardID == dto.CardID);
                if (exist!=null)
                {
                    throw new ErrorCodeException(-1, $"开户记录已存在，无法重复申请！");
                }
                Live_BankOpenAccount model = new Live_BankOpenAccount()
                {
                    ID = Guid.NewGuid(),
                    CardID = dto.CardID,
                    Address = dto.Address,
                    BankType = 0,
                    BatchNo = 0,
                    CardBackPath = dto.CardBackPath,
                    CardFrontPath = dto.CardFrontPath,
                    CardEndDate = dto.CardEndDate,
                    CardStartDate = dto.CardStartDate,
                    CompanyName = string.Empty,
                    Name = dto.Name,
                    BirthDate = dto.BirthDate,
                    Sex = dto.Sex,
                    Phone = dto.Phone,
                    CardType = "身份证",
                    CompanyAddress = userInfo.CompanyAddress,
                    CompanyID = 0,
                    CreateTime = DateTime.Now,
                    CustomerName = dto.CustomerName,
                    CustomerNo = userInfo.CompanyNo,
                    UserEntryID = dto.UserEntryID,
                    FailMsg = "",
                    OpenAreaNo = string.Empty,
                    OpenWebNo = string.Empty,
                    PdfPath = string.Empty,
                    Remark = string.Empty,
                    SendDate = DateTime.MinValue,
                    SignedPath = string.Empty,
                    SignedTime = DateTime.MinValue,
                    SmsCode = string.Empty,
                    SmsContent = string.Empty,
                    SmsSendTime = DateTime.MinValue,
                    Status = 0,
                    UserID = dto.UserID,
                    SignStatus = Model.Enums.CommonSignStatusEnumType.无,
                };
                await _service.Add(model);
            }
           
            return Ok();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            await _service.Delete(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Signing([FromForm] Guid id, [FromForm] string name,[FromForm] string smsCode)
        {

            var setting = await _settingsService.GetMasterSettings();
            var record = await _service.Get(f => f.ID == id);
            if (record == null)
            {
                throw new ErrorCodeException(-1, "合同文件不存在");
            }
            if (record.SignStatus != CommonSignStatusEnumType.待签署)
            {
                throw new ErrorCodeException(-1, "此状态无法签署！");
            }


            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ErrorCodeException(-1, "请输入姓名");
            }

            if (record.Name != name.Trim())
            {
                throw new ErrorCodeException(-1, "姓名不匹配");
            }
            if (string.IsNullOrWhiteSpace(smsCode))
            {
                throw new ErrorCodeException(-1, "验证码不能为空");
            }
            if (record.SmsCode != smsCode.Trim())
            {
                throw new ErrorCodeException(-1, "验证码不匹配");
            }
            record.FailMsg = string.Empty;
            List<KeywordSigner> signers = new List<KeywordSigner>();
            if (!string.IsNullOrWhiteSpace(record.EmpSealPath))
            {
                KeywordSigner keywordSigner = new KeywordSigner(record.Name, record.CardID, record.EmpSealPath, record.EmpSealKey, 120, null);
                signers.Add(keywordSigner);
            }
            var smResp = await _fxqService.KeywordPdfContractSign(setting.AliBucketDomain.TrimEnd('/') + record.PdfPath, signers.ToArray(), 1, null, null, new SignDate(1, null, null, null));
            if (smResp != null)
            {
                if (smResp.Code == 10000)
                {
                    record.SignedPath = smResp.Data;
                    record.SignedTime = DateTime.Now;
                    record.SignStatus = CommonSignStatusEnumType.待存证;
                    record.FailMsg = string.Empty;
                }
                else
                {
                    throw new ErrorCodeException(-1, $"签章失败，异常信息：{smResp?.Msg}");
                }


                if (!string.IsNullOrWhiteSpace(record.SignedPath))
                {
                    var pdfFileBase64 = await _fxqService.DownloadFile(record.SignedPath);
                    if (!string.IsNullOrWhiteSpace(pdfFileBase64))
                    {

                        using (var ms = new MemoryStream(Convert.FromBase64String(pdfFileBase64)))
                        {
                            var ossResp = await _ossService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf", true, false, "CashSignRecord");
                            if (ossResp.Code == 0)
                            {
                                record.SignedPath = ossResp.Data.ToString();
                                record.FailMsg = string.Empty;
                            }
                            else
                            {
                                throw new ErrorCodeException(-1, "合同文件下载失败！");
                            }
                        }
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, "下载已签署文件失败！");
                    }
                }
                // 存证
                var czRes = await _fxqService.EvidenceFull(setting.AliBucketDomain.TrimEnd('/') + record.SignedPath, "2", null);
                if (czRes != null && czRes.Code == 10000)
                {
                    record.FileHash = czRes.Data.Hash;
                    record.SignStatus= CommonSignStatusEnumType.已完成;
                }
                else
                {
                    throw new ErrorCodeException(-1, "合同存证异常！");
                }

                await _service.Update(record);

            }
            else
            {
                throw new ErrorCodeException(-1, "签署异常，请稍后再试！");
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListAgreeFilesByUserID(int userID)
        {
            List<CommonSignStatusEnumType> statusEnumTypes = new List<CommonSignStatusEnumType>() { CommonSignStatusEnumType .待签署, CommonSignStatusEnumType.待存证, CommonSignStatusEnumType.已完成 };  
            var records = await _service.List(f => f.UserID == userID&&!string.IsNullOrWhiteSpace(f.PdfPath)&&statusEnumTypes.Contains(f.SignStatus)&&!string.IsNullOrWhiteSpace(f.SmsCode));
            var dtoList = records.Select(f => new SingleFileSealDto()
            {
                CardID = f.CardID,
                CustomerName = f.CustomerName,
                Description = "工商银行开户同意协议书",
                FilePath = f.PdfPath,
                ID = f.ID,
                Name = f.Name,
                SignedFilePath = f.SignedPath ?? string.Empty,
                Title = "工商银行开户",
                StatusName = string.IsNullOrWhiteSpace(f.SignedPath) ? "待签署" : "已签署"
            }).ToList();
            return Ok(dtoList);
        }

        [HttpGet]

        [HttpPut]
        public async Task<IActionResult> Reset([FromForm]Guid id)
        {
            var model = await _service.Get(f => f.ID == id);
            if (model.Status == OpenAccountStatusEnumType.已开户)
            {
                throw new ErrorCodeException(-1, "【已开户】状态无法重置数据！");
            }
            model.Status = OpenAccountStatusEnumType.待处理;
            model.FailMsg = string.Empty;
            model.WatermarkBackPath = string.Empty;
            model.WatermarkFrontPath = string.Empty;
            model.PdfPath = string.Empty;
            model.SignedPath = string.Empty;
            model.SmsCode = string.Empty;
            model.SmsContent = string.Empty;
            model.FileHash = string.Empty;
            model.SignedTime = null;
            model.SmsSendTime = null;
            await _service.Update(model);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> OpenCardQuery(string name,string cardID,int status)
        {
            var model = await _service.Get(t => t.Name == name && t.CardID == cardID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "人员不存在");
            }
            if (status == (int)OpenAccountProcessStatusEnumType.完成)
            {
                model.Status = OpenAccountStatusEnumType.已开户;

            }
            var i = await _service.Update(model);
            if (!i)
            {
                throw new ErrorCodeException(-1, name+"申请通过更新失败");
            }
            return Ok();
        }

    }



}
