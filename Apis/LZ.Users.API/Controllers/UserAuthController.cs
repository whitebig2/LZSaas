using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LZ.Users.API.Controllers
{
    
    public class UserAuthController(IAuthService authService,IUserService userService, IHttpService httpService,IEntryService entryService,ICompanyConfigService companyConfigService, IEventPublisher publisher) : UsersControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<RequestResultModel> OnLogin([FromForm] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ErrorCodeException(-1, "code错误");
            }
            
            var clientId = HttpContext.Request.Headers["client-id"].ToString();
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            var paras = new Dictionary<string, string>();
            paras.Add("Code", code);
            paras.Add("ClientID", clientId);
            try
            {
                var wx = await httpService.GetAsync<Model.WeiXinModel>(ServiceNames.ClientsService, "/api/auth/verifycode", paras);
                var opmodel = await userService.GetOpenID(wx.OpenID);
                if (opmodel == null)
                {
                    return new RequestResultModel (){ Code = -2, Message = "该用户不存在" };
                }
                var model = await userService.Get(opmodel.UserID);
                if (!model.Enabled)
                {
                    throw new ErrorCodeException(-1, "您已被管理员禁止登录");
                }
                if(!model.CompanyID.HasValue || !model.CompanyGID.HasValue)
                {
                    throw new ErrorCodeException(-1, "您的账号未指定人力公司");
                }
                var company = await companyConfigService.Get(model.CompanyGID);
                if (company == null || company.Company == null)
                {
                    throw new ErrorCodeException(-1, "未找到该人力公司");
                }
                var shopId = model.ShopID.HasValue ? model.ShopID.Value : 0;
                var userType = model.UserType.HasValue ? model.UserType.Value : 0;
                var token = await authService.GenerateUserTokenAsync(model, model.UserID, Model.Enums.PlatformType.XFZMini, model.CompanyID.Value, model.CompanyGID, company.Company.CutType, fullName: model.FullName, shopId: shopId,userType:userType);
                var isSign = model.SignDate.HasValue && model.SignDate.Value.Date == DateTime.Now.Date;
                var integral = !model.Integral.HasValue || model.Integral < 0 ? 0 : model.Integral;
                return new RequestResultModel() { Code = 200, Message = "获取成功", Data = new { token = token.Token, user = new { name = model.FullName, picture = model.Picture, phone = model.Phone, userid = model.UserID, nickname = model.NickName, cardStatus = model.UploadCardStatus, isReal = model.IsReal, status = model.Status,isDayOver=model.IsDayOver,usergid=model.UserGID, openid = model.OpenID, isBindMP = model.IsBindMP, integral, isSign,userType=model.UserType, isShowInfo = model.IsShowInfo, isRegPerfect = model.IsRegPerfect }, openid = wx.OpenID } };
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error("", ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Model.WeiXinModel user)
        {
            try
            {
                var clientId = HttpContext.Request.Headers["client-id"].ToString();
                if (string.IsNullOrEmpty(clientId))
                {
                    throw new ErrorCodeException(-1, "来源错误");
                }
                if (string.IsNullOrEmpty(user.Phone))
                {
                    var paras = new Dictionary<string, string>();
                    paras.Add("Encrypted", user.Encrypted);
                    paras.Add("Iv", user.Iv);
                    paras.Add("OpenId", user.OpenID);
                    paras.Add("ClientId", clientId);
                    var wx = await httpService.PostAsync(ServiceNames.ClientsService, "/api/auth/getphone", JsonConvert.SerializeObject(paras));
                    if (wx!=null && wx.Code == 200 && wx.Data != null)
                    {
                        user.Phone = wx.Data.ToString();
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, "手机号获取失败");
                    }
                    //var phoneNumber = EncryptHelper.DecryptPhoneNumber(user.OpenID, user.Encrypted, user.Iv);
                    //user.Phone = phoneNumber.phoneNumber;
                }
                
                var client = await httpService.GetAsync<Model.Live_Clients>(ServiceNames.ClientsService, "/api/clients/get?id=" + clientId);
                if (client == null)
                {
                    throw new ErrorCodeException(-1, "该小程序未注册");
                }
                var openModel=await userService.GetOpenID(user.OpenID);
                var model = new Model.Live_User();
                if (openModel == null)
                {
                    var opensModel = new Model.Live_UserOpenIds();
                    opensModel.OpenId= user.OpenID;
                    opensModel.FromID = client.ID;
                    opensModel.OpenIdType = "mini";
                    //if (string.IsNullOrEmpty(user.Picture))
                    //{
                    //    return Ok(new { code = 3, msg = "您是第一次登录!", data = user.Phone });
                    //}
                    var isBindMP = !string.IsNullOrEmpty(user.MPOpenID) && user.ClientID.HasValue;
                    model = await userService.ExistPhone(user.Phone, client.CompanyID.Value);
                    if (model == null)
                    {
                        model = new Model.Live_User();
                        if (user.FUID.HasValue)
                        {
                            model.FUID = user.FUID;
                        }
                        if (user.UserID.HasValue)
                        {
                            model.PerUserID = user.UserID;
                        }
                        if (user.CustomerID.HasValue)
                        {
                            model.CusID = user.CustomerID;
                        }
                        if (string.IsNullOrEmpty(user.Picture))
                        {
                            Random r = new Random();
                            int num = r.Next(1, 30);
                            user.Picture = $"/images/avatar/{num}.jpg";
                        }
                        if (string.IsNullOrEmpty(user.Name))
                        {
                            user.Name =$"用户名{Common.Tool.GetRandomNumber(4)}";
                        }
                        model.CompanyID = client.CompanyID;
                        model.CompanyGID = client.CompanyGID;
                        model.Phone = user.Phone;
                        model.Picture = user.Picture;
                        model.UserName = user.Name;
                        model.NickName = user.Name;
                        model.FullName = user.Name;
                        model.CreateTime = DateTime.Now;
                        model.UserGID = Guid.NewGuid();
                        model.Enabled = true;
                        model.Type = 1;
                        model.UnionId = user.UnionID;
                        model.Integral = 0;
                        model.IsBindMP = isBindMP;
                        model.UserID = await userService.Add(model);
                        //await entryService.SetUser(model.Phone, model.UserID, model.CompanyID.Value);//已存在入职或待入职记录的，进行关联
                    }
                    else
                    {
                        if (user.UserID.HasValue)
                        {
                            model.PerUserID = user.UserID;
                        }
                        if (user.CustomerID.HasValue)
                        {
                            model.CusID = user.CustomerID;
                        }
                        model.UnionId = user.UnionID;
                        model.Picture = user.Picture;
                        model.UserName = user.Name;
                        model.NickName = user.Name;
                        model.Enabled = true;
                        model.Type = 1;
                        model.Phone = user.Phone;
                        model.IsBindMP = isBindMP;
                        if (!model.Integral.HasValue) model.Integral = 0;
                        await userService.Update(model);
                    }
                    if (string.IsNullOrEmpty(model.Qrcode))
                    {
                        publisher.Publish(new UserQrcodeEventData()
                        {
                            UserID = model.UserID,
                            CompanyID = model.CompanyID.Value,
                            CompanyGID = model.CompanyGID.Value
                        });
                    }
                    opensModel.UserID = model.UserID;
                    var opensModels = new List<Model.Live_UserOpenIds>();
                    opensModels.Add(opensModel);
                    if (isBindMP)
                    {
                        if (!await userService.ExistOpenID(user.MPOpenID, model.UserID, user.ClientID.Value))
                        {
                            var opensModelMp = new Model.Live_UserOpenIds();
                            opensModelMp.OpenId = user.MPOpenID;
                            opensModelMp.FromID = user.ClientID.Value;
                            opensModelMp.OpenIdType = "mp";
                            opensModelMp.UserID = model.UserID;
                            opensModels.Add(opensModelMp);
                        }
                    }
                    await userService.AddOpenID(opensModels);
                }
                else
                {
                    model = await userService.Get(openModel.UserID);
                    if (model == null)
                    {
                        throw new ErrorCodeException(-1, "您需重新注册");
                    }
                }
                if (!model.Enabled)
                {
                    throw new ErrorCodeException(-1, "您已被管理员禁止登陆，请联系客服");
                }
                if (!model.CompanyID.HasValue || !model.CompanyGID.HasValue)
                {
                    throw new ErrorCodeException(-1, "您的账号未指定人力公司"+model.UserID);
                }
                var company = await companyConfigService.Get(model.CompanyGID);
                if (company==null || company.Company==null)
                {
                    throw new ErrorCodeException(-1, "未找到该人力公司");
                }
                var shopId = model.ShopID.HasValue ? model.ShopID.Value : 0;
                var userType = model.UserType.HasValue ? model.UserType.Value : 0;
                var token = await authService.GenerateUserTokenAsync(model, model.UserID, Model.Enums.PlatformType.XFZMini, model.CompanyID.Value, model.CompanyGID, company.Company.CutType, fullName: model.FullName, shopId: shopId, userType: userType);
                var isSign = model.SignDate.HasValue && model.SignDate.Value.Date == DateTime.Now.Date;
                var integral = !model.Integral.HasValue || model.Integral < 0 ? 0 : model.Integral;
                return Ok(new { user = new { name = model.FullName, picture = model.Picture, phone = model.Phone, userid = model.UserID, nickname = model.NickName, cardStatus = model.UploadCardStatus, isReal = model.IsReal,status=model.Status, isDayOver = model.IsDayOver, usergid = model.UserGID, openid = user.OpenID,isBindMP=model.IsBindMP, integral, userType = model.UserType, isSign, isShowInfo = model.IsShowInfo, isRegPerfect = model.IsRegPerfect }, token = token.Token, openid = user.OpenID });

            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex.Message, ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OpenID(string code)
        {
            throw new ErrorCodeException(-1, "test");
            if (string.IsNullOrEmpty(code))
            {
                throw new ErrorCodeException(-1, "code错误");
            }
            var appid = HttpContext.Request.Headers["app-id"].ToString();
            if (string.IsNullOrEmpty(appid))
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            try
            {
                var paras = new Dictionary<string, string>();
                paras.Add("Code", code);
                paras.Add("AppID", appid);
                var wx = await httpService.GetAsync<Model.WeiXinModel>(ServiceNames.ClientsService, "/api/auth/verifycode", paras);
                return Ok(new { openid = wx.OpenID, unionid = wx.UnionID });
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error("", ex);
                throw new ErrorCodeException(-1, "网络开了个小差！");
               
            }
        }
    }
}
