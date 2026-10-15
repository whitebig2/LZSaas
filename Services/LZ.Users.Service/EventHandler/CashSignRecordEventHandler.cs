using LZ.Core.RabbitMQ.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData.Cash;
using LZ.Users.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Core.Interface;
using LZ.Core.Services;
using Newtonsoft.Json;

namespace LZ.Users.Service.EventHandler
{
    /// <summary>
    /// 现金签收单生成
    /// </summary>
    /// <param name="scopeFactory"></param>
    public class CashSignRecordEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<CashSignRecordEventData>
    {
        public async Task HandleAsync(CashSignRecordEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CashSignRecordEventHandler>>();
                var whiteListService = scope.ServiceProvider.GetRequiredService<IWhiteListService>();
                var entryService = scope.ServiceProvider.GetRequiredService<IEntryService>();
                var httpService = scope.ServiceProvider.GetRequiredService<IHttpService>();


                Console.WriteLine("现金签收单生成");

                var entry = await entryService.View(f => f.ID == @event.UserEntryID);
                if (entry!=null)
                {
                    CashSignRecordDto record = new CashSignRecordDto()
                    {
                        Name = entry.Name,
                        CardID = entry.CardID,
                        Sex = entry.Sex,
                        Age = entry.Age ?? 0,
                        Phone = entry.Phone,
                        CompanyID = entry.CompanyID ?? 0,
                        CompanyName = entry.CompanyName,
                        CustomerID = entry.CustomerID ?? 0,
                        CustomerName = entry.CustomerName,
                        PartnerID = entry.PartnerID ?? 0,
                        PartnerName = entry.PartnerName,
                        SignUnitName = entry.SignName,
                        UserEntryID = entry.ID,
                        UserID = entry.UserID ?? 0,
                        FailMsg = string.Empty,
                        CashType = string.Empty,
                        Status = Model.Enums.CashSignStatusEnumType.待生成,
                        ExpirationDate = DateTime.MaxValue,
                        SignUnitCompanyNo = entry.SignCompanyNo,

                    };
                    var whiteRecord=await whiteListService.Get(f =>f.Name==entry.Name&& f.IdentityCard == entry.CardID && f.CompanyID == entry.CompanyID);
                    if (whiteRecord != null)
                    {
                        record.CashType = whiteRecord.LabelName;
                    }
                    else
                    {
                        record.FailMsg = "现金白名单记录不存在，无法生成";
                        record.Status = Model.Enums.CashSignStatusEnumType.生成取消;
                        record.ExpirationDate = DateTime.MinValue;
                    }

                    Console.WriteLine(JsonConvert.SerializeObject(record));
                    var res = await httpService.PostAsync(ServiceNames.CustomerService, "/api/CashSignRecord/Add", JsonConvert.SerializeObject(record));

                }
               



            }
        }
    }
}
