using LZ.Core.Interface;
using LZ.Core.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar.IOC;

namespace LZ.Basic.Service.Jobs.Finance
{
    /// <summary>
    /// 从客户工资中获取暂缓工资推送到代发
    /// </summary>
    /// <returns></returns>
    [DisallowConcurrentExecution]
    public class HandleSalaryCashJob : JobBase, IJob
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
            var obj = await httpService.PostAsync(ServiceNames.FinanceService, "/api/jobtask/HandleSalaryCashJob", dic);

            //var db = base.Context;
            //Model.Live_SchedulerLog log = new Model.Live_SchedulerLog();
            //log.Content = "TransSingleSalaryRecordPushJob测试";
            //log.Remark = obj.ToString();
            //log.CreateTime = DateTime.Now;
            //db.Insertable(log).ExecuteCommand();
        }
    }
}
