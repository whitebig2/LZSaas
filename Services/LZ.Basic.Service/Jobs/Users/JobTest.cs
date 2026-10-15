using LZ.Core.Interface;
using LZ.Core.Services;
using NetTaste;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Senparc.CO2NET.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LZ.Basic.Service.Jobs.Users
{
    public class JobTest : JobBase, IJob
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
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var weakService = context.MergedJobDataMap["HttpSrvice"] as WeakReference;
            string? parameter = context.MergedJobDataMap["Parameter"] as string;
            if(!string.IsNullOrEmpty(parameter))
            {
                dic = CreateDictionary(parameter);
            }
            if (weakService != null && weakService.IsAlive)
            {
                var httpService = weakService.Target as IHttpService;
                var obj = await httpService.PostAsync(ServiceNames.UsersService, "/api/WhiteList/JobTest", dic);
                weakService = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }


        }
    }
}
