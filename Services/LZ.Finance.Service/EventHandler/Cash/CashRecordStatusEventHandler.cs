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
using LZ.Core.EventData.Cash;
using SqlSugar;
using LZ.ApiService.Interface;
using AutoMapper;
using LZ.Model.ModelExtensions;
using Consul;
using LZ.ApiService.Services;

namespace LZ.Finance.Service.EventHandler
{
    public class CashRecordStatusEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<CashRecordStatusEventData>
    {
        public async Task HandleAsync(CashRecordStatusEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CashRecordStatusEventHandler>>();
                var cashSendRecordService = scope.ServiceProvider.GetRequiredService<ISalaryCashSendRecordService>();
                var record = await cashSendRecordService.Get(f => f.ID == @event.SalaryCashSendRecordID);
                record.Status = @event.Status;
                record.Msg = @event.Msg;
                record.FilePath = @event.FilePath;
                record.SignerOssUrl = @event.SignerOssUrl;
                record.SignKeyword = @event.SignKeyword;
                await cashSendRecordService.Update(record);
            }
        }
    }
}
