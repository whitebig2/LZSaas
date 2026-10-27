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

namespace LZ.Basic.Service.Jobs.Customer
{

    /// <summary>
    ///  入离职合同--合同存证
    /// </summary>
    /// <returns></returns>
    [DisallowConcurrentExecution]
    public class HandleEntryContractEvidenceJob : JobBase, IJob
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
            var obj = await httpService.PostAsync(ServiceNames.CustomerService, "/api/jobtask/HandleEntryContractEvidenceJob", dic);

            //var db = base.Context;
            //Model.Live_SchedulerLog log = new Model.Live_SchedulerLog();
            //log.Content = "HandleGenerateEntryContractJob测试";
            //log.Remark = obj.ToString();
            //log.CreateTime = DateTime.Now;
            //db.Insertable(log).ExecuteCommand();


        }
    }
}
