using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;

namespace LZ.Users.Service.EventHandler
{
    public class UserOpenIDEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<UserOpenIDEventData>
    {
        public async Task HandleAsync(UserOpenIDEventData @event)
        {
            Common.StaticLog.Debug("开始处理openid新增事件！id:"+ @event.UserID);
            using var scope = scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();
            var personnelService = scope.ServiceProvider.GetService<IPersonnelService>();
            try
            {
                if (userService != null && personnelService != null)
                {
                    if (@event.Platform == Model.Enums.PlatformType.XFZMini)
                    {
                        if (!await userService.ExistOpenID(@event.OpenID, @event.UserID, @event.ClientID))
                        {
                            var opensModel = new Model.Live_UserOpenIds();
                            opensModel.OpenId = @event.OpenID;
                            opensModel.FromID = @event.ClientID;
                            opensModel.OpenIdType = @event.OpenIdType;
                            opensModel.UserID = @event.UserID;
                            await userService.AddOpenID(opensModel);
                            await userService.SetBindMP(@event.UserID, true);
                        }
                    }
                    else if (@event.Platform == Model.Enums.PlatformType.LZMini)
                    {
                        if (!await personnelService.ExistOpenID(@event.OpenID, @event.UserID, @event.ClientID))
                        {
                            var opensModel = new Model.Live_PersonnelOpenIds();
                            opensModel.OpenId = @event.OpenID;
                            opensModel.FromID = @event.ClientID;
                            opensModel.OpenIdType = @event.OpenIdType;
                            opensModel.UserID = @event.UserID;
                            await personnelService.AddOpenID(opensModel);
                            await personnelService.SetBindMP(@event.UserID, true);
                        }
                    }
                   
                }
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
