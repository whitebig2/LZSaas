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
    public class ClearDayTotalVisits : JobBase,IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {

            var redisService = context.MergedJobDataMap["RedisService"] as IRedisService;
            var dayTotalVisits = await redisService.GetAsync<int>(CoreRedisConstants.DayTotalVisits);
            await redisService.IncrByAsync(CoreRedisConstants.TotalVisits, dayTotalVisits);
            await redisService.SetAsync(CoreRedisConstants.DayTotalVisits, 0);

            var dayTotalChangeWork = await redisService.GetAsync<int>(CoreRedisConstants.DayTotalChangeWork);
            await redisService.IncrByAsync(CoreRedisConstants.TotalChangeWork, dayTotalChangeWork);
            await redisService.SetAsync(CoreRedisConstants.DayTotalChangeWork, 0);

            var dayTotalChangeCompany = await redisService.GetAsync<int>(CoreRedisConstants.DayTotalChangeCompany);
            await redisService.IncrByAsync(CoreRedisConstants.TotalChangeCompany, dayTotalChangeCompany);
            await redisService.SetAsync(CoreRedisConstants.DayTotalChangeCompany, 0);
        }
    }
}
