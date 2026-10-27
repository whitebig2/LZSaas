using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;
using LZ.Core.EventData.Insurance;
using LZ.Model.Enums;
using LZ.Model;
using SqlSugar.IOC;

namespace LZ.Users.Service.EventHandler
{
    public class BusinessInsuranceAddOrDelEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<BusinessInsuranceAddOrDelEventData>
    {
        public async Task HandleAsync(BusinessInsuranceAddOrDelEventData @event)
        {
            //Common.StaticLog.Debug("开始处理员工实名反馈事件！id:"+ @event.UserID);
            using (var scope = scopeFactory.CreateScope())
            {
                Common.StaticLog.Debug("商保增减员！UserEntryID:" + @event.UserEntryID);
                var sp = scope.ServiceProvider;
                var logger = scope.ServiceProvider.GetService<ILogger<BusinessInsuranceAddOrDelEventHandler>>();
                var recordService = scope.ServiceProvider.GetService<IBusinessInsuranceRecordService>();
                await recordService.ManageUserEntryPlan(@event.UserEntryID);
            }
        }
    }
}