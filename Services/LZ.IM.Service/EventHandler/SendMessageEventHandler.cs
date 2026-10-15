using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.IM.Service.EventHandler
{
    public class SendMessageEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<SendMessageEventData>
    {
        public async Task HandleAsync(SendMessageEventData @event)
        {
            try
            {
                if (string.IsNullOrEmpty(@event.From) || string.IsNullOrEmpty(@event.Json)) return;
                using var scope = scopeFactory.CreateScope();
                var hubContext = scope.ServiceProvider.GetService<IHubContext<LZ.Core.Services.MessageService>>();
                await hubContext.Clients.Groups(@event.From).SendAsync("ReceiveMessage", @event.Json);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
        }
    }
}
