using Microsoft.AspNetCore.Mvc;
using LZ.Core;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.Customer.Service.Interface;
using LZ.Customer.API;
using LZ.Core.Attributes;
using LZ.Model.PagedModel;

namespace LZ.Customer.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    
    public class JobClassController(IJobClassService jobClassService) : CustomerControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_JobClass>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name.Contains(query.Name));
            }
            if (query.Enabled.HasValue)
            {
                expable.And(it => it.Enabled== query.Enabled);
            }
            var exp = expable.ToExpression();
            var models = await jobClassService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_JobClass model)
        {
            await jobClassService.Add(model);
            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_JobClass> Get(int id)
        {
            var model = await jobClassService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await jobClassService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await jobClassService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await jobClassService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await jobClassService.Set(id, false);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_JobClass model)
        {
            if (!await jobClassService.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await jobClassService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await jobClassService.Delete(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            return Ok();
        }
    }
}
