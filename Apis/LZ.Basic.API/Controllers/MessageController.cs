using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class MessageController(IMessageService messageService,IAuthService authService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BasicPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_Message>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            } else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                expable.And(it => it.PersonnelID == umodel.UserID);
            }
            if(query.Status.HasValue && query.Status > 0)
            {
                expable.And(it => it.Status == query.Status);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Title))
            {
                expable.And(t => t.Title.Contains(query.Title));
            }
            var exp = expable.ToExpression();
            var models = await messageService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_Message model)
        {
            var user = CurrentUser;
            var umodel = await authService.UserInfo<Model.Live_Personnel>();
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            int cid = 0;
            if (user.PlatformType == Model.Enums.PlatformType.LZMini || user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                cgid = user.CompanyGID;
                cid = user.CompanyID;
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            model.CreateTime = DateTime.Now;
            model.PersonnelID = umodel.UserID;
            model.Status = 1;
            model.Name = umodel.FullName;
            await messageService.Add(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Model.Live_Message model)
        {
            if(model.Status==2 && model.EditTime==null)
            {
                model.EditTime = DateTime.Now;
            }
            await messageService.Update(model);
            return Ok();
        }

        [HttpGet]
        public async Task<Model.Live_Message> Get(int id)
        {
            return await messageService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await messageService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await messageService.Delete(id);
            return Ok();
        }
    }
}
