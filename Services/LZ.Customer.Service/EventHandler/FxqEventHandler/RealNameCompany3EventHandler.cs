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

namespace LZ.Customer.Service.EventHandler
{
    /// <summary>
    /// 全网三要素
    /// </summary>
    /// <param name="scopeFactory"></param>
    public class RealNameCompany3EventHandler(IServiceScopeFactory scopeFactory) :IEventHandler<RealNameCompany3EventData>
    {
        public async Task HandleAsync(RealNameCompany3EventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var logger = scope.ServiceProvider.GetService<ILogger<RealNameCompany3EventHandler>>();
                var publisher = scope.ServiceProvider.GetService<IEventPublisher>();
                var signerService = scope.ServiceProvider.GetService<IContractSignerService>();
                var fxqService = scope.ServiceProvider.GetService<IFxqService>();
                var aliyunOSSService = scope.ServiceProvider.GetService<AliyunOSSService>();
                logger.LogInformation($"{@event.Name}进行{@event.RealNameType}开始");
                Live_ContractSigner entity = await signerService.Get(f => 
                f.Name == @event.Name 
                && f.Idno == @event.CompanyNo 
                && f.LegalPerson == @event.LegalPerson
                && f.IsRealNameSuc
                && @event.RealNameType == f.RealNameType);
                if (entity == null)
                {
                    entity = new Live_ContractSigner()
                    {
                        ID = Guid.NewGuid(),
                        Name = @event.Name,
                        Idno = @event.CompanyNo,
                        LegalPerson = @event.LegalPerson,

                        IsCompany = true,
                        Phone = string.Empty,
                        IsEnabled = false,
                        Expired = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsRealNameSuc = false,
                        OssUrl = string.Empty,
                        RealNameTime = null,
                        RealNameType = @event.RealNameType,
                        Remark = string.Empty,
                        BankNo = string.Empty,
                        LegalPersonIdCardNo = string.Empty,
                    };
                    var realNameResp = await fxqService.CompanyAuthentication3(@event.Name, @event.CompanyNo,@event.LegalPerson);
                    if (realNameResp != null)
                    {
                        if (realNameResp.Code == 10000)
                        {
                            if (realNameResp.Data.Status == 1)
                            {
                                entity.RealNameTime = DateTime.Now;
                                entity.IsRealNameSuc = true;
                            }
                            else
                            {
                                entity.IsRealNameSuc = false;
                                if (realNameResp.Data.Status == 2)
                                {
                                    entity.Remark = "全不匹配";
                                }
                                else if (realNameResp.Data.Status == 12)
                                {
                                    entity.Remark = "公司匹配|法人不匹配";
                                }
                                else if (realNameResp.Data.Status == 13)
                                {
                                    entity.Remark = "公司不匹配|法人匹配";
                                }
                            }
                        }
                        else
                        {
                            entity.Remark = $"{@event.Name}进行{@event.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                            entity.IsRealNameSuc = false;

                        }
                        logger.LogInformation(entity.Remark);

                    }
                    else
                    {
                        entity.Remark = $"{@event.Name}进行{@event.RealNameType},认证失败,返回值为null";
                        entity.IsRealNameSuc = false;
                        logger.LogInformation(entity.Remark);
                    }
                    logger.LogInformation(entity.Remark);

                    }
                    else
                    {
                        entity.Remark = $"{@event.Name}进行{@event.RealNameType},认证失败,返回值为null";
                        entity.IsRealNameSuc = false;
                        logger.LogInformation(entity.Remark);
                    }
                var failList = await signerService.List(f => f.Name == @event.Name
            && f.Name == @event.Name
            && f.Idno == @event.CompanyNo
            && f.LegalPerson == @event.LegalPerson
            && f.RealNameType == entity.RealNameType
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
                        var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno,true);
                        // 生成电子签名事件触发
                        publisher.Publish(electronicSignatureEventData);
                    }

                }
            }
        }
    }
