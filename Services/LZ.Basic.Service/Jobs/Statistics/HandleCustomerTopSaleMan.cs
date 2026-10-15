using LZ.Basic.Service.Interface;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ;
using Quartz;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text.Json;
using LZ.Basic.Service.Jobs.Customer;
using LZ.Core.Interface;
using LZ.Core.Services;

namespace LZ.Basic.Service.Jobs.Statistics
{
    public class HandleCustomerTopSaleMan : JobBase, IJob
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
            var obj = await httpService.PostAsync(ServiceNames.CustomerService, "/api/jobtask/HandleEntryContractSendSmsJob", dic);

            //var db = base.Context;
            //Model.Live_SchedulerLog log = new Model.Live_SchedulerLog();
            //log.Content = "HandleEntryContractSendSmsJob测试";
            //log.Remark = obj.ToString();
            //log.CreateTime = DateTime.Now;
            //db.Insertable(log).ExecuteCommand();


        }
    }
}
