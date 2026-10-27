using LZ.Core;

namespace LZ.Invoice.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var appModel = new LZ.Model.AppliationModel();
            appModel.ModuleKey = "invoice-api";
            appModel.ModuleTitle = "发票服务";
            appModel.HasMQ = false;
            appModel.HasAddMVC = true;
            await builder.InitAppliation(appModel);
        }
    }
}
