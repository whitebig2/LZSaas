using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "finance-api";
appModel.ModuleTitle = "财务服务";
appModel.HasMQ=true;
await builder.InitAppliation(appModel);
