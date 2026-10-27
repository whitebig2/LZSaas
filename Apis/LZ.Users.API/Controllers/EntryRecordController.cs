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
    public class EntryRecordController(IEntryRecordService entryRecordService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var expable = Expressionable.Create<Model.Live_V_UserEntryRecord>();
            if (query.EntryID>0)
            {
                expable.And(it => it.EntryID==query.EntryID);
            }
            if (query.UserID.HasValue && query.UserID>0)
            {
                expable.And(it => it.UserID==query.UserID);
            }
            var exp = expable.ToExpression();
            var models = await entryRecordService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
    }
}
