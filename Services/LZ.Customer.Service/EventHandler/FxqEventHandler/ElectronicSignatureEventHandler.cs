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
using Senparc.CO2NET.Extensions;
using Microsoft.IdentityModel.Tokens;
using LZ.Core.Interface;

namespace LZ.Customer.Service.EventHandler
{
    /// <summary>
    /// 银行卡四要素
    /// </summary>
    /// <param name="scopeFactory"></param>
    public class ElectronicSignatureEventHandler(IServiceScopeFactory scopeFactory) : IEventHandler<ElectronicSignatureEventData>
    {
        public async Task HandleAsync(ElectronicSignatureEventData @event)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var logger = scope.ServiceProvider.GetService<ILogger<FxqRealNameEventHandler>>();
                var signerService = scope.ServiceProvider.GetService<IContractSignerService>();
                var fxqService = scope.ServiceProvider.GetService<IFxqService>();
                var aliyunOSSService = scope.ServiceProvider.GetService<AliyunOSSService>();
                var settingsService = scope.ServiceProvider.GetService<ISettingsService>();

                var setting =await settingsService.GetMasterSettings();
                logger.LogInformation($"{@event.Name}生成电子签名开始");
                // 已实名认证成功的签署人
                var signerList = await signerService.List(f => f.Name == @event.Name && f.Idno == @event.IdCardNo && f.IsRealNameSuc);
                //需要生成电子签名人签署人
                List<Live_ContractSigner> signerUpdateList = signerList.Where(f=>string.IsNullOrWhiteSpace(f.OssUrl)).ToList();

                if (!signerUpdateList.Any())
                {
                    return;
                }
                string signerOssUrl=string.Empty;

                // 已存在电子签名
                if (signerList.Any() && signerList.Any(f => !f.OssUrl.IsNullOrWhiteSpace()))
                {
                    signerOssUrl = signerList.First(f => !f.OssUrl.IsNullOrWhiteSpace()).OssUrl;
                }
                else
                {
                    if (@event.IsCompany)
                    {
                        var resp = await fxqService.CompanyElectronicSeal(@event.Name, null, @event.IdCardNo, null, null, null, 0, null);
                        if (resp != null && resp.Code == 10000)
                        {
                            var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                            using (var ms = new MemoryStream(fileBase64))
                            {
                                var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false);
                                if (ossResp.Code == 0)
                                {
                                    signerOssUrl = setting.AliBucketDomain.TrimEnd('/') + ossResp.Data;
                                }
                                else
                                {
                                    signerOssUrl = string.Empty;

                                    foreach (var item in signerUpdateList)
                                    {
                                        item.Remark = "电子签名获取成功，上传阿里云Oss 失败！";
                                    }
                                }
                            }
                        }
                        else {

                            foreach (var item in signerUpdateList)
                            {
                                item.Remark = $"电子签名获取失败！异常状态码：{resp.Code},异常信息：{resp.Msg}";
                            }
                        }
                    }
                    else
                    {
                        var resp = await fxqService.ElectronicSeal(@event.Name, 0, 0, 0, 0);
                        if (resp != null && resp.Code == 10000)
                        {
                            var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                            using (var ms = new MemoryStream(fileBase64))
                            {
                                var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false);
                                if (ossResp.Code == 0)
                                {
                                    signerOssUrl = setting.AliBucketDomain + ossResp.Data;
                                }
                                else
                                {
                                    signerOssUrl = string.Empty;
                                    foreach (var item in signerUpdateList)
                                    {
                                        item.Remark = "电子签名获取成功，上传阿里云Oss 失败！";
                                    }
                                }
                            }
                        }
                        else {

                            foreach (var item in signerUpdateList)
                            {
                                item.Remark = $"电子签名获取失败！异常状态码：{resp.Code},异常信息：{resp.Msg}";
                            }
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(signerOssUrl))
                {
                    foreach (var item in signerUpdateList)
                    {
                        item.OssUrl = signerOssUrl;
                        item.IsEnabled = true;
                    }
                    await signerService.Update(signerUpdateList);
                }
                else
                {
                    await signerService.Update(signerUpdateList);
                }




            }
        }
    }
}
