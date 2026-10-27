using LZ.Core.EventData;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model.Exceptions;
using LZ.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.CO2NET.Extensions;
using Microsoft.Extensions.Logging;
using LZ.Core.Interface;
using LZ.Core.Services;
using LZ.Model.Enums;

namespace LZ.Finance.Service.EventHandler
{
    public class PrepaidApplyEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<PrepaidApplyEventData>
    {
        public async Task HandleAsync(PrepaidApplyEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {

                var financeOtherService = scope.ServiceProvider.GetRequiredService<IFinanceOtherService>();
                var prepaidPartnerAccountService = scope.ServiceProvider.GetRequiredService<IPrepaidPartnerAccountService>();
                var prepaidApplyRecordService = scope.ServiceProvider.GetRequiredService<IPrepaidApplyRecordService>();
                var httpService = scope.ServiceProvider.GetRequiredService<IHttpService>();
                var  logger = scope.ServiceProvider.GetRequiredService<ILogger<PrepaidApplyEventHandler>>();
                logger.LogInformation($"员工入职记录ID:{@event.UserEntryID},申请金额：{@event.ApplyAmount}，申请类型：{@event.ApplyType}");

                var model = await httpService.GetAsync<Model.Live_V_UserEntry>(ServiceNames.UsersService, "/api/entry/view?id=" + @event.UserEntryID);
                if (model == null)
                {
                    throw new ErrorCodeException(-1, "入职信息不存在！");
                }
                Guid? pid = null;
                if (model.OperateModel > (int)OperateModel.Routine && model.JointPartnerGID.HasValue)
                {
                    pid = model.JointPartnerGID;
                }
                else
                {
                    pid = model.PartnerGID;
                }
                var prepaidAccount = await prepaidPartnerAccountService.Get(f => f.IsEnabled && f.PartnerGID == pid.Value);
                if (prepaidAccount == null)
                {
                    throw new ErrorCodeException(-1, "服务商账户信息不存在！");
                }

                Live_PrepaidApplyRecord applyRecord = new Live_PrepaidApplyRecord()
                {
                    ID = Guid.NewGuid(),
                    ApplyAmount = @event.ApplyAmount,
                    Bank = model.BankName,
                    BankNo = model.BankNumber,
                    CreateTime = DateTime.Now,
                    CustomerGID = model.CustomerGID.Value,
                    IdCardNo = model.CardID,
                    PartnerID = model.PartnerID.Value,
                    Name = model.Name,
                    PrepaidPartnerAccountID = prepaidAccount.ID,
                    SigningUnitID = model.SigningUnitGID.Value,
                    UserEntryID = model.ID,
                    ApplyType = @event.ApplyType,
                    Status = 0,
                    CompanyGID = model.CompanyGID.Value,
                    CompanyID = model.CompanyID.Value,
                    UserID = model.UserID
                };

                await prepaidApplyRecordService.Add(applyRecord);
            }
        }
    }
}
