using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Core;
using LZ.Basic.Service.Interface;

namespace LZ.Basic.Service.EventHandler
{
    public class OperationLogEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<OperationLogEventData>
    {
        public async Task HandleAsync(OperationLogEventData @event)
        {
            using var scope = scopeFactory.CreateScope();
            var operationLogService = scope.ServiceProvider.GetService<IOperationLogService>();
            var model = new LZ.Model.SYS_OperationLog()
            {
                Code = @event.Code,
                Content = @event.Content,
                Json = @event.Json,
                UserID = @event.UserID,
                IpAddress = @event.IpAddress,
                UserAgent = @event.UserAgent,
                Method = @event.Method,
                CreateTime = @event.CreateTime,
                Platform= @event.Platform,
                FullName = @event.FullName,
                CompanyID = @event.CompanyID
            };
            await operationLogService.Insert(model);
        }
    }
}
