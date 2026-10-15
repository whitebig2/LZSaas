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

namespace LZ.Basic.Service.Jobs.Basic
{
    public class SchedulerTestJob : JobBase, IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                SchedulerTestEventData eventData = new SchedulerTestEventData()
                {
                    Content = "定时任务测试",
                    Remark = "备注"
                };
                var eventName = eventData.GetType().FullName;
                var body = JsonSerializer.Serialize(eventData);
                var factory = CreateConnectionFactory();
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true);
                    channel.BasicPublish(ExchangeName, eventName, null, Encoding.UTF8.GetBytes(body));
                }
            }
           catch(Exception ex)
            {
                Common.StaticLog.Error(ex);
            }

            return Task.CompletedTask;
        }
    }
}
