using LZ.Core.Interface;
using LZ.Core.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Jobs.BusInsurance
{
  
    /// <summary>
    /// 利宝保险 商保推送
    /// </summary>

    [DisallowConcurrentExecution]
    public class PushYangGuangProcessJob : JobBase, IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string? parameter = context.MergedJobDataMap["Parameter"] as string;
            if (!string.IsNullOrEmpty(parameter))
            {
                dic = CreateDictionary(parameter);
            }
            var httpService = context.MergedJobDataMap["HttpSrvice"] as IHttpService;
            var obj = await httpService.PostAsync(ServiceNames.UsersService, "/api/jobtask/PushYangGuangProcessJob", dic);
        }
    }
}
