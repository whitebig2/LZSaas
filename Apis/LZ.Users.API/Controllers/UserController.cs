using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Users.API.CommonHelp;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.Security.Cryptography;
using System.Text;

namespace LZ.Users.API.Controllers
{
    public class UserController(IUserService userService, IAuthService authService, IAliyunOSSService aliyunOSSService,ICompanyConfigService companyConfigService,IEntryService entryService,IPersonnelItemsService personnelItemsService, IEventPublisher publisher, IRedisService redisService,IBlackListService blackListService, IHttpService httpService,IPersonnelService personnelService, IJDYSyncService jdySyncService) : UsersControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_User>();
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(it => it.UserName.Contains(query.UserName));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.FullName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Phone))
            {
                expable.And(it => it.Phone == query.Phone);
            }
            if (query.Enabled.HasValue)
            {
                expable.And(it => it.Enabled == query.Enabled);
            }
            if(query.Status.HasValue)
            {
                expable.And(it => it.Status == query.Status);
            }
            //是否商城推广员
            expable.AndIF(query.IsShopPromotion.HasValue, it => it.IsShopPromotion == query.IsShopPromotion);
            //是否总后台账号和自营公司
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
                if(user.PlatformType == Model.Enums.PlatformType.MiniAdmin && query.From == "shop")
                {
                    expable.And(it => it.IsShopPromotion);
                    if (user.UserType == (int)CompanyUserType.Shop)
                    {
                        expable.And(it => it.ShopID == user.ShopID);
                    }
                }
            }
            else
            {
                expable.AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
            }
            int pid = 0;
            if(query.From=="black" && CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var info = await authService.UserInfo<Model.Live_Partner>();
                pid = info.PartnerID;
                expable.And(it => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.UserID == it.UserID && s.PartnerID == info.PartnerID).Any());
            }
            var exp = expable.ToExpression();
            var models = await userService.List(exp, query);
            return Ok(new { items = models, count = query.Count, pid });
        }
        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var uid = CurrentUser.UserID;
            var user = await userService.Get(uid);
            if (!user.IsReal)
            {
                user.CardID = "";
                user.IdcardBackImage = "";
                user.IdcardFrontImage = "";
                user.BankNumber = "";
                user.BankImage = "";
                if(user.QrcodeGID.HasValue && user.Status==(int)UserStatus.WaitEntry)
                {
                    var qrcodeInfo = await personnelItemsService.Get(user.QrcodeGID.Value);
                    if (qrcodeInfo != null && qrcodeInfo.IsUserContact)
                    {
                        user.EmergencyContact = qrcodeInfo.EmergencyContact;
                        user.EmergencyContactPhone = qrcodeInfo.EmergencyContactPhone;
                    }
                }
            }
            return Ok(user);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetParas()
        {
            string[] moneys = { "0-2000", "2000-3000", "3000-4000", "4000-5000", "5000-6000", "7000-8000", "8000以上" };
            return Ok(moneys);
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_User model)
        {
            if (await userService.ExistName(model.UserName))
            {
                throw new ErrorCodeException(-1, "该用户名已存在!");
            }
            model.UserGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await userService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<int> TurntableCount()
        {
            var user= await authService.UserInfo<Model.Live_User>();
            if (!user.TurntableDate.HasValue || user.TurntableDate.Value.Date!=DateTime.Now.Date)
            {
                user.TurntableCount = 0;
            }
            return user.TurntableCount.HasValue ? user.TurntableCount.Value : 0;
        }
        [HttpPut]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<bool> UpdateTurntable([FromForm] int userId,[FromForm] bool isClear)
        {
            return await userService.UpdateTurntable(userId, isClear);
        }
        [HttpGet]
        public async Task<Model.Live_User> Get()
        {
            return await authService.UserInfo<Model.Live_User>();
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public async Task<RequestResultModel> WeiXinInfo()
        {
            var id = HttpContext.Request.Headers["client-id"].ToString();
            var uid = CurrentUser.UserID;
            var model = await userService.Get(uid);
            if (model == null)
            {
                throw new ErrorCodeException(-4, "用户不存在，请退出重新登录!");
            }
            //var openid = "";
            ////var mpClient=await 
            //var openModel = await userService.GetOpenID(uid, Convert.ToInt32(id));
            //if (openModel != null)
            //{
            //    openid = openModel.OpenId;
            //}
            if (string.IsNullOrEmpty(model.Qrcode))//二维码不存在就重新生成二维码
            {
                publisher.Publish(new UserQrcodeEventData()
                {
                    UserID = model.UserID,
                    CompanyID = model.CompanyID.Value,
                    CompanyGID = model.CompanyGID.Value
                });
            }
            await authService.UpdateUserInfo<Model.Live_User>(model, model.UserID, Model.Enums.PlatformType.XFZMini);//更新缓存
            var isPersonnel = await personnelService.Exist(it => it.Phone == model.Phone && it.CompanyID == model.CompanyID);
            var isSign = model.SignDate.HasValue && model.SignDate.Value.Date == DateTime.Now.Date;
            var integral = !model.Integral.HasValue || model.Integral < 0 ? 0 : model.Integral;
            return new RequestResultModel() { Code = 200, Message = "获取成功", Data = new { name = model.FullName, picture = model.Picture, phone = model.Phone, userid = model.UserID, nickname = model.NickName, cardStatus = model.UploadCardStatus, isReal = model.IsReal, status = model.Status, isDayOver = model.IsDayOver, usergid = model.UserGID, isPersonnel,isBindMP=model.IsBindMP, integral,qrcode= model.Qrcode, isSign, userType = model.UserType, isShowInfo=model.IsShowInfo, isRegPerfect=model.IsRegPerfect } };
        }
        [HttpPut]
        public async Task<IActionResult> BatchChangBank()
        {
            var models = await userService.List(it=>!SqlFunc.IsNullOrEmpty(it.BankNumber) && SqlFunc.IsNullOrEmpty(it.BankProvince) && it.Status==2, new Model.PagedModel.UserPagedModel() { Page=0,Top=500});
            foreach(var m in models)
            {
                publisher.Publish(new UserBankEventData()
                {
                    UserID = m.UserID,
                    BankNumber = m.BankNumber
                });
                Thread.Sleep(100);
            }
            return Ok();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Model.Live_User model)
        {
            if (model == null)
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            if (string.IsNullOrEmpty(model.OpenID))
            {
                throw new ErrorCodeException(-1, "小程序openid错误！");
            }
            if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.OpenID))
            {
                throw new ErrorCodeException(-1, "姓名和手机号不能为空！");
            }
            try
            {
                var guid = HttpContext.Request.Headers["company-guid"].ToString();
                var clientId = HttpContext.Request.Headers["client-id"].ToString();
                if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(clientId))
                {
                    throw new ErrorCodeException(-1, "来源错误");
                }
                if (await userService.ExistOpenID(model.OpenID, 0, Convert.ToInt32(clientId),"mini"))
                {
                    throw new ErrorCodeException(-1, "该微信已注册");
                }
                var companyConfig = await companyConfigService.Get(new Guid(guid));
                if (await userService.Exist(it => it.Phone == model.Phone && it.CompanyID == companyConfig.Company.CompanyID))
                {
                    throw new ErrorCodeException(-1, "该手机号已注册");
                }
                var client = await httpService.GetAsync<Model.Live_Clients>(ServiceNames.ClientsService, "/api/clients/get?id=" + clientId);
                if (client == null)
                {
                    throw new ErrorCodeException(-1, "该小程序未注册");
                }
                var userModel = new Model.Live_User();
                userModel.FullName = model.FullName;
                userModel.Phone = model.Phone;
                userModel.CompanyGID = companyConfig.Company.CompanyGID;
                userModel.CompanyID = companyConfig.Company.CompanyID;
                if (model.BirthDate.HasValue) userModel.BirthDate = model.BirthDate;
                if (!string.IsNullOrEmpty(model.Address)) userModel.Address = model.Address;
                if (!string.IsNullOrEmpty(model.City)) userModel.City = model.City;
                if (!string.IsNullOrEmpty(model.Province)) userModel.Province = model.Province;
                if (!string.IsNullOrEmpty(model.District)) userModel.District = model.District;
                if (!string.IsNullOrEmpty(model.JobCity)) userModel.JobCity = model.JobCity;
                if (!string.IsNullOrEmpty(model.JobProvince)) userModel.JobProvince = model.JobProvince;
                if (!string.IsNullOrEmpty(model.JobDistrict)) userModel.JobDistrict = model.JobDistrict;
                if (model.MinMoney.HasValue) userModel.MinMoney = model.MinMoney;
                if (model.MaxMoney.HasValue) userModel.MaxMoney = model.MaxMoney;
                if (!string.IsNullOrEmpty(model.Education)) userModel.Education = model.Education;
                if (!string.IsNullOrEmpty(model.Sex)) userModel.Sex = model.Sex;
                if (model.Age.HasValue) userModel.Age = model.Age;
                if (!string.IsNullOrEmpty(model.Jobs)) userModel.Jobs = model.Jobs;
                if (!string.IsNullOrEmpty(model.JobNames)) userModel.JobNames = model.JobNames;
                if (!string.IsNullOrEmpty(model.Experience)) userModel.Experience = model.Experience;
                if (!string.IsNullOrEmpty(model.WorkExperience)) userModel.WorkExperience = model.WorkExperience;
                if (!string.IsNullOrEmpty(model.CustomizeField)) userModel.CustomizeField = model.CustomizeField;
                if (!string.IsNullOrEmpty(model.Picture)) userModel.Picture = model.Picture;
                if (!string.IsNullOrEmpty(model.NickName)) userModel.NickName = model.NickName;
                if (!string.IsNullOrEmpty(model.WeiXinOpenId) && !string.IsNullOrEmpty(model.ClientId))
                {
                    userModel.IsBindMP = true;
                }
                userModel.IsCleanroomSuit = model.IsCleanroomSuit;
                userModel.IsCrossRegion = model.IsCrossRegion;
                userModel.IsNightShift = model.IsNightShift;
                userModel.WorkSkill = model.WorkSkill;
                userModel.OtherWorkSkill = model.OtherWorkSkill;
                userModel.UnionId = model.UnionId;
                userModel.UserName = model.NickName;
                userModel.FromType = string.IsNullOrEmpty(model.FromType) ? "xfz" : model.FromType;
                userModel.FUID = model.FUID;
                userModel.CreateTime = DateTime.Now;
                userModel.UserGID = Guid.NewGuid();
                userModel.Integral = 0;
                userModel.Enabled = true;
                userModel.Type = 1;
                userModel.SkillType= model.SkillType;
                userModel.JobSkills = model.JobSkills;
                userModel.SpecialCertificate= model.SpecialCertificate;
                userModel.IsRegPerfect = true;
                userModel.RegFromType = model.RegFromType;
                userModel.UserID=  await userService.Add(userModel);
                publisher.Publish(new UserQrcodeEventData()//生成二维码
                {
                    UserID = userModel.UserID,
                    CompanyID = userModel.CompanyID.Value,
                    CompanyGID = userModel.CompanyGID.Value
                });
                if (companyConfig.Config.IsShopRegisterIntegral)//是否开启注册积分
                {
                    var log = new IntegralLogEventData()
                    {
                        UserID = userModel.UserID,
                        CompanyID = userModel.CompanyID.Value,
                        CompanyGID = userModel.CompanyGID.Value,
                        IntegralType = IntegralType.Register,
                        IsUpdate = true
                    };
                    publisher.Publish(log);
                    if (userModel.FUID.HasValue && userModel.FUID > 0)//推荐人
                    {
                        var flog = new IntegralLogEventData()
                        {
                            UserID = userModel.FUID.Value,
                            CompanyID = userModel.CompanyID.Value,
                            CompanyGID = userModel.CompanyGID.Value,
                            FromUserID = userModel.UserID,
                            IntegralType = IntegralType.Recommend,
                            IsUpdate=true
                        };
                        if (userModel.FromType == "lz" || userModel.FromType == "bannerreg" || userModel.FromType == "sharejob")//来自职喜或者易拉宝的的对接职喜账号
                        {
                            flog.UserID = 0;
                            flog.ToLZUserID = userModel.FUID.Value;
                        }
                        publisher.Publish(flog);
                    }
                }
                var opensModels = new List<Model.Live_UserOpenIds>();
                var opensModel = new Model.Live_UserOpenIds();
                opensModel.OpenId = model.OpenID;
                opensModel.FromID = client.ID;
                opensModel.OpenIdType = "mini";
                opensModel.UserID = userModel.UserID;
                opensModels.Add(opensModel);
                if (userModel.IsBindMP)
                {
                    var opensModelMp = new Model.Live_UserOpenIds();
                    opensModelMp.OpenId = model.WeiXinOpenId;
                    opensModelMp.FromID = Convert.ToInt32(model.ClientId);
                    opensModelMp.OpenIdType = "mp";
                    opensModelMp.UserID = userModel.UserID;
                    opensModels.Add(opensModelMp);
                }
                await userService.AddOpenID(opensModels);
                var token = await authService.GenerateUserTokenAsync(userModel, userModel.UserID, Model.Enums.PlatformType.XFZMini, userModel.CompanyID.Value, userModel.CompanyGID, companyConfig.Company.CutType, fullName: userModel.FullName);

                return Ok(new { user = new { name = userModel.FullName, picture = userModel.Picture, phone = userModel.Phone, userid = userModel.UserID, nickname = userModel.NickName, cardStatus = userModel.UploadCardStatus, isReal = userModel.IsReal, status = userModel.Status, isDayOver = userModel.IsDayOver, usergid = userModel.UserGID, openid = model.OpenID, isBindMP = userModel.IsBindMP,qrcode=userModel.Qrcode, integral = userModel.Integral, isRegPerfect=userModel.IsRegPerfect }, token = token.Token });
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PerfectRegister([FromBody] Model.Live_User model)
        {
            if (model == null)
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone))
            {
                throw new ErrorCodeException(-1, "姓名和手机号不能为空！");
            }
            var user = CurrentUser;
            try
            {
                if (await userService.Exist(it => it.Phone == model.Phone && it.CompanyID == user.CompanyID && it.UserID!=user.UserID))
                {
                    throw new ErrorCodeException(-1, "该手机号已注册");
                }
                var userModel = await userService.Get(user.UserID);
                userModel.FullName = model.FullName;
                var isReg = userModel.IsRegPerfect;
                if (model.BirthDate.HasValue) userModel.BirthDate = model.BirthDate;
                if (!string.IsNullOrEmpty(model.Address)) userModel.Address = model.Address;
                if (!string.IsNullOrEmpty(model.City)) userModel.City = model.City;
                if (!string.IsNullOrEmpty(model.Province)) userModel.Province = model.Province;
                if (!string.IsNullOrEmpty(model.District)) userModel.District = model.District;
                if (!string.IsNullOrEmpty(model.JobCity)) userModel.JobCity = model.JobCity;
                if (!string.IsNullOrEmpty(model.JobProvince)) userModel.JobProvince = model.JobProvince;
                if (!string.IsNullOrEmpty(model.JobDistrict)) userModel.JobDistrict = model.JobDistrict;
                if (model.MinMoney.HasValue) userModel.MinMoney = model.MinMoney;
                if (model.MaxMoney.HasValue) userModel.MaxMoney = model.MaxMoney;
                if (!string.IsNullOrEmpty(model.Education)) userModel.Education = model.Education;
                if (!string.IsNullOrEmpty(model.Sex)) userModel.Sex = model.Sex;
                if (model.Age.HasValue) userModel.Age = model.Age;
                if (!string.IsNullOrEmpty(model.Jobs)) userModel.Jobs = model.Jobs;
                if (!string.IsNullOrEmpty(model.JobNames)) userModel.JobNames = model.JobNames;
                if (!string.IsNullOrEmpty(model.Experience)) userModel.Experience = model.Experience;
                if (!string.IsNullOrEmpty(model.WorkExperience)) userModel.WorkExperience = model.WorkExperience;
                if (!string.IsNullOrEmpty(model.CustomizeField)) userModel.CustomizeField = model.CustomizeField;
                if (!string.IsNullOrEmpty(model.Picture)) userModel.Picture = model.Picture;
                if (!string.IsNullOrEmpty(model.NickName)) userModel.NickName = model.NickName;
                if (!string.IsNullOrEmpty(model.FromType)) userModel.FromType = model.FromType;
                if (!string.IsNullOrEmpty(model.NickName)) userModel.UserName = model.NickName;
                userModel.SkillType = model.SkillType;
                userModel.JobSkills = model.JobSkills;
                userModel.SpecialCertificate = model.SpecialCertificate;
                userModel.IsRegPerfect = true;
                userModel.RegFromType = model.RegFromType;
                await userService.Update(userModel);
                var companyConfig = await companyConfigService.Get(user.CompanyGID);
                if (companyConfig.Config.IsShopRegisterIntegral && !isReg)//是否开启注册积分
                {
                    var log = new IntegralLogEventData()
                    {
                        UserID = userModel.UserID,
                        CompanyID = userModel.CompanyID.Value,
                        CompanyGID = userModel.CompanyGID.Value,
                        IntegralType = IntegralType.Register,
                        IsUpdate = true
                    };
                    publisher.Publish(log);
                    if (userModel.FUID.HasValue && userModel.FUID > 0)//推荐人
                    {
                        var flog = new IntegralLogEventData()
                        {
                            UserID = userModel.FUID.Value,
                            CompanyID = userModel.CompanyID.Value,
                            CompanyGID = userModel.CompanyGID.Value,
                            FromUserID = userModel.UserID,
                            IntegralType = IntegralType.Recommend,
                            IsUpdate = true
                        };
                        if (userModel.FromType == "lz" || userModel.FromType == "bannerreg" || userModel.FromType == "sharejob")//来自职喜或者易拉宝的的对接职喜账号
                        {
                            flog.UserID = 0;
                            flog.ToLZUserID = userModel.FUID.Value;
                        }
                        publisher.Publish(flog);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        [HttpGet]
        public async Task<int> Integral(int userid)
        {
            var model = await userService.Get(it=>it.UserID== userid);
            if (model == null || !model.Integral.HasValue || model.Integral<0)
            {
                return 0;
            }
            else
            {
                return model.Integral.Value;
            }
        }
        [HttpPut]
        public async Task<IActionResult> SaveQrcode([FromForm] int id, [FromForm] string qrcode)
        {
            if (string.IsNullOrEmpty(qrcode) || id == 0)
            {
                throw new ErrorCodeException(-1, "请先上传二维码");
            }
            await userService.SaveQrcode(id, qrcode);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> SaveAvatar([FromForm] string avatar, [FromForm] string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(avatar))
            {
                throw new ErrorCodeException(-1, "请先上传头像或者昵称");
            }
            await userService.SaveAvatar(CurrentUser.UserID, avatar, name);
            return Ok();
        }

        /// <summary>
        /// 上报小程序实时位置（进入小程序时调用，覆盖上次位置）：
        /// 更新 MiniProvince/MiniCity/MiniDistrict/MiniAddress + 经纬度坐标，
        /// 供人岗匹配距离分档（推荐/派单）与画像"最近登录地"展示。
        /// 参数由小程序端定位授权后解析传入。
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> SaveLocation([FromBody] Model.Dtos.UserLocationDto model)
        {
            var user = CurrentUser;
            if (model == null)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            if (string.IsNullOrEmpty(model.MiniProvince) && string.IsNullOrEmpty(model.MiniCity)
                && string.IsNullOrEmpty(model.MiniDistrict) && string.IsNullOrEmpty(model.MiniAddress)
                && !model.Latitude.HasValue && !model.Longitude.HasValue)
            {
                throw new ErrorCodeException(-1, "位置信息不能为空");
            }
            if (model.Latitude.HasValue && (model.Latitude.Value < -90 || model.Latitude.Value > 90))
            {
                throw new ErrorCodeException(-1, "纬度不正确");
            }
            if (model.Longitude.HasValue && (model.Longitude.Value < -180 || model.Longitude.Value > 180))
            {
                throw new ErrorCodeException(-1, "经度不正确");
            }

            var userModel = await userService.Get(user.UserID);
            if (userModel == null)
            {
                throw new ErrorCodeException(-1, "用户不存在");
            }

            if (!string.IsNullOrEmpty(model.MiniProvince)) userModel.MiniProvince = model.MiniProvince;
            if (!string.IsNullOrEmpty(model.MiniCity)) userModel.MiniCity = model.MiniCity;
            if (!string.IsNullOrEmpty(model.MiniDistrict)) userModel.MiniDistrict = model.MiniDistrict;
            if (!string.IsNullOrEmpty(model.MiniAddress)) userModel.MiniAddress = model.MiniAddress;
            if (model.Latitude.HasValue) userModel.Latitude = model.Latitude.Value;
            if (model.Longitude.HasValue) userModel.Longitude = model.Longitude.Value;

            await userService.Update(userModel);
            //await redisService.SetAsync(string.Format(CoreRedisConstants.UserInfo, user.Token), userModel, TimeSpan.FromHours(10));
            return Ok(new { success = true });
        }
        [HttpPut]
        public async Task<RequestResultModel> SaveInfo([FromBody] Model.UserModel model)
        {
            var user = CurrentUser;
            if (model.User == null || model.User.UserID != user.UserID)
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            var userModel = await userService.Get(user.UserID);
            if (string.IsNullOrEmpty(model.User.FullName) || string.IsNullOrEmpty(model.User.Phone))
            {
                throw new ErrorCodeException(-1, "姓名和手机号不能为空！");
            }
            try
            {
                if(userModel.IsReal && userModel.FullName!= model.User.FullName)
                {
                    userModel.IsReal=false;
                }
                userModel.FullName = model.User.FullName;
                userModel.Phone = model.User.Phone;
                if (model.User.BirthDate.HasValue) userModel.BirthDate = model.User.BirthDate;
                if (!string.IsNullOrEmpty(model.User.Address)) userModel.Address = model.User.Address;
                if (!string.IsNullOrEmpty(model.User.City)) userModel.City = model.User.City;
                if (!string.IsNullOrEmpty(model.User.Province)) userModel.Province = model.User.Province;
                if (!string.IsNullOrEmpty(model.User.District)) userModel.District = model.User.District;
                if (model.User.MinMoney.HasValue) userModel.MinMoney = model.User.MinMoney;
                if (model.User.MaxMoney.HasValue) userModel.MaxMoney = model.User.MaxMoney;
                if (!string.IsNullOrEmpty(model.User.Education)) userModel.Education = model.User.Education;
                if (!string.IsNullOrEmpty(model.User.Sex)) userModel.Sex = model.User.Sex;
                if (model.User.Age.HasValue) userModel.Age = model.User.Age;
                if (!string.IsNullOrEmpty(model.User.Jobs)) userModel.Jobs = model.User.Jobs;
                if (!string.IsNullOrEmpty(model.User.JobNames)) userModel.JobNames = model.User.JobNames;
                if (!string.IsNullOrEmpty(model.User.Experience)) userModel.Experience = model.User.Experience;
                if (!string.IsNullOrEmpty(model.User.WorkExperience)) userModel.WorkExperience = model.User.WorkExperience;
                userModel.SkillType = model.User.SkillType;
                userModel.JobSkills = model.User.JobSkills;
                userModel.SpecialCertificate = model.User.SpecialCertificate;
                if (!string.IsNullOrEmpty(model.User.CustomizeField)) userModel.CustomizeField = model.User.CustomizeField;
                userModel.UpTime = DateTime.Now;
                RequestResultModel? res = null;
                if (model.Paras != null && model.Paras.QrcodeGID.HasValue)
                {
                    userModel.QrcodeGID = model.Paras.QrcodeGID;
                    res = await ScanCode(userModel, model.Paras);
                }
                await userService.Update(userModel);
                await redisService.SetAsync(string.Format(CoreRedisConstants.UserInfo, user.Token), userModel, TimeSpan.FromHours(10));
                if (res != null)
                {
                    if (res.Code == -1)
                    {
                        throw new ErrorCodeException(-1, res.Message);
                    }
                    else if (res.Code == 200)
                    {
                        var status = (int)res.Data;
                        if (status==(int)UserStatus.Entry)
                        {
                            res.Message = "恭喜！您已入职成功！";
                        }
                        else
                        {
                            if (userModel.IsReal)
                            {
                                res.Message = "您已实名，请联系招聘人员，操作入职！";
                            }
                            else
                            {
                                if(status == (int)UserStatus.WaitEntry)
                                {
                                    res.Code = 201;
                                    res.Message = "您还未实名，请前往填写入职信息";
                                }
                                else
                                {
                                    res.Message = "提交成功，请前往面试！";
                                }
                            }
                        }
                    }
                }
                else
                {
                    res = new RequestResultModel();
                    res.Code = 200;
                    res.Message = "提交成功！";
                    var record = new Live_UserEntryRecord();
                    record.UserID = user.UserID;
                    record.CreateTime = DateTime.Now;
                    record.About = "更新了简历信息";
                    record.Status = userModel.Status;
                    SendEntryRecord(record, userModel.FullName);
                }
                return res;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        private async Task<RequestResultModel> ScanCode(Model.Live_User model,Model.UserParas paras)
        {
            var user = CurrentUser;
            var qrcode = await personnelItemsService.Get(paras.QrcodeGID.Value);
            var record = new Live_UserEntryRecord();
            record.UserID = user.UserID;
            record.CreateTime= DateTime.Now;
            record.Status = (int)UserStatus.Untreated;
            if (qrcode == null)
            {
                record.About = "二维码不存在！";
                SendEntryRecord(record, model.FullName);
                return new RequestResultModel(-1, "该二维码不存在！");
            }
            record.PerUserID = qrcode.UserID;
            record.CID = qrcode.CID;
            if (!qrcode.Enabled)
            {
                record.About = "扫了"+qrcode.FullName + "的二维码，但该二维码已被禁用！";
                SendEntryRecord(record, model.FullName);
                return new RequestResultModel(-1, "该二维码已被禁用！");
            }
            var company = await companyConfigService.Get((Guid)user.CompanyGID);
            var companyConfig = company.Config;
            if (companyConfig == null)
            {
                record.About = "扫了" + qrcode.FullName + "的二维码，但公司信息未配置！";
                SendEntryRecord(record, model.FullName);
                return new RequestResultModel(-1, "公司信息未配置！");
            }
            var entryStatus = companyConfig.UserIsToWaitEntry ? (int)UserStatus.WaitEntry : (int)UserStatus.WaitInterview;
            var contractStatus = 0;
            if (qrcode.EntryModel.HasValue && qrcode.EntryModel != (int)EntryModel.Normal)
            {
                //直接入职
                if(qrcode.EntryModel == (int)EntryModel.DirectEntry)
                {
                    if (model.IsReal)
                    {
                        contractStatus = (int)ContractStatus.EntryWaitCreate;
                        entryStatus = (int)UserStatus.Entry;
                    }
                    else
                    {
                        entryStatus = (int)UserStatus.WaitEntry;
                    }
                }
                else
                {
                    entryStatus = (int)UserStatus.WaitEntry;
                }
            }
            //在职时是否能直接跳过
            if (model.Status == (int)UserStatus.Entry && !companyConfig.UserIsEntryOther)
            {
                record.About = "扫了" + qrcode.FullName + "的二维码，已在职！";
                SendEntryRecord(record, model.FullName);
                return new RequestResultModel(-1, "保存成功，您已在职，无需再次入职！");
            }
            if (!string.IsNullOrEmpty(model.CardID) && !companyConfig.UserIsEntryOther)
            {
                if(await entryService.Exist(it => it.UserID == user.UserID && it.Status == (int)UserStatus.Entry))
                {
                    record.About = "扫了" + qrcode.FullName + "的二维码，该身份证已入职！";
                    SendEntryRecord(record, model.FullName);
                    return new RequestResultModel(-1, "该身份证号已入职，请招聘人员处理！");
                }
            }
            var status = new int[] { (int)UserStatus.WaitInterview, (int)UserStatus.WaitEntry };
            var exist = await entryService.Exist(it => it.UserID == user.UserID && status.Contains(it.Status.Value));
            if (!companyConfig.UserIsEntryCover && exist)
            {
                record.About = "扫了" + qrcode.FullName + "的二维码，已存在面试记录！";
                SendEntryRecord(record, model.FullName);
                return new RequestResultModel(-1, "已存在面试记录，请招聘人员处理！");
            }
            var statusO = new int[] { (int)UserStatus.WaitInterview, (int)UserStatus.WaitEntry, (int)UserStatus.Entry };
            var cuExist = await entryService.Exist(it => it.UserID == user.UserID && it.CustomerGID==qrcode.CustomerGID && statusO.Contains(it.Status.Value));
            if (cuExist)
            {
                record.About = "扫了" + qrcode.FullName + "的二维码，已存在同单位面试记录！";
                SendEntryRecord(record, model.FullName);
                return new RequestResultModel(-1, "您已在该单位存在待入职或在职记录，请联系招聘人员处理！");
            }
            //是否存在劳务经纪人推荐的
            //var mexpable = Expressionable.Create<Live_UserEntry>();
            //mexpable.And(it => it.UserID == user.UserID && it.Status==(int)UserStatus.WaitScanCode && it.CompanyID == user.CompanyID);
            //var mexp = mexpable.ToExpression();
            //var oldEntryModel = await entryService.Get(mexp);
            //if (oldEntryModel != null)
            //{
            //    oldEntryModel.Name = model.FullName;
            //    oldEntryModel.Sex = model.Sex;
            //    oldEntryModel.Age = model.Age;
            //    oldEntryModel.Status = entryStatus;
            //    oldEntryModel.PerUserID = model.PerUserID;
            //    oldEntryModel.PartnerID = qrcode.PartnerID;
            //    oldEntryModel.PartnerGID = qrcode.PartnerGID;
            //    oldEntryModel.QrcodeGID = paras.QrcodeGID;
            //    oldEntryModel.ContractStatus = contractStatus;
            //    oldEntryModel.Phone = model.Phone;
            //    oldEntryModel.Name = model.FullName;
            //    oldEntryModel.Sex = model.Sex;
            //    oldEntryModel.Age = model.Age;
            //    //招聘模式，服务商与客户的服务商不一致标记为联营
            //    oldEntryModel.OperateModel = qrcode.PartnerID != qrcode.CustomerPartnerID ? qrcode.OperateModel : (int)Model.Enums.OperateModel.Routine;
            //    if (oldEntryModel.OperateModel != (int)Model.Enums.OperateModel.Routine)
            //    {
            //        oldEntryModel.JointPartnerID = qrcode.JointPartnerID;
            //        oldEntryModel.JointPartnerGID = qrcode.JointPartnerGID;
            //    }
            //    if (entryStatus == (int)UserStatus.Entry)
            //    {
            //        oldEntryModel.EntryTime = DateTime.Today;
            //        oldEntryModel.ContractDate = DateTime.Today;
            //        if (!string.IsNullOrEmpty(qrcode.EntryFromType))
            //        {
            //            oldEntryModel.FromType = qrcode.EntryFromType;
            //        }
            //        if (!string.IsNullOrEmpty(qrcode.EntryWorkClass))
            //        {
            //            oldEntryModel.WorkClass = qrcode.EntryWorkClass;
            //        }
            //        if (!string.IsNullOrEmpty(qrcode.EntryWorkName))
            //        {
            //            oldEntryModel.WorkName = qrcode.EntryWorkName;
            //        }
            //       var isEntry = await entryService.UserEntryAffairs(oldEntryModel, true);
            //        if (isEntry)
            //        {
            //            var contact = new UserEntryContractEventData(oldEntryModel.ID, ContractEnumType.入职合同, Guid.NewGuid());
            //            publisher.Publish(contact);
            //        }
            //    }
            //    else
            //    {
            //        if(model.Status != (int)UserStatus.Entry)
            //        {
            //            model.Status = entryStatus;
            //        }
            //        await entryService.Update(oldEntryModel);
            //    }
            //    record.EntryID = oldEntryModel.ID;
            //    record.Status = oldEntryModel.Status;
            //    record.About = "扫了" + qrcode.CustomerName + "的二维码，修改了劳务经纪人推荐的人员信息！";
            //    SendEntryRecord(record);
            //    return new RequestResultModel(200, "", entryStatus);
            //}
            var entryModel = new Live_UserEntry();
            entryModel.UserID = user.UserID;
            entryModel.CustomerID = qrcode.CID;
            entryModel.CustomerGID= qrcode.CustomerGID;
            entryModel.PartnerID = qrcode.PartnerID;
            entryModel.PartnerGID = qrcode.PartnerGID;
            entryModel.CreateTime=DateTime.Now;
            entryModel.PerUserID = qrcode.UserID;
            entryModel.IsUser = true;
            entryModel.Status = entryStatus;
            entryModel.QrcodeGID = paras.QrcodeGID;
            entryModel.CompanyID = user.CompanyID;
            entryModel.CompanyGID = user.CompanyGID;
            entryModel.ContractStatus = contractStatus;
            entryModel.Phone = model.Phone;
            entryModel.Name = model.FullName;
            entryModel.Sex = model.Sex;
            entryModel.Age = model.Age;
            if (model.IsReal)
            {
                entryModel.IdcardFrontImage = model.IdcardFrontImage;
                entryModel.IdcardBackImage = model.IdcardBackImage;
                entryModel.BankImage = model.BankImage;
                entryModel.IdcardAddress = model.IdcardAddress;
                entryModel.Nation = model.Nation;
                entryModel.Authority = model.Authority;
                entryModel.CardStartDate = model.CardStartDate;
                entryModel.CardEndDate = model.CardEndDate;
                entryModel.BankNumber = model.BankNumber;
                entryModel.BankName = model.BankName;
                entryModel.BankID = model.BankID;
                entryModel.BankCardName = model.BankCardName;
                entryModel.BankCardType = model.BankCardType;
                entryModel.CardID = model.CardID;
                entryModel.BankProvince = model.BankProvince;
                entryModel.BankCity = model.BankCity;
            }
            entryModel.MaritalStatus = model.MaritalStatus;
            entryModel.MajorDiseases = model.MajorDiseases;
            entryModel.EmergencyContact = model.EmergencyContact;
            entryModel.EmergencyContactPhone = model.EmergencyContactPhone;
            entryModel.IdPhoto = model.IdPhoto;
            entryModel.IsBlack = model.IsBlack;
            entryModel.CustomizeField = model.EntryCustomizeField;
            entryModel.IsReal = model.IsReal;
            entryModel.IntDate = DateTime.Now;
            entryModel.BusinessModel = qrcode.BusinessModel;
            entryModel.SkillType = model.SkillType;
            entryModel.JobSkills = model.JobSkills;
            entryModel.SpecialCertificate=model.SpecialCertificate;
            //招聘模式
            if (qrcode.OperateModel == null) qrcode.OperateModel = (int)Model.Enums.OperateModel.Routine;
            if (qrcode.OperateModel == (int)Model.Enums.OperateModel.Joint && qrcode.PartnerID != qrcode.CustomerPartnerID)//联营招聘
            {
                entryModel.OperateModel = (int)Model.Enums.OperateModel.Joint;
                entryModel.JointPartnerID = qrcode.JointPartnerID;
                entryModel.JointPartnerGID = qrcode.JointPartnerGID;
            }
            else
            {
                entryModel.OperateModel = qrcode.OperateModel;
            }
           if(qrcode.EntryModel == (int)EntryModel.DirectEntry || qrcode.EntryModel == (int)EntryModel.AuditDirectEntry)
            {
                if (!string.IsNullOrEmpty(qrcode.EntryFromType))
                {
                    entryModel.FromType = qrcode.EntryFromType;
                }
                if (!string.IsNullOrEmpty(qrcode.EntryWorkClass))
                {
                    entryModel.WorkClass = qrcode.EntryWorkClass;
                }
                if (!string.IsNullOrEmpty(qrcode.EntryWorkName))
                {
                    entryModel.WorkName = qrcode.EntryWorkName;
                }
            }
            if (entryStatus == (int)UserStatus.Entry)//直接入职
            {
                entryModel.EntryTime= DateTime.Today;
                entryModel.ContractDate = DateTime.Today;
                EntryCommon.NoEntryTime(companyConfig);//判断是否在入职时间段
                await entryService.UserEntryAffairs(entryModel, companyConfig.UserIsEntryOther,user);
            }
            else
            {
                entryModel.ID = await entryService.Add(entryModel);
            }
            if (qrcode.EntryModel == (int)EntryModel.DirectEntry && !model.IsReal)
            {
                model.EntryID = entryModel.ID;//直接入职的记录入职记录id方便实名后操作
            }
           
            model.Status = entryStatus;
            //将其他记录作废
            if (companyConfig.UserIsEntryCover && exist)
            {
                await entryService.UpdateStatus(UserStatus.Cancel, it => it.UserID == user.UserID && status.Contains(it.Status.Value) && it.ID!= entryModel.ID);
            }
            record.About = "扫了" + qrcode.FullName + "的二维码，新增了入职信息！";
            record.EntryID = entryModel.ID;
            record.Status = entryStatus;
            SendEntryRecord(record,model.FullName);
            if (string.IsNullOrEmpty(model.BankProvince))//获取银行卡所属地
            {
                publisher.Publish(new UserBankEventData()
                {
                    UserID = user.UserID,
                    BankNumber = model.BankNumber
                });
            }
            if (companyConfig.EnableDockJDY)
            {
                await jdySyncService.UserSync(entryModel.ID);//同步简道云
            }
            return new RequestResultModel(200, "", entryStatus);
        }
        /// <summary>
        /// 简道云查询工资
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpGet]
        public async Task<IActionResult> GetJDYWage()
        {
            var user = CurrentUser;
            var userInfo = await authService.UserInfo<Model.Live_User>();
            if (string.IsNullOrEmpty(userInfo.CardID))
            {
                throw new ErrorCodeException(-1, "身份证为空，不可查询！");
            }
            var company = await companyConfigService.Get(user.CompanyGID);
            var res = await jdySyncService.GetWageInfo(userInfo.CardID, company);
            return Ok(res);
        }
        private void SendEntryRecord(Model.Live_UserEntryRecord model,string name="")
        {
            try
            {
                var user = CurrentUser;
                publisher.Publish(new EntryRecordEventData()
                {
                    UserID = model.UserID.HasValue ? model.UserID.Value : 0,
                    EntryID = model.EntryID.HasValue ? model.EntryID.Value : 0,
                    PerUserID = model.PerUserID.HasValue ? model.PerUserID.Value : 0,
                    About = model.About,
                    CID = model.CID.HasValue ? model.CID.Value : 0,
                    Status = model.Status.HasValue ? model.Status.Value : 0,
                    OperateFrom = user.PlatformType,
                    OperateUserID = user.UserID,
                    OperateFullName = !string.IsNullOrEmpty(name)?name:user.FullName
                });
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
        }
        
        [HttpPut]
        public async Task<IActionResult> ChangeBank([FromBody] Model.Live_User model)
        {
            var user = CurrentUser;
            if (model == null || model.UserID != user.UserID)
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            var userModel = await userService.Get(user.UserID);
            userModel.BankNumber = model.BankNumber;
            userModel.BankName = model.BankName;
            userModel.BankID = model.BankID;
            userModel.BankCardName = model.BankCardName;
            userModel.BankCardType = model.BankCardType;
            userModel.BankImage = model.BankImage;
            if (!string.IsNullOrEmpty(model.BankProvince))
            {
                userModel.BankCity = model.BankCity;
                userModel.BankProvince = model.BankProvince;
            }
            await userService.Update(userModel);
            await entryService.UpdateBank(userModel);
            var record = new Live_UserEntryRecord();
            record.About = "小程序修改银行卡信息！修改为" + userModel.BankNumber + ";" + userModel.BankName;
            record.UserID = user.UserID;
            record.CreateTime = DateTime.Now;
            if (string.IsNullOrEmpty(model.BankProvince))
            {
                publisher.Publish(new UserBankEventData()
                {
                    UserID = user.UserID,
                    BankNumber = model.BankNumber
                });
            }
            SendEntryRecord(record);
            return Ok();
        }
        [HttpPut]
        public async Task<RequestResultModel> SaveEntryInfo([FromBody] Model.UserModel fromModel)
        {
            var user = CurrentUser;
            var model = fromModel.User;
            if (model == null || model.UserID != user.UserID)
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            var userModel = await userService.Get(user.UserID);
            if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Phone))
            {
                throw new ErrorCodeException(-1, "姓名和手机号不能为空！");
            }
            if (string.IsNullOrEmpty(model.CardID))
            {
                throw new ErrorCodeException(-1, "身份证不能为空！");
            }
            try
            {
                if (userModel.Status != (int)UserStatus.Entry || string.IsNullOrEmpty(userModel.FullName))
                {
                    userModel.FullName = model.FullName;
                }
                userModel.Sex=model.Sex;
                userModel.Age = model.Age;
                userModel.BirthDate = model.BirthDate;
                userModel.IdcardFrontImage = model.IdcardFrontImage;
                userModel.IdcardBackImage = model.IdcardBackImage;
                userModel.BankImage = model.BankImage;
                userModel.IdcardAddress = model.IdcardAddress;
                userModel.Nation = model.Nation;
                userModel.Authority = model.Authority;
                userModel.CardStartDate = model.CardStartDate;
                userModel.CardEndDate = model.CardEndDate;
                userModel.BankNumber = model.BankNumber;
                userModel.BankName = model.BankName;
                userModel.BankID = model.BankID;
                userModel.BankCardName = model.BankCardName;
                userModel.BankCardType = model.BankCardType;
                userModel.MaritalStatus = model.MaritalStatus;
                userModel.MajorDiseases = model.MajorDiseases;
                userModel.EmergencyContact = model.EmergencyContact;
                userModel.EmergencyContactPhone = model.EmergencyContactPhone;
                userModel.CardID = model.CardID;
                userModel.UploadCardStatus = model.UploadCardStatus;
                userModel.IdPhoto= model.IdPhoto;
                userModel.EntryCustomizeField = model.EntryCustomizeField;
                userModel.UpTime = DateTime.Now;
                userModel.BankProvince = model.BankProvince;
                userModel.BankCity = model.BankCity;
                RequestResultModel? res = null;
                if (userModel.Status != (int)UserStatus.Entry)
                {

                    //黑名单
                    var company = await companyConfigService.Get(user.CompanyGID);
                    var expable = Expressionable.Create<Model.Live_BlackList>();
                    expable.And(it => it.IdentityCard == model.CardID);
                    if (company.Config != null)
                    {
                        if (!company.Config.UserIsUsePublicBlcak)
                        {
                            expable.And(it => it.CompanyID == user.CompanyID);
                        }
                        if (!company.Config.UserIsInputResume)//如果不用填写简介就覆盖会员信息里面的数据
                        {
                            userModel.City = model.City;
                            userModel.Province = model.Province;
                            userModel.District = model.District;
                            userModel.Address = model.Address;
                            userModel.Education = model.Education;
                        }
                    }
                    var exp = expable.ToExpression();
                    userModel.IsBlack = await blackListService.Exist(exp);
                    //实名认证
                    userModel.IsReal = await RealAuth(company.Config.RealAuthModel, model);
                    userModel.IsCaAuth = await CaAuth(model.CardID);
                    //根据身份证匹配线下在职记录

                    if (await entryService.SetUser(userModel, user.CompanyID))
                    {
                        await userService.Update(userModel);
                        await redisService.SetAsync(string.Format(CoreRedisConstants.UserInfo, user.Token), userModel, TimeSpan.FromHours(10));
                        res = new RequestResultModel();
                        res.Code = 200;
                        res.Message = "实名认证成功，您已关联线下入职记录！";
                        return res;
                    }

                    if (fromModel.Paras != null && fromModel.Paras.QrcodeGID.HasValue)
                    {
                        if (!await entryService.Exist(it => it.UserID == user.UserID && it.QrcodeGID == fromModel.Paras.QrcodeGID && it.Status == (int)UserStatus.WaitEntry))//防止小程序二维码guid未被清除,后期删除判断
                        {
                            userModel.QrcodeGID = fromModel.Paras.QrcodeGID;
                            res = await ScanCode(userModel, fromModel.Paras);
                        }
                    }
                }
                await userService.Update(userModel);
                await redisService.SetAsync(string.Format(CoreRedisConstants.UserInfo, user.Token), userModel, TimeSpan.FromHours(10));
                if (res != null)
                {
                    if (res.Code == -1)
                    {
                        throw new ErrorCodeException(-1, res.Message);
                    }
                    else if (res.Code == 200)
                    {
                        var status = (int)res.Data;
                        if (status == (int)UserStatus.Entry)
                        {
                            res.Message = "恭喜！您已入职成功！";
                        }
                        else
                        {
                            res.Message = "您已实名，请联系招聘人员，操作入职！";
                        }
                        res.Data = new { isToAuth = true };
                    }
                }
                else
                {
                    var isDirectEntry = false;
                    await entryService.UpdateEntryInfo(userModel);
                    //直接入职操作
                    if (userModel.EntryID.HasValue && userModel.EntryID>0 && userModel.Status== (int)UserStatus.WaitEntry)
                    {
                        var entry = await entryService.View(it => it.ID == userModel.EntryID);
                        if (entry != null && entry.Status==(int)UserStatus.WaitEntry && entry.EntryModel==(int)EntryModel.DirectEntry)
                        {
                            var entryModel = await entryService.Get(it => it.ID == userModel.EntryID);
                            entryModel.Status = (int)UserStatus.Entry;
                            entryModel.EntryTime = DateTime.Today;
                            entryModel.ContractDate = DateTime.Today;
                            isDirectEntry = await entryService.UserEntryAffairs(entryModel, true,user) > 0;
                        }
                    }
                    res = new RequestResultModel();
                    res.Code = 200;
                    var record = new Live_UserEntryRecord();
                    if(userModel.Status == (int)UserStatus.Entry)//入职后修改入职信息
                    {
                        res.Message = "入职信息修改成功！";
                        record.About = "修改入职信息";
                    }
                    else
                    {
                        if (isDirectEntry)
                        {
                            res.Message = "实名认证成功，您已入职成功！";
                            record.About = "实名认证，直接入职";
                        }
                        else
                        {
                            res.Message = "实名认证成功，请联系招聘人员，操作入职！";
                            record.About = "实名认证";
                        }
                        res.Data = new { isToAuth = true };
                    }
                    
                    record.UserID = user.UserID;
                    record.CreateTime = DateTime.Now;
                    if (string.IsNullOrEmpty(model.BankProvince) && string.IsNullOrEmpty(model.BankProvince))//获取银行卡所属地
                    {
                        publisher.Publish(new UserBankEventData()
                        {
                            UserID = user.UserID,
                            BankNumber = model.BankNumber
                        });
                    }
                    SendEntryRecord(record, model.FullName);
                }
                
                return res;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                throw new ErrorCodeException(-1, ex.Message);
            }
        }
        private async Task<bool> RealAuth(RealAuthModel type, Model.Live_User model)
        {
            var url = "";
            switch (type)
            {
                case RealAuthModel.Two:
                    url = "/api/fxqapi/realnameperson2";
                    break;
                case RealAuthModel.Three:
                    url = "/api/fxqapi/realnameperson3";
                    break;
                case RealAuthModel.BankThree:
                    url = "/api/fxqapi/realnamepersonbank3";
                    break;
                case RealAuthModel.Four:
                    url = "/api/fxqapi/realnameperson4";
                    break;
            }
            if (url != "")
            {
                var paras=new Dictionary<string, string>();
                paras.Add("name", model.FullName);
                paras.Add("idCardNo", model.CardID);
                paras.Add("phone", model.Phone);
                paras.Add("bankNo", model.BankNumber);
                var p = System.Text.Json.JsonSerializer.Serialize(paras);
                var res =  await httpService.PostAsync<Model.Live_ContractSigner>(ServiceNames.CustomerService, url, p);
                if (res != null && res.Code==200)
                {
                    var signer = res.Data;
                    if (!signer.IsRealNameSuc)
                    {
                        throw new ErrorCodeException(-1, signer.Remark);
                    }
                    return true;
                }
                else
                {
                    Common.StaticLog.Debug(System.Text.Json.JsonSerializer.Serialize(res));
                    throw new ErrorCodeException(-1, "认证错误！");
                }
            }
            return false;
        }


        private async Task<bool> CaAuth(string cardID)
        {
            try
            {
                var caAuthResp = await httpService.GetAsync<CaAuthCardIdDto>(ServiceNames.CustomerService, $"/api/CaAuthIdentity/GetCaAuthStatus?cardID={cardID}");
                if (caAuthResp != null)
                {
                    return caAuthResp.IsCaAuth;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Debug($"获取Ca认证信息失败，Message:{ex.Message}");
                return false;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Sign()
        {
            var user = await authService.UserInfo<Model.Live_User>();
            if(user.SignDate.HasValue && user.SignDate.Value.Date == DateTime.Now.Date)
            {
                throw new ErrorCodeException(-1, "您已签到！");
            }
            await userService.SetSignDate(user.UserID);
            user.SignDate = DateTime.Now.Date;
            await authService.UpdateUserInfo<Model.Live_User>(user, user.UserID, PlatformType.XFZMini);
            var record = new IntegralLogEventData();
            record.UserID = user.UserID;
            record.Integral = 1;
            record.IntegralType = IntegralType.Sign;
            record.IsUpdate = true;
            record.CompanyGID = user.CompanyGID.Value;
            record.CompanyID = user.CompanyID.Value;
            publisher.Publish(record);
            return Ok();
        }
        [HttpGet]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<Model.Live_UserOpenIds> GetOpenID(int id, int clientId)
        {
            return await userService.GetOpenID(id, clientId);
        }
        [HttpGet]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<bool> GetOpenIDExist(string openId, int clientId)
        {
            return await userService.ExistOpenId(it => it.OpenId == openId && it.FromID == clientId);
        }
        [HttpGet]
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        public async Task<Model.Live_User> UserInfo(int id)
        {
            var user = CurrentUser;
            int companyid = user.CompanyID; ;
            if (user.CompanyType == Model.Enums.CompanyType.Self.ToString().ToLower())
            {
                companyid = 0;
            }
            return await userService.Get(id, companyid);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_User model)
        {
            if (!await userService.Exist(model.UserID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await userService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> SetUserWrite([FromForm] Guid id, [FromForm] int type)
        {
            const int allowedRoles = (int)(UserTypeEnum.WriteUser | UserTypeEnum.PromoterUser);
            var shopId = (type & allowedRoles) != 0 ? CurrentUser.ShopID : 0;
            await userService.SetUserWrite(id, type, shopId);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> SetShopPay([FromForm] int id, [FromForm] bool has)
        {
            await userService.SetShopPay(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await userService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await userService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await userService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await userService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var user = CurrentUser;
            var model = await userService.Get(id, user.CompanyID);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            if(await entryService.Exist(it => it.UserID == id))
            {
                throw new ErrorCodeException(-1, "已存在入职记录不可删除该人员");
            }
            await userService.Delete(id, user.CompanyID);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ChangePicture([FromForm] IFormFile file)
        {
            if (file!=null)
            {
                var res =await aliyunOSSService.UploadImage(file);
                if (res.Code == 0)
                {
                    //return Ok(res.Message);
                    var count = await userService.ChangePicture(CurrentUser.UserID, res.Data.ToString());
                    if (count == 0)
                    {
                        throw new ErrorCodeException(-1, "修改错误");
                    }
                    return Ok(res.Data.ToString());
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
        /// 小程序H5 ca认证获取基础信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GeCaAuthInfoByUserID(Guid userGid)
        {
            var model = await userService.Get(f => f.UserGID == userGid);
            var appInfo = await userService.GetUserFxqApp(f => f.UserGID == userGid);
            string appName = "默认应用";
            if (appInfo!=null&&!string.IsNullOrWhiteSpace(appInfo.FxqAppName))
            {
                appName = appInfo.FxqAppName.Trim();
            }

            CaAuthIdentityDto caAuthIdentityDto = null;
            if (model != null && model.IsReal)
            {
                caAuthIdentityDto = new CaAuthIdentityDto()
                {
                    AuthType = "Face",
                    CardID = model.CardID,
                    Name = model.FullName,
                    Phone = model.Phone,
                    StatusName = "未认证",
                    UserGid = model.UserGID ?? Guid.Empty,
                    FxqAppName= appName,
                };
            }
            return Ok(caAuthIdentityDto);
        }
    }
}
