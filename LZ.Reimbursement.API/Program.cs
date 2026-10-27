using LZ.Core;
using LZ.Model;

var builder = WebApplication.CreateBuilder(args);
await builder.InitAppliation(new AppliationModel
{
    ModuleKey = "reimbursement-api",
    ModuleTitle = "报销审核服务"
});
