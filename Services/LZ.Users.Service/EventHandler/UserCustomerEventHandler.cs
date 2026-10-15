using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Services;

namespace LZ.Users.Service.EventHandler
{
    public class UserCustomerEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<UserCustomerEventData>
    {
        public async Task HandleAsync(UserCustomerEventData @event)
        {
            Common.StaticLog.Debug("开始处理员工入职获取客户固定信息！id:"+ @event.EntryID);
            using var scope = scopeFactory.CreateScope();
            var entryService = scope.ServiceProvider.GetService<IEntryService>();
            var httpService = scope.ServiceProvider.GetService<IHttpService>();
            try
            {
                if (entryService != null && httpService != null)
                {
                    var model = await httpService.GetAsync<Model.Live_Customer>(ServiceNames.CustomerService, "/api/customer/get?id=" + @event.CustomerID);
                    if (model != null)
                    {
                      await  entryService.UpdateCustomer(@event.EntryID, model.EntryWorkClass, model.EntryWorkName, model.EntryFromType);
                    }
                }
                
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
