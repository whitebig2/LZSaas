using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.Extensions;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class UserShopController(IUserShopService userShopService, ICompanyConfigService companyConfigService,IHttpService httpService,ISettingsService settingsService) : UsersControllerBase
    {
        [HttpGet]
        //[PlatformLimit(Model.Enums.PlatformType.Admin)]
        //[PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        //[PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Shop_UserShop>();
            expable.AndIF(!string.IsNullOrEmpty(query.Name), it => it.ShopName.Contains(query.Name));
            expable.AndIF(query.ShopType.HasValue, it => it.ShopType == query.ShopType);
            expable.AndIF(query.Status.HasValue, it => it.Status == query.Status);
            if (user.PlatformType == Model.Enums.PlatformType.Admin)
            {
                if (query.CompanyID.HasValue)
                {
                    expable.And(it => it.CompanyID == query.CompanyID);
                }
            }
            else
            {
                var companygid = HttpContext.Request.Headers["company-guid"].ToString();
                if (!string.IsNullOrEmpty(companygid))
                {
                    var companyinfo = await companyConfigService.Get(new Guid(companygid));
                    expable.And(it => it.CompanyID == companyinfo.Company.CompanyID);
                }
                else
                {
                    expable.And(it => it.CompanyID == user.CompanyID);
                }
            }
            var exp = expable.ToExpression();
            var models = await userShopService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        [OperationLogAction("新增店铺：{model.ShopName}")]
        public async Task<IActionResult> Add([FromBody] Model.Shop_UserShop model)
        {
            var user = CurrentUser;
            model.ShopGID = Guid.NewGuid();
            model.CreateTime = DateTime.Now;
            model.CompanyGID = user.CompanyGID;
            model.CompanyID= user.CompanyID;
            var paras = new Dictionary<string, string>();
            paras.Add("companyId", user.CompanyID.ToString());
            paras.Add("type", "xfz");
            paras.Add("appType", "mini");
            paras.Add("page", $"/pagesMall/shop/detail?shopgid={model.ShopGID}");
            var result = await httpService.PostAsync(ServiceNames.ClientsService, "/api/clients/createqrcode", paras);
            if (result != null && result.Code == 200)
            {
                var setting = await settingsService.GetMasterSettings();
                model.Qrcode = setting.AliBucketDomain + result.Data.ToString();
            }
            else
            {
                throw new ErrorCodeException(-1, "生成二维码错误!");
            }
            await userShopService.Add(model);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Shop_UserShop> Get(Guid id)
        {
            return await userShopService.Get(it=>it.ShopGID==id);
        }
        [HttpPut]
        [OperationLogAction("更新店铺：{model.ShopName}")]
        public async Task<IActionResult> Update([FromBody] Model.Shop_UserShop model)
        {
            //if (string.IsNullOrEmpty(model.Qrcode))
            //{
            //    var user = CurrentUser;
            //    var paras = new Dictionary<string, string>();
            //    paras.Add("companyId", user.CompanyID.ToString());
            //    paras.Add("type", "xfz");
            //    paras.Add("appType", "mini");
            //    paras.Add("page", $"/pagesMall/shop/detail?shopgid={model.ShopGID}");
            //    var result = await httpService.PostAsync(ServiceNames.ClientsService, "/api/clients/createqrcode", paras);
            //    if (result != null && result.Code == 200)
            //    {
            //        var setting = await settingsService.GetMasterSettings();
            //        model.Qrcode = setting.AliBucketDomain + result.Data.ToString();
            //    }
            //    else
            //    {
            //        throw new ErrorCodeException(-1, "生成二维码错误!");
            //    }
            //}
           
            await userShopService.Update(model);
            return Ok();
        }
    }
}
