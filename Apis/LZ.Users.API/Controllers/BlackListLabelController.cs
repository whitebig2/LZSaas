using LZ.ApiService.Interface;
using LZ.Core.Attributes;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class BlackListLabelController(IBlackListLabelService blackListLabelService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_BlackListLabel>();
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini || CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                expable.And(t => t.CompanyID == 0 || t.CompanyID == cid);
            }
            else
            {
                expable.And(t => t.CompanyID == 0);
            }

            var exp = expable.ToExpression();
            var models = await blackListLabelService.List(exp, query, "CompanyID asc,sort asc");
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_BlackListLabel model)
        {
            int cid = 0;
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var expable = Expressionable.Create<Model.Live_BlackListLabel>();
            if(CurrentUser.PlatformType == Model.Enums.PlatformType.Admin)
            {
                cid = 0;
                expable.And(t => t.CompanyID == cid);
            } else
            {
                cid = CurrentUser.CompanyID;
                cgid = CurrentUser.CompanyGID;
                expable.And(t => t.CompanyID == 0 || t.CompanyID == cid);
            }
            expable.And(t => t.Title == model.Title);
            if (await blackListLabelService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该标签已存在!");
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            await blackListLabelService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_BlackListLabel> Get(int id)
        {
            return await blackListLabelService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_BlackListLabel model)
        {
            if (!await blackListLabelService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await blackListLabelService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await blackListLabelService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await blackListLabelService.Delete(id);
            return Ok();
        }
    }
}
