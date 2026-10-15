using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System.Linq.Expressions;
using LZ.Basic.Service.Jobs.Basic;
using Quartz.Impl;
using Quartz;
using Quartz.Impl.Matchers;
using LZ.Model.Exceptions;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.RabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LZ.Core.EventData;
using LZ.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Senparc.Weixin.WxOpen.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LZ.Core.Redis;

namespace LZ.Basic.Service.Services
{
    public class SchedulerService : ServicesBase<Live_Scheduler>, ISchedulerService, IScopedDependency
    {
        public async Task<List<Model.Live_Scheduler>> List(Expression<Func<Model.Live_Scheduler, bool>> exp, BasicPagedModel query, string order = "")
        {
            var count = new RefAsync<int>();
            var sco = base.Context.Queryable<Live_Scheduler>().Where(exp);
            if (!string.IsNullOrEmpty(order))
            {
                sco.OrderBy(order);
            }

            if (query.Page == 0)
            {
                return await sco.ToListAsync();
            }
            var list = await sco.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<int> Add(Model.Live_Scheduler model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Add(Model.Live_SchedulerLog model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

        public async Task<int> Adds(List<Model.Live_Scheduler> models)
        {
            var db = base.Context;
            return await db.Insertable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Update(Model.Live_Scheduler model)
        {
            var db = base.Context;
            model.EditTime = DateTime.Now;
            return await db.Updateable(model).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var db = base.Context;
            return await db.Deleteable<Model.Live_Scheduler>().Where(t => t.ID == id).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> Exist(Expression<Func<Live_Scheduler, bool>> exp)
        {
            return await base.Context.Queryable<Live_Scheduler>().AnyAsync(exp);
        }
        public async Task<Model.Live_Scheduler> Get(int id)
        {
            var db = base.Context;
            return await db.Queryable<Model.Live_Scheduler>().Where(t => t.ID == id).FirstAsync();
        }
        public async Task<bool> ExistID(int id)
        {
            return await base.Context.Queryable<Live_Scheduler>().AnyAsync(it => it.ID == id);
        }

        public async Task<int> AddLog(Model.Live_SchedulerLog model)
        {
            var db = base.Context;
            return await db.Insertable(model).ExecuteCommandAsync();
        }

       

        public async Task<bool> RunJob(Model.Live_Scheduler sch, IHttpService httpService, IRedisService redisService)
        {
            var factory = new StdSchedulerFactory();
            var scheduler = factory.GetScheduler().Result;
            // scheduler.JobFactory = new InjectingJobFactory(serviceProvider);
            await scheduler.Start();
            bool isRun = false;
            try
            {
                JobKey key = new JobKey(sch.Name+sch.JobName, sch.JobGroup);
                if (await scheduler.CheckExists(key))
                {
                    await scheduler.ResumeJob(key);
                }
                else
                {
                    
                    Jobs.JobFactoryService factoryServicee = new Jobs.JobFactoryService();
                    IJobDetail job;
                    try
                    {
                        var jobDetai = factoryServicee.CreateJob(sch.JobName);
                        if(sch.Method==1)
                        {
                            //var map = new JobDataMap { { "HttpSrvice", new WeakReference(httpService) } };
                            var map = new JobDataMap { { "HttpSrvice", httpService } };
                            if (!string.IsNullOrEmpty(sch.Parameter) && sch.Parameter!="[]")
                            {
                                map.Add("Parameter", sch.Parameter);
                            }
                            map.Add("RedisService", redisService);
                            jobDetai.SetJobData(map);
                        }

                        
                        job = jobDetai.WithIdentity(sch.Name + sch.JobName, sch.JobGroup)
                            .Build();

                    }
                    catch(Exception e)
                    {
                        Common.StaticLog.Error(e.ToString());
                        throw new ErrorCodeException(-1, "未创建定时任务，无法匹配");
                    }
                   
                    //var job = JobBuilder.Create<TestJob>()
                    //.WithIdentity(sch.JobName, sch.JobGroup)
                    //.Build();
                    //3、创建一个触发器
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(sch.TriggerName, sch.TriggerGroup)
                        .WithCronSchedule(sch.Cron)
                        .Build();
                    await scheduler.ScheduleJob(job, trigger);
                }
                isRun = true;
            }
            catch (Exception e)
            {
                throw new ErrorCodeException(-1, e.ToString());
            }
            return isRun;
        }

        public async Task<bool> PauseJob(Model.Live_Scheduler sch)
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();
            bool isPause = false;
            try
            {
                JobKey key = new JobKey(sch.Name + sch.JobName, sch.JobGroup);
                if (await scheduler.CheckExists(key))
                {
                    await scheduler.PauseJob(key);
                }
                isPause = true;
            }
            catch (Exception e)
            {
                throw new ErrorCodeException(-1, e.ToString());
            }
            return isPause;
        }


        public async Task<bool> DeleteJob(Model.Live_Scheduler sch)
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();
            bool isDele = true;
            try
            {
                JobKey key = new JobKey(sch.Name + sch.JobName, sch.JobGroup);
                if (await scheduler.CheckExists(key))
                {
                    await scheduler.DeleteJob(key);
                }
            }
            catch (Exception e)
            {
                throw new ErrorCodeException(-1, e.ToString());
            }
            Common.StaticLog.Error(sch.Name + sch.JobName + ":删除成功");
            return isDele;
        }
       



        public async Task<List<LZ.Model.Model.SchedulerModel>> Process()
        {
            List<LZ.Model.Model.SchedulerModel> models = new List<LZ.Model.Model.SchedulerModel>();
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();
            var list = scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            foreach (var item in list.Result)
            {
                JobKey key = new JobKey(item.Name, item.Group);
                var trigger = scheduler.GetTriggersOfJob(key);
                foreach (var tri in trigger.Result)
                {
                    var cron = (ICronTrigger)tri;
                    LZ.Model.Model.SchedulerModel model = new LZ.Model.Model.SchedulerModel();
                    model.jobName = item.Name;
                    model.jobGroup = item.Group;
                    model.Cron = string.IsNullOrEmpty(cron.CronExpressionString)?"未知":cron.CronExpressionString;
                    model.triggerName = cron.Key.Name;
                    model.triggerGroup = cron.Key.Group;
                    TriggerKey tkey = new TriggerKey(model.triggerName, model.triggerGroup);
                    var state = await scheduler.GetTriggerState(tkey);

                    switch (state.ToString())
                    {
                        case "Normal":
                            model.statusName = "运行中";
                            break;
                        case "Paused":
                            model.statusName = "暂停中";
                            break;
                        case "Complete":
                            model.statusName = "已完成";
                            break;
                        case "Error":
                            model.statusName = "有错误";
                            break;
                        case "Blocked":
                            model.statusName = "阻塞中";
                            break;
                        case "None":
                            model.statusName = "不存在";
                            break;
                    }
                    models.Add(model);
                }
            }
            return models;
        }
    }
}
