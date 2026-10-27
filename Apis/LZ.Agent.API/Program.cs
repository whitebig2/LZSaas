using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "agent-api";
appModel.ModuleTitle = "智能体服务";
appModel.HasMQ = true;
await builder.InitAppliation(appModel);