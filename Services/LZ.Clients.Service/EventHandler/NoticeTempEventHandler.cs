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
using System.Text.Json;

namespace LZ.Clients.Service.EventHandler
{
    public class NoticeTempEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<NoticeTempEventData>
    {
        public async Task HandleAsync(NoticeTempEventData @event)
        {
            Common.StaticLog.Debug("开始处理新增订阅消息事件！id:"+ @event.ClientID);
            using var scope = scopeFactory.CreateScope();
            var clientsService = scope.ServiceProvider.GetService<IClientsService>();
            var clientNoticeTemplateService = scope.ServiceProvider.GetService<IClientNoticeTemplateService>();
            var noticeTemplateService = scope.ServiceProvider.GetService<INoticeTemplateService>();
            var httpService = scope.ServiceProvider.GetService<IHttpService>();
            try
            {
                if (clientsService!=null && clientNoticeTemplateService != null && noticeTemplateService != null)
                {
                    var client = await clientsService.Get(@event.ClientID);
                    if (client != null)
                    {
                        //var token = await Senparc.Weixin.WxOpen.Containers.AccessTokenContainer.TryGetAccessTokenAsync(client.AppID, client.AppSecret);
                        var token = "";
                        if (client.AppType == "mp")
                        {
                            token = await Senparc.Weixin.MP.Containers.AccessTokenContainer.GetAccessTokenAsync(client.AppID);
                        }
                        else
                        {
                            token = await Senparc.Weixin.WxOpen.Containers.AccessTokenContainer.GetAccessTokenAsync(client.AppID);
                        }
                        var expable = Expressionable.Create<Model.Live_NoticeTemplate>();
                        expable.And(it => it.Type == @event.Type);
                        var exp = expable.ToExpression();
                        
                        var models = await noticeTemplateService.List(exp, new PagedModelBase() { Page = 0 });
                        var addModels = new List<Model.Live_ClientNoticeTemplate>();
                        foreach (var model in models)
                        {
                            var kids = model.KidList.Split(',');
                            //var kidList = new List<int>();
                            //foreach (var kid in kids)
                            //{
                            //    kidList.Add(int.Parse(kid));
                            //}
                            var paras = new Dictionary<string, string>();
                            paras.Add("tid", model.TID);
                            for (int i = 0; i < kids.Length; i++)
                            {
                                paras.Add("kidList["+i+"]", kids[i]);
                            }
                            paras.Add("sceneDesc", model.SceneDesc);
                           var hres = await httpService.PostFormAsync("https://api.weixin.qq.com/wxaapi/newtmpl/addtemplate?access_token=" + token, paras);
                            var res = JsonSerializer.Deserialize<Senparc.Weixin.MP.AdvancedAPIs.NewTmpl.NewTmplJson.AddTemplateJsonResult>(hres);
                            //var res = await Senparc.Weixin.MP.AdvancedAPIs.NewTmpl.NewTmplApi.AddTemplateAsync(token,model.TID,kidList.ToArray(),model.SceneDesc);
                            if (res.errcode == 0)
                            {
                                var m = new Model.Live_ClientNoticeTemplate();
                                m.ClientID=@event.ClientID;
                                m.TID=model.TID;
                                m.NoticeID = model.ID;
                                m.TempID = res.priTmplId;
                                addModels.Add(m);
                            }
                            else
                            {
                                Common.StaticLog.Debug(res.errmsg+"；code:"+ res.errcode);
                            }
                            Thread.Sleep(100);
                        }
                        await clientNoticeTemplateService.Add(addModels);
                    }
                   
                }
              
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
            }
            
        }
    }
}
