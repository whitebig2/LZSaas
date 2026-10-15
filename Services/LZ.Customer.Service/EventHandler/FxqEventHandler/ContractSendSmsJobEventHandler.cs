//using LZ.ApiService.Interface;
//using LZ.ApiService.Services;
//using LZ.Core.EventData;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Model.Enums;
//using LZ.Model.FxqModel;
//using LZ.Model;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using LZ.Core.EventData.FxqEventData;
//using LZ.Core.EventData.Contract;
//using LZ.Model.ModelExtensions;
//using SqlSugar;
//using SqlSugar.IOC;
//using NPOI.HSSF.Record;
//using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;

//namespace LZ.Customer.Service.EventHandler.FxqEventHandler
//{
//    public class ContractSendSmsJobEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<ContractSendSmsJobEventData>
//    {

//        public async Task HandleAsync(ContractSendSmsJobEventData @event)
//        {
//            using (var scope = scopeFactory.CreateScope())
//            {
//                var sp = scope.ServiceProvider;
//                var logger = sp.GetRequiredService<ILogger<ContractSendSmsJobEventHandler>>();
//                var signRecordService = sp.GetRequiredService<IContractRecordService>();
//                var eventPublisher = sp.GetRequiredService<IEventPublisher>();
//                var records = await signRecordService.List(f =>f.IsNotice==false&&f.ContractType!= ContractEnumType.入职声明&& f.Status == SignStatusEnumType.待发送短信 && f.ReadySigningDate != null && f.ReadySigningDate.Value <= DateTime.Today);
//                var batchNoList = records.Select(f => f.ID).Distinct().ToList();
//                var eventDataList = batchNoList.Select(f => new UserEntryContractSendSmsEventData(f));
//                foreach (var item in eventDataList)
//                {
//                    eventPublisher.Publish(item);
//                }
//            }
//        }
//    }
//}
