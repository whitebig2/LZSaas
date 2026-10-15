using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Core.Services;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using System.IO;
using LZ.ApiService.Interface;

namespace LZ.Clients.Service.EventHandler
{
    public class PersonnelQrcodeEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<PersonnelQrcodeEventData>
    {
        public async Task HandleAsync(PersonnelQrcodeEventData @event)
        {
            Common.StaticLog.Debug("开始处理二维码生成事件！companyId:"+ @event.CompanyID+";appid:"+ @event);
            using var scope = scopeFactory.CreateScope();
            var clientsService = scope.ServiceProvider.GetService<IClientsService>();
            var companyConfigService = scope.ServiceProvider.GetService<ICompanyConfigService>();
            var aliyunOSSService = scope.ServiceProvider.GetService<IAliyunOSSService>();
            var httpService = scope.ServiceProvider.GetService<IHttpService>();
            try
            {
                if (companyConfigService != null && clientsService!=null && aliyunOSSService!=null && httpService!=null)
                {
                    var companyModel = await companyConfigService.Get(@event.CompanyID);
                    var clientsModel = await clientsService.Get(@event.CompanyID, "xfz");
                    if (clientsModel != null)
                    {
                        var customerModel = await httpService.GetAsync<Model.Live_Customer>(ServiceNames.CustomerService, "/api/customer/get?id="+@event.CustomerID);
                        if (customerModel != null)
                        {
                            var timeName = DateTime.Now.ToString("yyyyMMdd");
                            var qrcode = "qrcode/"+ timeName+"/" + Guid.NewGuid().ToString("N") + ".png";
                            var path = Directory.GetCurrentDirectory() + "/uploadfile/" + qrcode;
                            var result = await WxAppApi.GetWxaCodeAsync(clientsModel.AppID, path, "pages/userinfo/index?userid=" + @event.UserID + "&customerid=" + @event.CustomerID + "&companyid=" + @event.CompanyID, 700);
                           // Common.StaticLog.Debug(JsonConvert.SerializeObject(result));
                            await Common.ImageSharp.CombinText(path, "", customerModel.CustomerName);
                            var qrcodepath = await aliyunOSSService.UploadFile(path, qrcode);
                            if (qrcodepath != "error")
                            {
                                var paras = new Dictionary<string, string>();
                                paras.Add("UserID", @event.UserID.ToString());
                                paras.Add("CustomerID", @event.CustomerID.ToString());
                                paras.Add("Qrcode", qrcodepath);
                                await httpService.PutAsync(ServiceNames.UsersService, "/api/personnel/saveqrcode", paras);
                            }
                            else
                            {
                                Common.StaticLog.Error("上传二维码错误");
                            }
                           
                        }
                        else
                        {
                            Common.StaticLog.Error("客户信息获取失败！customerId:" + @event.CustomerID + ";appid:" + @event);
                        }
                    }
                    else
                    {
                        Common.StaticLog.Error("客户端不存在！customerId:" + @event.CustomerID + ";appid:" + @event);
                    }
                }
              
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
