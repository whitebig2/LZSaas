//using LZ.Core.EventData;
//using LZ.Core.EventData.Insurance;
//using LZ.Core.RabbitMQ.EventBus;
//using LZ.Customer.Service.Interface;
//using LZ.Model;
//using LZ.Model.Enums;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Senparc.CO2NET.Extensions;
//using SqlSugar.IOC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Policy;
//using System.Text;
//using System.Threading.Tasks;

//namespace LZ.Customer.Service.EventHandler.InsuranceHandler
//{
//    public class InsuranceStatusEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<InsuranceStatusEventData>
//    {
//        public async Task HandleAsync(InsuranceStatusEventData @event)
//        {

//            using (var scope = scopeFactory.CreateScope())
//            {
//                var publisher = scope.ServiceProvider.GetService<IEventPublisher>();
//                var recordService = scope.ServiceProvider.GetService<IUserEntryInsuracneRecordService>();
//                var records = await recordService.List(f => f.SendStatus== InsuranceSendStatusEnumType.待增员 || f.SendStatus == InsuranceSendStatusEnumType.待减员);
//                UserInsuranceInsuranceStatusUpdateEventData eventData = new UserInsuranceInsuranceStatusUpdateEventData()
//                {
//                    UserEntryID = @event.UserEntryID,
//                    InsuranceStatus = UserEntryInsuranceStatusEnumType.已增员,
//                };


//                if (records.Any(f => f.SendStatus == InsuranceSendStatusEnumType.增员异常))
//                {
//                    eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.增员异常;
//                    publisher.Publish(eventData);
//                }
//                else if (records.Any(f => f.SendStatus == InsuranceSendStatusEnumType.减员异常))
//                {

//                    eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.减员异常;
//                    publisher.Publish(eventData);
//                }
//                else if (records.Any(f => f.SendStatus == InsuranceSendStatusEnumType.待增员))
//                {
//                    eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.待增员;
//                    publisher.Publish(eventData);

//                }
//                else if (records.Any(f => f.SendStatus == InsuranceSendStatusEnumType.待减员))
//                {
//                    eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.待减员;
//                    publisher.Publish(eventData);
//                }
//                else if (records.Any(f => f.SendStatus == InsuranceSendStatusEnumType.已增员))
//                {
//                    eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.已增员;
//                    publisher.Publish(eventData);
//                }
//                else if (records.Any(f => f.SendStatus == InsuranceSendStatusEnumType.已减员))
//                {
//                    eventData.InsuranceStatus = UserEntryInsuranceStatusEnumType.已减员;
//                    publisher.Publish(eventData);
//                }


//            }
//        }
//    }
//}
