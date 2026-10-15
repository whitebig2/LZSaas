using LZ.Basic.Service.Interface;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.EventHandler
{
    public class NotificationEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<NotificationEventData>
    {
        public async Task HandleAsync(NotificationEventData @event)
        {
            using var scope = scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetService<INotificationService>();
            var model = new LZ.Model.Live_Notification()
            {
                CompanyID = @event.CompanyID,
                CompanyGID = @event.CompanyGID,
                Content = @event.Content,
                PartnerID = @event.PartnerID,
                PartnerGID = @event.PartnerGID,
                CreateTime = DateTime.Now,
                Status = @event.Status,
                Type = @event.Type,
                TypeName = @event.TypeName,
                Title = @event.Title
            };
            await notificationService.Add(model);
        }
    }
}
