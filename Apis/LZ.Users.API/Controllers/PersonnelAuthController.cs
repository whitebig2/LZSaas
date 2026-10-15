using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LZ.Users.API.Controllers
{
    
    public class PersonnelAuthController(IAuthService authService,IPersonnelService personnelService, IHttpService httpService,ICompanyConfigService companyConfigService, IEventPublisher publisher) : UsersControllerBase
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
                var opmodel = await personnelService.GetOpenID(wx.OpenID);
                if (opmodel == null)
                {
                    return new RequestResultModel (){ Code = -2, Message = "该用户不存在" };
                }
                var model = await personnelService.Get(opmodel.UserID);
                if (!model.Enabled)
                {
                    throw new ErrorCodeException(-1, "您已被管理员禁止登录");
                }
                if (!model.CompanyID.HasValue || !model.CompanyGID.HasValue)
                {
                    throw new ErrorCodeException(-1, "您的账号未指定人力公司");
                }
                var company = await companyConfigService.Get(model.CompanyGID);
                if (company == null || company.Company == null)
                {
                    throw new ErrorCodeException(-1, "未找到该人力公司");
                }
                var token = await authService.GenerateUserTokenAsync(model, model.UserID, Model.Enums.PlatformType.LZMini, model.CompanyID.Value, model.CompanyGID, company.Company.CutType, fullName: model.FullName);
                return new RequestResultModel() { Code = 200, Message = "获取成功", Data = new { token = token.Token, user = new { name = model.FullName, picture = model.Picture, phone = model.Phone, userid = model.UserID, nickname = model.NickName, type = model.UserType, isBindMP = model.IsBindMP, clientQrcode = model.ClientQrcode }, openid = wx.OpenID } };
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error("", ex);
                throw new ErrorCodeException(-1, "网络开了个小差！");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Model.WeiXinModel user)
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
                if (wx != null && wx.Code == 200 && wx.Data != null)
                {
                    user.Phone = wx.Data.ToString();
                }
                else
                {
                    throw new ErrorCodeException(-1, "手机号获取失败");
                }
            }

            //var client = await httpService.GetAsync<Model.Live_Clients>(ServiceNames.ClientsService, "/api/clients/get?id=" + clientId);
            //if (client == null)
            //{
            //    throw new ErrorCodeException(-1, "该小程序未注册");
            //}
            var openModel = await personnelService.GetOpenID(user.OpenID);
            var model = new Model.Live_Personnel();
            if (openModel == null)
            {
                var opensModel = new Model.Live_PersonnelOpenIds();
                opensModel.OpenId = user.OpenID;
                opensModel.FromID = Convert.ToInt32(clientId);
                opensModel.OpenIdType = "mini";
                if (string.IsNullOrEmpty(user.Picture))
                {
                    return Ok(new { code = 3, msg = "您是第一次登录!", data = user.Phone });
                }
                model = await personnelService.ExistPhone(user.Phone);
                if (model == null)
                {
                    //throw new ErrorCodeException(-1, "该手机号未登记，请联系客服！");
                    return Ok(new { code = 4, msg = "该手机号未注册，请前往注册页面!" });
                }
                else
                {
                    model.Picture = user.Picture;
                    model.UserName = user.Name;
                    model.NickName = user.Name;
                    await personnelService.Update(model);
                }
                opensModel.UserID = model.UserID;
                await personnelService.AddOpenID(opensModel);
            }
            else
            {
                model = await personnelService.Get(openModel.UserID);
                if (model == null)
                {
                    throw new ErrorCodeException(-1, "该手机号已被删除，请联系客服");
                }
            }
            if (!model.Enabled)
            {
                throw new ErrorCodeException(-1, "您已被管理员禁止登陆，请联系客服");
            }
            if (!model.CompanyID.HasValue || !model.CompanyGID.HasValue)
            {
                throw new ErrorCodeException(-1, "您的账号未指定人力公司");
            }
            var company = await companyConfigService.Get(model.CompanyGID);
            if (company == null || company.Company == null)
            {
                throw new ErrorCodeException(-1, "未找到该人力公司");
            }
            var token = await authService.GenerateUserTokenAsync(model, model.UserID, Model.Enums.PlatformType.LZMini, model.CompanyID.Value, model.CompanyGID, company.Company.CutType, fullName: model.FullName);
            if (string.IsNullOrEmpty(model.ClientQrcode))
            {
                publisher.Publish(new UserQrcodeEventData()
                {
                    UserID = model.UserID,
                    CompanyID = model.CompanyID.Value,
                    CompanyGID = model.CompanyGID.Value,
                    FromType = "lz"
                });
            }
            return Ok(new { user = new { name = model.FullName, picture = model.Picture, phone = model.Phone, userid = model.UserID, nickname = model.NickName, type = model.UserType, isBindMP = model.IsBindMP, clientQrcode= model.ClientQrcode }, token = token.Token, openid = user.OpenID });
        }
    }
}
