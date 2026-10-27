using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Interface;
using LZ.Clients.Service.Interface;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LZ.Clients.Service.EventHandler
{
    public class ClientReleaseEventHandler(IServiceScopeFactory scopeFactory): IEventHandler<ClientReleaseEventData>
    {
        public async Task HandleAsync(ClientReleaseEventData @event)
        {
            Common.StaticLog.Debug("开始处理应用发布事件！id:"+ @event.ClientID);
            using var scope = scopeFactory.CreateScope();
            var clientsService = scope.ServiceProvider.GetService<IClientsService>();
            var settingsService = scope.ServiceProvider.GetService<ISettingsService>();
            var httpService = scope.ServiceProvider.GetService<IHttpService>();
            var nodeServices = scope.ServiceProvider.GetService<INodeServices>();
            try
            {
                if (clientsService!=null && settingsService!=null && nodeServices!=null && httpService!=null)
                {
                    var settings = await settingsService.GetMasterSettings();
                    var client = await clientsService.Get(@event.ClientID);
                    if (client != null && client.ReleaseStatus==3)
                    {
                        var path = settings.MiniReleasePath;
                        var miniPath=path+"minis/"+ client.GID.ToString();
                        var commPath = path + "minis/common";
                        if (!Directory.Exists(miniPath))
                        {
                            Directory.CreateDirectory(miniPath);
                        }
                        foreach (string dirPath in Directory.GetDirectories(commPath, "*", SearchOption.AllDirectories))
                        {
                            var newPath = dirPath.Replace(commPath, miniPath);
                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                            }
                        }
                        foreach (string dirPath in Directory.GetFiles(commPath, "*.*", SearchOption.AllDirectories))
                        {
                            var newPath = dirPath.Replace(commPath, miniPath);
                            if (File.Exists(newPath))
                            {
                                File.Delete(newPath);
                            }
                            File.Copy(dirPath, newPath);
                        }
                        var vendor = miniPath + "/common/vendor.js";
                        var appJson = miniPath + "/app.json";
                        var projectJson = miniPath + "/project.config.json";

                        string content =await File.ReadAllTextAsync(vendor);
                        content = content.Replace("{config}", "{" + $"AppTitle:\"{client.Name}\",AppDescription:\"{client.Name}\",AppID:\"{client.AppID}\",ClientID:\"{client.ID}\",CompanyGID:\"{client.CompanyGID}\",ApiUrl:\"{settings.ApiServicesUrl}\",IMUrl:\"{settings.IMServicesUrl}\",OssUrl:\"{settings.AliBucketDomain}\",StaticUrl:\"{settings.StaticUrl}\",PublishTime:\"\"" + "}");
                        //Common.StaticLog.Debug("{" + $"AppTitle:\"{client.Name}\",AppDescription:\"{client.Name}\",AppID:\"{client.AppID}\",ClientID:\"{client.ID}\",CompanyGID:\"{client.CompanyGID}\",ApiUrl:\"{settings.ApiServicesUrl}\",IMUrl:\"{settings.IMServicesUrl}\",OssUrl:\"{settings.AliBucketDomain}\",StaticUrl:\"{settings.StaticUrl}\",PublishTime:\"\"" + "}");
                       await File.WriteAllTextAsync(vendor, content);

                       var appContent = await File.ReadAllTextAsync(appJson);
                        appContent = appContent.Replace("{name}", client.Name);
                        await File.WriteAllTextAsync(appJson, appContent);

                        var proContent = await File.ReadAllTextAsync(projectJson);
                        proContent = proContent.Replace("{name}", client.Name).Replace("{appid}", client.AppID);
                        await File.WriteAllTextAsync(projectJson, proContent);
                        var paras = new Dictionary<string, string>();
                        paras.Add("projectPath", "/files/minis/" + client.GID.ToString());
                        paras.Add("privateKeyPath", "/files/" + client.PrivateKeyPath);
                        paras.Add("version", client.Version);
                        paras.Add("appid", client.AppID);
                        paras.Add("desc", settings.MiniReleaseDesc);
                        var minres = await httpService.PostJsonAsync(settings.MiniReleaseApiUrl, JsonConvert.SerializeObject(paras));
                        //string res = await nodeServices.InvokeAsync<string>(path + "/index.js", client.AppID, "/minis/"+ client.GID.ToString(), client.PrivateKeyPath, client.Version).ConfigureAwait(false);
                        //Common.StaticLog.Debug(res);
                        //if (res.IndexOf("Error") > -1)
                        //{
                        //   await clientsService.ChangeStatus(client.ID, 4);
                        //}
                        //else
                        //{
                        //    await clientsService.ChangeStatus(client.ID, 2, DateTime.Now);
                        //}
                       // var minresObj = JsonConvert.DeserializeObject<JArray>(minres);
                        if (minres.IndexOf("successful") >-1)
                        {
                            await clientsService.ChangeStatus(client.ID, 2, DateTime.Now);
                        }
                        else
                        {
                            await clientsService.ChangeStatus(client.ID, 4);
                            Common.StaticLog.Error(minres);
                        }
                    }
                }
            }
            catch (Exception ex) { 
                Common.StaticLog.Error(ex);
                await clientsService.ChangeStatus(@event.ClientID, 4);
            }
        }
    }
}
