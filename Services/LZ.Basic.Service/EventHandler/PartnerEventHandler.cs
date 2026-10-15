using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.EventHandler
{
    public class PartnerEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<PartnerEventData>
    {
        public async Task HandleAsync(PartnerEventData @event)
        {
            using var scope = scopeFactory.CreateScope();
            var partnerService = scope.ServiceProvider.GetService<IPartnerService>();
            var publisher = scope.ServiceProvider.GetService<IEventPublisher>();
            var model = new LZ.Model.Live_Partner()
            {
                PartnerGID = Guid.NewGuid(),
                PartnerName = @event.PartnerName,
                CompanyGID = @event.CompanyGID,
                CompanyID = @event.CompanyID,
                EntryDays = 0,
                LimitMoney = 0,
                CreateTime = @event.CreateTime,
                Enabled = true,
                PartnerType = @event.PartnerType,
            };
            await partnerService.Add(model);
            var record = new PersonnelEventData();
            record.PartnerID = model.PartnerID;
            record.PartnerGID = model.PartnerGID.Value;
            record.CompanyID = model.CompanyID.Value;
            record.CompanyGID = model.CompanyGID.Value;
            if (@event.CustomerID > 0)
            {
                record.CustomerID = @event.CustomerID;
                record.CustomerGID = @event.CustomerGID;
            }
            record.UserType = (int)Model.Enums.PersonnelType.Headed;
            record.FullName = @event.FullName;
            record.Phone = @event.Phone;
            publisher.Publish(record);
        }
    }
}
