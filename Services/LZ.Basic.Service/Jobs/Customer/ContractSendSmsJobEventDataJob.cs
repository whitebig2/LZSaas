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

namespace LZ.Basic.Service.Jobs.Customer
{
    [DisallowConcurrentExecution]
    internal class ContractSendSmsJobEventDataJob : JobBase, IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Common.StaticLog.Error("ContractSendSmsJobEventDataJob定时任务");
            try
            {
                ContractSendSmsJobEventDataJob eventData = new ContractSendSmsJobEventDataJob();
                var eventName = eventData.GetType().FullName;
                var body = JsonSerializer.Serialize(eventData);
                var factory = CreateConnectionFactory();
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true);
                    channel.BasicPublish(ExchangeName, eventName, null, Encoding.UTF8.GetBytes(body));
                }
                // eventPublisher.Publish(eventData);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }

            return Task.CompletedTask;
        }
    }
}
