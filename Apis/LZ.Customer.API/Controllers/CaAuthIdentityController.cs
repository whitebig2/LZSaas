using Humanizer;
using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql.Replication;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;

namespace LZ.Customer.API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class CaAuthIdentityController : ApiControllerBase
    {
        private readonly ILogger<CaAuthIdentityController> _logger;
        private readonly ICaAuthIdentityService _service;
        private readonly IFxqService _fxqService;
        private readonly  IHttpService _httpService;
        private readonly  IWebHostEnvironment _env;

        public CaAuthIdentityController(IFxqService fxqService, ICaAuthIdentityService service, ILogger<CaAuthIdentityController> logger, IHttpService httpService, IWebHostEnvironment env)
        {
            _fxqService = fxqService;
            _service = service;
            _logger = logger;
            _httpService = httpService;
            _env = env;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetNav()
        {
            string url = $"https://company.cn-labor.cn/web-sign/auth-identity";
            var res = await _fxqService.CaAge18Apply("郑康", "320722199203033956", url,"默认应用");
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string name, string idno, string idCardNo, string signUnitName, CaAuthIdentityStatusEnumType? status, [FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Live_CaAuthIdentity>();
            expable.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            expable.AndIF(!string.IsNullOrWhiteSpace(idno), f => f.CardID.Contains(idno));
            expable.AndIF(status.HasValue, f => f.Status == status.Value);
            var exp = expable.ToExpression();
            var models = await _service.PageList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> TestSeal()
        {
           var res= await _fxqService.CompanyElectronicSeal("宁波莱博网络科技有限公司", "91330212MA2CH79K4J", "123456789123456789", 0, 0, 0, 1, 0);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserID(Guid userGid)
        {
            List<Live_CaAuthIdentity> models = null;
            CaAuthIdentityDto recordDto;
            recordDto = await _httpService.GetAsync<CaAuthIdentityDto>(ServiceNames.UsersService, "/api/user/GeCaAuthInfoByUserID?userGid=" + userGid);
            if (recordDto != null && !string.IsNullOrWhiteSpace(recordDto.CardID))
            {
                models = await _service.List(f => f.CardID == recordDto.CardID);
            }
            else {
                recordDto = new CaAuthIdentityDto()
                {
                    CardID = string.Empty,
                    Name = string.Empty,
                    Phone = string.Empty,
                    StatusName = "未认证",
                    UserGid = userGid,
                    AuthType = "Face"

                };
            }
            if (models == null || models.Count <= 0&& userGid!=Guid.Empty)
            {
                models = await _service.List(f => f.UserGid == userGid);
            }
            if (models != null&& models.Count > 0)
            {

                if (models.Any(f => f.Status == CaAuthIdentityStatusEnumType.认证成功))
                {
                    var model = models.First(f => f.Status == CaAuthIdentityStatusEnumType.认证成功);
                    recordDto = new CaAuthIdentityDto()
                    {
                        AuthType = model.AuthType,
                        CardID = model.CardID,
                        Name = model.Name,
                        Phone = model.Phone,
                        StatusName = model.Status == CaAuthIdentityStatusEnumType.认证成功 ? "已认证" : "未认证",
                        UserGid = userGid,
                        AuthDate = model.AuthTime?.ToString("yyyy-MM-dd")
                    };
                }
                else
                {
                    var model = models.FirstOrDefault();
                    recordDto = new CaAuthIdentityDto()
                    {
                        AuthType = model.AuthType,
                        CardID = model.CardID,
                        Name = model.Name,
                        Phone = model.Phone,
                        StatusName = model.Status == CaAuthIdentityStatusEnumType.认证成功 ? "已认证" : "未认证",
                        UserGid = userGid,
                        AuthDate = model.AuthTime?.ToString("yyyy-MM-dd")
                    };
                }
            }

            return Ok(recordDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetDtoByID(Guid id)
        {
            var model = await _service.Get(f => f.ID == id);
            CaAuthIdentityDto recordDto = null;
            recordDto = new CaAuthIdentityDto()
            {
                AuthType = model.AuthType,
                CardID = model.CardID,
                Name = model.Name,
                Phone = model.Phone,
                StatusName = model.Status == CaAuthIdentityStatusEnumType.认证成功 ? "已认证" : "未认证",
                UserGid = model.UserGid.Value,
                AuthDate = model.AuthTime?.ToString("yyyy-MM-dd"),
            };

            return Ok(recordDto);
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _service.Get(f => f.ID == id);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> CaAgreeApply([FromForm]Guid userGid, [FromForm] string name, [FromForm] string phone, [FromForm] string cardID, [FromForm] string authType, [FromForm] bool isApp)
        {
            try
            {
                var models = await _service.List(f => f.CardID == cardID);

                if (models != null && models.Any(f => f.Status == CaAuthIdentityStatusEnumType.认证成功))
                {
                    throw new ErrorCodeException(-1, "已认证完成，无需重复认证！");
                }



                var recordDto = await _httpService.GetAsync<CaAuthIdentityDto>(ServiceNames.UsersService, "/api/user/GeCaAuthInfoByUserID?userGid=" + userGid);

                Live_CaAuthIdentity authIdentity = new Live_CaAuthIdentity()
                {
                    CreateTime = DateTime.Now,
                    ID = Guid.NewGuid(),
                    Name = name,
                    UserID = 0,
                    UserGid= userGid,
                    Phone = phone,
                    CardID = cardID,
                    AuthType = authType,
                    IsAgree = false,
                    Status = Model.Enums.CaAuthIdentityStatusEnumType.待阅读协议申请书,
                    IsApp= isApp,
                    OrderNo=Guid.NewGuid().ToString("N"),
                    AppName= recordDto?.FxqAppName??"默认应用",
                };


                if (authType == "Face")
                {
                    // 取消18岁一下认证流程
                    //int age = IdCardHelper.GetAge(authIdentity.CardID.Trim());
                    //if (age<18)
                    //{
                    //    // 小于18周岁专用
                    //    authIdentity.AgreeRedictUrl = $"https://company.cn-labor.cn/web-sign/auth-identity?id={authIdentity.ID}&actionType=AuthFaceAgeOk";
                    //    var agreeRes = await _fxqService.CaAge18Apply(authIdentity.Name, authIdentity.CardID, authIdentity.AgreeRedictUrl);
                    //    if (agreeRes.Code == 10000)
                    //    {
                    //        authIdentity.AgreeUrl = agreeRes.Data.AgreementUrl;
                    //        authIdentity.FaceAgeApplyId = agreeRes.Data.ApplyId;
                    //        //authIdentity.AgreeVerifyId = agreeRes.Data.ApplyId;
                    //        await _service.Add(authIdentity);
                    //        return Ok(authIdentity.AgreeUrl);
                    //    }
                    //    else
                    //    {
                    //        throw new ErrorCodeException(-1, agreeRes.Msg);
                    //    }
                    //}

                    if (!isApp)
                    {


                        

                        authIdentity.AgreeRedictUrl = $"https://company.cn-labor.cn/web-sign/auth-identity?id={authIdentity.ID}&actionType=AgreeOk";
                        var agreeRes = await _fxqService.CaAgreementApply(authIdentity.AgreeRedictUrl, authIdentity.AppName);
                        if (agreeRes.Code == 10000)
                        {
                            authIdentity.AgreeUrl = agreeRes.Data.AgreementUrl;
                            authIdentity.AgreeVerifyId = agreeRes.Data.verifyId;
                            await _service.Add(authIdentity);
                            return Ok(authIdentity.AgreeUrl);
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, agreeRes.Msg);
                        }

                    }
                    else
                    {
                        authIdentity.FaceRedictUrl = $"https://company.cn-labor.cn/web-sign/auth-identity?id={authIdentity.ID}&actionType=AgreeAndFaceOk";
                        var agreeRes = await _fxqService.CaAgreementAndFaceApply(name, cardID, authIdentity.ID.ToString("N"), authIdentity.FaceRedictUrl, authIdentity.OrderNo, authIdentity.AppName);
                        if (agreeRes.Code == 10000)
                        {
                            authIdentity.AgreeVerifyId = agreeRes.Data.verifyId;
                            authIdentity.AgreeUrl = agreeRes.Data.AgreementUrl;
                            authIdentity.FaceUrl = agreeRes.Data.AgreementUrl;
                            authIdentity.AppName = recordDto?.FxqAppName;
                            await _service.Add(authIdentity);
                            return Ok(authIdentity.AgreeUrl);
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, agreeRes.Msg);
                        }
                    }
                }
                else if (authType=="SMS")
                {

                    //获取短信验证码

                    authIdentity.AgreeRedictUrl = $"https://company.cn-labor.cn/web-sign/auth-identity?id={authIdentity.ID}&actionType=AgreeSmsOk";
                    var agreeRes = await _fxqService.CaAgreementApply(authIdentity.AgreeRedictUrl, authIdentity.AppName);
                    if (agreeRes.Code == 10000)
                    {
                        authIdentity.AgreeUrl = agreeRes.Data.AgreementUrl;
                        authIdentity.AgreeVerifyId = agreeRes.Data.verifyId;
                        await _service.Add(authIdentity);
                        return Ok(authIdentity.AgreeUrl);
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, agreeRes.Msg);
                    }


                }
                else
                {
                    throw new ErrorCodeException(-1, "未知的认证类型");
                }

                
                
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CaFaceApply([FromForm]Guid id,[FromForm]string isAgree,[FromForm] string verifyId)
        {
            try
            {
                _logger.LogInformation($"参数：{id}_{isAgree}_{verifyId}");
                var record = await _service.Get(f => f.ID == id && f.AgreeVerifyId == verifyId);
                record.IsAgree = true;

                record.FaceOrderId = Guid.NewGuid().ToString("N");
                record.OrderNo = record.FaceOrderId;
                //record.FaceRedictUrl = $"https://company.cn-labor.cn/web-sign/auth-identity?id={record.ID}";
                record.FaceRedictUrl = $"https://company.cn-labor.cn/web-sign/auth-identity?id={record.ID}&actionType=AuthFaceOk";
                var faceRes = await _fxqService.CaFaceApply(record.Name, record.CardID, record.ID.ToString("N"), record.FaceRedictUrl, record.FaceOrderId, record.AgreeVerifyId, record.AppName);
                if (faceRes.Code == 10000)
                {
                    record.FaceUrl = faceRes.Data;
                    record.FaceVerifyId = faceRes.VerifyId;
                    record.Status = Model.Enums.CaAuthIdentityStatusEnumType.待认证;
                    await _service.Update(record);
                    return Ok(record.FaceUrl);
                }
                else
                {
                    throw new ErrorCodeException(-1, faceRes.Msg);
                }
            }
            catch (Exception ex)
            {

                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CaAuthFaceComplete([FromForm] Guid id, [FromForm] string code, [FromForm] string newSignature, [FromForm] string h5faceId, [FromForm] string liveRate, [FromForm] string verifyId)
        {
            try
            {
                var record = await _service.Get(f => f.ID == id);
                record.FaceRescode = code;
                record.FaceResNewSignature = newSignature;
                record.FaceResH5faceId = h5faceId;
                //record.LiveRate = liveRate;
                record.FaceResLiveRate = liveRate;
                record.FaceVerifyId = verifyId;

                if (code == "0")
                {
                    record.AuthTime = DateTime.Now;
                    record.Status = Model.Enums.CaAuthIdentityStatusEnumType.认证成功;
                }
                else
                {
                    record.Status = Model.Enums.CaAuthIdentityStatusEnumType.认证失败;
                }
                var recordDto = new CaAuthIdentityDto()
                {
                    AuthType = record.AuthType,
                    CardID = record.CardID,
                    Name = record.Name,
                    Phone = record.Phone,
                    StatusName = record.Status == Model.Enums.CaAuthIdentityStatusEnumType.认证成功? "已认证" : "认证失败",
                    UserGid = record.UserGid??Guid.Empty,
                    AuthDate = record.AuthTime?.ToString("yyyy-MM-dd")
                };
                await _service.Update(record);
                if (record.Status== CaAuthIdentityStatusEnumType.认证成功)
                {
                    CaAuthCardIdDto dto = new CaAuthCardIdDto() { CardID = record.CardID, IsCaAuth = true };
                    await _httpService.PostAsync(ServiceNames.UsersService, "/api/Entry/ChangeCaAuthStatus", JsonConvert.SerializeObject(dto));
                }

               


                return Ok(recordDto);
            }
            catch (Exception ex)
            {

                throw new ErrorCodeException(-1, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CaAuthFaceAgeComplete([FromForm] Guid id, [FromForm] string code, [FromForm] string sign, [FromForm] string appKey, [FromForm] string state, [FromForm] string nonce, [FromForm] string version, [FromForm] string signMethod, [FromForm] string timestamp)
        {
            try
            {
                var record = await _service.Get(f => f.ID == id);
                record.FaceAgeCode = code;
                record.FaceAgeState = state;
                record.FaceAgeTimestamp = timestamp;

                if (state == "OVER")
                {
                    record.AuthTime = DateTime.Now;
                    record.Status = Model.Enums.CaAuthIdentityStatusEnumType.认证成功;
                }
                else
                {
                    record.Status = Model.Enums.CaAuthIdentityStatusEnumType.认证失败;
                }
                var recordDto = new CaAuthIdentityDto()
                {
                    AuthType = record.AuthType,
                    CardID = record.CardID,
                    Name = record.Name,
                    Phone = record.Phone,
                    StatusName = record.Status == Model.Enums.CaAuthIdentityStatusEnumType.认证成功 ? "已认证" : "认证失败",
                    UserGid = record.UserGid ?? Guid.Empty,
                    AuthDate = record.AuthTime?.ToString("yyyy-MM-dd")
                };
                await _service.Update(record);
                if (record.Status == CaAuthIdentityStatusEnumType.认证成功)
                {
                    CaAuthCardIdDto dto = new CaAuthCardIdDto() { CardID = record.CardID, IsCaAuth = true };
                    await _httpService.PostAsync(ServiceNames.UsersService, "/api/Entry/ChangeCaAuthStatus", JsonConvert.SerializeObject(dto));
                }




                return Ok(recordDto);
            }
            catch (Exception ex)
            {

                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> AgreeTest()
        {

            //var downRes = await _fxqService.CaAgreementApply("https://company.cn-labor.cn/web-sign/auth-identity");
            //var downRes = await _fxqService.CaFaceApply("郑康", "320722199203033956", "1", "https://company.cn-labor.cn/web-sign/auth-identity", "1", "U_20260707201125670843260604416");
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> SendCaAuthNotice( [FromForm] string cardID, [FromForm] string phone,  [FromForm] Guid? userGid)
        {
          var mode = _service.Get(f => f.CardID == cardID&&f.Status== CaAuthIdentityStatusEnumType.认证成功);
            if (mode != null)
            {
                throw new ErrorCodeException(-1,"已认证员工，无法执行此操作！");
            }
            else {
                if (userGid.HasValue)
                {
                    await _fxqService.SmsSend(phone, $"请点击链接完成入职实名认证：https://company.cn-labor.cn/web-sign/auth-identity?userGid={userGid.Value}");
                }
                else
                {
                    await _fxqService.SmsSend(phone, $"请点击链接完成入职实名认证：https://company.cn-labor.cn/web-sign/auth-identity");
                }
            }
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> GetCaSmsCode([FromForm] Guid id, [FromForm]string phone, [FromForm] string isAgree ,[FromForm] string verifyId)
        {
            var mode =await _service.Get(f =>f.ID==id);

            if (mode == null)
            {
                throw new ErrorCodeException(-1, "认证信息不存在,请稍后再试！");
            }
            else
            {
                mode.IsAgree = isAgree=="1";
               
                if (mode.AuthType!="SMS")
                {
                    throw new ErrorCodeException(-1, "此认证方式无需获取短信验证码！");
                }
                if (mode.IsAgree)
                {
                    var res = await _fxqService.CaSendSmsApply(phone, "Ca实名认证验证码： %s ，验证码有效期限为10分钟", null,mode.AppName);
                    if (res.Code == 10000)
                    {
                        mode.SmsPhone = phone;
                        mode.SmsTime = res.Data.Time;
                        mode.SmsMsgId = res.Data.MsgId;
                       await _service.Update(mode);
                    }
                    else {
                        throw new ErrorCodeException(-1, $"验证码获取失败！{res.Msg}");
                    }
                }
                else {
                    throw new ErrorCodeException(-1, "CA阅读协议未阅读完成！");
                }
                

            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CaValidateSmsCode([FromForm] Guid id, [FromForm] string smsCode)
        {
            if (string.IsNullOrWhiteSpace(smsCode))
            {
                throw new ErrorCodeException(-1, "验证码不能为空！");
            }
            var authIdentity = await _service.Get(f => f.ID == id);
            if (authIdentity == null)
            {
                throw new ErrorCodeException(-1, "认证信息不存在,请稍后再试！");
            }
            else
            {

                if (authIdentity.Status== CaAuthIdentityStatusEnumType.认证成功)
                {
                    throw new ErrorCodeException(-1, "已认证完成，无需重复认证！");
                }
                var res = await _fxqService.CaValidateSmsCodeApply(authIdentity.Name, authIdentity.CardID, authIdentity.SmsPhone, smsCode, authIdentity.AgreeVerifyId, authIdentity.AppName);

                if (res.Code!=10000)
                {
                    throw new ErrorCodeException(-1, "异常信息:"+res.Msg);
                }

                if (res.Data.State == 1)
                {
                    authIdentity.Status = CaAuthIdentityStatusEnumType.认证成功;
                    authIdentity.AuthTime = DateTime.Now;
                    await _service.Update(authIdentity);
                }
                else
                {
                    authIdentity.Status = CaAuthIdentityStatusEnumType.认证失败;
                    await _service.Update(authIdentity);
                }
            }


            if (authIdentity.Status == CaAuthIdentityStatusEnumType.认证成功)
            {
                CaAuthCardIdDto dto = new CaAuthCardIdDto() { CardID = authIdentity.CardID, IsCaAuth = true };
                await _httpService.PostAsync(ServiceNames.UsersService, "/api/Entry/ChangeCaAuthStatus", JsonConvert.SerializeObject(dto));
            }

            var recordDto = new CaAuthIdentityDto()
            {
                AuthType = authIdentity.AuthType,
                CardID = authIdentity.CardID,
                Name = authIdentity.Name,
                Phone = authIdentity.Phone,
                StatusName = authIdentity.Status == CaAuthIdentityStatusEnumType.认证成功 ? "已认证" : "未认证",
                UserGid = authIdentity.UserGid.Value,
                AuthDate = authIdentity.AuthTime?.ToString("yyyy-MM-dd"),
            };
            return Ok($"https://company.cn-labor.cn/web-sign/auth-identity?id={authIdentity.ID}");
        }

        [HttpGet]
        public async Task<IActionResult> GetCaAuthStatus(string cardID)
        {
            if (string.IsNullOrWhiteSpace(cardID))
            {
                return Ok(new CaAuthCardIdDto { CardID = cardID, IsCaAuth = false });
            }
            var isAuth= await _service.Exist(f => f.CardID == cardID && f.Status == CaAuthIdentityStatusEnumType.认证成功);
            return Ok( new CaAuthCardIdDto { CardID=cardID,IsCaAuth= isAuth });
        }


        [HttpGet]
        public async Task<IActionResult> ListCaAuthStatus(int days)
        {
            var dt = DateTime.Today.AddDays(-days);
            var modelList = await _service.List(f => f.AuthTime != null && f.AuthTime.Value >= dt && f.Status == CaAuthIdentityStatusEnumType.认证成功);
            var records = modelList.Select(f => new CaAuthCardIdDto()
            {
                CardID = f.CardID,
                IsCaAuth = true
            }).ToList();
            return Ok(records);
        }
    }
}
