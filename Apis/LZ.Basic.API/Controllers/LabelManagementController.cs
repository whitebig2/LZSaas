using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums.Label;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class LabelManagementController(ILabelManagementService labelManagementService,IAuthService authService, IPartnerService partnerService) :BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BasicPagedModel query, int? type)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create < Model.Live_LabelManagement>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var pmodel = await authService.UserInfo<Model.Live_Partner>();
                if (query.From == "partner")
                {
                    expable.And(t => t.PartnerID == pmodel.PartnerID);
                }
                else
                {
                    expable.And(t => t.PartnerID == pmodel.PartnerID || t.PartnerID == 0);
                }
                
            }

            if (type.HasValue)
            {
                expable.And(t => t.Type == type);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.Name.Contains(query.Name));
            }
            var exp = expable.ToExpression();
            var models = await labelManagementService.List(exp, query, "PartnerID asc, CreateTime asc");
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> InjuryList([FromQuery] BasicPagedModel query, int? type)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_LabelManagement>();
            expable.And(t => t.CompanyID == 1);

            if (type.HasValue)
            {
                expable.And(t => t.Type == type);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.Name.Contains(query.Name));
            }
            var exp = expable.ToExpression();
            var models = await labelManagementService.List(exp, query, "CreateTime asc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_LabelManagement model)
        {
            var user = CurrentUser;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            var expable = Expressionable.Create<Model.Live_LabelManagement>();
            model.PartnerID = 0;
            if (user.PlatformType == Model.Enums.PlatformType.LZMini || user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = user.CompanyGID;
                cid = user.CompanyID;
                
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var pmodel = await authService.UserInfo<Model.Live_Partner>();
                var p = await partnerService.Get(pmodel.PartnerID);
                model.PartnerID = pmodel.PartnerID;
                model.PartnerGID = pmodel.PartnerGID;
                model.PartnerName = p.PartnerName;
            }
            expable.And(t => t.CompanyID == cid);
            expable.And(t => t.Name == model.Name);
            if (await labelManagementService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该标签已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            model.EditTime = DateTime.Now;
            await labelManagementService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_LabelManagement> Get(int id)
        {
            return await labelManagementService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }

        [HttpGet]
        public async Task<Model.Live_LabelManagement> GetInjury()
        {
            return await labelManagementService.Get(t=>t.Type==(int)LabelManagementEnum.理赔材料清单);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_LabelManagement model)
        {
            if (!await labelManagementService.Exist(t=>t.ID==model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            model.EditTime = DateTime.Now;
            await labelManagementService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await labelManagementService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await labelManagementService.Delete(id);
            return Ok();
        }
    }
}
