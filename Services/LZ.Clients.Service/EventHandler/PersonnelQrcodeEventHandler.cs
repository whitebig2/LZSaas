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
    public class PersonnelQrcodeEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<PersonnelQrcodeEventData>
    {
        public async Task HandleAsync(PersonnelQrcodeEventData @event)
        {
            Common.StaticLog.Debug("开始处理二维码生成事件！id:"+ @event.FromID);
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
                    var itemModel = await httpService.GetAsync<Model.Live_V_PersonnelItems>(ServiceNames.UsersService, "/api/personnelitems/get?id=" + @event.FromID);
                    if (itemModel != null && itemModel.CompanyGID.HasValue && itemModel.CompanyID.HasValue)
                    {
                        var companyModel = await companyConfigService.Get(itemModel.CompanyGID);
                        var isMp = companyModel.Config.CreateQrcodeType == CreateQrcodeType.MP;
                        var clientsModel = companyModel.Config.CreateQrcodeType == CreateQrcodeType.Mobile? null : await clientsService.Get(itemModel.CompanyID.Value, "xfz", isMp ? "mp" : "mini");
                        if (clientsModel != null || companyModel.Config.CreateQrcodeType == CreateQrcodeType.Mobile)
                        {
                            var timeName = DateTime.Now.ToString("yyyy/MM/dd");
                            var qrcode = "qrcode/" + timeName + "/" + Guid.NewGuid().ToString("N") + ".png";
                            var path = Directory.GetCurrentDirectory() + "/uploadfile/" + qrcode;
                            var funPath = Directory.GetCurrentDirectory() + "/uploadfile/qrcode/" + timeName;
                            if (!Directory.Exists(funPath))
                            {
                                Directory.CreateDirectory(funPath);
                            }
                            //公众号二维码
                            if (companyModel.Config.CreateQrcodeType == CreateQrcodeType.MP)
                            {
                                var result = await QrCodeApi.CreateAsync(clientsModel.AppID, 100, 0, QrCode_ActionName.QR_LIMIT_STR_SCENE, itemModel.GID.ToString());
                                if (result.errcode == 0)
                                {
                                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                                    {
                                        QrCodeApi.ShowQrCode(result.ticket, fs);//下载二维码
                                    }
                                }
                                else
                                {
                                    Common.StaticLog.Error("公众号二维码生成错误！id:" + @event.FromID);
                                    return;
                                }
                            }
                            else if (companyModel.Config.CreateQrcodeType == CreateQrcodeType.Mini)
                            {
                                var result = await WxAppApi.GetWxaCodeAsync(clientsModel.AppID, path, "pages/login/scanPage?qr_id=" + itemModel.GID, 700);
                            }
                            else
                            {
                                var settings = await settingsService.GetMasterSettings();
                                await Common.Tool.CearteQrcode(10, settings.MobileUrl+ "/scan/?id=" + itemModel.GID,path);
                            }
                            // Common.StaticLog.Debug(JsonConvert.SerializeObject(result));
                            await Common.ImageSharp.CombinText(path, "", itemModel.CustomerName);
                            var qrcodepath = await aliyunOSSService.UploadFile(path, qrcode);
                            if (qrcodepath != "error")
                            {
                                var paras = new Dictionary<string, string>();
                                paras.Add("ID", @event.FromID.ToString());
                                paras.Add("Qrcode", qrcodepath);
                                await httpService.PutAsync(ServiceNames.UsersService, "/api/personnelitems/saveqrcode", paras);
                            }
                            else
                            {
                                Common.StaticLog.Error("上传二维码错误");
                            }
                        }
                        else
                        {
                            Common.StaticLog.Error("客户端不存在！id:" + @event.FromID);
                        }
                    }
                    else
                    {
                        Common.StaticLog.Error("客户关联不存在！id:" + @event.FromID);
                    }
                }
              
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
