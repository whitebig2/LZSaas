using LZ.ApiService.Services;
using LZ.Core;
using LZ.Core.RabbitMQ.EventBus;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;


var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "basic-api";
appModel.ModuleTitle = "»ù´¡·þÎñ";
appModel.HasMQ = true;
var builder = WebApplication.CreateBuilder(args);

await builder.InitAppliation(appModel);






