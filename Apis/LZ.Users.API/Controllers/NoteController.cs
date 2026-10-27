using LZ.Users.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using Microsoft.AspNetCore.Mvc;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.ApiService.Interface;
using LZ.Model.PagedModel;

namespace LZ.Users.API.Controllers
{
    public class NoteController(INoteService noteService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_V_UserEntryNote>();
            if (query.UserID.HasValue)
            {
                expable.And(it => it.UserID== query.UserID);
            }
            if (query.EntryID.HasValue)
            {
                expable.And(it => it.EntryID == query.EntryID);
            }
            var exp = expable.ToExpression();
            var models = await noteService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_UserEntryNote model)
        {
            model.CreateTime = DateTime.Now;
            model.PersonnelID = CurrentUser.UserID;
            await noteService.Add(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await noteService.Delete(id);
            return Ok();
        }
    }
}
