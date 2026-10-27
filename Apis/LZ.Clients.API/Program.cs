using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "clients-api";
appModel.ModuleTitle = "客户端服务";
appModel.HasWeixin = true;
appModel.HasNode = true;
appModel.HasMQ = true;
appModel.HasAddMVC = true;
await builder.InitAppliation(appModel);
