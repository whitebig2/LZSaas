using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "user-api";
appModel.ModuleTitle = "会员服务";
appModel.HasMQ = true;
await builder.InitAppliation(appModel);