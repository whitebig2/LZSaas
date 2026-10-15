using LZ.ApiService.Interface;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;

namespace LZ.Users.Service.EventHandler
{
    public class UserBankEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<UserBankEventData>
    {
        public async Task HandleAsync(UserBankEventData @event)
        {
            Common.StaticLog.Debug("开始处理银行所属地查询事件！id:"+ @event.UserID);
            using var scope = scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>();
            var entryService = scope.ServiceProvider.GetService<IEntryService>();
            var aliyunCommonService = scope.ServiceProvider.GetService<IAliyunCommonService>();
            try
            {
                if (userService != null && !string.IsNullOrEmpty(@event.BankNumber) && @event.UserID>0)
                {
                    var res = await aliyunCommonService.BankCardInfo(@event.BankNumber);
                    if (res != null)
                    {
                        if (string.IsNullOrEmpty(res.province))
                        {
                            res.province = "识别失败";
                        }
                        await userService.ChangeBankArea(@event.UserID, res.province, res.city);
                        if (@event.EntryID > 0)
                        {
                            await entryService.ChangeBankArea(@event.EntryID, res.province, res.city);
                        }
                        else
                        {
                            await entryService.ChangeUserBankArea(@event.UserID, res.province, res.city);
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
