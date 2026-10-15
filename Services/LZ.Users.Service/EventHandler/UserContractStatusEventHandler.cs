using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LZ.Users.Service.EventHandler
{
    public class UserContractStatusEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<UserContractStatusEventData>
    {
        public async Task HandleAsync(UserContractStatusEventData @event)
        {
            Common.StaticLog.Debug("开始处理合同状态反馈事件！id:"+ @event.EntryID);
            using var scope = scopeFactory.CreateScope();
            var entryService = scope.ServiceProvider.GetService<IEntryService>();
            var jdySyncService = scope.ServiceProvider.GetService<IJDYSyncService>();
            try
            {
                if (entryService != null)
                {
                    //await entryService.UpdateContractStatus(@event.Status, @event.EntryID);
                    await entryService.UpdateContractInfo(@event.Status, @event.ContractDate, @event.ContractStartDate, @event.ContractEndDate, @event.EntryID);
                    await jdySyncService.UserContractSync(@event.EntryID);//同步简道云合同状态
                }
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
        }
    }
}
