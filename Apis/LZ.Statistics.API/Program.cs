using LZ.Core;

var builder = WebApplication.CreateBuilder(args);

var appModel = new LZ.Model.AppliationModel();
appModel.ModuleKey = "statistics-api";
appModel.ModuleTitle = "统计服务";

await builder.InitAppliation(appModel);