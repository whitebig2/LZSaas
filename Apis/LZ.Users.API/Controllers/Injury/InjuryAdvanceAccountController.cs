using LZ.Core.Interface;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SqlSugar;
using LZ.Core;
using LZ.Model.Enums;
using System.IO;
using NPOI.SS.Formula.Functions;
using System.Security.Cryptography;
using Senparc.Weixin.WxOpen.Entities;
using NPOI.SS.UserModel;
using LZ.Core.Attributes;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.ModelExtensions;
using LZ.ApiService.Services;
using LZ.Model.FxqModel;
using LZ.ApiService.Interface;
using System.Security.Policy;
using System;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using LZ.Core.CommonHandle;
using LZ.Core.RabbitMQ;
namespace LZ.Users.API.Controllers.Injury
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryAdvanceAccountController(IInjuryAdvanceAccountService injuryAdvanceAccountService,IAuthService authService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_InjuryAdvanceAccount>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            //else
            //{
            //    expable.And(it => it.CompanyID == user.CompanyID);
            //}
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.PartnerName.Contains(query.Name));
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                
            }

            var exp = expable.ToExpression();

            var models = await injuryAdvanceAccountService.List(exp, query, "ID desc");
            return Ok(new { items = models, count = query.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_InjuryAdvanceAccount model)
        {

            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            var expable = Expressionable.Create<Model.Live_InjuryAdvanceAccount>();
            //if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            //{
            //    var user = CurrentUser;
            //    cgid = user.CompanyGID;
            //    cid = user.CompanyID;
            //}
            if(!model.PartnerID.HasValue)
            {
                throw new ErrorCodeException(-1, "未选择服务商");
            }
            if (await injuryAdvanceAccountService.Exist(t=>t.PartnerGID== model.PartnerGID))
            {
                throw new ErrorCodeException(-1, "该服务商已经创建账号");
            }
            model.GID = Guid.NewGuid();
            model.AvailableAmount = (model.TotalAmount ?? 0M) - model.UsedAmount ;
            await injuryAdvanceAccountService.Add(model);


            
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_InjuryAdvanceAccount> Get(int id)
        {
            return await injuryAdvanceAccountService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<Model.Live_V_InjuryAdvanceAccount> WeiXinGet()
        {
            var umodel = await authService.UserInfo<Model.Live_Personnel>();
            return await injuryAdvanceAccountService.GetView(t => t.PartnerID==umodel.PartnerID);
        }
        [HttpGet]
        public async Task<Model.Live_V_InjuryAdvanceAccount> View(int id)
        {
            return await injuryAdvanceAccountService.GetView(t=>t.ID==id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryAdvanceAccount model)
        {
            if (!await injuryAdvanceAccountService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryAdvanceAccountService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryAdvanceAccountService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryAdvanceAccountService.Delete(id);
            return Ok();
        }
    }
}
