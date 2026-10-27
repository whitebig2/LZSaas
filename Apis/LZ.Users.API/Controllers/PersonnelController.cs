using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System.Text;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    public class PersonnelController(IPersonnelService personnelService, IAliyunOSSService aliyunOSSService, ICompanyConfigService companyConfigService, IAuthService authService, IEntryService entryService, IPermissionService permissionService, IHttpService httpService, IEventPublisher publisher,IJDYService jDYService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_Personnel>();
            if (!string.IsNullOrEmpty(query.UserName))
            {
                expable.And(it => it.UserName.Contains(query.UserName));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.FullName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(it => SqlFunc.Subqueryable<Model.Live_V_PersonnelItems>().Where(s => s.UserID == it.UserID && s.CustomerName.Contains(query.CustomerName)).Any());
            }
            if (!string.IsNullOrEmpty(query.Phone))
            {
                expable.And(it => it.Phone == query.Phone);
            }
            if (!string.IsNullOrEmpty(query.Types))
            {
                var types = query.Types.Split(',');
                expable.And(it => types.Contains(it.UserType.ToString()));
            }
            if (query.PartnerID > 0)
            {
                expable.And(it => it.PartnerID == query.PartnerID);
            }
            expable.AndIF(query.CustomerGID.HasValue, it => SqlFunc.Subqueryable<Model.Live_Customer>().Where(s => s.CustomerGID == query.CustomerGID && s.PartnerID == it.PartnerID).Any());
            if (query.Enabled.HasValue)
            {
                expable.And(it => it.Enabled == query.Enabled);
            }
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID>0, it => it.CompanyID == query.CompanyID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var auth = await permissionService.CompanyLicenseList("users", "manage,service,show,edit,add", user.UserID);
                if (!auth[0] && !auth[1] && !auth[2])
                {
                    var puser = await authService.UserInfo<Model.SYS_CompanyUser>();
                    if (puser.PartnerID.HasValue)
                    {
                        expable.And(it => it.PartnerID == puser.PartnerID);
                    }
                }
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {

                expable.And(it => it.CompanyID == user.CompanyID);
                var userinfo = await authService.UserInfo<Model.Live_Personnel>();
                expable.And(it => it.PartnerID == userinfo.PartnerID);
            }
            var exp = expable.ToExpression();
            var models = await personnelService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        [OperationLogAction("新增驻厂人员：{model.FullName}")]
        public async Task<IActionResult> Add([FromBody] Model.Live_Personnel model)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            var user = CurrentUser;
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
                model.CompanyID = user.CompanyID;
                model.CompanyGID = user.CompanyGID;
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
                model.CompanyID = user.CompanyID;
                model.CompanyGID = user.CompanyGID;
                var userinfo = await authService.UserInfo<Model.Live_Personnel>();
                model.PartnerID = userinfo.PartnerID;
                model.PartnerName = userinfo.PartnerName;
                model.PartnerGID = userinfo.PartnerGID;
                model.UserType = model.UserType == (int)Model.Enums.PersonnelType.Factory ? (int)Model.Enums.PersonnelType.Factory : (int)Model.Enums.PersonnelType.Platform;
                model.Enabled = true;
            }
            else
            {
                expable.And(it => it.CompanyID == model.CompanyID);
            }
            expable.And(it => it.Phone == model.Phone);
            var exp = expable.ToExpression();
            if (await personnelService.Exist(exp))
            {
                throw new ErrorCodeException(-1, "该手机号已存在!");
            }
            model.UserGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await personnelService.Add(model);
            return Ok();
        }

        [HttpPost]
        [OperationLogAction("日结工接口新增驻厂人员：{model.FullName}")]
        public async Task<int> AddJoin(Model.Live_Personnel model)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.CompanyID == model.CompanyID);
            expable.And(it => it.Phone == model.Phone);
            var exp = expable.ToExpression();
            if (await personnelService.Exist(exp))
            {
                throw new ErrorCodeException(-1, "该手机号已存在!");
            }
            model.UserGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            var index = await personnelService.Add(model);
            return index;
        }
        [HttpGet]
        public async Task<Model.CompanyModel> GetCompanyConfig()
        {
            return await companyConfigService.Get((Guid)CurrentUser.CompanyGID);
        }
        [HttpGet]
        public async Task<Model.RequestResultModel> GetInfo()
        {
            var model = await authService.UserInfo<Live_Personnel>();
            if (string.IsNullOrEmpty(model.ClientQrcode))
            {
                publisher.Publish(new UserQrcodeEventData()
                {
                    UserID = model.UserID,
                    CompanyID = model.CompanyID.Value,
                    CompanyGID = model.CompanyGID.Value,
                    FromType="lz"
                });
            }
            await authService.UpdateUserInfo<Model.Live_Personnel>(model, model.UserID, Model.Enums.PlatformType.LZMini);//更新缓存
            return new RequestResultModel() { Code = 200, Message = "获取成功", Data = new { name = model.FullName, picture = model.Picture, phone = model.Phone, userid = model.UserID, nickname = model.NickName, type = model.UserType,isBindMP=model.IsBindMP, clientQrcode=model.ClientQrcode } };
        }
        [HttpGet]
        public async Task<Model.Live_Personnel> Get(int id)
        {
            return await personnelService.Get(id, CurrentUser.CompanyID);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_Personnel> GetHeaded(int partnerId)
        {
            return await personnelService.Get(it => it.PartnerID == partnerId && it.UserType == (int)PersonnelType.Headed && it.IsDefault);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetQrcode(int userId)
        {
            var model = await personnelService.Get(userId);
            if (model == null)
            {
                return Ok();
            }
            return Ok(new { name = model.FullName, qrcode = model.PrivateQrcode,phone=model.Phone });
        }
        [HttpGet]
        public async Task<IActionResult> GetUserAIInfo()
        {
            var model = await personnelService.GetView(CurrentUser.UserID);
            var infos = new StringBuilder();
            if (!string.IsNullOrEmpty(model.PartnerName))
            {
                infos.Append($"所属服务商：{model.PartnerName}；");
            }
            if (model.UserType.HasValue)
            {
                var type = (Model.Enums.PersonnelType)model.UserType;
                infos.Append($"系统角色：{Tool.GetDescription(type)}；");
            }
            return Ok(infos.ToString());
        }
        /// <summary>
        /// 同步简道云信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SyncJDYUsers()
        {
            var user = CurrentUser;
            var company = await companyConfigService.Get(user.CompanyGID);
            if (company.Config.EnableDockJDY)
            {
                var paras = new Dictionary<string, object>();
                paras.Add("dept_no", 1);
                paras.Add("has_child", true);
                var userRes = await jDYService.GetUsers(paras, company.Config);
                var users = JsonConvert.DeserializeObject<Model.JDYUserModel>(userRes);
                var extUserRes = await jDYService.GetExternalUsers(company.Config);
                var extUsers = JsonConvert.DeserializeObject<Model.JDYUserModel>(extUserRes);
                var models = await personnelService.List(it => it.CompanyID == user.CompanyID,new UserPagedModel() { Page=0});
                var upModels = new List<Model.Live_Personnel>();
                foreach(var model in models)
                {
                    var upModel=new Model.Live_Personnel();
                    upModel.UserID= model.UserID;
                    if (users.Users != null && users.Users.Count() > 0)
                    {
                        var jdyUser = users.Users.Find(it=>it.name==model.FullName);
                        if (jdyUser != null)
                        {
                            upModel.JDYNumber = jdyUser.username;
                        }
                    }
                    if (extUsers.member_list != null && extUsers.member_list.Count() > 0 && string.IsNullOrEmpty(upModel.JDYNumber))
                    {
                        var jdyUser = extUsers.member_list.Find(it => it.name == model.FullName);
                        if (jdyUser != null)
                        {
                            upModel.JDYNumber = jdyUser.username;
                        }
                    }
                    if (!string.IsNullOrEmpty(upModel.JDYNumber))
                    {
                        upModels.Add(upModel);
                    }
                }
                await personnelService.UpdateJDYNumber(upModels);
            }
            
            return Ok();
        }
        [HttpGet]
        public async Task<bool> ExistPhone(string phone)
        {
            return await personnelService.Exist(it=>it.Phone==phone);
        }
        [HttpGet]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<Model.Live_PersonnelOpenIds> GetOpenID(int id, int clientId)
        {
            return await personnelService.GetOpenID(id, clientId);
        }
        [HttpPut]
        [OperationLogAction("更新驻厂人员：{model.FullName}")]
        public async Task<IActionResult> Update([FromBody] Model.Live_Personnel model)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            var user = CurrentUser;
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
                model.CompanyID = user.CompanyID;
                model.CompanyGID = user.CompanyGID;
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
                model.CompanyID = user.CompanyID;
                model.CompanyGID = user.CompanyGID;
                var userinfo = await authService.UserInfo<Model.Live_Personnel>();
                model.PartnerID = userinfo.PartnerID;
                model.PartnerName = userinfo.PartnerName;
            }
            else
            {
                expable.And(it => it.CompanyID == model.CompanyID);
            }
            expable.And(it => it.Phone == model.Phone);
            expable.And(it => it.UserID != model.UserID);
            var exp = expable.ToExpression();
            if (await personnelService.Exist(exp))
            {
                throw new ErrorCodeException(-1, "该手机号已存在!");
            }
            if (!await personnelService.Exist(model.UserID, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var oldModel = new Model.Live_Personnel();
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                oldModel = await personnelService.Get(model.UserID);
            }
            await personnelService.Update(model);
            await authService.UpdateUserInfo(model, model.UserID, Model.Enums.PlatformType.LZMini);
            if (user.PlatformType == Model.Enums.PlatformType.Admin && oldModel.CompanyID != model.CompanyID)//总后台修改所属人力公司需要更新token缓存
            {
                var company = await companyConfigService.Get(model.CompanyGID);
                await authService.UpdateUserToken(company.Company, model.UserID, Model.Enums.PlatformType.LZMini);
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePrivateQrcode([FromForm] string qrcode)
        {
            if (string.IsNullOrEmpty(qrcode))
            {
                throw new ErrorCodeException(-1, "请先上传微信二维码");
            }
            await personnelService.UpdatePrivateQrcode(CurrentUser.UserID, qrcode);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> SaveClientQrcode([FromForm] int id,[FromForm] string qrcode)
        {
            if (string.IsNullOrEmpty(qrcode) || id==0)
            {
                throw new ErrorCodeException(-1, "请先上传二维码");
            }
            await personnelService.UpdateClientQrcode(id, qrcode);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await personnelService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await personnelService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await personnelService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await personnelService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        [OperationLogAction("删除驻厂人员id：{id}")]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var user = CurrentUser;
            if (!await personnelService.Exist(id, user.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var expable = Expressionable.Create<Model.Live_UserEntry>();
            expable.And(it => it.PerUserID == id);
            var exp = expable.ToExpression();
            if (await entryService.Exist(exp))
            {
                throw new ErrorCodeException(-1, "您要删除的人员已在蓝领中关联不可删除！");
            }
            await personnelService.Delete(id);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Attendance(int cid, int type)
        {
            var user = CurrentUser;
            Model.Live_CompanyAuthorize authorize = new Live_CompanyAuthorize();
            UserPagedModel userPagedModel = new UserPagedModel();
            userPagedModel.Page = 0;
            var expable = Expressionable.Create<Model.Live_V_Personnel>();
            List<Live_V_Personnel> list = new List<Live_V_Personnel>();
            expable.And(t => t.Enabled);

            if (type == 1)
            {
                expable.And(t => t.CompanyID == user.CompanyID);
                list = await personnelService.List(expable.ToExpression(), userPagedModel);
            }
            else if (type == 2)
            {
                var paras = new Dictionary<string, string>();
                paras.Add("cid", cid.ToString());
                var obj = await httpService.GetAsync(ServiceNames.CompanyService, "/api/CompanyAuthorize/CustomerGet", paras);
                if (obj.Message == "Success")
                {
                    authorize = JsonConvert.DeserializeObject<Model.Live_CompanyAuthorize>(obj.Data.ToString());
                    if (authorize.ToCompanyID.HasValue)
                    {
                        expable.And(t => t.CompanyID == authorize.ToCompanyID);
                        list = await personnelService.List(expable.ToExpression(), userPagedModel);
                    }


                }
            }
            return Ok(new { items = list, authorize });
        }
        [HttpPut]
        public async Task<IActionResult> ChangePicture([FromForm] IFormFile file)
        {
            if (file != null)
            {
                var res = await aliyunOSSService.UploadImage(file);
                if (res.Code == 0)
                {
                    //return Ok(res.Message);
                    var count = await personnelService.ChangePicture(CurrentUser.UserID, res.Data.ToString());
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
    }
}
