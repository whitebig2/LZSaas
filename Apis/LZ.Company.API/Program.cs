using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "company-api";
appModel.ModuleTitle = "公司服务";
appModel.HasMQ = true;
await builder.InitAppliation(appModel);
