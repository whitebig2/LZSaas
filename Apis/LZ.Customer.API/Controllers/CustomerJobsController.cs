using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;

namespace LZ.Customer.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class CustomerJobsController(ICustomerJobsService customerJobsService, ICustomerService customerService,IEntryJobService entryJobService ) : CustomerControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var models = await customerJobsService.List(it=>it.CustomerID== query.CID);
            return Ok(models);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_CustomerJobs model)
        {
            if (await customerJobsService.Exist(it => it.CustomerID == model.CustomerID && it.EntryJobID == model.EntryJobID))
            {
                throw new ErrorCodeException(-1, "该工种已关联，请重新新增");
            }
            if(await customerJobsService.Add(model) > 0 && model.IsDefault)
            {
                var work = await entryJobService.Get(model.EntryJobID);
                if (work != null)
                {
                    await customerService.ChangeWork(model.CustomerID,work.Name,work.Type);
                }
            }
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ChangeDefault([FromForm] int id, [FromForm] int jobid, [FromForm] bool isDefault)
        {
            if (!await customerJobsService.Exist(it => it.CustomerID == id && it.EntryJobID == jobid))
            {
                throw new ErrorCodeException(-1, "不存在该关联");
            }
           var has = await customerJobsService.UpdateDefault(id, jobid, isDefault);
            if (isDefault && has>0)
            {
                var model = await entryJobService.Get(jobid);
                if (model != null)
                {
                    await customerService.ChangeWork(id, model.Name, model.Type);
                }
            }

            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id, [FromForm] int jobid)
        {
            var user = CurrentUser;
            if (!await customerJobsService.Exist(it=>it.CustomerID==id && it.EntryJobID==jobid && it.CompanyID==user.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await customerJobsService.Delete(id, jobid);
            return Ok();
        }
    }
}
