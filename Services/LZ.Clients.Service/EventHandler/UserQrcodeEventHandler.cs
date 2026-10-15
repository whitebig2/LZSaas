using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Interface;
using LZ.Core.Services;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using System.IO;
using LZ.ApiService.Interface;
using LZ.Clients.Service.Interface;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using LZ.Model.Enums;

namespace LZ.Clients.Service.EventHandler
{
    public class UserQrcodeEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<UserQrcodeEventData>
    {
        public async Task HandleAsync(UserQrcodeEventData @event)
        {
            Common.StaticLog.Debug("开始处理幸福职用户二维码生成事件！id:"+ @event.UserID);
            using var scope = scopeFactory.CreateScope();
            var clientsService = scope.ServiceProvider.GetService<IClientsService>();
            var companyConfigService = scope.ServiceProvider.GetService<ICompanyConfigService>();
            var aliyunOSSService = scope.ServiceProvider.GetService<IAliyunOSSService>();
            var httpService = scope.ServiceProvider.GetService<IHttpService>();
            var settingsService = scope.ServiceProvider.GetService<ISettingsService>();
            try
            {
                if (companyConfigService != null && clientsService!=null && aliyunOSSService!=null && httpService!=null)
                {
                    if (string.IsNullOrEmpty(@event.FromAction))
                    {
                        @event.FromAction = "reg";
                    }
                    if (@event.CompanyID>0)
                    {
                        var companyModel = await companyConfigService.Get(@event.CompanyGID);
                        var isMp = companyModel.Config.CreateQrcodeType == CreateQrcodeType.MP;
                        var clientsModel = await clientsService.Get(@event.CompanyID, "xfz", isMp ? "mp" : "mini");
                        if (clientsModel != null)
                        {
                            var timeName = DateTime.Now.ToString("yyyy/MM/dd");
                            var qrcode = "qrcode/" + timeName + "/" + Guid.NewGuid().ToString("N") + ".png";
                            var path = Directory.GetCurrentDirectory() + "/uploadfile/" + qrcode;
                            var funPath = Directory.GetCurrentDirectory() + "/uploadfile/qrcode/" + timeName;
                            if (!Directory.Exists(funPath))
                            {
                                Directory.CreateDirectory(funPath);
                            }
                            if (string.IsNullOrEmpty(@event.FromType)) @event.FromType = "xfz";
                            //公众号二维码
                            if (companyModel.Config.CreateQrcodeType == CreateQrcodeType.MP)
                            {
                                var result = await QrCodeApi.CreateAsync(clientsModel.AppID, 100, 0, QrCode_ActionName.QR_LIMIT_STR_SCENE, $"{@event.FromAction}|{@event.UserID}|{@event.FromType}");
                                if (result.errcode == 0)
                                {
                                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                                    {
                                        QrCodeApi.ShowQrCode(result.ticket, fs);//下载二维码
                                    }
                                }
                                else
                                {
                                    Common.StaticLog.Error("公众号二维码生成错误！id:" + @event.UserID);
                                    return;
                                }
                            }
                            else
                            {
                                var result = await WxAppApi.GetWxaCodeAsync(clientsModel.AppID, path, $"pagesUser/user/reg?fuid={@event.UserID}&from={@event.FromType}" , 700);
                            }
                            var qrcodepath = await aliyunOSSService.UploadFile(path, qrcode);
                            if (qrcodepath != "error")
                            {
                                var paras = new Dictionary<string, string>();
                                paras.Add("ID", @event.UserID.ToString());
                                paras.Add("Qrcode", qrcodepath);
                                if (@event.FromType == "lz")
                                {
                                    await httpService.PutAsync(ServiceNames.UsersService, "/api/personnel/saveclientqrcode", paras);
                                }
                                else
                                {
                                    await httpService.PutAsync(ServiceNames.UsersService, "/api/user/saveqrcode", paras);
                                }
                                    
                            }
                            else
                            {
                                Common.StaticLog.Error("上传二维码错误");
                            }
                        }
                        else
                        {
                            Common.StaticLog.Error("客户端不存在！id:" + @event.UserID);
                        }
                    }
                    else
                    {
                        Common.StaticLog.Error("客户关联不存在！id:" + @event.UserID);
                    }
                }
              
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
