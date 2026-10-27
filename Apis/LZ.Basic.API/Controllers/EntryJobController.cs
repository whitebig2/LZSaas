using LZ.Basic.Service.Interface;
using LZ.Common;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Runtime.InteropServices;

namespace LZ.Basic.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class EntryJobController(IEntryJobService entryJobService,IRedisService redisService) : BasicControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery]PagedModelBase query)
        {
            var user = CurrentUser;
            if (query.From == "cache")//读取缓存数据
            {
                query.Page = 0;
                var items = await redisService.GetAsync<List<Live_EntryJob>>(string.Format(CoreRedisConstants.CompanyWorks, "All"));
                if (items != null && items.Count > 0)
                {
                    return Ok(new { items = items, count = 0 });
                }
            }
            var expable = Expressionable.Create<Model.Live_EntryJob>();

            expable.AndIF(!string.IsNullOrEmpty(query.Name), it => it.Name.Contains(query.Name));

            expable.AndIF(!string.IsNullOrEmpty(query.Keyword), it => it.Type.Contains(query.Keyword));

            var exp= expable.ToExpression();
            var models = await entryJobService.List(exp, query);
            if (query.From == "cache")//读取缓存数据
            {
                await redisService.SetAsync(string.Format(CoreRedisConstants.CompanyWorks, "All"), models, TimeSpan.FromHours(10));
            }
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]Model.Live_EntryJob model)
        {
            var user = CurrentUser;
            //model.CompanyID= user.CompanyID;
            model.CompanyID = 0;
            model.CreateTime = DateTime.Now;
            await entryJobService.Add(model);
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyWorks, "All"));
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpGet]
        public async Task<IActionResult> ExportExcel([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_EntryJob>();
            expable.AndIF(!string.IsNullOrEmpty(query.Name), it => it.Name.Contains(query.Name));

            expable.AndIF(!string.IsNullOrEmpty(query.Keyword), it => it.Type.Contains(query.Keyword));
            var exp = expable.ToExpression();
            query.Page = 0;
            var models = await entryJobService.List(exp, query);
            var excelData = models.Select(it => new EntryJobExcelTemplate()
            {
                Name = it.Name,
                Type = it.Type
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(excelData, "工种");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = DateTime.Now.ToString("yyyy-MM-dd") + "工种.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }
        [HttpGet]
        public async Task<Model.Live_EntryJob> Get(int id)
        {
            var model= await entryJobService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_EntryJob model)
        {
            var user = CurrentUser;
            if (!await entryJobService.Update(model))
            {
                throw new ErrorCodeException(-1, "修改错误");
            }
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyWorks, "All"));
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            await entryJobService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            await entryJobService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var user = CurrentUser;
            var has = await entryJobService.Delete(id, user.CompanyID);
            if (!has)
            {
                throw new ErrorCodeException(-1, "删除失败");
            }
            await redisService.DeleteAsync(string.Format(CoreRedisConstants.CompanyWorks, "All"));
            return Ok();
        }
    }
}
