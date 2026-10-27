using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core.Attributes;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;




namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
   
    public class UpdateRecordController(IUpdateRecordService updateRecordService, IAuthService authService) : BasicControllerBase
    {
        [HttpGet]
        
        public async Task<IActionResult> List([FromQuery] BasicPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_UpdateRecord>();
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(t => t.UserName.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Title))
            {
                expable.And(t => t.Title.Contains(query.Title));
            }
            var exp = expable.ToExpression();
            var models = await updateRecordService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetVesion(string vesion, int? type)
        {
            var expable = Expressionable.Create<Model.Live_UpdateRecord>();
            if (!string.IsNullOrEmpty(vesion))
            {
                expable.And(t => t.Vesion==vesion);
            }
            if (type.HasValue)
            {
                expable.And(t => t.ServiceType==type);
            }
            var exp = expable.ToExpression();
            var model = await updateRecordService.Get(exp);
            return Ok(new { model });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_UpdateRecord model)
        {
            var user = CurrentUser;
            var umodel = await authService.UserInfo<Model.SYS_CompanyUser>();
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
            model.UserID = umodel.UserID;
            model.UserName = umodel.FullName;
            await updateRecordService.Add(model);
            return Ok(new { model });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Model.Live_UpdateRecord model)
        {
            await updateRecordService.Update(model);
            return Ok(new { model});
        }

        [HttpGet]
        public async Task<Model.Live_UpdateRecord> Get(int id)
        {
            return await updateRecordService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await updateRecordService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await updateRecordService.Delete(id);
            return Ok();
        }
    }
}
