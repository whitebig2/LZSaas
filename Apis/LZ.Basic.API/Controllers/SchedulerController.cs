using LZ.Basic.Service.Interface;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using LZ.Basic.Service.Jobs;
using Microsoft.VisualBasic;
using Quartz;
using System.Drawing.Drawing2D;
using static Quartz.Logging.OperationName;
using LZ.Common;
using System.Net.Http;
using LZ.Basic.Service.Services;
using LZ.Core.Interface;
using LZ.Core.Redis;

namespace LZ.Basic.API.Controllers
{
    public class SchedulerController(ISchedulerService schedulerService, IHttpService httpService, IRedisService redisService) : BasicControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] BasicPagedModel query)
        {
            var cid = CurrentUser.CompanyID;
            var expable = Expressionable.Create<Model.Live_Scheduler>();
            if(query.Enabled.HasValue)
            {
                expable.And(t => t.IsRun == query.Enabled);
            }
            var exp = expable.ToExpression();
            var models = await schedulerService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_Scheduler model)
        {
            var expable = Expressionable.Create<Model.Live_Scheduler>();
            expable.And(t => t.Name == model.Name);
            if (await schedulerService.Exist(expable.ToExpression()))
            {
                throw new ErrorCodeException(-1, "该标签已存在!");
            }

            model.CreateTime = DateTime.Now;
            model.EditTime = DateTime.Now;
            if(model.IsRun)
            {
                bool isRun = await schedulerService.RunJob(model, httpService,redisService);
                model.IsRun = isRun;
            }
            await schedulerService.Add(model);


            return Ok();
        }
        [HttpGet]
        public async Task<Model.Live_Scheduler> Get(int id)
        {
            return await schedulerService.Get(id);
            //return await userService.Get(CurrentUser.UserID);
        }
        [HttpGet]
        public string test()
        {

            return MoneyTypeChange.test()+ "basci新增";
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_Scheduler model)
        {
            if (!await schedulerService.ExistID(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await schedulerService.DeleteJob(model);
            if (model.IsRun)
            {
                var isRun = await schedulerService.RunJob(model, httpService, redisService);
                model.IsRun = isRun;
                model.EditTime = DateTime.Now;
            }
            var models = await schedulerService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await schedulerService.ExistID(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await schedulerService.Delete(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] int id, [FromForm] string method)
        {
            var sch = await schedulerService.Get(id);
            var isOk = false;
            if (method == "resum")
            {
                isOk = await schedulerService.RunJob(sch, httpService, redisService);
                if (isOk)
                {
                    sch.IsRun = true;
                    sch.EditTime = DateTime.Now;
                    await schedulerService.Update(sch);
                    return Ok("运行成功");
                }
                
            }
            else if (method == "pause")
            {
                isOk = await schedulerService.PauseJob(sch);
                if (isOk)
                {
                    sch.IsRun = false;
                    sch.EditTime = DateTime.Now;
                    await schedulerService.Update(sch);
                    return Ok("暂停成功");
                }
            }
            else if (method == "delete")
            {
                isOk = await schedulerService.DeleteJob(sch);
                if (isOk)
                {
                    await schedulerService.Delete(id);
                    return Ok("删除成功");
                }
            }
            return Ok("操作错误");
        }

        [HttpGet]
        public async Task<IActionResult> RunJob(int id)
        {

            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddScoped<ISchedulerService, SchedulerService>();
            //var serviceProvider = serviceCollection.BuildServiceProvider();
            var model = await schedulerService.Get(id);
            await schedulerService.RunJob(model, httpService, redisService);


            return Ok();
        }

        [HttpGet]
        public IActionResult Keys()
        {

            JobFactoryService factoryServicee = new JobFactoryService();
            var keys = factoryServicee.KeyList();
            return Ok(keys);
        }



        [HttpGet]
        public async Task<IActionResult> Process()
        {
            List<LZ.Model.Model.SchedulerModel> models = new List<LZ.Model.Model.SchedulerModel>();
            models = await schedulerService.Process();
           

            return Ok(new { items= models });
        }

        [HttpGet]
        public async Task<IActionResult> StartAll()
        {
            Jobs.QuartzStartup b = new Jobs.QuartzStartup();
            await b.Start(httpService, redisService);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> JobTest([FromForm]string content, [FromForm] string remark)
        {
            Model.Live_SchedulerLog log = new Model.Live_SchedulerLog();
            log.Content = content;
            log.Remark = remark;
            log.CreateTime = DateTime.Now;
            await schedulerService.Add(log);
            return Ok();
        }

    }
}
