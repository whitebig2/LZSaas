using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "shopbasic-api";
appModel.ModuleTitle = "商城基础服务";
appModel.HasMQ = true;
await builder.InitAppliation(appModel);