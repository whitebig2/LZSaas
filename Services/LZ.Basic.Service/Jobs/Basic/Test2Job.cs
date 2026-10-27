using LZ.Basic.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Core.Services;
using Quartz;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Jobs.Basic
{
    public class Test2Job : JobBase,IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //var db = base.Context;
            //Model.Live_SchedulerLog log = new Model.Live_SchedulerLog();
            //log.Content = "Test2Job测试";
            //log.CreateTime = DateTime.Now;
            //db.Insertable(log).ExecuteCommand();

            Dictionary<string, string> dic = new Dictionary<string, string>();

            string? parameter = context.MergedJobDataMap["Parameter"] as string;
            if (!string.IsNullOrEmpty(parameter))
            {
                dic = CreateDictionary(parameter);
            }

            var httpService = context.MergedJobDataMap["HttpSrvice"] as IHttpService;
            var obj = await httpService.PostAsync(ServiceNames.BasicService, "/api/Scheduler/JobTest", dic);
            var redisService = context.MergedJobDataMap["RedisService"] as IRedisService;
            var dayTotalVisits = await redisService.GetAsync<int>(CoreRedisConstants.DayTotalVisits);
            await redisService.IncrByAsync(CoreRedisConstants.TotalVisits, dayTotalVisits);
            await redisService.SetAsync(CoreRedisConstants.DayTotalVisits, 0);
        }
    }
}
