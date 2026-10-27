using LZ.Company.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Numerics;
using System.Security.Policy;

namespace LZ.Company.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    public class CompanyController(ICompanyService companyService,IAuthService authService,ICompanyUserService companyUserService,ICompanyConfigService companyConfigService,IEventPublisher publisher,IHttpService httpService) : CompanyControllerBase
    {
        [HttpGet]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        public async Task<IActionResult> List(Guid? cityPartnerID, [FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.SYS_Company>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.CompanyName.Contains(query.Name));
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                if (user.CompanyID == 1)
                {
                    expable.And(it => it.CompanyID>1);
                }
                else
                {
                    expable.And(it => it.CompanyID == 0);
                }
            }
            if (!string.IsNullOrEmpty(query.Types))
            {
                expable.And(it=>it.CutType==query.Types);
            }
            expable.AndIF(cityPartnerID.HasValue, f => f.CityPartnerID!=null&& f.CityPartnerID.Value == cityPartnerID.Value);


            var exp = expable.ToExpression();
            var models = await companyService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> PerFunctionViewList([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_CompanyPerFunction>();
            
            if (user.PlatformType== Model.Enums.PlatformType.LZMini)
            {
                var userinfo = await authService.UserInfo<Live_Personnel>();
                expable.And(it=>it.CompanyID==user.CompanyID);
                expable.And(it => SqlFunc.Subqueryable<Live_PersonnelFunctionItems>().Where(t => t.CompanyID == user.CompanyID && t.UserType == userinfo.UserType && t.PFID == it.ID).Any());
            }
            else if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                if (query.CID.HasValue)
                {
                    expable.And(it=>it.CompanyID==query.CID);
                }
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await companyService.PerFunctionList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> PerFunctionList([FromQuery] PagedModelBase query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_CompanyPerFunction>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                if (query.CID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CID);
                }
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await companyService.PerFunctionList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
       
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.SYS_Company model)
        {
            model.CompanyGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            model.Password = model.Password;
            if(!string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Password))
            {
                if(await companyUserService.ExistName(model.UserName))
                {
                    throw new ErrorCodeException(-1, "该用户已存在！");
                }
            }
            await companyService.Add(model);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPost]
        public async Task<IActionResult> SetPerFunction([FromBody] List<Model.Live_CompanyPerFunction> list)
        {
            if (list.Count == 0)
            {
                throw new ErrorCodeException(-1, "内容不能为空！");
            }
            await companyService.SetPerFunction(list[0].CompanyID, list);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<Model.SYS_Company> Get(int id)
        {
            var user = CurrentUser;
            if(user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                id = user.CompanyID;
            }
            var model =await companyService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "该信息不存在");
            }
            return model;
        }
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> GetInfo(Guid id)
        {
            var model = await companyService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "该公司不存在");
            }
            return Ok(new { companyName = model.CompanyName });
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SYS_Company model)
        {
            if (!await companyService.Exist(model.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await companyService.Update(model);
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost]
        [OperationLogAction("注册公司：{model.CompanyName}")]
        public async Task<IActionResult> Register([FromBody] SYS_Company model)
        {
            if (string.IsNullOrEmpty(model.CompanyName) || string.IsNullOrEmpty(model.Contact) || string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.CompanyNo))
            {
                throw new ErrorCodeException(-1, "请正确填写信息！");
            }
            if (model.Phone.Length != 11)
            {
                throw new ErrorCodeException(-1, "手机号错误！");
            }
            model.CompanyGID = Guid.NewGuid();
            if (await companyService.Exist(it => it.CompanyName == model.CompanyName || it.CompanyNo == model.CompanyNo))
            {
                throw new ErrorCodeException(-1, "该公司名称或统一社会信用代码‌已被注册！");
            }
            var phoneRes = await httpService.GetAsync(ServiceNames.UsersService, $"/api/personnel/existphone?phone={model.Phone}");
            if (phoneRes != null && phoneRes.Code == 200 && Convert.ToBoolean(phoneRes.Data))
            {
                throw new ErrorCodeException(-1, "该手机号已被注册！");
            }
            model.CreateTime = DateTime.Now;
            model.CutType = Model.Enums.CompanyType.Party.ToString().ToLower();
            model.Password = Common.Tool.GetRandomNumber(6);
            model.Status = 1;
            model.Config = "{}";
            if(await companyUserService.ExistName(model.Contact))
            {
                model.UserName = model.Contact + "_" + Common.Tool.GetRandomNumber(4);
            }
            else
            {
                model.UserName = model.Contact;
            }
            model.CompanyID = await companyService.Add(model);
            var record = new PartnerEventData();
            record.PartnerName = "默认服务商";
            record.CompanyID = model.CompanyID;
            record.CompanyGID = model.CompanyGID;
            record.FullName = model.Contact;
            record.PartnerType = (int)Model.Enums.PartnerType.Ordinary;
            record.Phone = model.Phone;
            publisher.Publish(record);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpPut]
        public async Task<IActionResult> UpdateConfig([FromBody] Model.CompanyConfig model)
        {
            var user = CurrentUser;
            var oldModel = await companyConfigService.Get(user.CompanyGID);
            if (!await companyService.UpdateConfig(model,user.CompanyGID.Value))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            //var oldConfig = oldModel.Config;
            //if (oldConfig.WeiXinKeyFile!=model.WeiXinKeyFile || oldConfig.WeiXinMerchantID!=model.WeiXinMerchantID || oldConfig.WeiXinPaySecret != model.WeiXinPaySecret)
            //{
            //    var tenPayV3Config = new Senparc.Weixin.Entities.SenparcWeixinSetting()
            //    {
            //        TenPayV3_AppId = model.WeiXinOpenAppId,
            //        TenPayV3_AppSecret = model.WeiXinOpenAppSecret,
            //        TenPayV3_MchId = model.WeiXinMerchantID,
            //        TenPayV3_Key = model.WeiXinPaySecret,
            //        TenPayV3_CertPath = (Directory.GetCurrentDirectory() + model.WeiXinKeyFile).Replace("\\", "\\\\").Replace("/", "\\\\"),
            //        TenPayV3_CertSecret = model.WeiXinMerchantID,
            //    };
            //    var tenPayV3Info = new Senparc.Weixin.TenPay.V3.TenPayV3Info(tenPayV3Config);
            //    Senparc.Weixin.TenPay.V3.TenPayV3InfoCollection.Register(tenPayV3Info, model.WeiXinMerchantName);
            //}
            return Ok();
        }
        //[HttpDelete]
        //public async Task<IActionResult> Delete([FromForm] int id)
        //{
        //    if (!await companyService.Delete(id))
        //    {
        //        throw new ErrorCodeException(-1, "你要删除的数据不存在");
        //    }
        //    return Ok();
        //}
    }
}
