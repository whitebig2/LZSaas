using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Xml;
using Quartz;
using LZ.Model.PagedModel;
using LZ.Basic.Service.Services;
using SqlSugar;
using LZ.Core.Interface;
using LZ.Core.Redis;

namespace LZ.Basic.API.Jobs
{
    public class QuartzStartup()
    {
        public async Task Start(IHttpService httpService,IRedisService redisService)
        {
            //XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());
            //ISchedulerFactory factory = new StdSchedulerFactory();
            //IScheduler sched = await factory.GetScheduler();
            //await processor.ProcessFileAndScheduleJobs(System.IO.Directory.GetCurrentDirectory() + "/Config/quartz_jobs.xml", sched);
            //await sched.Start();
            Common.StaticLog.Error("启动运行定时任务");
            SchedulerService schedulerService = new SchedulerService();
            Model.PagedModel.BasicPagedModel query = new Model.PagedModel.BasicPagedModel();
            query.Page = 0;
            query.Enabled = true;
            var expable = Expressionable.Create<Model.Live_Scheduler>();
            expable.And(t => t.IsRun == query.Enabled);
            var models = await schedulerService.List(expable.ToExpression(), query);
            if (models.Count > 0)
            {
                foreach (var x in models)
                {
                    await schedulerService.RunJob(x, httpService, redisService);
                }
            }
        }
    }
}
