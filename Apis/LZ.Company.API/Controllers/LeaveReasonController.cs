using LZ.Company.Service.Interface;
using LZ.Company.Service.Services;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace LZ.Company.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class LeaveReasonController(ILeaveReasonService leaveReasonService, IRedisService redisService) : CompanyControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]BasicPagedModel query)
        {
            var user = CurrentUser;
            var key = user.CompanyID.ToString();
            var expable = Expressionable.Create<Model.Live_LeaveReason>();
            if (query.From == "cache")//读取缓存数据
            {
                var items = await redisService.GetAsync<List<Live_LeaveReason>>(string.Format(CoreRedisConstants.CompanyLeaveReason, key));
                if (items != null && items.Count > 0)
                {
                    return Ok(new { items = items, count = 0 });
                }
            }
            if (user.PlatformType== Model.Enums.PlatformType.Admin)
            {

            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            var exp= expable.ToExpression();
            var models = await leaveReasonService.List(exp, query);
            if (query.From == "cache")
            {
                await redisService.SetAsync(string.Format(CoreRedisConstants.CompanyLeaveReason, key), models, TimeSpan.FromHours(10));
            }
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_LeaveReason model)
        {
            var user = CurrentUser;
            model.CompanyID = user.CompanyID;
            model.CompanyGID = user.CompanyGID;
            await leaveReasonService.Add(model);
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyLeaveReason, user.CompanyID));
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_LeaveReason> Get(int id)
        {
            var model = await leaveReasonService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_LeaveReason model)
        {
            if (!await leaveReasonService.Update(model))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            var user = CurrentUser;
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyLeaveReason, user.CompanyID));
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
          var has = await leaveReasonService.Delete(id,CurrentUser.CompanyID);
            if (!has)
            {
                throw new ErrorCodeException(-1, "删除失败");
            }
            var user = CurrentUser;
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyLeaveReason, user.CompanyID));
            return Ok();
        }
    }
}
