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
    public class CompanyDictionaryController(ICompanyDictionaryService CompanyDictionaryService, IRedisService redisService) : CompanyControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]BasicPagedModel query)
        {
            var key = "";
            var expable = Expressionable.Create<Model.SYS_CompanyDictionary>();
            if (query.PID.HasValue)
            {
                key = query.PID.ToString().ToLower();
                expable.And(it => it.PID ==query.PID);
            }
            if (!string.IsNullOrEmpty(query.Ids))
            {
                key = query.Ids.ToLower();
                var ids = query.Ids.Split(',');
                expable.And(it => it.PID.HasValue && ids.Contains(it.PID.ToString()));
            }
            //if (query.From == "cache" && !string.IsNullOrEmpty(key))//读取缓存数据
            //{
            //    var items = await redisService.GetAsync<List<SYS_CompanyDictionary>>(string.Format(CoreRedisConstants.CompanyDictionary, key));
            //    if (items != null && items.Count > 0)
            //    {
            //        return Ok(new { items = items, count = 0 });
            //    }
            //}
            if (CurrentUser.PlatformType== Model.Enums.PlatformType.Admin || (query.PID.HasValue && query.PID == Guid.Empty))
            {
                expable.AndIF(query.CompanyID.HasValue, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == CurrentUser.CompanyID);
            }
            var exp= expable.ToExpression();
            var models = await CompanyDictionaryService.List(exp, query);
            //if (query.From == "cache" && !string.IsNullOrEmpty(key))
            //{
            //    await redisService.SetAsync(string.Format(CoreRedisConstants.CompanyDictionary, key), models, TimeSpan.FromHours(10));
            //}
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.SYS_CompanyDictionary model)
        {
            model.GUID = Guid.NewGuid();
            model.CompanyID = CurrentUser.CompanyID;
            await CompanyDictionaryService.Add(model);
            if(model.PID.HasValue)
            {
                await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyDictionary, model.PID.ToString().ToLower()));
            }
            return Ok();
        }
        [HttpGet]
        public async Task<Model.SYS_CompanyDictionary> Get(int id)
        {
            var model = await CompanyDictionaryService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SYS_CompanyDictionary model)
        {
            if (!await CompanyDictionaryService.Update(model))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            if (model.PID.HasValue)
            {
                await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyDictionary, model.PID.ToString().ToLower()));
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await CompanyDictionaryService.Set(id, true))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (await CompanyDictionaryService.Set(id, false))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
          var has = await CompanyDictionaryService.Delete(id,CurrentUser.CompanyID);
            if (!has)
            {
                throw new ErrorCodeException(-1, "删除失败");
            }
            var model = await CompanyDictionaryService.Get(id);
            if (model!=null && model.PID.HasValue)
            {
                await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyDictionary, model.PID.ToString().ToLower()));
            }
            return Ok();
        }
    }
}
