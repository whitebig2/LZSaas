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
    /// 每个月预估服务费计算任务
    /// </summary>
    /// <returns></returns>
    [DisallowConcurrentExecution]
    public class CustomerServiceChargeJob : JobBase, IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var httpService = context.MergedJobDataMap["HttpSrvice"] as IHttpService;
            var obj = await httpService.GetAsync(ServiceNames.FinanceService, "/api/customerServiceCharge/batchtest");
        }
    }
}
