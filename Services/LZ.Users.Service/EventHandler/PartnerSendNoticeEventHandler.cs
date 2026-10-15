using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Services;
using System.Security.Policy;

namespace LZ.Users.Service.EventHandler
{
    public class PartnerSendNoticeEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<PartnerSendNoticeEventData>
    {
        public async Task HandleAsync(PartnerSendNoticeEventData @event)
        {
            Common.StaticLog.Debug("开始处理发送服务商通知！id:"+ @event.PartnerID);
            using var scope = scopeFactory.CreateScope();
            var entryService = scope.ServiceProvider.GetService<IEntryService>();
            var publisher = scope.ServiceProvider.GetService<IEventPublisher>();
            var personnelService = scope.ServiceProvider.GetService<IPersonnelService>();
            try
            {
                if (entryService != null && personnelService != null)
                {
                    var models = await personnelService.List(it => it.PartnerID == @event.PartnerID && it.UserType == (int)@event.UserType && it.Enabled, new Model.PagedModel.UserPagedModel() { Page = 0 });
                    var record = new SendNoticeEventData();
                    record.Platform = Model.Enums.PlatformType.LZMini;
                    record.SendType = Model.Enums.NoticeSendEnumType.All;
                    record.NoticeType = Model.Enums.NoticeEnumType.Remind;
                    record.Keyword1 = @event.Title;
                    record.Keyword3 = @event.Message;
                    record.Keyword4 = DateTime.Now.ToString("yyyy-MM-dd");
                    if (!string.IsNullOrEmpty(@event.Page))
                    {
                        record.Page = @event.Page;
                    }
                    foreach (var m in models)
                    {
                        record.Keyword5 = m.FullName;
                        record.UserID = m.UserID;
                        publisher.Publish(record);
                        Thread.Sleep(100);
                    }
                }
                
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
