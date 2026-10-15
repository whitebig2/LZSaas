using LZ.ApiService.Interface;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using static System.Net.WebRequestMethods;

namespace LZ.Customer.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class ContractFileConfigController(IContractFileConfigService service ,ISettingsService settingsService, IAliyunOSSService aliyunOSSService) : CustomerControllerBase
    {

        public record SigningAndCooperationUnitQuery(bool? IsSigningUnit,bool? IsCooperationUnit);

        [HttpGet]
        public async Task<IActionResult> PageList(string name, ContractEnumType? contractType, [FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_ContractFileConfig>();

            expable.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            expable.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(name));
            expable.AndIF(contractType.HasValue, f => f.ContractEnumType==contractType.Value);
            
            var models = await service.List(expable.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_ContractFileConfig>();
            expable.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);

            expable.And(f => f.IsEnabled);
            if(!string.IsNullOrEmpty(query.IDS))
            {
                var arr = query.IDS.Split(",");
                expable.And(t => arr.Contains(t.ID.ToString()));
            }
           
            var exp = expable.ToExpression();
            var models = await service.List(exp, query);
            return Ok(new { items = models, count = query.Count});
        }




        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var setting = await settingsService.GetMasterSettings();
            var uploadRes = await aliyunOSSService.UploadFile(file);
            uploadRes.Code = 200;
            return Ok(new { filePath = setting.AliBucketDomain + uploadRes.Data });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_ContractFileConfigExtension model)
        {
            model.ImageOssUrl = string.Empty;
            model.IsOss = true;
            model.CreateTime = DateTime.Now;
            model.ID = Guid.NewGuid();

            if (model.CompanyID==null|| model.CompanyID.Value<=0)
            {
                model.CompanyID = CurrentUser.CompanyID;
            }
            await service.AddNav(model);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await service.GetNav(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetType(int type)
        {
            var model = await service.Get(f => f.ContractEnumType == (ContractEnumType)type);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Live_ContractFileConfigExtension model)
        {
            if (!await service.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await service.UpdateNav(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await service.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var model = await service.GetNav(f => f.ID == id);
            await service.DeleteNav(model);
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCustomerGID(Guid customerGID)
        {
            var model = await service.GetView(f => f.CustomerGID == customerGID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
    }
}
