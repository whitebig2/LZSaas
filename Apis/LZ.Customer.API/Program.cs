using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "customer-api";
appModel.ModuleTitle = "客户服务";
appModel.HasMQ = true;
appModel.HasAddMVC = true;

await builder.InitAppliation(appModel);
LZ.Common.StaticLog.Error("basic开始了。。。");