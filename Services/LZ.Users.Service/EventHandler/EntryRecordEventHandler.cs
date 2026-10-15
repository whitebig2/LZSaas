using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;
using LZ.Common;

namespace LZ.Users.Service.EventHandler
{
    public class EntryRecordEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<EntryRecordEventData>
    {
        public async Task HandleAsync(EntryRecordEventData @event)
        {
            Common.StaticLog.Debug("开始处理入职记录日志事件！id:"+ @event.EntryID);
            using var scope = scopeFactory.CreateScope();
            var entryRecordService = scope.ServiceProvider.GetService<IEntryRecordService>();
            try
            {
                if (entryRecordService != null)
                {
                    var model = new Model.Live_UserEntryRecord();
                    model.CreateTime = DateTime.Now;
                    model.Status = @event.Status;
                    model.UserID = @event.UserID;
                    model.EntryID = @event.EntryID;
                    model.EntryTime = @event.EntryTime;
                    model.LeaveTime = @event.LeaveTime;
                    model.About = @event.About;
                    model.CID = @event.CID;
                    model.PerUserID = @event.PerUserID;
                    model.OperateFullName= @event.OperateFullName;
                    model.OperateUserID = @event.OperateUserID;
                    if (@event.OperateFrom != null)
                    {
                        model.OperateFrom = (int)@event.OperateFrom;
                        model.OperateFromName = Tool.GetDescription(@event.OperateFrom);
                    }
                    await entryRecordService.Add(model);
                }
                
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
