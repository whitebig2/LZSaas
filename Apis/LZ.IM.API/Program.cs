using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "im-api";
appModel.ModuleTitle = "IM·þÎñ";
appModel.HasMQ = true;
appModel.HasAddHub = true;
await builder.InitAppliation(appModel);