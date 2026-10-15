using LZ.ApiService.Interface;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Customer.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Design.Internal;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Asn1.Sec;
using Senparc.Weixin.MP.AdvancedAPIs.Card;

namespace LZ.Customer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CaAuthController : Controller
    {

        private readonly ICaAuthIdentityService _service;
        private readonly IHttpService _httpService;
        private readonly IFxqService _fxqService;

        public CaAuthController(ICaAuthIdentityService service, IHttpService httpService, IFxqService fxqService)
        {
            _service = service;
            _httpService = httpService;
            _fxqService = fxqService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AuthIdentity(Guid? userGid, Guid? id, string actionType, string isAgree, string verifyId, string code, string newSignature, string h5faceId, string liveRate, string orderNo)
        {
            CaAuthIdentityViewModel user = null;
            Live_CaAuthIdentity model = null;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                model = await _service.Get(f => f.ID == id.Value);

                if (!string.IsNullOrWhiteSpace(actionType))
                {
                    if (actionType == "AgreeOk")
                    {
                        model.IsAgree = isAgree == "1";
                        model.AgreeVerifyId = verifyId;
                        model.Status = CaAuthIdentityStatusEnumType.待认证;

                        if (model.AuthType == "SMS")
                        {
                            user = new CaAuthIdentityViewModel
                            {
                                Id=model.ID,
                                Name = model.Name,
                                IdCardNumber = model.CardID,
                                PhoneNumber = model.Phone,
                                AuthMethod = model.AuthType,
                                IsAuthenticated = model.Status == Model.Enums.CaAuthIdentityStatusEnumType.认证成功,
                                IsApp = IsMiniProgramRequest(),
                                IsSendSms = true,
                            };

                           await _service.Update(model);
                            return View(user);
                        }


                    }
                    else if (actionType == "AgreeAndFaceOk")
                    {
                        model.FaceRescode = code;
                        model.FaceResNewSignature = newSignature;
                        model.FaceResH5faceId = h5faceId;
                        model.LiveRate = liveRate;
                        model.FaceVerifyId = verifyId;
                    }
                    else if (actionType == "AuthFaceOk")
                    {
                        model.FaceRescode = code;
                        model.FaceResNewSignature = newSignature;
                        model.FaceResH5faceId = h5faceId;
                        model.LiveRate = liveRate;
                        model.FaceVerifyId = verifyId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.FaceRescode) && model.FaceRescode == "0")
                    {
                        model.Status = CaAuthIdentityStatusEnumType.认证成功;
                    }
                    else
                    {
                        model.Status = CaAuthIdentityStatusEnumType.认证失败;
                    }
                    await _service.Update(model);
                }


            }
            if (model == null)
            {
                if (userGid.HasValue && userGid.Value != Guid.Empty)
                {
                    var recordDto = await _httpService.GetAsync<CaAuthIdentityDto>(ServiceNames.UsersService, "/api/user/GeCaAuthInfoByUserID?userGid=" + userGid);
                    if (recordDto != null)
                    {
                        model = new Live_CaAuthIdentity()
                        {
                            Name = recordDto.Name,
                            CardID = recordDto.CardID,
                            Phone = recordDto.Phone,
                            Status = Model.Enums.CaAuthIdentityStatusEnumType.待认证
                        };
                    }

                }
            }
            if (model != null)
            {

                user = new CaAuthIdentityViewModel
                {
                    Name = model.Name,
                    IdCardNumber = model.CardID,
                    PhoneNumber = model.Phone,
                    AuthMethod = model.AuthType,
                    IsAuthenticated = model.Status == Model.Enums.CaAuthIdentityStatusEnumType.认证成功
                };

            }
            else
            {
                user = new CaAuthIdentityViewModel
                {
                    Name = "",
                    IdCardNumber = "",
                    PhoneNumber = "",
                    AuthMethod = "Face",
                    IsAuthenticated = false,
                };

            }
            user.IsApp = IsMiniProgramRequest();
            return View(user);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AuthFace(Guid? userGid,Guid? id,string actionType,string isAgree,string verifyId,string code,string newSignature,string h5faceId,string liveRate,string orderNo)
        {
            CaAuthIdentityViewModel user = null;
            Live_CaAuthIdentity model = null;
            if (id.HasValue && id.Value != Guid.Empty)
            {
                model = await _service.Get(f => f.ID == id.Value);

                if (string.IsNullOrWhiteSpace(actionType))
                {
                    if (actionType == "AgreeOk")
                    {
                        model.IsAgree = isAgree == "1";
                        model.AgreeVerifyId = verifyId;
                    }
                    else if (actionType == "AgreeAndFaceOk")
                    {
                        model.FaceRescode = code;
                        model.FaceResNewSignature = newSignature;
                        model.FaceResH5faceId = h5faceId;
                        model.LiveRate = liveRate;
                        model.FaceVerifyId = verifyId;
                    }
                    else if (actionType == "AuthFaceOk")
                    {
                        model.FaceRescode = code;
                        model.FaceResNewSignature = newSignature;
                        model.FaceResH5faceId = h5faceId;
                        model.LiveRate = liveRate;
                        model.FaceVerifyId = verifyId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.FaceRescode) && model.FaceRescode == "0")
                    {
                        model.Status = CaAuthIdentityStatusEnumType.认证成功;
                    }
                    else
                    {
                        model.Status = CaAuthIdentityStatusEnumType.认证失败;
                    }
                    await _service.Update(model);
                }


            }
            if (model==null)
            {
                if (userGid.HasValue&&userGid.Value!=Guid.Empty)
                {
                   var  recordDto = await _httpService.GetAsync<CaAuthIdentityDto>(ServiceNames.UsersService, "/api/user/GeCaAuthInfoByUserID?userGid=" + userGid);
                    if (recordDto!=null)
                    {
                        model = new Live_CaAuthIdentity()
                        {
                            Name = recordDto.Name,
                            CardID = recordDto.CardID,
                            Phone = recordDto.Phone,
                            Status = Model.Enums.CaAuthIdentityStatusEnumType.待认证
                        };
                    }
                   
                }
            }
            if (model != null)
            {
             
                user = new CaAuthIdentityViewModel
                {
                    Name = model.Name,
                    IdCardNumber = model.CardID,
                    PhoneNumber = model.Phone,
                    AuthMethod = model.AuthType,
                    IsAuthenticated = model.Status == Model.Enums.CaAuthIdentityStatusEnumType.认证成功
                };

            }
            else {
                user = new CaAuthIdentityViewModel
                {
                    Name = "",
                    IdCardNumber = "",
                    PhoneNumber = "",
                    AuthMethod = "Face",
                    IsAuthenticated = false,
                };

            }
            user.IsApp = IsMiniProgramRequest();
            return View(user);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AuthSubmit(CaAuthIdentityViewModel dto)
        {
            try
            {
                var models = await _service.List(f => f.CardID == dto.IdCardNumber);

                if (models != null && models.Any(f => f.Status == CaAuthIdentityStatusEnumType.认证成功))
                {
                    throw new ErrorCodeException(-1, "已认证完成，无需重复认证！");
                }
                Live_CaAuthIdentity authIdentity = null;
                if (dto.Id != null)
                {
                    authIdentity = await _service.Get(f => f.ID == dto.Id);
                }
                else {
                    authIdentity = new Live_CaAuthIdentity()
                    {
                        CreateTime = DateTime.Now,
                        ID = Guid.NewGuid(),
                        Name = dto.Name,
                        UserID = 0,
                        UserGid = dto.UserGid,
                        Phone = dto.PhoneNumber,
                        CardID = dto.IdCardNumber,
                        AuthType = dto.AuthMethod,
                        IsAgree = false,
                        Status = Model.Enums.CaAuthIdentityStatusEnumType.待阅读协议申请书,
                        IsApp = IsMiniProgramRequest(),
                        OrderNo = Guid.NewGuid().ToString("N")
                    };
                }
               

                if (dto.AuthMethod=="Face")
                {
                    if (!authIdentity.IsApp)
                    {
                        authIdentity.AgreeRedictUrl = $"https://api.cn-labor.cn/api/customer/CaAuth/AuthIdentity?id={authIdentity.ID}&actionType=AgreeOk";
                        var agreeRes = await _fxqService.CaAgreementApply(authIdentity.AgreeRedictUrl, authIdentity.AppName);
                        if (agreeRes.Code == 10000)
                        {
                            authIdentity.AgreeUrl = agreeRes.Data.AgreementUrl;
                            authIdentity.AgreeVerifyId = agreeRes.Data.verifyId;
                            await _service.Add(authIdentity);
                            return Redirect(authIdentity.AgreeUrl);
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, agreeRes.Msg);
                        }
                    }
                    else
                    {
                        authIdentity.FaceRedictUrl = $"https://api.cn-labor.cn/api/customer/CaAuth/AuthIdentity?id={authIdentity.ID}&actionType=AgreeAndFaceOk";
                        var agreeRes = await _fxqService.CaAgreementAndFaceApply(authIdentity.Name, authIdentity.CardID, authIdentity.ID.ToString("N"), authIdentity.FaceRedictUrl, authIdentity.OrderNo,authIdentity.AppName);
                        if (agreeRes.Code == 10000)
                        {
                            authIdentity.AgreeUrl = agreeRes.Data.AgreementUrl;
                            authIdentity.FaceUrl = agreeRes.Data.AgreementUrl;
                            await _service.Add(authIdentity);
                            //return Ok(authIdentity.AgreeUrl);
                            return Redirect(authIdentity.AgreeUrl);
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, agreeRes.Msg);
                        }
                    }
                }
                else {

                    if (authIdentity.Status == CaAuthIdentityStatusEnumType.待阅读协议申请书)
                    {
                        authIdentity.AgreeRedictUrl = $"https://api.cn-labor.cn/api/customer/CaAuth/AuthIdentity?id={authIdentity.ID}&actionType=AgreeOk";
                        var agreeRes = await _fxqService.CaAgreementApply(authIdentity.AgreeRedictUrl, authIdentity.AppName);
                        if (agreeRes.Code == 10000)
                        {
                            authIdentity.AgreeUrl = agreeRes.Data.AgreementUrl;
                            authIdentity.AgreeVerifyId = agreeRes.Data.verifyId;
                            await _service.Add(authIdentity);
                            return Redirect(authIdentity.AgreeUrl);
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, agreeRes.Msg);
                        }
                    }
                    else {

                        var res = await _fxqService.CaValidateSmsCodeApply(authIdentity.Name, authIdentity.CardID, authIdentity.Phone, dto.SmsCode, authIdentity.AgreeVerifyId, authIdentity.AppName);
                        if (res.Data.State == 1)
                        {
                            authIdentity.Status = CaAuthIdentityStatusEnumType.认证成功;
                            authIdentity.AuthTime = DateTime.Now;
                            await _service.Update(authIdentity);
                        }
                        else {
                            authIdentity.Status = CaAuthIdentityStatusEnumType.认证失败;
                        }



                        var user = new CaAuthIdentityViewModel
                        {
                            Name = authIdentity.Name,
                            IdCardNumber = authIdentity.CardID,
                            PhoneNumber = authIdentity.Phone,
                            AuthMethod = authIdentity.AuthType,
                            IsAuthenticated = authIdentity.Status == Model.Enums.CaAuthIdentityStatusEnumType.认证成功
                        };

                        //return View(user);

                        return RedirectToAction("AuthIdentity",new { id= authIdentity .ID});
                    }

                 
                }
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, ex.Message);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult>SendSmsCode([FromBody]CaAuthIdentityViewModel dto)
        {
            try
            {
                var model = await _service.Get(f => f.ID == dto.Id);
                if (model.AuthType=="SMS")
                {
                  await  _fxqService.CaSendSmsApply(model.Phone,null,null,model.AppName);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, ex.Message);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendSmsCode11()
        {
            try
            {


                return RedirectToAction("AuthIdentity", new { id = "E99B3BD2-E31A-4AEC-8950-784CF400F5BC" });
            }
            catch (Exception ex)
            {
                throw new ErrorCodeException(-1, ex.Message);
            }
        }




        private bool IsMiniProgramRequest()
        {
            var userAgent = Request.Headers["User-Agent"].ToString();

            // 1. 基础判断：是否来自微信环境 (包含公众号和小程序)
            if (string.IsNullOrEmpty(userAgent) || !userAgent.Contains("MicroMessenger", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // 2. 进阶判断：是否特指小程序环境
            // 注意：部分旧版本或特定场景下可能不包含 MiniProgram 字样，
            // 如果业务允许公众号也访问同一接口，建议只判断 MicroMessenger
            if (userAgent.Contains("MiniProgram", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // 如果只想区分“微信内”而非严格“小程序”，返回 true 即可
            // 这里根据需求，通常包含 MicroMessenger 即视为微信环境
            return true;
        }
    }
}
