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
    public class DictionaryController(IDictionaryService dictionaryService,IRedisService redisService) : BasicControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]BasicPagedModel query)
        {
            var key = "";
            var expable = Expressionable.Create<Model.SYS_Dictionary>();
            if (query.PID.HasValue)
            {
                key = query.PID.ToString().ToLower();
                expable.And(it => it.PID ==query.PID);
            }
            if (query.Enabled.HasValue)
            {
                expable.And(it => it.Enabled == query.Enabled);
            }
            if (!string.IsNullOrEmpty(query.Ids))
            {
                key = query.Ids.ToLower();
                var ids = query.Ids.Split(',');
                expable.And(it => it.PID.HasValue && ids.Contains(it.PID.ToString()));
            }

            if (!string.IsNullOrEmpty(query.Types))
            {
                var types = query.Types.Split(',');
                expable.And(it => types.Contains(it.NO.ToString()));
            }

            if (query.From== "cache" && !string.IsNullOrEmpty(key))//读取缓存数据
            {
                var items = await redisService.GetAsync<List<SYS_Dictionary>>(string.Format(CoreRedisConstants.Dictionary, key));
                if (items != null && items.Count > 0)
                {
                    return Ok(new { items = items, count = 0 });
                }
            }
            var exp= expable.ToExpression();
            var models = await dictionaryService.List(exp, query);
            if (query.From == "cache" && !string.IsNullOrEmpty(key))
            {
                await redisService.SetAsync(string.Format(CoreRedisConstants.Dictionary, key), models, TimeSpan.FromHours(10));
            }
            return Ok(new { items = models, count = query.Count });
        }



        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.SYS_Dictionary model)
        {
            model.GUID = Guid.NewGuid();
            await dictionaryService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.SYS_Dictionary> Get(int id)
        {
            if (!await dictionaryService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return await dictionaryService.Get(id);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.SYS_Dictionary model)
        {
            if (!await dictionaryService.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await dictionaryService.Update(model);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await dictionaryService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await dictionaryService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await dictionaryService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await dictionaryService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await dictionaryService.Delete(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            return Ok();
        }

        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> GetChildrenByName(string name)
        {
            var entity = await dictionaryService.Get(f => f.Enabled && f.Name == name);
            if (entity == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            var expable = Expressionable.Create<Model.SYS_Dictionary>();
            expable.And(f => f.Enabled);
            expable.And(f => f.PID == entity.GUID);
            var data = await dictionaryService.List(expable.ToExpression(), new PagedModelBase { Page = 0 });
            return Ok(data);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> TreeList([FromQuery] BasicPagedModel query)
        {
            var key = "";
            var expable = Expressionable.Create<Model.SYS_Dictionary>();
            if (query.PID.HasValue)
            {
                key = query.PID.ToString().ToLower();
                expable.And(it => it.PID == query.PID);
            }
            if (!string.IsNullOrEmpty(query.Ids))
            {
                key = query.Ids.ToLower();
                var ids = query.Ids.Split(',');
                expable.And(it => it.PID.HasValue && ids.Contains(it.PID.ToString()));
            }

            if (!string.IsNullOrEmpty(query.Types))
            {
                var types = query.Types.Split(',');
                expable.And(it => types.Contains(it.NO.ToString()));
            }

            var exp = expable.ToExpression();
            var models = await dictionaryService.List(exp, query);
            string subIDS = "";
            foreach(var x in models)
            {
                string dot = subIDS == "" ? "" : ",";
                subIDS += dot + x.GUID;
            }
            var subExpable = Expressionable.Create<Model.SYS_Dictionary>();
            if (!string.IsNullOrEmpty(subIDS))
            {
                key = subIDS.ToLower();
                var ids = subIDS.Split(',');
                subExpable.And(t => t.Enabled);
                subExpable.And(it => it.PID.HasValue && ids.Contains(it.PID.ToString()));
            }
            var smodels = await dictionaryService.List(subExpable.ToExpression(), query);

            return Ok(new { items = models,sub=smodels });
        }
    }
}
