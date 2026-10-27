using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "product-api";
appModel.ModuleTitle = "产品服务";
appModel.HasMQ = true;
await builder.InitAppliation(appModel);