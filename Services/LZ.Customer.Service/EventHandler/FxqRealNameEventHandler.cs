using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Customer.Service.Interface;
using LZ.Model.Enums;
using LZ.Model.FxqModel;
using LZ.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.ApiService.Interface;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using LZ.ApiService.Services;

namespace LZ.Customer.Service.EventHandler
{
    public class FxqRealNameEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<FxqRealNameEventData>
    {
        public async Task HandleAsync(FxqRealNameEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var logger = scope.ServiceProvider.GetService<ILogger<FxqRealNameEventHandler>>();
                var signerService = scope.ServiceProvider.GetService<IContractSignerService>();
                var fxqService = scope.ServiceProvider.GetService<IFxqService>();
                var aliyunOSSService = scope.ServiceProvider.GetService<AliyunOSSService>();
                logger.LogInformation($"{@event.Name}进行{@event.RealNameType}开始");
                Live_ContractSigner entity = await signerService.Get(f => f.Name == @event.Name && f.Idno == @event.Idno && f.IsRealNameSuc && @event.RealNameType == f.RealNameType);
                if (entity == null)
                {
                    entity = new Live_ContractSigner()
                    {
                        ID = Guid.NewGuid(),
                        CreateTime = DateTime.Now,
                        Idno = @event.Idno,
                        IsEnabled = false,
                        Expired = DateTime.Now,
                        Name = @event.Name,
                        IsRealNameSuc = false,
                        OssUrl = string.Empty,
                        RealNameTime = null,
                        RealNameType = @event.RealNameType,
                        Remark = string.Empty,
                        LegalPerson = @event.LegalPerson,
                        BankNo = @event.BankCardNo,
                        LegalPersonIdCardNo = @event.CertNo,
                        Phone = @event.Phone,
                    };

                    if (@event.RealNameType == RealNameEnumType.企业三要素 || @event.RealNameType == RealNameEnumType.企业四要素)
                    {
                        entity.IsCompany = true;
                        if (@event.RealNameType == RealNameEnumType.企业三要素)
                        {
                            FxqResponse<CompanyRealName3Response> realNameResp = null;
                            realNameResp = await fxqService.CompanyAuthentication3(entity.Name, entity.Idno, entity.LegalPerson);

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
                        }
                        else if (@event.RealNameType == RealNameEnumType.企业四要素)
                        {
                            FxqResponse<CompanyRealName4Response> realNameResp = null;

                            realNameResp = await fxqService.CompanyAuthentication4(entity.Name, entity.Idno, entity.LegalPerson, entity.LegalPersonIdCardNo);

                            if (realNameResp != null)
                            {
                                if (realNameResp.Code == 10000)
                                {

                                    if (realNameResp.Data.entNameMatch == 1
                                        && realNameResp.Data.entRegNoMatch == 1
                                        && realNameResp.Data.repNameMatch == 1
                                        && realNameResp.Data.repIdNoMatch == 1)
                                    {
                                        entity.IsRealNameSuc = true;
                                        entity.RealNameTime = DateTime.Now;
                                    }
                                    else
                                    {

                                        if (realNameResp.Data.entNameMatch == 2)
                                        {
                                            entity.Remark += "|公司名称不匹配";
                                        }
                                        if (realNameResp.Data.entRegNoMatch == 2)
                                        {
                                            entity.Remark += "|工商注册号不匹配";
                                        }
                                        if (realNameResp.Data.repNameMatch == 2)
                                        {
                                            entity.Remark += "|法人姓名不匹配";
                                        }
                                        if (realNameResp.Data.repIdNoMatch == 2)
                                        {
                                            entity.Remark += "|法人身份证不匹配";
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

                        }
                    }
                    else
                    {
                        FxqResponse<RealNameStatus> realNameResp = null;

                        switch (@event.RealNameType)
                        {
                            case RealNameEnumType.公安二要素:
                                realNameResp = await fxqService.IdentityAuthentication2(@event.Name, @event.Idno);
                                break;
                            case RealNameEnumType.全网三要素:
                                realNameResp = await fxqService.IdentityAuthentication3(@event.Name, @event.Idno, @event.Phone);
                                break;
                            case RealNameEnumType.银行卡三要素:
                                realNameResp = await fxqService.IdentityBankAuthentication3(@event.Name, @event.Idno, @event.BankCardNo);
                                break;
                            case RealNameEnumType.银行卡四要素:
                                realNameResp = await fxqService.IdentityBankAuthentication4(@event.Name, @event.Idno, @event.BankCardNo, @event.Phone);
                                break;
                            default:
                                break;
                        }
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
                            }
                            else
                            {
                                entity.Remark = $"{@event.Name}进行{@event.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                                entity.IsRealNameSuc = false;
                                logger.LogInformation(entity.Remark);
                            }
                        }

                    }


                    var failList = await signerService.List(f => f.Name == entity.Name && f.RealNameType == entity.RealNameType && f.Idno == entity.Idno && f.IsRealNameSuc == false);
                    if (failList.Any())
                    {
                        foreach (var item in failList)
                        {
                            await signerService.Delete(item.ID);
                        }
                    }
                    //已实名认证
                    if (entity.IsRealNameSuc)
                    {
                        if (entity.IsCompany)
                        {
                            var resp = await fxqService.CompanyElectronicSeal(entity.Name, null, null, null, null, null, 0, null);
                            if (resp != null && resp.Code == 10000)
                            {
                                var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                                using (var ms = new MemoryStream(fileBase64))
                                {
                                    var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false);
                                    if (ossResp.Code == 0)
                                    {
                                        entity.IsEnabled = true;
                                        entity.OssUrl = "https://lz-uplod.oss-cn-shanghai.aliyuncs.com/" + ossResp.Data;
                                    }
                                    else
                                    {
                                        entity.IsEnabled = false;
                                        entity.Remark = "电子签名获取成功，上传阿里云Oss 失败！";
                                    }
                                }
                            }
                            else
                            {
                                entity.IsEnabled = false;
                                entity.Remark = $"电子签名获取失败，异常信息：{resp.Msg}";
                            }
                        }
                        else
                        {

                            var resp = await fxqService.ElectronicSeal(entity.Name, 0);
                            if (resp != null && resp.Code == 10000)
                            {
                                var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                                using (var ms = new MemoryStream(fileBase64))
                                {
                                    var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false);
                                    if (ossResp.Code == 0)
                                    {
                                        entity.IsEnabled = true;
                                        entity.OssUrl = "https://lz-uplod.oss-cn-shanghai.aliyuncs.com/" + ossResp.Data;
                                    }
                                    else
                                    {
                                        entity.IsEnabled = false;
                                        entity.Remark = "电子签名获取成功，上传阿里云Oss 失败！";
                                    }
                                }
                            }
                            else
                            {
                                entity.IsEnabled = false;
                                entity.Remark = $"电子签名获取失败，异常信息：{resp.Msg}";
                            }

                        }
                    }

                    await signerService.Add(entity);
                    // 此处触发 实名认证结果通知


                    logger.LogInformation($"{@event.Name}进行{@event.RealNameType}结束");

                }
            }
        }
    }
}
