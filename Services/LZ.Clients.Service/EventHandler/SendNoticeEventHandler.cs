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
using LZ.Clients.Service.Services;
using LZ.Model.PagedModel;
using SqlSugar;
using Senparc.Weixin.Entities.TemplateMessage;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using LZ.Model;
using Microsoft.AspNetCore.NodeServices;
using System.Text;

namespace LZ.Clients.Service.EventHandler
{
    public class SendNoticeEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<SendNoticeEventData>
    {
        public async Task HandleAsync(SendNoticeEventData @event)
        {
            Common.StaticLog.Debug("开始处理发送通知事件！id:"+ @event.UserID);
            using var scope = scopeFactory.CreateScope();
            var clientsService = scope.ServiceProvider.GetService<IClientsService>();
            var clientNoticeTemplateService = scope.ServiceProvider.GetService<IClientNoticeTemplateService>();
            var companyConfigService = scope.ServiceProvider.GetService<ICompanyConfigService>();
            var authService = scope.ServiceProvider.GetService<IAuthService>();
            var httpService = scope.ServiceProvider.GetService<IHttpService>(); 
            var IMSService = scope.ServiceProvider.GetService<IIMSService>();
            try
            {
                if (clientsService!=null && clientNoticeTemplateService != null && companyConfigService != null && authService!=null && httpService != null && IMSService!=null)
                {
                    
                    var clientType = "";
                    var isMp = false;
                    var imToType = 3;
                    var imContent = "";
                    Guid? companyGID=null;
                    var companyId = 0;
                    switch (@event.Platform)
                    {
                        case Model.Enums.PlatformType.XFZMini:
                            {
                                clientType = "xfz";
                                var user = await authService.UserInfo<Model.Live_User>(@event.UserID, @event.Platform);
                                companyGID = user.CompanyGID;
                                companyId = user.CompanyID.Value;
                            }
                            break;
                        case Model.Enums.PlatformType.LZMini:
                            {
                                var user = await authService.UserInfo<Model.Live_Personnel>(@event.UserID, @event.Platform);
                                companyGID = user.CompanyGID;
                                clientType = "lz";
                                imToType = 2;
                                isMp = true;
                            }
                            break;
                        default:
                            return;
                    }
                    var companyModel = await companyConfigService.Get(companyGID);
                    if(@event.Platform == Model.Enums.PlatformType.XFZMini)
                    {
                        isMp = companyModel.Config.UserIsXFZMPNotice;
                    }
                    if (@event.SendType == Model.Enums.NoticeSendEnumType.All)
                    {
                        var client = await clientsService.Get(companyId, clientType, isMp ? "mp" : "mini");
                        if (client != null)
                        {
                            var openId = "";
                            switch (@event.Platform)
                            {
                                case Model.Enums.PlatformType.XFZMini:
                                    {
                                        var openModel = await httpService.GetAsync<Model.Live_UserOpenIds>(ServiceNames.UsersService, "/api/user/getopenid?id=" + @event.UserID + "&clientId=" + client.ID);
                                        if (openModel == null)
                                        {
                                            Common.StaticLog.Error("获取openid失败！id:" + @event.UserID);
                                            return;
                                        }
                                        openId = openModel.OpenId;
                                    }
                                    break;
                                case Model.Enums.PlatformType.LZMini:
                                    {
                                        var openModel = await httpService.GetAsync<Model.Live_PersonnelOpenIds>(ServiceNames.UsersService, "/api/personnel/getopenid?id=" + @event.UserID + "&clientId=" + client.ID);
                                        if (openModel == null)
                                        {
                                            Common.StaticLog.Error("获取openid失败！id:" + @event.UserID);
                                            return;
                                        }
                                        openId = openModel.OpenId;
                                    }
                                    break;
                                default:
                                    return;
                            }
                            var temp = await clientNoticeTemplateService.Get(client.ID, (int)@event.NoticeType);
                            if (temp != null)
                            {
                                imContent = temp.Name;
                                if (isMp)
                                {
                                    var miniClient = await clientsService.Get(companyId, clientType, "mini");
                                    await SendMessage(temp, @event, openId, miniClient.AppID);
                                }
                                else
                                {
                                    await SendSubscribe(temp, @event, openId);
                                }
                            }
                        }
                        else
                        {
                            Common.StaticLog.Error("获取客户端信息失败！id:" + @event.UserID);
                        }
                    }
                    else
                    {
                        imContent = @event.Keyword1;
                    }
                    if (!string.IsNullOrEmpty(imContent))
                    {
                        var message = "";
                        //switch (@event.NoticeType)
                        //{
                        //    case Model.Enums.NoticeEnumType.UserPayslip:
                        //        {
                        //            keyword1 = temp.Name;
                        //            keyword4 = @event.Keyword1;
                        //        }
                        //        break;
                        //    case Model.Enums.NoticeEnumType.UserAdvance:
                        //        {
                        //            keyword1 = temp.Name;
                        //            keyword2 = "预支金额：" + @event.Keyword2;
                        //            keyword3 = @event.Keyword1;
                        //            keyword4 = @event.Keyword3;
                        //        }
                        //        break;
                        //    case Model.Enums.NoticeEnumType.UserContract:
                        //        {
                        //            //keyword1 = @event.Keyword4;
                        //            //keyword2 = @event.Keyword5;
                        //            keyword2 = @event.Keyword3;
                        //            keyword3 = @event.Keyword4;
                        //            keyword4 = @event.Keyword5;
                        //        }
                        //        break;
                        //    case Model.Enums.NoticeEnumType.Remind:
                        //        {
                        //            keyword2 = @event.Keyword3;
                        //            keyword3 = @event.Keyword5;
                        //        }
                        //        break;
                        //}
                        //发送即使消息
                        var immodel = new Model.IMMessage();
                        immodel.fromuid = 2;
                        immodel.fromtype = 0;
                        immodel.chattype = "sysuser";
                        immodel.type = "text";
                        immodel.content = "{\"text\":\"" + imContent + "\",\"url\":\"" + @event.Page + "\"}";
                        immodel.title = imContent;
                        immodel.totype = imToType;
                        immodel.touid = @event.UserID;
                        await IMSService.SendMessage(immodel);
                    }
                    
                }
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
        private async Task<string> SendMessage(Model.Live_V_ClientNoticeTemplate temp, SendNoticeEventData @event, string openId,string miniAppID)
        {
            try
            {
                var keyword1 = @event.Keyword1;
                var keyword2 = @event.Keyword2;
                var keyword3 = @event.Keyword3;
                var keyword4 = @event.Keyword4;
                var keyword5 = @event.Keyword5;
                switch (@event.NoticeType)
                {
                    case Model.Enums.NoticeEnumType.UserPayslip:
                        {
                            keyword1 = temp.Name;
                            keyword4 = @event.Keyword1;
                        }
                        break;
                    case Model.Enums.NoticeEnumType.UserAdvance:
                        {
                            keyword1 = temp.Name;
                            keyword2 = "预支金额：" + @event.Keyword2;
                            keyword3 = @event.Keyword1;
                            keyword4 = @event.Keyword3;
                        }
                        break;
                    case Model.Enums.NoticeEnumType.UserContract:
                        {
                            //keyword1 = @event.Keyword4;
                            //keyword2 = @event.Keyword5;
                            keyword2 = @event.Keyword3;
                            keyword3 = @event.Keyword4;
                            keyword4 = @event.Keyword5;
                        }
                        break;
                    case Model.Enums.NoticeEnumType.Remind:
                        {
                            keyword2 = @event.Keyword3;
                            keyword3 = @event.Keyword5;
                        }
                        break;
                }
                var data = new
                {
                    keyword1 = new TemplateDataItem(keyword1),
                    keyword2 = new TemplateDataItem(keyword2),
                    keyword3 = new TemplateDataItem(keyword3),
                    keyword4 = new TemplateDataItem(keyword4),
                    keyword5 = new TemplateDataItem(keyword5),
                };
                var templateModel_MiniProgram = new TemplateModel_MiniProgram();
                templateModel_MiniProgram.appid = miniAppID;
                templateModel_MiniProgram.pagepath = @event.Page;
                var result =await TemplateApi.SendTemplateMessageAsync(temp.AppID, openId, temp.TempID, "", data, templateModel_MiniProgram, 10000);
                if (result.errcode != 0)
                {
                    Common.StaticLog.Debug(JsonConvert.SerializeObject(result));
                    Common.StaticLog.Debug(result.errmsg);
                    return result.errmsg;
                }
                return "ok";
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                if (ex.Message.IndexOf("43004") > 0)
                {
                    return "该员工未关注公众号";
                }
                else if (ex.Message.IndexOf("40003") > 0)
                {
                    return "openid错误";
                }
                else
                {
                    return "发送错误";
                }
            }
        }
        private async Task SendSubscribe(Model.Live_V_ClientNoticeTemplate temp, SendNoticeEventData @event, string openId)
        {
            try
            {
                var tempData = new TemplateMessageData();
                var fields = temp.Fields.Split(',');
                for (int i = 0; i < fields.Length; i++)
                {
                    var value = "";
                    switch (i)
                    {
                        case 0:
                            value = @event.Keyword1;
                            break;
                        case 1:
                            value = @event.Keyword2;
                            break;
                        case 2:
                            value = @event.Keyword3;
                            break;
                        case 3:
                            value = @event.Keyword4;
                            break;
                        case 4:
                            value = @event.Keyword5;
                            break;
                    }
                    tempData.Add(fields[i], new TemplateMessageDataValue(value));
                }
                await Senparc.Weixin.WxOpen.AdvancedAPIs.MessageApi.SendSubscribeAsync(temp.AppID, openId, temp.TempID, tempData, @event.Page);
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
            }
        }
    }
}
