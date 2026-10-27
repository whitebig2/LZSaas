using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Customer.Service.Interface;
using LZ.Model.Enums;
using LZ.Model.FxqModel;
using LZ.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Services;
using LZ.Core.Interface;

namespace LZ.Customer.Service.EventHandler
{
    /// <summary>
    /// 银行卡四要素
    /// </summary>
    /// <param name="scopeFactory"></param>
    public class RealNameBank4EventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<RealNameBank4EventData>
    {
        public async Task HandleAsync(RealNameBank4EventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var logger = scope.ServiceProvider.GetService<ILogger<FxqRealNameEventHandler>>();
                var publisher = scope.ServiceProvider.GetService<IEventPublisher>();
                var signerService = scope.ServiceProvider.GetService<IContractSignerService>();
                var fxqService = scope.ServiceProvider.GetService<IFxqService>();
                var aliyunOSSService = scope.ServiceProvider.GetService<AliyunOSSService>();
                var sendMQService = scope.ServiceProvider.GetService<ISendMQService>();

                logger.LogInformation($"{@event.Name}进行{@event.RealNameType}开始");
                Live_ContractSigner entity = await signerService.Get(f => 
                f.Name == @event.Name 
                && f.Idno == @event.IdCardNo 
                && f.BankNo == @event.BankNo 
                && f.Phone == @event.Phone
                && f.IsRealNameSuc 
                && @event.RealNameType == f.RealNameType);
                if (entity == null)
                {
                    entity = new Live_ContractSigner()
                    {
                        ID = Guid.NewGuid(),
                        Name = @event.Name,
                        Idno = @event.IdCardNo,
                        BankNo = @event.BankNo,
                        Phone = @event.Phone,
                        IsEnabled = false,
                        Expired = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsRealNameSuc = false,
                        OssUrl = string.Empty,
                        RealNameTime = null,
                        RealNameType = @event.RealNameType,
                        Remark = string.Empty,
                        LegalPerson = string.Empty,
                        LegalPersonIdCardNo = string.Empty,
                    };
                    var realNameResp = await fxqService.IdentityBankAuthentication4(@event.Name, @event.IdCardNo, @event.BankNo,@event.Phone);
                    if (realNameResp == null)
                    {
                        entity.Remark = $"{@event.Name}进行{@event.RealNameType},认证失败,返回值为null";
                        entity.IsRealNameSuc = false;
                        logger.LogInformation(entity.Remark);
                    }
                    else
                    {
                        if (realNameResp.Data != null && realNameResp.Data.State == 1)
                        {
                            entity.Remark = $"{@event.Name}进行{@event.RealNameType},认证成功";
                            entity.IsRealNameSuc = true;
                            entity.RealNameTime = DateTime.Now;
                            logger.LogInformation(entity.Remark);
                            var mqMsg = new { type = "contractInfoAuth", Title = "二/三/四要素认证", entity.Name, CardID = entity.Idno };
                            sendMQService.SendStatisticsMessage(mqMsg.ToCamelCaseJson());
                        }
                        else
                        {
                            entity.Remark = $"{@event.Name}进行{@event.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                            entity.IsRealNameSuc = false;
                            logger.LogInformation(entity.Remark);
                        }
                    }
                    var failList = await signerService.List(f => 
                    f.Name == @event.Name
                && f.Idno == @event.IdCardNo
                && f.BankNo == @event.BankNo
                && f.Phone == @event.Phone 
                && f.IsRealNameSuc == false);
                    if (failList.Any())
                    {
                        foreach (var item in failList)
                        {
                            await signerService.Delete(item.ID);
                        }
                    }
                    await signerService.Add(entity);
                    logger.LogInformation($"{@event.Name}进行{@event.RealNameType}结束");
                    if (entity.IsRealNameSuc)
                    {
                        var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno,false);
                        // 生成电子签名事件触发
                        publisher.Publish(electronicSignatureEventData);
                    }

                }
            }
        }
    }
}
