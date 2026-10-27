using LZ.Core;
using LZ.Core.Attributes;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
namespace LZ.Users.API.Controllers.Injury
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class InjuryDocumentLabelController(IInjuryDocumentLabelService injuryDocumentLabelService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] Model.PagedModel.UserPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_InjuryDocumentLabel>();
            //if (CurrentUser.PlatformType == Model.Enums.PlatformType.Admin || CurrentUser.PlatformType == Model.Enums.PlatformType.Api)
            //{
            //    expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            //}
            //else
            //{
            //    expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            //}
            query.Order = "sort asc";
            var exp = expable.ToExpression();
            var models = await injuryDocumentLabelService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_InjuryDocumentLabel model)
        {
            if (await injuryDocumentLabelService.ExistName(model.Name))
            {
                throw new ErrorCodeException(-1, "保险公司已存在");
            }
            int cid = 0;//11
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                cid = CurrentUser.CompanyID;
                cgid = CurrentUser.CompanyGID;
            }
            model.CompanyID = cid;
            model.CompanyGID = cgid;
            await injuryDocumentLabelService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_InjuryDocumentLabel> Get(int id)
        {
            return await injuryDocumentLabelService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_InjuryDocumentLabel model)
        {
            if (!await injuryDocumentLabelService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await injuryDocumentLabelService.Update(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await injuryDocumentLabelService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await injuryDocumentLabelService.Delete(id);
            return Ok();
        }
    }
}
