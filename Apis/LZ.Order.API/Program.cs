using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "order-api";
appModel.ModuleTitle = "¶©µ¥·þÎñ";
appModel.HasMQ = true;
appModel.HasWeiXinPay = true;
appModel.HasWeixin = true;

await builder.InitAppliation(appModel);