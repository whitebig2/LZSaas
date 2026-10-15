using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;

namespace LZ.Users.Service.EventHandler
{
    public class UserRealEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<UserRealEventData>
    {
        public async Task HandleAsync(UserRealEventData @event)
        {
            Common.StaticLog.Debug("开始处理员工实名反馈事件！id:"+ @event.UserID);
            using var scope = scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();
            try
            {
                if (userService != null)
                {
                   await userService.ChangeReal(@event.UserID, @event.Success, @event.Message, @event.RealLevel);
                }
                
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
