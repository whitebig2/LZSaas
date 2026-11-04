using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

Console.Title = "- 公司服务";

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "company-api";
appModel.ModuleTitle = "公司服务";
appModel.HasMQ = true;
await builder.InitAppliation(appModel);
