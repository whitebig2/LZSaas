using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class PartnerController(IPartnerService partnerService,IAuthService authService, IEventPublisher publisher, ISettingsService settingsService,IHttpService httpService) : BasicControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_Partner>();
            var user = CurrentUser;
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.PartnerName.Contains(query.Name));
            }
            if (query.Enabled.HasValue)
            {
                expable.And(it => it.Enabled == query.Enabled);
            }
            if (query.IsBudget.HasValue)
            {
                expable.And(it => it.IsBudget == query.IsBudget);
            }
            if (!string.IsNullOrEmpty(query.PartnerIDS))
            {
                var pids= query.PartnerIDS.Split(',');
                expable.And(it=>pids.Contains(it.PartnerID.ToString()));
            }
            if (query.PartnerType>0)
            {
                expable.And(it => it.PartnerType == query.PartnerType);
            }
            if (query.CityPartnerID.HasValue)
            {
                expable.And(it => it.CityPartnerID == query.CityPartnerID);
            }
            //工伤垫付获取全部的服务商包括第三方
            if (query.From != "injury")
            {
                if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin || user.PlatformType == Model.Enums.PlatformType.LZMini)
                {
                    expable.And(it => it.CompanyID == user.CompanyID);
                }
               
            }
            if (query.CompanyID.HasValue)
            {
                expable.And(it => it.CompanyID == query.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await partnerService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        public async Task<IActionResult> CustomerList([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_PartnerCustomer>();
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var userInfo = await authService.UserInfo<Model.Live_Personnel>();
                expable.And(it => it.PID == userInfo.PartnerID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var userInfo = await authService.UserInfo<Model.SYS_CompanyUser>();
                expable.And(it => it.PID == userInfo.PartnerID);
            }
            var exp = expable.ToExpression();
            var models = await partnerService.CustomerList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        [OperationLogAction("新增服务商：{model.PartnerName}")]
        public async Task<IActionResult> Add([FromBody] Model.Live_Partner model)
        {
            var user = CurrentUser;
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                model.CompanyID = user.CompanyID;
                model.CompanyGID = user.CompanyGID;
            }
            if(await partnerService.Exist(it=>it.PartnerName==model.PartnerName && it.CompanyID == model.CompanyID))
            {
                throw new ErrorCodeException(-1, "该服务商名称已存在！");
            }
            model.PartnerGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            await partnerService.Add(model);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost]
        [OperationLogAction("注册服务商")]
        public async Task<IActionResult> Register([FromForm] string companyName, [FromForm] string name, [FromForm] string phone, [FromForm] int type)
        {
            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || type==0)
            {
                throw new ErrorCodeException(-1, "请正确填写信息！");
            }
            if (phone.Length != 11)
            {
                throw new ErrorCodeException(-1, "手机号错误！");
            }
            var model = new Model.Live_Partner();
            var settings = await settingsService.GetMasterSettings();
            model.CompanyID = settings.DefaultCompanyID;
            model.CompanyGID =Guid.Parse(settings.DefaultCompanyGID);
            model.PartnerName = companyName;
            if (await partnerService.Exist(it => it.PartnerName == model.PartnerName && it.CompanyID == model.CompanyID))
            {
                throw new ErrorCodeException(-1, "该服务商名称已存在！");
            }
            var phoneRes = await httpService.GetAsync(ServiceNames.UsersService, $"/api/personnel/existphone?phone={phone}");
            if (phoneRes!=null && phoneRes.Code==200 && Convert.ToBoolean(phoneRes.Data))
            {
                throw new ErrorCodeException(-1, "该手机号已被注册！");
            }
            model.PartnerGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            model.EntryDays = 0;
            model.LimitMoney = 0;
            model.Enabled = true;
            model.PartnerType = type;
            await partnerService.Add(model);
            var record = new PersonnelEventData();
            record.PartnerID = model.PartnerID;
            record.PartnerGID = model.PartnerGID.Value;
            record.CompanyID = model.CompanyID.Value;
            record.CompanyGID = model.CompanyGID.Value;
            record.FullName = name;
            record.PartnerName = companyName;
            record.UserType = (int)Model.Enums.PersonnelType.Headed;
            record.Phone = phone;
            record.IsRegisterCompanyUser = true;
            publisher.Publish(record);
            return Ok();
        }
        [HttpGet]
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        public async Task<int> Integral()
        {
            var umodel = await authService.UserInfo<Model.Live_Personnel>();
            if (!umodel.PartnerID.HasValue)
            {
                return 0;
            }
            else
            {
                var model = await partnerService.Get(umodel.PartnerID.Value);
                return model.Integral ?? 0;
            }
                
        }
        [HttpGet]
        public async Task<Model.Live_Partner> Get(int id)
        {
            return await partnerService.Get(id, CurrentUser.CompanyID);
        }
        [HttpGet]
        public async Task<Model.Live_Partner> GetInfo(Guid id)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_Partner>();
            expable.AndIF(user.CompanyID > 0, it => it.CompanyID == user.CompanyID);
            expable.And(it => it.PartnerGID == id);
            return await partnerService.Get(expable.ToExpression());
        }
        [HttpGet]
        public async Task<Model.Live_Partner> GetByName(string name)
        {
            return await partnerService.Get(f=>f.PartnerName==name);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<Model.Live_Partner> GetMyPartner(int id)
        {
            var info  = await authService.UserInfo<Model.SYS_CompanyUser>();
            if(!info.PartnerID.HasValue)
            {
                throw new ErrorCodeException(-1, "该账号没有关联服务商");
            }
            return await partnerService.Get(info.UserID, CurrentUser.CompanyID);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<Model.Live_Partner> GetWXPartner(int id)
        {
            var info = await authService.UserInfo<Model.Live_Partner>();
            return await partnerService.Get(info.PartnerID);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpPut]
        [OperationLogAction("更新服务商：{model.PartnerName}")]
        public async Task<IActionResult> Update([FromBody] Model.Live_Partner model)
        {
            var user = CurrentUser;
            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                model.CompanyID = user.CompanyID;
                model.CompanyGID = user.CompanyGID;
            }
            if (await partnerService.Exist(it => it.PartnerName == model.PartnerName && it.CompanyID == model.CompanyID && model.PartnerID!=it.PartnerID))
            {
                throw new ErrorCodeException(-1, "该服务商名称已存在！");
            }
            await partnerService.Update(model);
            return Ok();
        }
        [HttpPut]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<bool> ChangeIntegral([FromForm] Guid id, [FromForm] int integral)
        {
            return await partnerService.ChangeIntegral(id, integral);
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await partnerService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await partnerService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await partnerService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await partnerService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        [OperationLogAction("删除服务商id：{id}")]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await partnerService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await partnerService.Delete(id);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> ListByCityPartnerID(Guid cityPartnerID, int? partnerType, bool? isBusinessPartner)
        {
            var expable = Expressionable.Create<Model.Live_Partner>();
            expable.And(f => f.Enabled);
            expable.And(f => f.CityPartnerID != null && f.CityPartnerID.Value == cityPartnerID);

            if (partnerType.HasValue&& partnerType > 0)
            {
                expable.And(it => it.PartnerType == partnerType.Value);
            }
            if (isBusinessPartner.HasValue )
            {
                expable.And(it => it.IsBusinessPartner == isBusinessPartner.Value);
            }
            var exp = expable.ToExpression();
            var models = await partnerService.List(exp, new UserPagedModel { Page = 0 });
            return Ok(new { items = models });
        }
    }


  
}
