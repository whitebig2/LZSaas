using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Jobs.Customer
{
    [DisallowConcurrentExecution]
    public class GenerateSettlementJob : IJob, IScopedDependency
    {

        public async Task Execute(IJobExecutionContext context)
        {
            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    var sp = scope.ServiceProvider;
            //    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            //    var httpClient = httpClientFactory.CreateClient(ServiceNames.FinanceService);
            //    var resp = await httpClient.PutAsync("/api/SalaryRecord/GenerateSalaryRecordSettlementJob", null);
            //    Common.StaticLog.Debug(resp.ToString());
            //}
            Common.StaticLog.Error("郑工定时任务");
            var httpService = context.MergedJobDataMap["HttpSrvice"] as IHttpService;
            var obj = await httpService.PostAsync(ServiceNames.FinanceService, "/api/SalaryRecord/GenerateSalaryRecordSettlementJob","");
            Common.StaticLog.Error(obj.ToString());

        }
    }
}
