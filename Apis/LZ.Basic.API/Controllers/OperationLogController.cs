using LZ.Basic.Service.Interface;
using LZ.Basic.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    public class OperationLogController(IOperationLogService operationLogService,IRedisService redisService) : BasicControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]BasicPagedModel query)
        {
            var expable = Expressionable.Create<Model.SYS_OperationLog>();
            expable.AndIF(query.StartTime.HasValue, it => it.CreateTime >= query.StartTime.Value);
            expable.AndIF(query.EndTime.HasValue, it => it.CreateTime <= query.EndTime.Value);
            expable.AndIF(!string.IsNullOrEmpty(query.Keyword), it => it.Content.Contains(query.Keyword));

            expable.AndIF(!string.IsNullOrEmpty(query.Name), it => it.FullName== query.Name);

            expable.AndIF(!string.IsNullOrEmpty(query.From), it => it.Platform == Convert.ToInt32(query.From));
            if (CurrentUser.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            var exp= expable.ToExpression();
            var models = await operationLogService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
    }
}
