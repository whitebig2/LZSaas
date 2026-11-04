using LZ.ApiService.Services;
using LZ.Core;
using LZ.Core.RabbitMQ.EventBus;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;

Console.Title = "- 基础服务";

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "basic-api";
appModel.ModuleTitle = "基础服务";
appModel.HasMQ = true;
var builder = WebApplication.CreateBuilder(args);

await builder.InitAppliation(appModel);






