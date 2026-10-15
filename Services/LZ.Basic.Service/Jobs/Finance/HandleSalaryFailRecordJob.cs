//using LZ.Core.Interface;
//using LZ.Core.Services;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using Quartz;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SqlSugar.IOC;

//namespace LZ.Basic.Service.Jobs.Finance
//{

//    /// <summary>
//    ///  客户工资系统暂缓处理定时任务
//    /// </summary>
//    /// <returns></returns>
//    [DisallowConcurrentExecution]
//    public class HandleSalaryFailRecordJob : JobBase, IJob
//    {
//        public async Task Execute(IJobExecutionContext context)
//        {
//            Dictionary<string, string> dic = new Dictionary<string, string>();

//            string? parameter = context.MergedJobDataMap["Parameter"] as string;
//            if (!string.IsNullOrEmpty(parameter))
//            {
//                dic = CreateDictionary(parameter);
//            }

//            var httpService = context.MergedJobDataMap["HttpSrvice"] as IHttpService;
//            var obj = await httpService.PostAsync(ServiceNames.FinanceService, "/api/jobtask/HandleSalaryFailRecordJob", dic);

//        }
//    }
//}
