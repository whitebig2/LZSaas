using LZ.ApiService.Interface;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SqlSugar;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BlackListController(IBlackListService blackListService, IAuthService authService, ICompanyConfigService companyConfigService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query, string label)
        {
            var cid = CurrentUser.CompanyID; 
            
            var expable = Expressionable.Create<Model.Live_V_BlackList>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                if(CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                    expable.And(it => it.CompanyID == cid);
                    if (info.UserType == 2)
                    {
                        expable.And(t => t.PartnerID == info.PartnerID);
                    }
                }
                else if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
                {
                    var info = await authService.UserInfo<Model.Live_Partner>();
                    expable.And(t => t.PartnerID == info.PartnerID);
                }
            }
            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name));
            expable.AndIF(!string.IsNullOrEmpty(query.CardID), t => t.IdentityCard.Contains(query.CardID));
            expable.AndIF(!string.IsNullOrEmpty(label), t => t.LabelNames.Contains(label));
            var exp = expable.ToExpression();
            var models = await blackListService.ListView(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_BlackList model)
        {
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var expable = Expressionable.Create<Model.Live_BlackList>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin)
            {
                expable.And(t => t.CompanyID == 0);
            }
            else
            {
                cid = CurrentUser.CompanyID;
                cgid = CurrentUser.CompanyGID;
                expable.And(t => t.CompanyID == 0 || t.CompanyID == cid);
                if(CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
                {
                    var info = await authService.UserInfo<Model.SYS_CompanyUser>();
                    if (info.UserType == 2)
                    {
                        model.PartnerID = info.PartnerID;
                    }
                }
                else if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
                {
                    var info = await authService.UserInfo<Model.Live_Partner>();
                    model.PartnerID = info.PartnerID;
                }
                
            }
            expable.And(t => t.IdentityCard == model.IdentityCard);
            if (await blackListService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "身份证已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            await blackListService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_BlackList> Get(int id)
        {
            return await blackListService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public async Task<Model.RequestResultModel> CheckCardID(string cardId)
        {
            if (string.IsNullOrEmpty(cardId))
            {
                throw new ErrorCodeException(-1, "身份证错误");
            }
            var user = CurrentUser;
            var company = await companyConfigService.Get((Guid)user.CompanyGID);
            var expable = Expressionable.Create<Model.Live_V_BlackList>();
            expable.And(it => it.IdentityCard == cardId);
            if (company.Config != null && !company.Config.UserIsUsePublicBlcak)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp = expable.ToExpression();
            var models = await blackListService.ListView(exp,new Model.PagedModel.UserPagedModel() { Page=0});
            var res = new Model.RequestResultModel();
            if (models.Count == 0)
            {
                res.Code = 0;
                res.Data = true;
            }
            else
            {
                var msg = new StringBuilder();
                var tabs=new StringBuilder();
                foreach (var model in models)
                {
                    tabs.Append(model.LabelNames);
                    msg.Append($"标签内容{model.LabelNames}");
                    if (model.CreateTime.HasValue)
                    {
                        msg.Append($"于{model.CreateTime.Value.ToString("yyyy-MM-dd")},");
                    }
                    if (model.PartnerID.HasValue)
                    {
                        msg.Append($"被服务商{model.PartnerName}加入黑名单,");
                    }
                    else
                    {
                        msg.Append("被平台加入黑名单,");
                    }
                }
                var tabContent = tabs.ToString().Replace("其他","");
                res.Message= msg.ToString();
                if (company.Config.IsBlackNoEntry && tabContent.Length>0)
                {
                    res.Code = 202;
                    res.Data = false;
                    res.Message += "禁止入职！";
                }
                else
                {
                    res.Code = 201;
                    res.Data = true;
                }
            }
            return res;
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BlackList model)
        {
            if (!await blackListService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await blackListService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await blackListService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await blackListService.Delete(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel([FromQuery] Model.PagedModel.UserPagedModel query,string label)
        {
            query.Page = 0;
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_V_BlackList>();

            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(t => t.CompanyID==cid);
            }
            expable.AndIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name));
            expable.AndIF(!string.IsNullOrEmpty(query.CardID), t => t.IdentityCard.Contains(query.CardID));
            expable.AndIF(!string.IsNullOrEmpty(label), t => t.LabelNames.Contains(label));
            var list = await blackListService.ListView(expable.ToExpression(), query);
            var result = blackListService.ExportExcel(list);
            string base64String = Convert.ToBase64String(result);
            return Ok(new { base64String, fileName = "黑名单.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
    }
}
