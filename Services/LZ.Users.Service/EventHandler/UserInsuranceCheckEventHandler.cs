using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.EventHandler
{
    public class UserInsuranceInsuranceStatusUpdateEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<UserInsuranceInsuranceStatusUpdateEventData>
    {
        public async Task HandleAsync(UserInsuranceInsuranceStatusUpdateEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var entryService = scope.ServiceProvider.GetRequiredService<IEntryService>();
                var entryRecord = await entryService.Get(@event.UserEntryID);
                entryRecord.InsuranceStatus = @event.InsuranceStatus;
                await entryService.Update(entryRecord);
            }
        }
    }
}
